using System;
using System.Runtime.InteropServices;

namespace Luma;

public static class ConsoleHelper
{
    private const int STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    public static void EnableVT()
    {
        var handle = GetStdHandle(STD_OUTPUT_HANDLE);
        if (GetConsoleMode(handle, out uint mode))
        {
            SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }
    }
    
    // Theme colors
    public const string Reset = "\x1b[0m";
    public const string Text = "\x1b[38;2;210;210;215m";
    public const string TextMuted = "\x1b[38;2;120;120;130m";
    public const string Accent = "\x1b[38;2;80;190;255m";
    public const string Selected = "\x1b[38;2;255;255;255m";

    public static void ClearScreen()
    {
        Console.Write("\x1b[2J\x1b[H");
    }

    public static void ResetCursor()
    {
        Console.SetCursorPosition(0, 0);
    }
}
