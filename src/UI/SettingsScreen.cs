using System;
using System.Collections.Generic;

namespace Luma.UI;

public abstract class SettingsScreen : Screen
{
    protected int SelectedIndex = 0;
    protected abstract string Title { get; }
    
    public abstract int ItemCount { get; }
    public abstract void DrawItem(int index, bool isSelected);
    public abstract void IncrementValue(int index);
    public abstract void DecrementValue(int index);
    public abstract void OnExecute(int index);

    public override void Render()
    {
        ConsoleHelper.ResetCursor();
        
        Console.WriteLine(Pad($"\n  {ConsoleHelper.Text}{Loc.T(Title)}"));
        Console.WriteLine(Pad("")); // Empty line
        
        for (int i = 0; i < ItemCount; i++)
        {
            DrawItem(i, i == SelectedIndex);
        }
        
        Console.WriteLine(Pad(""));
        Console.WriteLine(Pad(""));
        Console.WriteLine(Pad($"  {ConsoleHelper.TextMuted}github.com/monmu-dev{ConsoleHelper.Reset}"));
        // Fill trailing space for couple lines to avoid artifacts from longer menus
        for(int i=0; i<5; i++) Console.WriteLine(Pad(""));
    }

    public override void HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.UpArrow)
        {
            SelectedIndex--;
            if (SelectedIndex < 0) SelectedIndex = ItemCount - 1;
            Render();
        }
        else if (key.Key == ConsoleKey.DownArrow)
        {
            SelectedIndex++;
            if (SelectedIndex >= ItemCount) SelectedIndex = 0;
            Render();
        }
        else if (key.Key == ConsoleKey.LeftArrow)
        {
            DecrementValue(SelectedIndex);
            Render();
        }
        else if (key.Key == ConsoleKey.RightArrow)
        {
            IncrementValue(SelectedIndex);
            Render();
        }
        else if (key.Key == ConsoleKey.Enter)
        {
            OnExecute(SelectedIndex);
            if (ScreenManager.CurrentScreen == this)
            {
                Render();
            }
        }
        else if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Backspace)
        {
            ScreenManager.GoBack();
        }
    }
}
