using ECommons.DalamudServices;

namespace RotationSolver.Basic.Configuration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA2211 
public class OtherConfiguration
{
    public static HashSet<uint> HostileCastingArea = new();
    public static HashSet<uint> HostileCastingTank = new();

    public static SortedList<uint, float> AnimationLockTime = new();

    public static Dictionary<uint, string[]> NoHostileNames = new();
    public static Dictionary<uint, string[]> NoProvokeNames = new();
    public static Dictionary<uint, Vector3[]> BeneficialPositions = new();

    public static HashSet<uint> DangerousStatus = new();

    public static HashSet<uint> InvincibleStatus = new();

    public static Dictionary<uint, byte> ActionAOECounts = new()
    {
        { (uint) ActionID.Gravity, 2},
        { (uint) ActionID.FeatherRain, 1},
        { (uint) ActionID.Eruption, 1},
        { (uint) ActionID.QuickNock, 2},
        { (uint) ActionID.ShadowBite, 2},
        { (uint) ActionID.RainOfDeath, 2},
        { (uint) ActionID.BladeShower, 2},
        { (uint) ActionID.RisingWindmill, 2},
        { (uint) ActionID.BloodShower, 2},
        { (uint) ActionID.FanDance2, 2},
        { (uint) ActionID.Unleash, 2},
        { (uint) ActionID.StalwartSoul, 2},
        { (uint) ActionID.DemonSlice, 2},
        { (uint) ActionID.DemonSlaughter, 2},
        { (uint) ActionID.SpreadShot, 2},
        { (uint) ActionID.AutoCrossbow, 2},
        { (uint) ActionID.Katon, 2},
        { (uint) ActionID.Scatter, 2},
        { (uint) ActionID.WhorlOfDeath, 2},
        { (uint) ActionID.ArtOfWar, 2},
        { (uint) ActionID.Dyskrasia, 2},
        { (uint) ActionID.Overpower, 2},
        { (uint) ActionID.MythrilTempest, 2},
        { (uint) ActionID.SteelCyclone, 2},
    };

    public static Dictionary<uint, float> ActionTTK = new()
    {
        { (uint) ActionID.Combust, 20},
        { (uint) ActionID.VenomousBite, 30},
        { (uint) ActionID.WindBite, 30},
        { (uint) ActionID.IronJaws, 30},
        { (uint) ActionID.BioBlaster, 10},
        { (uint) ActionID.TwinSnakes, 10},
        { (uint) ActionID.Demolish, 12},
        { (uint) ActionID.ShadowOfDeath, 10},
        { (uint) ActionID.Higanbana, 40},
        { (uint) ActionID.Bio, 20},
        { (uint) ActionID.EukrasianDosis, 20},
        { (uint) ActionID.Aero, 20},
    };

    public static Dictionary<uint, float> ActionHealRatio = new();

    public static RotationSolverRecord RotationSolverRecord = new();

    public static void Init()
    {
        if (!Directory.Exists(Svc.PluginInterface.ConfigDirectory.FullName))
        {
            Directory.CreateDirectory(Svc.PluginInterface.ConfigDirectory.FullName);
        }

        Task.Run(() => InitOne(ref DangerousStatus, nameof(DangerousStatus)));

        Task.Run(() => InitOne(ref InvincibleStatus, nameof(InvincibleStatus)));

        Task.Run(() => InitOne(ref NoHostileNames, nameof(NoHostileNames)));

        Task.Run(() => InitOne(ref NoProvokeNames, nameof(NoProvokeNames)));

        Task.Run(() => InitOne(ref AnimationLockTime, nameof(AnimationLockTime)));

        Task.Run(() => InitOne(ref HostileCastingArea, nameof(HostileCastingArea)));

        Task.Run(() => InitOne(ref HostileCastingTank, nameof(HostileCastingTank)));

        Task.Run(() => InitOne(ref BeneficialPositions, nameof(BeneficialPositions)));

        Task.Run(() => InitOne(ref ActionAOECounts, nameof(ActionAOECounts)));

        Task.Run(() => InitOne(ref ActionTTK, nameof(ActionTTK)));

        Task.Run(() => InitOne(ref ActionHealRatio, nameof(ActionHealRatio)));

        Task.Run(() => InitOne(ref RotationSolverRecord, nameof(RotationSolverRecord), false));
    }

    public static Task Save()
    {
        return Task.Run(async () =>
        {
            await SaveDangerousStatus();
            await SaveInvincibleStatus();
            await SaveNoHostileNames();
            await SaveAnimationLockTime();
            await SaveHostileCastingArea();
            await SaveHostileCastingTank();
            await SaveBeneficialPositions();
            await SaveRotationSolverRecord();
            await SaveNoProvokeNames();
            await SaveActionAOECounts();
            await SaveActionTTK();
            await SaveActionHealRatio();
        });
    }

    public static Task SaveActionHealRatio()
    {
        return Task.Run(() => Save(ActionHealRatio, nameof(ActionHealRatio)));
    }

    public static Task SaveActionTTK()
    {
        return Task.Run(() => Save(ActionTTK, nameof(ActionTTK)));
    }

    public static Task SaveActionAOECounts()
    {
        return Task.Run(() => Save(ActionAOECounts, nameof(ActionAOECounts)));
    }
    public static Task SaveRotationSolverRecord()
    {
        return Task.Run(() => Save(RotationSolverRecord, nameof(RotationSolverRecord)));
    }
    public static Task SaveNoProvokeNames()
    {
        return Task.Run(() => Save(NoProvokeNames, nameof(NoProvokeNames)));
    }

    public static Task SaveBeneficialPositions()
    {
        return Task.Run(() => Save(BeneficialPositions, nameof(BeneficialPositions)));
    }

    public static Task SaveHostileCastingArea()
    {
        return Task.Run(() => Save(HostileCastingArea, nameof(HostileCastingArea)));
    }

    public static Task SaveHostileCastingTank()
    {
        return Task.Run(() => Save(HostileCastingTank, nameof(HostileCastingTank)));
    }

    public static Task SaveDangerousStatus()
    {
        return Task.Run(() => Save(DangerousStatus, nameof(DangerousStatus)));
    }

    public static Task SaveInvincibleStatus()
    {
        return Task.Run(() => Save(InvincibleStatus, nameof(InvincibleStatus)));
    }

    public static Task SaveNoHostileNames()
    {
        return Task.Run(() => Save(NoHostileNames, nameof(NoHostileNames)));
    }

    public static Task SaveAnimationLockTime()
    {
        return Task.Run(() => Save(AnimationLockTime, nameof(AnimationLockTime)));
    }

    private static string GetFilePath(string name)
    {
        var directory = Svc.PluginInterface.ConfigDirectory.FullName;
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
        try
        {
            File.WriteAllText(path,
            JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.None,
            }));
        }
        catch (Exception ex)
        {
            Svc.Log.Warning(ex, $"Failed to save the file to {path}");
        }
    }

    private static void InitOne<T>(ref T value, string name, bool download = true)
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
                Svc.Log.Warning(ex, $"Failed to load {name}.");
            }
        }
        else if (download)
        {
            try
            {
                using var client = new HttpClient();
                var str = client.GetStringAsync($"https://raw.githubusercontent.com/{Service.USERNAME}/{Service.REPO}/main/Resources/{name}.json").Result;

                File.WriteAllText(path, str);
                value = JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Error,
                    Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                    {
                        args.ErrorContext.Handled = true;
                    }
                });
            }
            catch
            {
                SavePath(value, path);
            }
        }
        else
        {
            SavePath(value, path);
        }
    }
}
#pragma warning restore CA2211
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member