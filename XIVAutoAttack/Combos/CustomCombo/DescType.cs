namespace XIVAutoAttack.Combos.CustomCombo
{
    public enum DescType : byte
    {
        Description,
        BreakingAction,
        HealArea,
        HealSingle,
        DefenseArea,
        DefenseSingle,
        MoveAction,
        OtherCommands,
    }

    internal static class DescTypeExtension
    {
        public static string ToName(this DescType type) => type switch
        {
            DescType.Description => "循环说明",
            DescType.BreakingAction => "爆发技能",
            DescType.HealArea => "范围治疗",
            DescType.HealSingle => "单体治疗",
            DescType.DefenseArea => "范围防御",
            DescType.DefenseSingle => "单体防御",
            DescType.MoveAction => "移动技能",
            DescType.OtherCommands => "命令说明",
            _ => "Unknown",
        };
    }
}
