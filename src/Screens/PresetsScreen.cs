using System.Collections.Generic;
using Luma.UI;

namespace Luma.Screens;

public class PresetsScreen : MenuScreen
{
    protected override string Title => "Presets";

    protected override IReadOnlyList<string> Items => new[]
    {
        "Rust: Vibrant",
        "Rust: Night Vision",
        "Rust: Metro",
        "Rust: Old",
        "Default",
        "Back"
    };

    protected override void OnExecute(int index)
    {
        switch (index)
        {
            case 0: // Rust Vibrant (Purple)
                State.ConfigState.Brightness = 80;
                State.ConfigState.Contrast = 90;
                State.ConfigState.Gamma = 35; 
                State.ConfigState.Temperature = "Purple";
                break;
            case 1: // Rust Night Vision
                State.ConfigState.Brightness = 100;
                State.ConfigState.Contrast = 45; // Sharper
                State.ConfigState.Gamma = 70; // Less washed out
                State.ConfigState.Temperature = "Cool";
                break;
            case 2: // Rust Metro
                State.ConfigState.Brightness = 100;
                State.ConfigState.Contrast = 55;
                State.ConfigState.Gamma = 75; 
                State.ConfigState.Temperature = "Neutral";
                break;
            case 3: // Rust Old
                State.ConfigState.Brightness = 70;
                State.ConfigState.Contrast = 80;
                State.ConfigState.Gamma = 40;
                State.ConfigState.Temperature = "Cool";
                break;
            case 4: // Default
                State.ConfigState.Brightness = 50;
                State.ConfigState.Contrast = 50;
                State.ConfigState.Gamma = 50;
                State.ConfigState.Temperature = "Neutral";
                break;
            case 5: // Back
                ScreenManager.GoBack();
                return;
        }
        
        ScreenManager.GoBack();
    }
}
