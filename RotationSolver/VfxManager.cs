using ECommons.DalamudServices;
using ECommons.Reflection;

namespace RotationSolver;
#if DEBUG
internal static class VfxManager
{
    internal static void Remove()
    {
        _vfxSpawnRemove.Value?.Invoke(null, []);
    }

    internal static void OnGround(string path, bool canLoop)
    {
        _vfxSpawnOnGround.Value?.Invoke(null, [path, canLoop]);
    }

    private static readonly Lazy<Type?> _vfxSpawnType = new(() => GetTypeFromVfx("VfxSpawn"));
    private static readonly Lazy<Type?> _staticVfxType = new(() => GetTypeFromVfx("StaticVfx"));

    private static readonly Lazy<MethodInfo?> _vfxSpawnOnGround = new(() => GetMethodFromVfx("OnGround"));
    private static readonly Lazy<MethodInfo?> _vfxSpawnRemove = new(() => GetMethodFromVfx("Remove"));

    private static MethodInfo? GetMethodFromVfx(string method)
    {
        var value = _vfxSpawnType.Value.GetAllMethodInfo().FirstOrDefault(m => m.Name == method);
        if (value == null)
        {
            Svc.Log.Debug($"Failed to find the method {method}.");
        }
        return value;
    }

    private static Type? GetTypeFromVfx(string typeName)
    {
        var value = _vfxEditorAssembly?.Value?.GetTypes().FirstOrDefault(t => t.Name == typeName);
        if (value == null)
        {
            Svc.Log.Debug($"Failed to find the type {typeName}.");
        }
        return value;
    }

    private static readonly Lazy<Assembly?> _vfxEditorAssembly = new(() =>
    {
        DalamudReflector.TryGetDalamudPlugin("VFXEditor", out var instance);
        if (instance != null)
        {
            Svc.Log.Debug("Loaded VFX Editor successfully.");
        }
        return instance?.GetType().Assembly;
    });
}
#endif