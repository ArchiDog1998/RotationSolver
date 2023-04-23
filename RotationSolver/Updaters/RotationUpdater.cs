using Dalamud.Logging;
using RotationSolver.Localization;
using System;
using System.Text;

namespace RotationSolver.Updaters;

internal static class RotationUpdater
{
    public record CustomRotationGroup(ClassJobID jobId, ClassJobID[] classJobIds, ICustomRotation[] rotations);

    internal static SortedList<JobRole, CustomRotationGroup[]> CustomRotationsDict { get; private set; } = new SortedList<JobRole, CustomRotationGroup[]>();

    internal static SortedList<string, string> AuthorHashes { get; private set; } = new SortedList<string, string>();
    static CustomRotationGroup[] _customRotations { get; set; } = new CustomRotationGroup[0];

    public static async void GetAllCustomRotations(bool download, bool mustDownload)
    {
        var relayFolder = Service.Interface.ConfigDirectory.FullName;
        if (!Directory.Exists(relayFolder)) Directory.CreateDirectory(relayFolder);

        LoadRotationsFromLocal(relayFolder);

        if(download && Service.Config.DownloadRotations)await DownloadRotationsAsync(relayFolder, mustDownload);
    }

    static bool _isDownloading = false;
    private static async Task DownloadRotationsAsync(string relayFolder, bool mustDownload)
    {
        if (_isDownloading) return;
        _isDownloading= true;
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
        _isDownloading = false;
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

                using (var stream = new FileStream(filePath, File.Exists(filePath)
                    ? FileMode.Open : FileMode.CreateNew))
                {
                    await response.Content.CopyToAsync(stream);
                }
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
            var assembly = RotationHelper.LoadFrom(filePath);
            PluginLog.Log("Successfully loaded " + assembly.FullName);
            return assembly;
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
            .Where(Directory.Exists)
            //.Append(Path.GetDirectoryName(Assembly.GetAssembly(typeof(ICustomRotation)).Location))
#if DEBUG
            //.Append(relayFolder)
#else
            .Append(relayFolder)
#endif
            ;

        var assemblies = from dir in directories
                         where Directory.Exists(dir)
                         from l in Directory.GetFiles(dir, "*.dll")
                         select LoadOne(l) into a
                         where a != null
                         select a;

        AuthorHashes = new SortedList<string, string>(
            (from a in assemblies
             select (a, a.GetCustomAttribute<AuthorHashAttribute>()) into author
             where author.Item2 != null
             group author by author.Item2 into gr
             select (gr.Key.Hash, string.Join(", ", gr.Select(i => i.a.GetInfo().Author + " - " + i.a.GetInfo().Name))))
             .ToDictionary(i => i.Hash, i => i.Item2));

        _customRotations = (
            from a in assemblies
            from t in TryGetTypes(a)
            where t.GetInterfaces().Contains(typeof(ICustomRotation))
                 && !t.IsAbstract && !t.IsInterface
            select GetRotation(t) into rotation
            where rotation != null
            group rotation by rotation.JobIDs[0] into rotationGrp
            select new CustomRotationGroup(rotationGrp.Key, rotationGrp.First().JobIDs, CreateRotationSet(rotationGrp.ToArray()))).ToArray();

        CustomRotationsDict = new SortedList<JobRole, CustomRotationGroup[]>
            (_customRotations.GroupBy(g => g.rotations[0].Job.GetJobRole())
            .ToDictionary(set => set.Key, set => set.OrderBy(i => i.jobId).ToArray()));
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
            return new Type[0];
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
                        result = LocalizationManager.RightLang.Timeline_Ability;
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

    public static IAction[] RightRotationActions { get; private set; } = new IAction[0];

    public static void UpdateRotation()
    {
        var nowJob = (ClassJobID)Service.Player.ClassJob.Id;

        foreach (var group in _customRotations)
        {
            if (!group.classJobIds.Contains(nowJob)) continue;

            RightNowRotation = GetChooseRotation(group);
            RightRotationActions = RightNowRotation.AllActions;
            return;
        }
        RightNowRotation = null;
        RightRotationActions = new IAction[0];
    }

    internal static ICustomRotation GetChooseRotation(CustomRotationGroup group)
    {
        var has = Service.Config.RotationChoices.TryGetValue((uint)group.jobId, out var name);
       
        var rotation = group.rotations.FirstOrDefault(r => r.GetType().FullName == name);
        rotation ??= group.rotations.FirstOrDefault(r => r.IsAllowed(out _));
        rotation ??= group.rotations.FirstOrDefault();

        if (!has && rotation != null)
        {
            Service.Config.RotationChoices[(uint)group.jobId] = rotation.GetType().FullName;
        }
        return rotation;
    }
}
