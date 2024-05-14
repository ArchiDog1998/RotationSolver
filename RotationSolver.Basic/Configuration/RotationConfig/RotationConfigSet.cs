using ECommons.DalamudServices;
using System.Collections;

namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigSet : IRotationConfigSet
{
    public HashSet<IRotationConfig> Configs { get; } = new HashSet<IRotationConfig>(new RotationConfigComparer());

    public RotationConfigSet(ICustomRotation rotation)
    {
        foreach (var prop in rotation.GetType().GetRuntimeProperties())
        {
            var attr = prop.GetCustomAttribute<RotationConfigAttribute>();
            if (attr == null) continue;

            var type = prop.PropertyType;
            if (type == null) continue;

            if (type == typeof(bool))
            {
                Configs.Add(new RotationConfigBoolean(rotation, prop));
            }
            else if (type.IsEnum)
            {
                Configs.Add(new RotationConfigCombo(rotation, prop));
            }
            else if(type == typeof(float))
            {
                Configs.Add(new RotationConfigFloat(rotation, prop));
            }
            else if(type == typeof(int))
            {
                Configs.Add(new RotationConfigInt(rotation, prop));
            }
            else if(type == typeof(string))
            {
                Configs.Add(new RotationConfigString(rotation, prop));
            }
            else
            {
                Svc.Log.Error($"Failed to find the rotation config type {type.FullName ?? type.Name}");
            }
        }
    }

    public IEnumerator<IRotationConfig> GetEnumerator() => Configs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Configs.GetEnumerator();
}
