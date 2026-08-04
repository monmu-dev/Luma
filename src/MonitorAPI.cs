using System;
using System.Runtime.InteropServices;
using System.Management;

namespace Luma;

public static class MonitorAPI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RAMP
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Red;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Green;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Blue;
    }

    [DllImport("gdi32.dll")]
    private static extern bool SetDeviceGammaRamp(IntPtr hdc, ref RAMP lpRamp);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hdc);

    // dxva2.dll definitions for DDC/CI
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PHYSICAL_MONITOR
    {
        public IntPtr hPhysicalMonitor;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szPhysicalMonitorDescription;
    }

    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);
    private delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr dwData);

    [DllImport("dxva2.dll")]
    private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, out uint pdwNumberOfPhysicalMonitors);

    [DllImport("dxva2.dll")]
    private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("dxva2.dll")]
    private static extern bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);

    [DllImport("dxva2.dll")]
    private static extern bool SetMonitorContrast(IntPtr hMonitor, uint dwNewContrast);

    [DllImport("dxva2.dll")]
    private static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("user32.dll")]
    private static extern int GetDisplayConfigBufferSizes(uint flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

    [DllImport("user32.dll")]
    private static extern int QueryDisplayConfig(uint flags, ref uint numPathArrayElements, [Out] byte[] pathArray, ref uint numModeInfoArrayElements, [Out] byte[] modeInfoArray, IntPtr currentTopologyId);

    [DllImport("user32.dll")]
    private static extern int DisplayConfigSetDeviceInfo(IntPtr requestPacket);

    public static void SetHdr(bool enable)
    {
        uint pathCount, modeCount;
        if (GetDisplayConfigBufferSizes(2 /* QDC_ONLY_ACTIVE_PATHS */, out pathCount, out modeCount) == 0)
        {
            byte[] paths = new byte[pathCount * 72];
            byte[] modes = new byte[modeCount * 64];
            if (QueryDisplayConfig(2, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero) == 0)
            {
                for (int i = 0; i < pathCount; i++)
                {
                    int offset = i * 72;
                    long adapterId = BitConverter.ToInt64(paths, offset + 20);
                    uint targetId = BitConverter.ToUInt32(paths, offset + 28);

                    byte[] packet = new byte[24];
                    BitConverter.GetBytes((uint)11).CopyTo(packet, 0); // type DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE
                    BitConverter.GetBytes((uint)24).CopyTo(packet, 4); // size
                    BitConverter.GetBytes(adapterId).CopyTo(packet, 8); // adapterId
                    BitConverter.GetBytes(targetId).CopyTo(packet, 16); // id
                    BitConverter.GetBytes(enable ? 1u : 0u).CopyTo(packet, 20); // enableAdvancedColor

                    IntPtr ptr = Marshal.AllocHGlobal(24);
                    Marshal.Copy(packet, 0, ptr, 24);
                    DisplayConfigSetDeviceInfo(ptr);
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }
    }

    public static void SetBrightness(int percent)
    {
        SetDdcCi(percent, true);
        try
        {
            // Fallback for laptops via WMI
            using var mclass = new ManagementClass("WmiMonitorBrightnessMethods")
            {
                Scope = new ManagementScope(@"\\.\root\wmi")
            };
            using var instances = mclass.GetInstances();
            foreach (ManagementObject instance in instances)
            {
                instance.InvokeMethod("WmiSetBrightness", new object[] { 1, (byte)percent });
            }
        }
        catch { }
    }

    public static void SetContrast(int percent)
    {
        SetDdcCi(percent, false);
    }

    private static void SetDdcCi(int percent, bool isBrightness)
    {
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (hMonitor, hdcMonitor, lprcMonitor, dwData) =>
        {
            if (GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, out uint num))
            {
                var monitors = new PHYSICAL_MONITOR[num];
                if (GetPhysicalMonitorsFromHMONITOR(hMonitor, num, monitors))
                {
                    foreach (var pm in monitors)
                    {
                        if (isBrightness) SetMonitorBrightness(pm.hPhysicalMonitor, (uint)percent);
                        else SetMonitorContrast(pm.hPhysicalMonitor, (uint)percent);
                    }
                    DestroyPhysicalMonitors(num, monitors);
                }
            }
            return true;
        }, IntPtr.Zero);
    }

    public static void ApplyGammaAndTemperature(int gammaPercent, string temperature)
    {
        double gamma = gammaPercent / 50.0;
        if (gamma < 0.1) gamma = 0.1;
        
        double rBias = 1.0, gBias = 1.0, bBias = 1.0;
        if (temperature == "Cool") { rBias = 0.9; bBias = 1.0; }
        else if (temperature == "Warm") { rBias = 1.0; bBias = 0.8; }
        else if (temperature == "Purple") { rBias = 1.0; gBias = 0.85; bBias = 1.0; }
        
        RAMP ramp = new RAMP
        {
            Red = new ushort[256],
            Green = new ushort[256],
            Blue = new ushort[256]
        };

        for (int i = 1; i < 256; i++)
        {
            double val = i / 255.0;
            double result = Math.Pow(val, 1.0 / gamma) * 65535.0;
            
            ramp.Red[i] = (ushort)Math.Min(65535.0, Math.Max(0.0, result * rBias));
            ramp.Green[i] = (ushort)Math.Min(65535.0, Math.Max(0.0, result * gBias));
            ramp.Blue[i] = (ushort)Math.Min(65535.0, Math.Max(0.0, result * bBias));
        }

        IntPtr hdc = GetDC(IntPtr.Zero);
        SetDeviceGammaRamp(hdc, ref ramp);
        ReleaseDC(IntPtr.Zero, hdc);
    }
}
