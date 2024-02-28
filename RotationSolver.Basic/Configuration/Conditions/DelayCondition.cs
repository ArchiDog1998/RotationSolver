using ECommons.GameHelpers;

namespace RotationSolver.Basic.Configuration.Conditions;

internal abstract class DelayCondition : ICondition
{
    public float DelayMin = 0;
    public float DelayMax = 0;
    public float DelayOffset = 0;

    RandomDelay _delay = default;
    OffsetDelay _offsetDelay = default;

    public bool Not = false;

    [ThreadStatic]
    private static Stack<ICondition>? _callingStack;

    public bool IsTrue(ICustomRotation? rotation)
    {
        if (rotation == null) return false;

        _callingStack ??= new(64);

        if (_callingStack.Contains(this))
        {
            //Do something for recursion!
            return false;
        }

        if (_delay.GetRange == null)
        {
            _delay = new(() => (DelayMin, DelayMax));
        }

        if(_offsetDelay.GetDelay == null)
        {
            _offsetDelay = new(() => DelayOffset);
        }

        _callingStack.Push(this);
        var value = CheckBefore(rotation) && IsTrueInside(rotation);
        if (Not)
        {
            value = !value;
        }
        var result = _delay.Delay(_offsetDelay.Delay(value));
        _callingStack.Pop();

        return result;
    }

    protected abstract bool IsTrueInside(ICustomRotation rotation);

    public virtual bool CheckBefore(ICustomRotation rotation)
    {
        return Player.Available;
    }

    internal static bool CheckBaseAction(ICustomRotation rotation, ActionID id, ref IBaseAction? action)
    {
        if (id != ActionID.None && (action == null || (ActionID)action.ID != id))
        {
            action = rotation.AllBaseActions.FirstOrDefault(a => (ActionID)a.ID == id);
        }
        if (action == null) return false;
        return true;
    }

    internal static bool CheckMemberInfo<T>(ICustomRotation? rotation, ref string name, ref T? value) where T : MemberInfo
    {
        if (rotation == null) return false;

        if (!string.IsNullOrEmpty(name) && (value == null || value.Name != name))
        {
            var memberName = name;
            if (typeof(T).IsAssignableFrom(typeof(PropertyInfo)))
            {
                value = (T?)GetAllMembers(rotation.GetType(), RuntimeReflectionExtensions.GetRuntimeProperties).FirstOrDefault(m => m.Name == memberName);
            }
            else if (typeof(T).IsAssignableFrom(typeof(MethodInfo)))
            {
                value = (T?)GetAllMembers(rotation.GetType(), RuntimeReflectionExtensions.GetRuntimeMethods).FirstOrDefault(m => m.Name == memberName);
            }
        }
        return true;
    }

    private static IEnumerable<MemberInfo> GetAllMembers(Type? type, Func<Type, IEnumerable<MemberInfo>> getFunc)
    {
        if (type == null || getFunc == null) return Array.Empty<MemberInfo>();

        var methods = getFunc(type);
        return methods.Union(GetAllMembers(type.BaseType, getFunc));
    }
}
