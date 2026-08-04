using System;
using System.Collections.Generic;

namespace Luma.UI;

public abstract class MenuScreen : Screen
{
    protected int SelectedIndex = 0;
    protected abstract string Title { get; }
    protected abstract IReadOnlyList<string> Items { get; }
    
    public override void Render()
    {
        ConsoleHelper.ResetCursor();
        
        Console.WriteLine(Pad($"\n  {ConsoleHelper.Text}{Loc.T(Title)}"));
        Console.WriteLine(Pad("")); // Empty line
        
        for (int i = 0; i < Items.Count; i++)
        {
            if (i == SelectedIndex)
            {
                Console.WriteLine(Pad($"  {ConsoleHelper.Accent}> {ConsoleHelper.Selected}{Loc.T(Items[i])}{ConsoleHelper.Reset}"));
            }
            else
            {
                Console.WriteLine(Pad($"    {ConsoleHelper.Text}{Loc.T(Items[i])}{ConsoleHelper.Reset}"));
            }
        }
        
        // Blank lines to push footer down a bit, or just fixed position
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
            if (SelectedIndex < 0) SelectedIndex = Items.Count - 1;
            Render();
        }
        else if (key.Key == ConsoleKey.DownArrow)
        {
            SelectedIndex++;
            if (SelectedIndex >= Items.Count) SelectedIndex = 0;
            Render();
        }
        else if (key.Key == ConsoleKey.Enter)
        {
            OnExecute(SelectedIndex);
        }
        else if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Backspace)
        {
            ScreenManager.GoBack();
        }
    }

    protected abstract void OnExecute(int index);
}
