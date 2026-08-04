using System.Collections.Generic;
using Luma.Screens;

namespace Luma.UI;

public class MainMenuScreen : MenuScreen
{
    protected override string Title => "Luma";
    
    protected override IReadOnlyList<string> Items => new[]
    {
        "Display Settings",
        "Presets",
        "Language: EN",
        "About",
        "Exit"
    };

    protected override void OnExecute(int index)
    {
        switch (index)
        {
            case 0:
                ScreenManager.NavigateTo(new DisplaySettingsScreen());
                break;
            case 1:
                ScreenManager.NavigateTo(new PresetsScreen());
                break;
            case 2:
                Loc.IsRussian = !Loc.IsRussian;
                State.ConfigState.Save();
                break;
            case 3:
                ScreenManager.NavigateTo(new AboutScreen());
                break;
            case 4:
                ScreenManager.GoBack();
                break;
        }
    }
}
