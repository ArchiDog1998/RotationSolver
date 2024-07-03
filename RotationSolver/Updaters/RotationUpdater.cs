using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Rotations.Duties;
using RotationSolver.Data;
using RotationSolver.Helpers;
using XIVConfigUI;

namespace RotationSolver.Updaters;

internal static class RotationUpdater
{
    internal record CustomRotationGroup(Job JobId, Job[] ClassJobIds, Type[] Rotations);
    internal static SortedList<JobRole, CustomRotationGroup[]> CustomRotationsDict { get; private set; } = [];

    internal static CustomRotationGroup[] CustomRotations { get; set; } = [];
    internal static SortedList<uint, Type[]> DutyRotations { get; set; } = [];

    public static IAction[] RightRotationActions { get; private set; } = [];

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

            if (option.HasFlag(DownloadOption.Download) && Service.Config.DownloadRotations)
                await DownloadRotationsAsync(relayFolder, option.HasFlag(DownloadOption.MustDownload));

            if (option.HasFlag(DownloadOption.ShowList))
            {
                var assemblies = CustomRotationsDict
                    .SelectMany(d => d.Value)
                    .SelectMany(g => g.Rotations)
                    .Select(r => r.Assembly.FullName ?? string.Empty)
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
        var directories = Service.Config.OtherLibs
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

                    if (assembly != null && !assemblies.Any(a => a.FullName == assembly.FullName))
                    {
                        assemblies.Add(assembly);
                    }
                }
            }
        }

        DataCenter.AuthorHashes = [];
        foreach (var assembly in assemblies)
        {
            try
            {
                var authorHashAttribute = assembly.GetCustomAttribute<AuthorHashAttribute>();
                if (authorHashAttribute != null)
                {
                    var key = authorHashAttribute.Hash;
                    if (string.IsNullOrEmpty(key)) continue;

                    var value = $"{assembly.GetInfo().Author} - {assembly.GetInfo().Name}";

                    if (DataCenter.AuthorHashes.ContainsKey(key))
                    {
                        DataCenter.AuthorHashes[key] += $", {value}";
                    }
                    else
                    {
                        DataCenter.AuthorHashes.Add(key, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Svc.Log.Warning(ex, "Failed to get author's hash");
            }
        }

        DutyRotations = LoadDutyRotationGroup(assemblies);
        CustomRotations = LoadCustomRotationGroup(assemblies);
        var customRotationsGroupedByJobRole = new Dictionary<JobRole, List<CustomRotationGroup>>();
        foreach (var customRotationGroup in CustomRotations)
        {
            var job = customRotationGroup.Rotations[0].GetType().GetCustomAttribute<JobsAttribute>()?.Jobs[0] ?? Job.ADV;

            var jobRole = Svc.Data.GetExcelSheet<ClassJob>()!.GetRow((uint)job)!.GetJobRole();
            if (!customRotationsGroupedByJobRole.TryGetValue(jobRole, out var value))
            {
                value = [];
                customRotationsGroupedByJobRole[jobRole] = value;
            }

            value.Add(customRotationGroup);
        }

        CustomRotationsDict = [];
        foreach (var jobRole in customRotationsGroupedByJobRole.Keys)
        {
            var customRotationGroups = customRotationsGroupedByJobRole[jobRole];
            var sortedCustomRotationGroups = customRotationGroups.OrderBy(crg => crg.JobId).ToArray();
            CustomRotationsDict[jobRole] = sortedCustomRotationGroups;
        }
    }

    private static SortedList<uint, Type[]> LoadDutyRotationGroup(List<Assembly> assemblies)
    {
        var rotationList = new List<Type>();
        foreach (var assembly in assemblies)
        {
            foreach (var type in TryGetTypes(assembly))
            {
                if (type.IsAssignableTo(typeof(DutyRotation))
                    && !type.IsAbstract && type.GetConstructor([]) != null)
                {
                    rotationList.Add(type);
                }
            }
        }

        var result = new Dictionary<uint, List<Type>>();
        foreach (var type in rotationList)
        {
            var territories = type.GetCustomAttribute<DutyTerritoryAttribute>()?.TerritoryIds ?? [];

            foreach (var id in territories)
            {
                if (result.TryGetValue(id, out var list))
                {
                    list.Add(type);
                }
                else
                {
                    result[id] = [type];
                }
            }
        }

        return new(result.ToDictionary(i => i.Key, i => i.Value.ToArray()));
    }

    private static CustomRotationGroup[] LoadCustomRotationGroup(List<Assembly> assemblies)
    {
        var rotationList = new List<Type>();
        foreach (var assembly in assemblies)
        {
            foreach (var type in TryGetTypes(assembly))
            {
                if (type.GetInterfaces().Contains(typeof(ICustomRotation))
                    && !type.IsAbstract && !type.IsInterface && type.GetConstructor([]) != null)
                {
                    rotationList.Add(type);
                }
            }
        }

        var rotationGroups = new Dictionary<Job, List<Type>>();
        foreach (var rotation in rotationList)
        {
            var attr = rotation.GetCustomAttribute<JobsAttribute>();
            if (attr == null) continue;

            var jobId = attr.Jobs[0];
            if (!rotationGroups.TryGetValue(jobId, out var value))
            {
                value = [];
                rotationGroups.Add(jobId, value);
            }

            value.Add(rotation);
        }

        var result = new List<CustomRotationGroup>();
        foreach (var kvp in rotationGroups)
        {
            var jobId = kvp.Key;
            var rotations = kvp.Value.ToArray();

            result.Add(new CustomRotationGroup(jobId, rotations[0].GetCustomAttribute<JobsAttribute>()!.Jobs,
                rotations));
        }

        return [.. result];
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

        var GitHubLinks = Service.Config.GitHubLibs.Union(DownloadHelper.LinkLibraries ?? []);

        using (var client = new HttpClient())
        {
            foreach (var url in Service.Config.OtherLibs.Union(GitHubLinks.Select(Convert)))
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
            if (!Service.Config.AutoUpdateRotations && File.Exists(filePath)) return false;

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

    private static void PrintLoadedAssemblies(IEnumerable<string>? assemblies)
    {
        if (assemblies == null) return;

        foreach (var assembly in assemblies)
        {
            Svc.Chat.Print("Loaded: " + assembly);
        }
    }

    private static Assembly? LoadOne(string filePath)
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

        var dirs = Service.Config.OtherLibs;

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

    public static Type[] TryGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (Exception ex)
        {
            Svc.Log.Warning(ex, $"Failed to load the types from {assembly.FullName}");
            return [];
        }
    }

    public static IEnumerable<IGrouping<string, IAction>>? AllGroupedActions
        => GroupActions([
            .. DataCenter.RightNowRotation?.AllActions ?? [],
            .. DataCenter.RightNowDutyRotation?.AllActions ?? []]);

    public static IEnumerable<IGrouping<string, IAction>>? GroupActions(IEnumerable<IAction> actions)
       => actions?.GroupBy(a =>
       {
           if (a is IBaseAction act)
           {
               if (!act.Info.IsOnSlot) return string.Empty;

               string result;

               if (act.Action.ActionCategory.Row is 10 or 11)
               {
                   return "System Action";
               }
               else if (act.Action.IsRoleAction)
               {
                   return "Role Action";
               }
               else if (act.Info.IsLimitBreak)
               {
                   return "Limit Break";
               }
               else if (act.Info.IsDutyAction)
               {
                   return "Duty Action";
               }

               if (act.Info.IsRealGCD)
               {
                   result = "GCD";
               }
               else
               {
                   result = UiString.ActionAbility.Local(); 
               }

               if (act.Setting.IsFriendly)
               {
                   result += "-" + UiString.ActionFriendly.Local();
               }
               else
               {
                   result += "-" + UiString.ActionAttack.Local();
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
        UpdateCustomRotation();
        UpdateDutyRotation();
        RightRotationActions = ActionGroup.AllActions;
    }

    private static void UpdateDutyRotation()
    {
        if (!DutyRotations.TryGetValue(Svc.ClientState.TerritoryType, out var rotations)) return;

        var name = Service.Config.DutyRotationChoice ?? string.Empty;
        var type = GetChosenType(rotations, name);
        if (type != DataCenter.RightNowDutyRotation?.GetType())
        {
            DataCenter.RightNowDutyRotation?.Dispose();
            DataCenter.RightNowDutyRotation = GetRotation(type);
            if (type != null)
            {
                Service.Config.DutyRotationChoice = type.FullName ?? string.Empty;
            }
        }

        static DutyRotation? GetRotation(Type? t)
        {
            if (t == null) return null;
            try
            {
                return (DutyRotation?)Activator.CreateInstance(t);
            }
            catch (Exception ex)
            {
                Svc.Log.Error(ex, $"Failed to create the rotation: {t.Name}");
                return null;
            }
        }
    }

    private static void UpdateCustomRotation()
    {
        var nowJob = (Job)Player.Object.ClassJob.Id;

        foreach (var group in CustomRotations)
        {
            if (!group.ClassJobIds.Contains(nowJob)) continue;

            var rotation = GetChosenRotation(group);
            if (rotation != DataCenter.RightNowRotation?.GetType())
            {
                DataCenter.RightNowRotation?.Dispose();
                DataCenter.RightNowRotation = GetRotation(rotation);
            }
            return;
        }

        CustomRotation.MoveTarget = null;
        DataCenter.RightNowRotation?.Dispose();
        DataCenter.RightNowRotation = null;

        static ICustomRotation? GetRotation(Type? t)
        {
            if (t == null) return null;
            try
            {
                return (ICustomRotation?)Activator.CreateInstance(t);
            }
            catch (Exception ex)
            {
                Svc.Log.Error(ex, $"Failed to create the rotation: {t.Name}");
                return null;
            }
        }

        static Type? GetChosenRotation(CustomRotationGroup group)
        {
            var isPvP = DataCenter.IsPvP;

            var rotations = group.Rotations
                .Where(r =>
                {
                    var rot = r.GetCustomAttribute<RotationAttribute>();
                    if (rot == null) return false;
                    var type = rot.Type;

                    return isPvP ? type.HasFlag(CombatType.PvP) : type.HasFlag(CombatType.PvE);
                });

            var name = isPvP ? Service.Config.PvPRotationChoice : Service.Config.RotationChoice;
            return GetChosenType(rotations, name);
        }
    }

    private static Type? GetChosenType(IEnumerable<Type> types, string name)
    {
        var rotation = types.FirstOrDefault(r => r.FullName == name);

        rotation ??= types.FirstOrDefault(r => r.Assembly.FullName!.Contains("DefaultRotations", StringComparison.OrdinalIgnoreCase));

        rotation ??= types.FirstOrDefault();

        return rotation;
    }
}
