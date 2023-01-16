using XIVAutoAction.Localization;

namespace XIVAutoAction.Actions;

public enum EnemyLocation : byte
{
    None,
    Back,
    Side,
    Front,
}
public static class EnemyLocationExtensions
{
    public static string ToName(this EnemyLocation value) => value switch
    {
        EnemyLocation.None => LocalizationManager.RightLang.EnemyLocation_None,
        EnemyLocation.Back => LocalizationManager.RightLang.EnemyLocation_Back,
        EnemyLocation.Side => LocalizationManager.RightLang.EnemyLocation_Side,
        EnemyLocation.Front => LocalizationManager.RightLang.EnemyLocation_Front,
        _ => string.Empty,
    };
}
