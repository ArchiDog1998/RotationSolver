using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using RotationSolver.Basic.Configuration;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;

namespace RotationSolver.Updaters;

internal static class RotationUpdater
{
    internal static SortedList<JobRole, CustomRotationGroup[]> CustomRotationsDict { get; private set; } = new SortedList<JobRole, CustomRotationGroup[]>();

    internal static SortedList<string, string> AuthorHashes { get; private set; } = new SortedList<string, string>();
    internal static CustomRotationGroup[] CustomRotations { get; set; } = Array.Empty<CustomRotationGroup>();

    public static IAction[] RightRotationActions { get; private set; } = Array.Empty<IAction>();

    private static DateTime LastRunTime;

    static bool _isLoading = false;

    /// <summary>
    /// Retrieves custom rotations from local and/or downloads
    /// them from remote server based on DownloadOption
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public static async Task GetAllCustomRotationsAsync(DownloadOption option)
    {
        if (_isLoading) return;
        _isLoading = true;

        try
        {
            var relayFolder = Svc.PluginInterface.ConfigDirectory.FullName + "\\Rotations";
            Directory.CreateDirectory(relayFolder);

            if (option.HasFlag(DownloadOption.Local))
            {
                LoadRotationsFromLocal(relayFolder);
            }

            if (option.HasFlag(DownloadOption.Download) && Service.Config.GetValue(PluginConfigBool.DownloadRotations))
                await DownloadRotationsAsync(relayFolder, option.HasFlag(DownloadOption.MustDownload));

            if (option.HasFlag(DownloadOption.ShowList))
            {
                var assemblies = CustomRotationsDict
                    .SelectMany(d => d.Value)
                    .SelectMany(g => g.Rotations)
                    .Select(r => r.GetType().Assembly.FullName)
                    .Distinct()
                    .ToList();

                PrintLoadedAssemblies(assemblies);
            }
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Failed to get custom rotations");
        }
        finally
        {
            _isLoading = false;
        }
    }

    /// <summary>
    /// This method loads custom rotation groups from local directories and assemblies, creates a sorted list of
    /// author hashes, and creates a sorted list of custom rotations grouped by job role.
    /// </summary>
    /// <param name="relayFolder"></param>
    private static void LoadRotationsFromLocal(string relayFolder)
    {
        var directories = Service.Config.GlobalConfig.OtherLibs
            .Append(relayFolder)
            .Where(Directory.Exists);

        var assemblies = new List<Assembly>();

        foreach (var dir in directories)
        {
            if (Directory.Exists(dir))
            {
                foreach (var dll in Directory.GetFiles(dir, "*.dll"))
                {
                    var assembly = LoadOne(dll);

                    if (assembly != null)
                    {
                        assemblies.Add(assembly);
                    }
                }
            }
        }

        AuthorHashes = new SortedList<string, string>();
        foreach (var assembly in assemblies)
        {
            try
            {
                var authorHashAttribute = assembly.GetCustomAttribute<AuthorHashAttribute>();
                if (authorHashAttribute != null)
                {
                    var key = authorHashAttribute.Hash;
                    var value = $"{assembly.GetInfo().Author} - {assembly.GetInfo().Name}";

                    if (AuthorHashes.ContainsKey(key))
                    {
                        AuthorHashes[key] += $", {value}";
                    }
                    else
                    {
                        AuthorHashes.Add(key, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, "Failed to get author's hash");
            }
        }

        CustomRotations = LoadCustomRotationGroup(assemblies);
        var customRotationsGroupedByJobRole = new Dictionary<JobRole, List<CustomRotationGroup>>();
        foreach (var customRotationGroup in CustomRotations)
        {
            var jobRole = customRotationGroup.Rotations[0].ClassJob.GetJobRole();
            if (!customRotationsGroupedByJobRole.ContainsKey(jobRole))
            {
                customRotationsGroupedByJobRole[jobRole] = new List<CustomRotationGroup>();
            }
            customRotationsGroupedByJobRole[jobRole].Add(customRotationGroup);
        }

        CustomRotationsDict = new SortedList<JobRole, CustomRotationGroup[]>();
        foreach (var jobRole in customRotationsGroupedByJobRole.Keys)
        {
            var customRotationGroups = customRotationsGroupedByJobRole[jobRole];
            var sortedCustomRotationGroups = customRotationGroups.OrderBy(crg => crg.JobId).ToArray();
            CustomRotationsDict[jobRole] = sortedCustomRotationGroups;
        }
    }

    private static CustomRotationGroup[] LoadCustomRotationGroup(List<Assembly> assemblies)
    {
        var rotationList = new List<ICustomRotation>();
        foreach (var assembly in assemblies)
        {
            foreach (var type in TryGetTypes(assembly))
            {
                if (type.GetInterfaces().Contains(typeof(ICustomRotation))
                    && !type.IsAbstract && !type.IsInterface)
                {
                    var rotation = GetRotation(type);
                    if (rotation != null)
                    {
                        rotationList.Add(rotation);
                    }
                }
            }
        }

        var rotationGroups = new Dictionary<Job, List<ICustomRotation>>();
        foreach (var rotation in rotationList)
        {
            var jobId = rotation.Jobs[0];
            if (!rotationGroups.ContainsKey(jobId))
            {
                rotationGroups.Add(jobId, new List<ICustomRotation>());
            }
            rotationGroups[jobId].Add(rotation);
        }

        var result = new List<CustomRotationGroup>();
        foreach (var kvp in rotationGroups)
        {
            var jobId = kvp.Key;
            var rotations = kvp.Value.ToArray();
            result.Add(new CustomRotationGroup(jobId, rotations[0].Jobs, CreateRotationSet(rotations)));
        }


        return result.ToArray();
    }


    /// <summary>
    /// Downloads rotation files from a remote server and saves them to a local folder.
    /// The download list is obtained from a JSON file on the remote server.
    /// If mustDownload is set to true, it will always download the files, otherwise it will only download if the file doesn't exist locally. 
    /// </summary>
    /// <param name="relayFolder"></param>
    /// <param name="mustDownload"></param>
    /// <returns></returns>
    private static async Task DownloadRotationsAsync(string relayFolder, bool mustDownload)
    {
        // Code to download rotations from remote server
        bool hasDownload = false;

        var GitHubLinks = Service.Config.GlobalConfig.GitHubLibs.Union(DownloadHelper.LinkLibraries ?? Array.Empty<string>());

        using (var client = new HttpClient())
        {
            foreach (var url in Service.Config.GlobalConfig.OtherLibs.Union(GitHubLinks.Select(Convert)))
            {
                hasDownload |= await DownloadOneUrlAsync(url, relayFolder, client, mustDownload);
                var pdbUrl = Path.ChangeExtension(url, ".pdb");
                await DownloadOneUrlAsync(pdbUrl, relayFolder, client, mustDownload);
            }
        }
        if (hasDownload) LoadRotationsFromLocal(relayFolder);
    }

    private static string Convert(string value)
    {
        var split = value.Split('|');
        if (split == null || split.Length < 2) return value;
        var username = split[0];
        var repo = split[1];
        var file = split.Last();
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(repo) || string.IsNullOrEmpty(file)) return value;
        return $"https://GitHub.com/{username}/{repo}/releases/latest/download/{file}.dll";
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
            if (!Service.Config.GetValue(PluginConfigBool.AutoUpdateRotations) && File.Exists(filePath)) return false;

            //Download
            using (HttpResponseMessage response = await client.GetAsync(url))
            {
                if (File.Exists(filePath) && !mustDownload)
                {
                    var fileInfo = new FileInfo(filePath);
                    var header = response.Content.Headers;
                    if (header.LastModified.HasValue && header.LastModified.Value.UtcDateTime < fileInfo.LastWriteTimeUtc
                        && fileInfo.Length == header.ContentLength)
                    {
                        return false;
                    }
                    File.Delete(filePath);
                }

                using var stream = new FileStream(filePath, File.Exists(filePath)
                    ? FileMode.Open : FileMode.CreateNew);
                await response.Content.CopyToAsync(stream);
            }

            Svc.Log.Information($"Successfully download the {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, $"failed to download from {url}");
        }
        return false;
    }

    private static void PrintLoadedAssemblies(IEnumerable<string> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            Svc.Chat.Print("Loaded: " + assembly);
        }
    }

    private static Assembly LoadOne(string filePath)
    {
        try
        {
            return RotationHelper.LoadCustomRotationAssembly(filePath);
        }
        catch (Exception ex)
        {
            Svc.Log.Warning(ex, "Failed to load " + filePath);
        }
        return null;
    }

    // This method watches for changes in local rotation files by checking the
    // last modified time of the files in the directories specified in the configuration.
    // If there are new changes, it triggers a reload of the custom rotation.
    // This method uses Parallel.ForEach to improve performance.
    // It also has a check to ensure it's not running too frequently, to avoid hurting the FPS of the game.
    public static void LocalRotationWatcher()
    {
        if (DateTime.Now < LastRunTime.AddSeconds(2))
        {
            return;
        }

        var dirs = Service.Config.GlobalConfig.OtherLibs;

        foreach (var dir in dirs)
        {
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                continue;
            }

            var dlls = Directory.GetFiles(dir, "*.dll");

            // There may be many files in these directories,
            // so we opt to use Parallel.ForEach for performance.
            Parallel.ForEach(dlls, async dll =>
            {
                var loadedAssembly = new LoadedAssembly(
                    dll,
                    File.GetLastWriteTimeUtc(dll).ToString());

                int index = RotationHelper.LoadedCustomRotations.FindIndex(item => item.LastModified == loadedAssembly.LastModified);

                if (index == -1)
                {
                    await GetAllCustomRotationsAsync(DownloadOption.Local);
                }
            });
        }

        LastRunTime = DateTime.Now;
    }

    private static Type[] TryGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (Exception ex)
        {
            Svc.Log.Warning(ex, $"Failed to load the types from {assembly.FullName}");
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
            Svc.Log.Error(ex, $"Failed to load the rotation: {t.Name}");
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

    public static IEnumerable<IGrouping<string, IAction>> AllGroupedActions
        => GroupActions(DataCenter.RightNowRotation?.AllActions);

    public static IEnumerable<IGrouping<string, IAction>> GroupActions(IEnumerable<IAction> actions)
       => actions?.GroupBy(a =>
       {
           if (a is IBaseAction act)
           {
               if (!act.IsOnSlot) return string.Empty;

               string result;

               if (act.IsLimitBreak)
               {
                   return "Limit Break";
               }

               if (act.IsDutyAction)
               {
                   return "Duty Action";
               }

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

       }).Where(g => !string.IsNullOrEmpty(g.Key)).OrderBy(g => g.Key);

    public static void UpdateRotation()
    {
        var nowJob = (Job)Player.Object.ClassJob.Id;

        foreach (var group in CustomRotations)
        {
            if (!group.ClassJobIds.Contains(nowJob)) continue;

            var rotation = GetChosenRotation(group);
            if (rotation != DataCenter.RightNowRotation)
            {
                rotation?.OnTerritoryChanged();
            }
            DataCenter.RightNowRotation = rotation;
            RightRotationActions = DataCenter.RightNowRotation?.AllActions ?? Array.Empty<IAction>();
            DataCenter.Job = DataCenter.RightNowRotation?.Jobs[0] ?? Job.ADV;
            return;
        }

        CustomRotation.MoveTarget = null;
        DataCenter.RightNowRotation = null;
        RightRotationActions = Array.Empty<IAction>();
        DataCenter.Job = DataCenter.RightNowRotation?.Jobs[0] ?? Job.ADV;
    }

    internal static ICustomRotation GetChosenRotation(CustomRotationGroup group)
    {
        var isPvP = DataCenter.Territory?.IsPvpZone ?? false;

        var rotations =  group.Rotations.Where(r => isPvP ? r.Type.HasFlag(CombatType.PvP)
        : r.Type.HasFlag(CombatType.PvE));
        var name = isPvP ? Service.Config.GetJobConfig(group.JobId).PvPRotationChoice
            : Service.Config.GetJobConfig(group.JobId).RotationChoice;

        var rotation = rotations.FirstOrDefault(r => r.GetType().FullName == name);
        rotation ??= rotations.FirstOrDefault(r => r.GetType().Assembly.FullName.Contains("SupportersRotations", StringComparison.OrdinalIgnoreCase));

        rotation ??= rotations.FirstOrDefault(r => r.GetType().Assembly.FullName.Contains("DefaultRotations", StringComparison.OrdinalIgnoreCase));

        rotation ??= rotations.FirstOrDefault();

        return rotation;
    }
}
