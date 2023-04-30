using Dalamud.Logging;
using System.Xml.Linq;

namespace RotationSolver.Basic.Configuration;

public class OtherConfiguration
{
    public static Dictionary<uint, string[]> NoHostileNames = new Dictionary<uint, string[]>();

    public static SortedSet<uint> DangerousStatus = new SortedSet<uint>()
    {
        (uint)StatusID.Doom,
        (uint)StatusID.Amnesia,
        (uint)StatusID.Stun,
        (uint)StatusID.Stun2,
        (uint)StatusID.Sleep,
        (uint)StatusID.Sleep2,
        (uint)StatusID.Sleep3,
        (uint)StatusID.Pacification,
        (uint)StatusID.Pacification2,
        (uint)StatusID.Silence,
        (uint)StatusID.Slow,
        (uint)StatusID.Slow2,
        (uint)StatusID.Slow3,
        (uint)StatusID.Slow4,
        (uint)StatusID.Slow5,
        (uint)StatusID.Blind,
        (uint)StatusID.Blind2,
        (uint)StatusID.Blind3,
        (uint)StatusID.Paralysis,
        (uint)StatusID.Paralysis2,
        (uint)StatusID.Nightmare,
        (uint)StatusID.Necrosis,
    };

    public static SortedSet<uint> InvincibleStatus = new SortedSet<uint>()
    {
        (uint)StatusID.StoneSkin,
        (uint)StatusID.IceSpikesInvincible,
        (uint)StatusID.VortexBarrier,
    };

    public static void Init()
    {
        Task.Run(() => InitOne(ref DangerousStatus, nameof(DangerousStatus)));

        Task.Run(() => InitOne(ref InvincibleStatus, nameof(InvincibleStatus)));

        Task.Run(() => InitOne(ref NoHostileNames, nameof(NoHostileNames)));
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

    private static string GetFilePath(string name)
        => Service.Interface.ConfigDirectory.FullName + $"\\{name}.json";

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
        else SavePath(value, path);
    }
}
