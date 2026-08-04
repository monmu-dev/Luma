using System.Collections.Generic;

namespace Luma;

public static class Loc
{
    public static bool IsRussian { get; set; } = false;

    private static readonly Dictionary<string, string> Ru = new()
    {
        { "HDR", "HDR" },
        { "Games", "Игры" },
        { "Presets", "Пресеты" },
        { "Display Settings", "Настройки экрана" },
        { "About", "О программе" },
        { "Language: EN", "Язык: RU" },
        { "Exit", "Выход" },
        { "Back", "Назад" },
        { "Rust: Vibrant", "Rust: Яркие цвета" },
        { "Rust: Night Vision", "Rust: Ночное зрение" },
        { "Rust: Metro", "Rust: Метро" },
        { "Rust: Old", "Rust: Старая насыщенность" },
        { "Purple", "Пурпурный" },
        { "Default", "По умолчанию" },
        { "Autorun", "Автозапуск" },
        { "Applied!", "Применено!" },
        { "Brightness", "Яркость" },
        { "Contrast", "Контраст" },
        { "Gamma", "Гамма" },
        { "Temperature", "Температура" },
        { "Reset", "Сброс" },
        { "Reset Display", "Сбросить настройки" },
        { "Cool", "Холодный" },
        { "Neutral", "Нейтральный" },
        { "Warm", "Теплый" },

        { "Version 1.0.0", "Версия 1.0.0" },
        { "[Press Esc or Backspace to return]", "[Нажмите Esc или Backspace для возврата]" }
    };

    public static string T(string key)
    {
        if (IsRussian && Ru.TryGetValue(key, out var ruVal))
        {
            return ruVal;
        }
        return key; // Fallback to English
    }
}
