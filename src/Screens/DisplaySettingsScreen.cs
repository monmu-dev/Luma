using System;
using Luma.State;
using Luma.UI;

namespace Luma.Screens;

public class DisplaySettingsScreen : SettingsScreen
{
    protected override string Title => "Display Settings";
    public override int ItemCount => 7;

    public override void DrawItem(int index, bool isSelected)
    {
        string label = "";
        string value = "";

        switch (index)
        {
            case 0: label = "Brightness"; value = $"{ConfigState.Brightness}%"; break;
            case 1: label = "Contrast"; value = $"{ConfigState.Contrast}%"; break;
            case 2: label = "Gamma"; value = $"{ConfigState.Gamma}%"; break;
            case 3: label = "Temperature"; value = Loc.T(ConfigState.Temperature); break;
            case 4: label = "Autorun"; value = ConfigState.AutoRunEnabled ? "ON" : "OFF"; break;
            case 5: label = "Reset Display"; value = ""; break;
            case 6: label = "Back"; value = ""; break;
        }

        string prefix = isSelected ? $"  {ConsoleHelper.Accent}> {ConsoleHelper.Selected}" : $"    {ConsoleHelper.Text}";
        string formattedLabel = Loc.T(label).PadRight(20);
        
        Console.WriteLine(Pad($"{prefix}{formattedLabel}{value}{ConsoleHelper.Reset}"));
    }

    public override void IncrementValue(int index)
    {
        switch (index)
        {
            case 0: ConfigState.Brightness = Math.Min(100, ConfigState.Brightness + 1); break;
            case 1: ConfigState.Contrast = Math.Min(100, ConfigState.Contrast + 1); break;
            case 2: ConfigState.Gamma = Math.Min(100, ConfigState.Gamma + 1); break;
            case 3: CycleTemperature(1); break;
            case 4: ConfigState.AutoRunEnabled = !ConfigState.AutoRunEnabled; break;
        }
    }

    public override void DecrementValue(int index)
    {
        switch (index)
        {
            case 0: ConfigState.Brightness = Math.Max(0, ConfigState.Brightness - 1); break;
            case 1: ConfigState.Contrast = Math.Max(0, ConfigState.Contrast - 1); break;
            case 2: ConfigState.Gamma = Math.Max(0, ConfigState.Gamma - 1); break;
            case 3: CycleTemperature(-1); break;
            case 4: ConfigState.AutoRunEnabled = !ConfigState.AutoRunEnabled; break;
        }
    }

    public override void OnExecute(int index)
    {
        if (index == 4)
        {
            ConfigState.AutoRunEnabled = !ConfigState.AutoRunEnabled;
        }
        else if (index == 5)
        {
            ConfigState.Brightness = 50;
            ConfigState.Contrast = 50;
            ConfigState.Gamma = 50;
            ConfigState.Temperature = "Neutral";
        }
        else if (index == 6)
        {
            ScreenManager.GoBack();
        }
    }

    private void CycleTemperature(int dir)
    {
        string[] temps = { "Cool", "Neutral", "Warm" };
        int idx = Array.IndexOf(temps, ConfigState.Temperature);
        if (idx == -1) idx = 1;
        idx += dir;
        if (idx < 0) idx = temps.Length - 1;
        if (idx >= temps.Length) idx = 0;
        ConfigState.Temperature = temps[idx];
    }
}
