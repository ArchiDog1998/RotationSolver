using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.GamePad;
using Dalamud.Game.ClientState.Keys;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Commands;

namespace RotationSolver.Updaters;

internal static class InputUpdater
{
    static readonly SortedList<VirtualKey, bool> _keys = new SortedList<VirtualKey, bool>();
    static readonly SortedList<GamepadButtons, bool> _buttons = new SortedList<GamepadButtons, bool>();

    public static SpecialCommandType RecordingSpecialType { get ; set; }
    public static StateCommandType RecordingStateType { get ; set; }
    public static DateTime RecordingTime { get; set; } = DateTime.MinValue;

    internal static unsafe void UpdateCommand()
    {
        if (Service.Conditions[ConditionFlag.OccupiedInQuestEvent]
            || Service.Conditions[ConditionFlag.Occupied33]
            || Service.Conditions[ConditionFlag.Occupied38]
            || Service.Conditions[ConditionFlag.Jumping61]
            || Service.Conditions[ConditionFlag.BetweenAreas]
            || Service.Conditions[ConditionFlag.BetweenAreas51]
            || Service.Conditions[ConditionFlag.Mounted]
            || Service.Conditions[ConditionFlag.SufferingStatusAffliction2]
            || Service.Conditions[ConditionFlag.RolePlaying]
            || Service.Conditions[ConditionFlag.InFlight]) return;

        if (DateTime.Now - RecordingTime > TimeSpan.FromSeconds(10))
        {
            RecordingSpecialType = SpecialCommandType.None;
            RecordingStateType = StateCommandType.None;
        }

        foreach (var key in Service.KeyState.GetValidVirtualKeys())
        {
            if (key is VirtualKey.CONTROL) continue;
            if (key is VirtualKey.MENU) continue;
            if (key is VirtualKey.SHIFT) continue;

            var value = Service.KeyState[key];

            if (_keys.ContainsKey(key))
            {
                if (!_keys[key] && value)
                {
                    KeyDown(new KeyRecord(key, Service.KeyState[VirtualKey.CONTROL], 
                        Service.KeyState[VirtualKey.MENU], Service.KeyState[VirtualKey.SHIFT]));
                }
            }
            _keys[key] = value;
        }

        foreach (var button in Enum.GetValues<GamepadButtons>())
        {
            if (button is GamepadButtons.L2) continue;
            if (button is GamepadButtons.R2) continue;


            var value = Service.GamepadState.Raw(button) > 0.5f;
            if (_buttons.ContainsKey(button))
            {
                if (!_buttons[button] && value)
                {
                    ButtonDown(new ButtonRecord(button,
                        Service.GamepadState.Raw(GamepadButtons.L2) > 0.5f, 
                        Service.GamepadState.Raw(GamepadButtons.R2) > 0.5f));
                }
            }
            _buttons[button] = value;
        }
    }

    static readonly Dalamud.Game.Gui.Toast.QuestToastOptions QUEST = new Dalamud.Game.Gui.Toast.QuestToastOptions()
    {
        IconId = 101,
        PlaySound = true,
        DisplayCheckmark = true,
    };

    private static void KeyDown(KeyRecord key)
    {
        if (RecordingSpecialType != SpecialCommandType.None)
        {
            Service.Config.KeySpecial[RecordingSpecialType] = key;
            Service.ToastGui.ShowQuest($"{RecordingSpecialType}: {key.ToStr()}",
               QUEST);

            RecordingSpecialType = SpecialCommandType.None;
            Service.Config.Save();
            return;
        }
        else  if (RecordingStateType != StateCommandType.None )
        {
            Service.Config.KeyState[RecordingStateType] = key;
            Service.ToastGui.ShowQuest($"{RecordingStateType}: {key.ToStr()}",
                QUEST);

            RecordingStateType = StateCommandType.None;
            Service.Config.Save();
            return;
        }

        if (!Service.Config.UseKeyboardCommand) return;

        if (Service.Config.KeyState.ContainsValue(key))
        {
            Service.CommandManager.ProcessCommand(Service.Config.KeyState
                .FirstOrDefault(k => k.Value == key && k.Key != StateCommandType.None).Key.GetCommandStr());
        }

        if (Service.Config.KeySpecial.ContainsValue(key))
        {
            Service.CommandManager.ProcessCommand(Service.Config.KeySpecial
                .FirstOrDefault(k => k.Value == key && k.Key != SpecialCommandType.None).Key.GetCommandStr());
        }
    }

    private static void ButtonDown(ButtonRecord button)
    {
        if (RecordingSpecialType != SpecialCommandType.None)
        {
            Service.Config.ButtonSpecial[RecordingSpecialType] = button;
            Service.ToastGui.ShowQuest($"{RecordingSpecialType}: {button.ToStr()}",
                QUEST);

            RecordingSpecialType = SpecialCommandType.None;
            Service.Config.Save();
            return;
        }
        else if (RecordingStateType != StateCommandType.None)
        {
            Service.Config.ButtonState[RecordingStateType] = button;
            Service.ToastGui.ShowQuest($"{RecordingStateType}: {button.ToStr()}",
                QUEST);

            RecordingStateType = StateCommandType.None;
            Service.Config.Save();
            return;
        }

        if (!Service.Config.UseGamepadCommand) return;

        if (Service.Config.ButtonState.ContainsValue(button))
        {
            Service.CommandManager.ProcessCommand(Service.Config.ButtonState
                .FirstOrDefault(k => k.Value == button && k.Key != StateCommandType.None).Key.GetCommandStr());
        }

        if (Service.Config.ButtonSpecial.ContainsValue(button))
        {
            Service.CommandManager.ProcessCommand(Service.Config.ButtonSpecial
                .FirstOrDefault(k => k.Value == button && k.Key != SpecialCommandType.None).Key.GetCommandStr());
        }
    }
}
