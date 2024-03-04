using XIVPainter.Vfx;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Updaters.Omens;

internal static class OmenMain
{
    public static void Init()
    {
        CastingOmen.Init();
    }

    public static void Dispose()
    {
        CastingOmen.Dispose();
    }

    public static void Update()
    {
        CastingOmen.Update();
    }

    public static StaticVfx? CreateVfx(GameObject chara, Action action, Vector3? location, float? second)
    {
        if (!Service.Config.OmenCastingConfig.TryGetValue(action.RowId, out var config))
            Service.Config.OmenCastingConfig[action.RowId] = config = new();

        var omen = config.OmenPath ?? action.Omen.Value?.Path?.RawString;
        if (string.IsNullOrEmpty(omen)) return null;

        omen = $"vfx/omen/eff/{omen}.avfx";

        var x = config.X ?? (action.XAxisModifier > 0 ? action.XAxisModifier / 2 : action.EffectRange);
        var y = config.Y ?? action.EffectRange;
        var scale = new Vector3(x, 10, y);

        if (action.TargetArea)
        {
            if (location.HasValue)
            {
                return new(omen, location.Value, 0, scale);
            }
            else
            {
                return null;
            }
        }
        else
        {
            var rot = config.Rotation ?? 0;

            var result = new StaticVfx(omen, chara, scale)
            {
                RotateAddition = rot / 180 * MathF.PI,
            };

            if (second.HasValue)
            {
                result.DeadTime = DateTime.Now.AddSeconds(second.Value);
            }

            return result;
        }
    }
}
