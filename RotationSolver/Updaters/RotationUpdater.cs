using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;

using RotationSolver.Localization;
using System.Text;

namespace RotationSolver.Updaters;

internal static class RotationUpdater
{
    public record CustomRotationGroup(ClassJobID JobId, ClassJobID[] ClassJobIds, ICustomRotation[] Rotations);

    internal static SortedList<JobRole, CustomRotationGroup[]> CustomRotationsDict { get; private set; } = new SortedList<JobRole, CustomRotationGroup[]>();

    internal static SortedList<string, string> AuthorHashes { get; private set; } = new SortedList<string, string>();
    static CustomRotationGroup[] CustomRotations { get; set; } = Array.Empty<CustomRotationGroup>();

    //public static List<string> LoadedPlugins = new List<string>();

    [Flags]
    public enum DownloadOption : byte
    {
        Local = 0,
        Donwload = 1 << 0,
        MustDownload = Donwload | 1 << 1,
        ShowList = 1 << 2,
    }

    static bool _isLoading = false;

    public static void GetAllCustomRotations(DownloadOption option)
    {
        if (_isLoading) return;

        Task.Run(async () =>
        {
            _isLoading = true;

            var relayFolder = Service.Interface.ConfigDirectory.FullName + "\\Rotations";
            if (!Directory.Exists(relayFolder)) Directory.CreateDirectory(relayFolder);

            LoadRotationsFromLocal(relayFolder);

            if (option.HasFlag(DownloadOption.Donwload) && Service.Config.DownloadRotations)
                await DownloadRotationsAsync(relayFolder, option.HasFlag(DownloadOption.MustDownload));

            if (option.HasFlag(DownloadOption.ShowList))
            {
                foreach (var item in CustomRotationsDict
                .SelectMany(d => d.Value)
                .SelectMany(g => g.Rotations)
                .Select(r => r.GetType().Assembly.FullName).ToHashSet())
                {
                    Service.ChatGui.Print("Loaded: " + item);
                }
            }
            _isLoading = false;
        });
    }

    private static async Task DownloadRotationsAsync(string relayFolder, bool mustDownload)
    {
        bool hasDownload = false;
        using (var client = new HttpClient())
        {
            IEnumerable<string> libs = Service.Config.OtherLibs;
            try
            {
                var bts = await client.GetByteArrayAsync("https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Resources/downloadList.json");
                libs = libs.Union(JsonConvert.DeserializeObject<string[]>(Encoding.Default.GetString(bts)));
            }
            catch (Exception ex)
            {
                PluginLog.Log(ex, "Failed to load downloading List.");
            }

            foreach (var url in libs)
            {
                hasDownload |= await DownloadOneUrlAsync(url, relayFolder, client, mustDownload);
                var pdbUrl = Path.ChangeExtension(url, ".pdb");
                await DownloadOneUrlAsync(pdbUrl, relayFolder, client, mustDownload);
            }
        }
        if (hasDownload) LoadRotationsFromLocal(relayFolder);
    }

    private static async Task<bool> DownloadOneUrlAsync(string url, string relayFolder, HttpClient client, bool mustDownload)
    {
        try
        {
            var valid = Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uriResult)
                 && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (!valid) return false;
        }
        catch
        {
            return false;
        }
        try
        {
            var fileName = url.Split('/').LastOrDefault();
            if (string.IsNullOrEmpty(fileName)) return false;
            //if (Path.GetExtension(fileName) != ".dll") continue;
            var filePath = Path.Combine(relayFolder, fileName);
            if (!Service.Config.AutoUpdateRotations && File.Exists(filePath)) return false;

            //Download
            using (HttpResponseMessage response = await client.GetAsync(url))
            {
                if (File.Exists(filePath) && !mustDownload)
                {
                    if (new FileInfo(filePath).Length == response.Content.Headers.ContentLength)
                    {
                        return false;
                    }
                    File.Delete(filePath);
                }

                using var stream = new FileStream(filePath, File.Exists(filePath)
                    ? FileMode.Open : FileMode.CreateNew);
                await response.Content.CopyToAsync(stream);
            }

            PluginLog.Log($"Successfully download the {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            PluginLog.LogError(ex, $"failed to download from {url}");
        }
        return false;
    }

    private static Assembly LoadOne(string filePath)
    {
        try
        {
            return RotationHelper.LoadFrom(filePath);
        }
        catch (Exception ex)
        {
            PluginLog.Log(ex, "Failed to load " + filePath);
        }
        return null;
    }

    private static void LoadRotationsFromLocal(string relayFolder)
    {
        var directories = Service.Config.OtherLibs
            .Append(relayFolder)
            .Where(Directory.Exists);

        var assemblies = new List<Assembly>();

        foreach (var dir in directories)
        {
            if (Directory.Exists(dir))
            {
                var dlls = Directory.GetFiles(dir, "*.dll");
                foreach (var dll in dlls)
                {
                    var assembly = LoadOne(dll);

                    if (assembly != null)
                    {
                        assemblies.Add(assembly);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        AuthorHashes = new SortedList<string, string>(
            (from a in assemblies
             select (a, a.GetCustomAttribute<AuthorHashAttribute>()) into author
             where author.Item2 != null
             group author by author.Item2 into gr
             select (gr.Key.Hash, string.Join(", ", gr.Select(i => i.a.GetInfo().Author + " - " + i.a.GetInfo().Name))))
             .ToDictionary(i => i.Hash, i => i.Item2));

        CustomRotations = (
            from a in assemblies
            from t in TryGetTypes(a)
            where t.GetInterfaces().Contains(typeof(ICustomRotation))
                 && !t.IsAbstract && !t.IsInterface
            select GetRotation(t) into rotation
            where rotation != null
            group rotation by rotation.JobIDs[0] into rotationGrp
            select new CustomRotationGroup(rotationGrp.Key, rotationGrp.First().JobIDs, CreateRotationSet(rotationGrp.ToArray()))).ToArray();

        CustomRotationsDict = new SortedList<JobRole, CustomRotationGroup[]>
            (CustomRotations.GroupBy(g => g.Rotations[0].Job.GetJobRole())
            .ToDictionary(set => set.Key, set => set.OrderBy(i => i.JobId).ToArray()));
    }

    private static DateTime LastRunTime;
    public static void LocalRotationWatcher()
    {
        // This will cripple FPS is run on every frame.
        if (DateTime.Now < LastRunTime.AddSeconds(2)) return;

        var dirs = Service.Config.OtherLibs;

        foreach (var dir in dirs)
        {
            if (string.IsNullOrWhiteSpace(dir)) continue;
            var dlls = Directory.GetFiles(dir, "*.dll");
            
            foreach (var dll in dlls)
            {
                var loaded = new LoadedAssembly();
                loaded.Path = dll;
                loaded.LastModified = File.GetLastWriteTimeUtc(dll).ToString();

                int index = RotationHelper.LoadedCustomRotations.FindIndex(item => item.LastModified == loaded.LastModified);

                if (index == -1)
                {
                    GetAllCustomRotations(DownloadOption.Local);
                }
            }
        }

        LastRunTime = DateTime.Now;
    }

    private static Type[] TryGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch(Exception ex)
        {
            PluginLog.Warning(ex, $"Failed to load the types from {assembly.FullName}");
            return Array.Empty<Type>();
        }
    }

    private static ICustomRotation GetRotation(Type t)
    {
        try
        {
            return (ICustomRotation)Activator.CreateInstance(t);
        }
        catch (Exception ex) 
        {
            PluginLog.LogError(ex, $"Failed to load the rotation: {t.Name}");
            return null; 
        }
    }

    private static ICustomRotation[] CreateRotationSet(ICustomRotation[] combos)
    {
        var result = new List<ICustomRotation>(combos.Length);

        foreach (var combo in combos)
        {
            if (!result.Any(c => c.RotationName == combo.RotationName))
            {
                result.Add(combo);
            }
        }
        return result.ToArray();
    }

    public static ICustomRotation RightNowRotation { get; private set; }

    public static IEnumerable<IGrouping<string, IAction>> AllGroupedActions
        => RightNowRotation?.AllActions.GroupBy(a =>
            {
                if (a is IBaseAction act)
                {
                    string result;

                    if (act.IsRealGCD)
                    {
                        result = "GCD";
                    }
                    else
                    {
                        result = LocalizationManager.RightLang.Action_Ability;
                    }

                    if (act.IsFriendly)
                    {
                        result += "-" + LocalizationManager.RightLang.Action_Friendly;
                        if (act.IsEot)
                        {
                            result += "-Hot";
                        }
                    }
                    else
                    {
                        result += "-" + LocalizationManager.RightLang.Action_Attack;

                        if (act.IsEot)
                        {
                            result += "-Dot";
                        }
                    }
                    return result;
                }
                else if (a is IBaseItem)
                {
                    return "Item";
                }
                return string.Empty;

            }).OrderBy(g => g.Key);

    public static IAction[] RightRotationActions { get; private set; } = Array.Empty<IAction>();

    public static void UpdateRotation()
    {
        var nowJob = (ClassJobID)Service.Player.ClassJob.Id;

        foreach (var group in CustomRotations)
        {
            if (!group.ClassJobIds.Contains(nowJob)) continue;

            var rotation = GetChooseRotation(group);
            if (rotation != RightNowRotation)
            {
                rotation?.OnTerritoryChanged();
            }
            RightNowRotation = rotation;
            RightRotationActions = RightNowRotation.AllActions;
            return;
        }
        RightNowRotation = null;
        RightRotationActions = Array.Empty<IAction>();
    }

    internal static ICustomRotation GetChooseRotation(CustomRotationGroup group)
    {
        var has = Service.Config.RotationChoices.TryGetValue((uint)group.JobId, out var name);
       
        var rotation = group.Rotations.FirstOrDefault(r => r.GetType().FullName == name);
        rotation ??= group.Rotations.FirstOrDefault(r => r.IsAllowed(out _));
        rotation ??= group.Rotations.FirstOrDefault();

        if (!has && rotation != null)
        {
            Service.Config.RotationChoices[(uint)group.JobId] = rotation.GetType().FullName;
        }
        return rotation;
    }
}
