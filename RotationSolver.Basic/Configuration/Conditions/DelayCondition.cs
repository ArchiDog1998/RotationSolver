using ECommons.GameHelpers;

namespace RotationSolver.ActionSequencer;

internal abstract class DelayCondition : ICondition
{
    public float DelayMin = 0;
    public float DelayMax = 0;

    RandomDelay _delay = default;

    public bool IsTrue(ICustomRotation rotation)
    {
        if(_delay.GetRange == null)
        {
            _delay = new(() => (DelayMin, DelayMax));
        }

        return _delay.Delay(CheckBefore(rotation) && IsTrueInside(rotation));
    }

    public abstract bool IsTrueInside(ICustomRotation rotation);

    public virtual bool CheckBefore(ICustomRotation rotation)
    {
        return Player.Available;
    }

    internal static bool CheckBaseAction(ICustomRotation rotation, ActionID id, ref IBaseAction action)
    {
        if (id != ActionID.None && (action == null || (ActionID)action.ID != id))
        {
            action = rotation.AllBaseActions.FirstOrDefault(a => (ActionID)a.ID == id);
        }
        if (action == null) return false;
        return true;
    }

    internal static bool CheckMemberInfo<T>(ICustomRotation rotation, ref string name, ref T value) where T : MemberInfo
    {
        if (!string.IsNullOrEmpty(name) && (value == null || value.Name != name))
        {
            var memberName = name;
            if (typeof(T).IsAssignableFrom(typeof(PropertyInfo)))
            {
                value = (T)GetAllMethods(rotation.GetType(), RuntimeReflectionExtensions.GetRuntimeProperties).FirstOrDefault(m => m.Name == memberName);
            }
            else if (typeof(T).IsAssignableFrom(typeof(MethodInfo)))
            {
                value = (T)GetAllMethods(rotation.GetType(), RuntimeReflectionExtensions.GetRuntimeMethods).FirstOrDefault(m => m.Name == memberName);
            }
        }
        return true;
    }

    private static IEnumerable<MemberInfo> GetAllMethods(Type type, Func<Type, IEnumerable<MemberInfo>> getFunc)
    {
        if (type == null || getFunc == null) return Array.Empty<MemberInfo>();

        var methods = getFunc(type);
        return methods.Union(GetAllMethods(type.BaseType, getFunc));
    }
}
