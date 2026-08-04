using System;
using Luma.UI;

namespace Luma.Screens;

public class AboutScreen : Screen
{
    public override void Render()
    {
        ConsoleHelper.ResetCursor();
        
        Console.WriteLine(Pad($"\n  {ConsoleHelper.Text}Luma"));
        Console.WriteLine(Pad($"  {ConsoleHelper.TextMuted}Version 1.0.0{ConsoleHelper.Reset}"));
        
        Console.WriteLine(Pad("")); // Empty line
        
        Console.WriteLine(Pad($"  {ConsoleHelper.TextMuted}Telegram:{ConsoleHelper.Reset}"));
        Console.WriteLine(Pad($"  {ConsoleHelper.Accent}t.me/moonmudev{ConsoleHelper.Reset}"));

        Console.WriteLine(Pad("")); // Empty line
        
        Console.WriteLine(Pad($"  {ConsoleHelper.TextMuted}Github:{ConsoleHelper.Reset}"));
        Console.WriteLine(Pad($"  {ConsoleHelper.Accent}github.com/monmu-dev{ConsoleHelper.Reset}"));
        
        Console.WriteLine(Pad(""));
        Console.WriteLine(Pad($"  {ConsoleHelper.TextMuted}[Press Esc or Backspace to return]{ConsoleHelper.Reset}"));
        
        // Ensure trailing space is cleared to avoid ghosting
        for(int i=0; i<3; i++) Console.WriteLine(Pad(""));
    }

    public override void HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Backspace || key.Key == ConsoleKey.Enter)
        {
            ScreenManager.GoBack();
        }
    }
}
