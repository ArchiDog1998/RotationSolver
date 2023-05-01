using Dalamud.Game.ClientState.GamePad;
using Dalamud.Logging;

namespace RotationSolver.Basic.Configuration;

public class InputConfiguration
{
    public Dictionary<StateCommandType, KeyRecord> KeyState { get; set; } = new Dictionary<StateCommandType, KeyRecord>();
    public Dictionary<SpecialCommandType, KeyRecord> KeySpecial { get; set; } = new Dictionary<SpecialCommandType, KeyRecord>();
    public KeyRecord KeyDoAction { get; set; } = null;
    public Dictionary<StateCommandType, ButtonRecord> ButtonState { get; set; } = new Dictionary<StateCommandType, ButtonRecord>()
    {
        {StateCommandType.AutoTarget, new ButtonRecord( GamepadButtons.East, false, true) },
        {StateCommandType.ManualTarget, new ButtonRecord( GamepadButtons.North, false, true) },
        {StateCommandType.Cancel, new ButtonRecord( GamepadButtons.South, false, true) },
    };
    public Dictionary<SpecialCommandType, ButtonRecord> ButtonSpecial { get; set; } = new Dictionary<SpecialCommandType, ButtonRecord>()
    {
        {SpecialCommandType.EndSpecial, new ButtonRecord( GamepadButtons.West, false, true) },

        {SpecialCommandType.EsunaStanceNorth, new ButtonRecord( GamepadButtons.DpadRight, false, true) },
        {SpecialCommandType.MoveForward, new ButtonRecord( GamepadButtons.DpadUp, false, true) },
        {SpecialCommandType.MoveBack, new ButtonRecord( GamepadButtons.DpadDown, false, true) },
        {SpecialCommandType.RaiseShirk, new ButtonRecord( GamepadButtons.DpadLeft, false, true) },

        {SpecialCommandType.DefenseArea, new ButtonRecord( GamepadButtons.North, true, false) },
        {SpecialCommandType.DefenseSingle, new ButtonRecord( GamepadButtons.East, true, false) },
        {SpecialCommandType.HealArea, new ButtonRecord( GamepadButtons.South, true, false) },
        {SpecialCommandType.HealSingle, new ButtonRecord( GamepadButtons.West, true, false) },

        {SpecialCommandType.Burst, new ButtonRecord( GamepadButtons.DpadDown, true, false) },
        {SpecialCommandType.AntiKnockback, new ButtonRecord( GamepadButtons.DpadUp, true, false) },
    };

    public ButtonRecord ButtonDoAction { get; set; } = null;
}

public class OtherConfiguration
{
    public static InputConfiguration InputConfig = new InputConfiguration();
    public static SortedSet<uint> HostileCastingArea = new SortedSet<uint>();
    public static SortedSet<uint> HostileCastingTank = new SortedSet<uint>();

    public static SortedList<uint, float> AnimationLockTime = new SortedList<uint, float>();

    public static Dictionary<uint, string[]> NoHostileNames = new Dictionary<uint, string[]>();

    public static SortedSet<uint> DangerousStatus = new SortedSet<uint>();

    public static SortedSet<uint> InvincibleStatus = new SortedSet<uint>();

    public static void Init()
    {
        if (!Directory.Exists(Service.Interface.ConfigDirectory.FullName))
        {
            Directory.CreateDirectory(Service.Interface.ConfigDirectory.FullName);
        }

        Task.Run(() => InitOne(ref DangerousStatus, nameof(DangerousStatus)));

        Task.Run(() => InitOne(ref InvincibleStatus, nameof(InvincibleStatus)));

        Task.Run(() => InitOne(ref NoHostileNames, nameof(NoHostileNames)));

        Task.Run(() => InitOne(ref AnimationLockTime, nameof(AnimationLockTime)));

        Task.Run(() => InitOne(ref HostileCastingArea, nameof(HostileCastingArea)));

        Task.Run(() => InitOne(ref HostileCastingTank, nameof(HostileCastingTank)));

        Task.Run(() => InitOne(ref InputConfig, nameof(InputConfig)));
    }

    public static void Save()
    {
        SaveDangerousStatus();
        SaveInvincibleStatus();
        SaveNoHostileNames();
        SaveAnimationLockTime();
        SaveHostileCastingArea();
        SaveHostileCastingTank();
        SaveInputConfig();
    }

    public static void SaveInputConfig()
    {
        Task.Run(() => Save(InputConfig, nameof(InputConfig)));
    }

    public static void SaveHostileCastingArea()
    {
        Task.Run(() => Save(HostileCastingArea, nameof(HostileCastingArea)));
    }

    public static void SaveHostileCastingTank()
    {
        Task.Run(() => Save(HostileCastingTank, nameof(HostileCastingTank)));
    }

    public static void SaveDangerousStatus()
    {
        Task.Run(() => Save(DangerousStatus, nameof(DangerousStatus)));
    }

    public static void SaveInvincibleStatus()
    {
        Task.Run(() => Save(InvincibleStatus, nameof(InvincibleStatus)));
    }

    public static void SaveNoHostileNames()
    {
        Task.Run(() => Save(NoHostileNames, nameof(NoHostileNames)));
    }

    public static void SaveAnimationLockTime()
    {
        Task.Run(() => Save(AnimationLockTime, nameof(AnimationLockTime)));
    }

    private static string GetFilePath(string name)
    {
        var directory = Service.Interface.ConfigDirectory.FullName;
#if DEBUG
        var dir = @"E:\OneDrive - stu.zafu.edu.cn\PartTime\FFXIV\RotationSolver\Resources";
        if (Directory.Exists(dir)) directory = dir;
#endif

        return directory + $"\\{name}.json";
    }

    private static void Save<T>(T value, string name)
        => SavePath(value, GetFilePath(name));

    private static void SavePath<T>(T value, string path)
    {
        File.WriteAllTextAsync(path,
        JsonConvert.SerializeObject(value, Formatting.Indented));
    }

    private static void InitOne<T>(ref T value, string name)
    {
        var path = GetFilePath(name);
        if (File.Exists(path))
        {
            try
            {
                value = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                PluginLog.Warning(ex, $"Failed to load {name}.");
            }
        }
        else
        {
            try
            {
                var client = new HttpClient();
                var str = client.GetStringAsync($"https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Resources/{name}.json").Result;

                File.WriteAllText(path, str);
                value = JsonConvert.DeserializeObject<T>(str);
            }
            catch
            {
                SavePath(value, path);
            }
        }
    }
}
