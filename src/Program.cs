using System;
using Luma.Screens;
using Luma.UI;

namespace Luma;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "Luma";
        Luma.State.ConfigState.Load();
        ScreenManager.Run(new MainMenuScreen());
    }
}
