using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;

namespace QuickGPT.Classes
{
    internal class ShortcutManager
    {
        private const string HOTKEY_NAME = "QuickGPT_GlobalHotkey";

        public static string HotkeyToString(Key key, ModifierKeys modifiers)
        {
            List<string> parts = [];
            if (modifiers.HasFlag(ModifierKeys.Control))
                parts.Add("Ctrl");
            if (modifiers.HasFlag(ModifierKeys.Alt))
                parts.Add("Alt");
            if (modifiers.HasFlag(ModifierKeys.Shift))
                parts.Add("Shift");
            if (modifiers.HasFlag(ModifierKeys.Windows))
                parts.Add("Win");

            parts.Add(key.ToString());

            return string.Join("+", parts);
        }

        public static void RegisterHotkeyFromString(string hotkeyString)
        {
            var parts = hotkeyString.Split('+');
            ModifierKeys modifiers = ModifierKeys.None;
            Key key = Key.None;

            foreach (var part in parts)
            {
                switch (part)
                {
                    case "Ctrl":
                        modifiers |= ModifierKeys.Control;
                        break;
                    case "Alt":
                        modifiers |= ModifierKeys.Alt;
                        break;
                    case "Shift":
                        modifiers |= ModifierKeys.Shift;
                        break;
                    case "Win":
                        modifiers |= ModifierKeys.Windows;
                        break;
                    default:
                        if (Enum.TryParse(part, out Key parsedKey))
                            key = parsedKey;
                        break;
                }
            }

            if (key != Key.None)
            {
                HotkeyManager.Current.AddOrReplace(HOTKEY_NAME, key, modifiers, HotkeyAction);
            }
        }

        public static void RemoveHotkey()
        {
            HotkeyManager.Current.Remove(HOTKEY_NAME);
        }

        private static void HotkeyAction(object? sender, HotkeyEventArgs e)
        {
            PromptWindow promptWindow = new();
            promptWindow.Show();
            promptWindow.Activate();
        }
    }
}
