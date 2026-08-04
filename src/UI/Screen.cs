using System;

namespace Luma.UI;

public abstract class Screen
{
    public abstract void Render();
    public abstract void HandleInput(ConsoleKeyInfo key);
    
    protected string Pad(string text)
    {
        // Add padding to overwrite any existing characters on the line
        return text.PadRight(Console.WindowWidth - 1);
    }
}
