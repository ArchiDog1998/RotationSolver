using ECommons.DalamudServices;
using ECommons.Reflection;
using System.Runtime.InteropServices;

namespace RotationSolver;
#if DEBUG
internal static class VfxManager
{
    internal static void Remove()
    {
        _vfxSpawnRemove.Value?.Invoke(null, []);
    }

    internal static object? OnGround(string path, bool canLoop)
    {
        _vfxSpawnOnGround.Value?.Invoke(null, [path, canLoop]);
        return _vfxObjectGetter.Value?.GetValue(null);
    }

    internal static void UpdateScale(object? obj, Vector3 scale)
    {
        if (obj == null) return;
        _vfxObjectScale.Value?.Invoke(obj, [scale]);
    }

    internal unsafe static void Test(object? obj)
    {
        if (obj == null) return;

        var field = obj.GetType().BaseType!.GetRuntimeFields().FirstOrDefault(f => f.Name == "Vfx");
        if (field == null) return;

        var vfxStruct = (IntPtr)Pointer.Unbox((Pointer)field.GetValue(obj)!) + 0x45;
        Svc.Log.Error(Marshal.ReadInt32(vfxStruct).ToString("X"));
    }

    internal static void UpdateRotation(object? obj, Vector3 scale)
    {
        if (obj == null) return;
        _vfxObjectRotation.Value?.Invoke(obj, [scale]);
    }

    private static readonly Lazy<MethodInfo?> _vfxObjectScale = new(() =>
    {
        return GetMethodFromVfxObject("UpdateScale");
    });

    private static readonly Lazy<MethodInfo?> _vfxObjectRotation = new(() =>
    {
        return GetMethodFromVfxObject("UpdateRotation");
    });

    private static MethodInfo? GetMethodFromVfxObject(string name)
    {
        return _vfxObjectGetter?.Value?.GetValue(null)?.GetType().GetAllMethodInfo().FirstOrDefault(m => m.Name == name);
    }

    private static readonly Lazy<PropertyInfo?> _vfxObjectGetter = new(() =>
    {
        return _vfxSpawnType?.Value.GetAllPropertyInfo().FirstOrDefault(p => p.Name == "Vfx");
    });
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