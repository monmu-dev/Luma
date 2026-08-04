using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace Luma.State;

public class ConfigDto
{
    public bool HdrEnabled { get; set; } = true;
    public int Brightness { get; set; } = 50;
    public int Contrast { get; set; } = 50;
    public int Gamma { get; set; } = 50;
    public string Temperature { get; set; } = "Neutral";
    public bool IsRussian { get; set; } = false;
}

public static class ConfigState
{
    private static readonly string ConfigPath = Path.Combine(System.AppContext.BaseDirectory, "luma_config.json");

    public static void Load()
    {
        if (File.Exists(ConfigPath))
        {
            try
            {
                var json = File.ReadAllText(ConfigPath);
                var dto = JsonSerializer.Deserialize<ConfigDto>(json);
                if (dto != null)
                {
                    _hdrEnabled = dto.HdrEnabled;
                    _brightness = dto.Brightness;
                    _contrast = dto.Contrast;
                    _gamma = dto.Gamma;
                    _temperature = dto.Temperature;
                    Loc.IsRussian = dto.IsRussian;
                }
            }
            catch { }
        }

        // Apply settings immediately on startup
        MonitorAPI.SetHdr(_hdrEnabled);
        MonitorAPI.SetBrightness(_brightness);
        MonitorAPI.SetContrast(_contrast);
        MonitorAPI.ApplyGammaAndTemperature(_gamma, _temperature);
    }

    public static void Save()
    {
        try
        {
            var dto = new ConfigDto
            {
                HdrEnabled = _hdrEnabled,
                Brightness = _brightness,
                Contrast = _contrast,
                Gamma = _gamma,
                Temperature = _temperature,
                IsRussian = Loc.IsRussian
            };
            var json = JsonSerializer.Serialize(dto);
            File.WriteAllText(ConfigPath, json);
        }
        catch { }
    }

    private static bool _hdrEnabled = true;
    public static bool HdrEnabled
    {
        get => _hdrEnabled;
        set
        {
            _hdrEnabled = value;
            MonitorAPI.SetHdr(value);
            Save();
        }
    }

    private static int _brightness = 50;
    private static CancellationTokenSource? _brightnessCts;
    public static int Brightness
    {
        get => _brightness;
        set
        {
            _brightness = value;
            Debounce(ref _brightnessCts, () => MonitorAPI.SetBrightness(value));
            Save();
        }
    }

    private static int _contrast = 50;
    private static CancellationTokenSource? _contrastCts;
    public static int Contrast
    {
        get => _contrast;
        set
        {
            _contrast = value;
            Debounce(ref _contrastCts, () => MonitorAPI.SetContrast(value));
            Save();
        }
    }

    private static int _gamma = 50;
    private static CancellationTokenSource? _gammaCts;
    public static int Gamma
    {
        get => _gamma;
        set
        {
            _gamma = value;
            Debounce(ref _gammaCts, () => MonitorAPI.ApplyGammaAndTemperature(_gamma, _temperature));
            Save();
        }
    }

    private static string _temperature = "Neutral";
    private static CancellationTokenSource? _tempCts;
    public static string Temperature
    {
        get => _temperature;
        set
        {
            _temperature = value;
            Debounce(ref _tempCts, () => MonitorAPI.ApplyGammaAndTemperature(_gamma, _temperature));
            Save();
        }
    }

    public static bool AutoRunEnabled
    {
        get
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
                return key?.GetValue("Luma") != null;
            }
            catch { return false; }
        }
        set
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (value)
                {
                    string? exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
                    if (exePath != null) key?.SetValue("Luma", exePath);
                }
                else
                {
                    key?.DeleteValue("Luma", false);
                }
            }
            catch { }
        }
    }

    private static void Debounce(ref CancellationTokenSource? cts, System.Action action)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        var token = cts.Token;
        _ = ExecuteAsync(action, token);
    }

    private static async Task ExecuteAsync(System.Action action, CancellationToken token)
    {
        try
        {
            // Reduced to 20ms for extremely fast, smooth reaction while still preventing hardware flood
            await Task.Delay(20, token); 
            if (!token.IsCancellationRequested)
            {
                await Task.Run(action, token);
            }
        }
        catch (TaskCanceledException) { }
    }
}
