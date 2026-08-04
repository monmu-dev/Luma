using System;
using System.Collections.Generic;

namespace Luma.UI;

public static class ScreenManager
{
    public static Screen? CurrentScreen { get; private set; }
    private static readonly Stack<Screen> _history = new();
    
    public static bool IsRunning { get; private set; } = true;

    public static void NavigateTo(Screen screen)
    {
        if (CurrentScreen != null)
        {
            _history.Push(CurrentScreen);
        }
        
        CurrentScreen = screen;
        ConsoleHelper.ClearScreen();
        CurrentScreen.Render();
    }

    public static void GoBack()
    {
        if (_history.Count > 0)
        {
            CurrentScreen = _history.Pop();
            ConsoleHelper.ClearScreen();
            CurrentScreen.Render();
        }
        else
        {
            // Exit if we try to go back from the main menu
            IsRunning = false;
        }
    }

    public static void Run(Screen initialScreen)
    {
        Console.CursorVisible = false;
        ConsoleHelper.EnableVT();
        
        NavigateTo(initialScreen);

        while (IsRunning)
        {
            var key = Console.ReadKey(true);
            CurrentScreen?.HandleInput(key);
        }
        
        ConsoleHelper.ClearScreen();
        Console.CursorVisible = true;
        Console.WriteLine($"{ConsoleHelper.Reset}Exiting Luma...");
    }
}
