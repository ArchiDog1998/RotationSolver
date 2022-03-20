using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackAOEGCDFeature : BLMCombo
{
    public override string ComboFancyName => "群体GCD";

    public override string Description => "替换火2非常牛逼的群攻GCD。";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Fire2 };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (CanAddAbility(level, out uint act)) return act;

        //低于19级
        if (level < Levels.Fire2)
        {
            return Actions.Blizzard2;
        }
        //冰状态
        if (JobGauge.InUmbralIce)
        {
            // 低于58级，打冰2/玄冰到满蓝，保持雷不断。
            if (level < Levels.Blizzard4)
            {
                if (HaveEnoughMP)
                {
                    return Actions.Fire2;
                }

                if (level >= Levels.Thunder2 && TargetBuffDuration((ushort)Debuffs.Thunder2) < 3f)
                {
                    return Actions.Thunder2;
                }

                if (level >= Levels.Freeze)
                {
                    return Actions.Freeze;
                }
                return Actions.Blizzard2;
            }
            // 低于82级，打玄冰到满蓝，保持雷不断。
            else if (level < Levels.HighBlizzard2)
            {
                if (HaveEnoughMP)
                {
                    return Actions.Fire2;
                }

                if (JobGauge.UmbralHearts < 3)
                {
                    return Actions.Freeze;
                }

                //补雷
                if (TargetBuffDuration(Debuffs.Thunder4) < 3f)
                {
                    if (level >= Levels.Thunder4)
                    {
                        return Actions.Thunder4;
                    }
                    else if (TargetBuffDuration(Debuffs.Thunder2) < 3f)
                    {
                        return Actions.Thunder2;
                    }
                }


                return Actions.Freeze;
            }
            // 打高冰2到满蓝，保持雷不断。
            else
            {
                if (LocalPlayer.CurrentMp > 9000)
                {
                    return Actions.HighFire2;
                }

                if (JobGauge.UmbralHearts < 3)
                {
                    return Actions.Freeze;
                }

                if (TargetBuffDuration(Debuffs.Thunder4) < 3f)
                {
                    return Actions.Thunder4;
                }

                return Actions.HighBlizzard2;
            }
        }
        //火状态
        else if (JobGauge.InAstralFire)
        {
            // 低于50 级，打火2到没蓝。
            if (level < Levels.Flare)
            {

                if (LocalPlayer.CurrentMp < 3000)
                {
                    return Actions.Blizzard2;
                }

                return Actions.Fire2;
            }
            // 低于58级，打火2，最终转核爆。
            else if (level < Levels.Blizzard4)
            {
                if (LocalPlayer.CurrentMp == 0)
                {
                    return Actions.Blizzard2;
                }
                if (LocalPlayer.CurrentMp < 3800)
                {
                    return Actions.Flare;
                }
                return Actions.Fire2;
            }
            // 打双核爆，火2/高火2填充，保持雷不断。
            else
            {
                if (LocalPlayer.CurrentMp == 0)
                {
                    if (level >= Levels.HighBlizzard2)
                    {
                        return Actions.HighBlizzard2;
                    }
                    return Actions.Blizzard2;
                }

                if (JobGauge.UmbralHearts < 2)
                {
                    return Actions.Flare;
                }

                //补雷
                if (TargetBuffDuration(Debuffs.Thunder4) < 3f)
                {
                    if (level >= Levels.Thunder4)
                    {
                        return Actions.Thunder4;
                    }
                    else if (TargetBuffDuration(Debuffs.Thunder2) < 3f)
                    {
                        return Actions.Thunder2;
                    }
                }
                //补秽浊，如果过多
                if (level >= Levels.Foul && JobGauge.PolyglotStacks == 2)
                {
                    return Actions.Foul;
                }

                //上火2/高火2
                if (level >= Levels.HighBlizzard2)
                {
                    return Actions.HighFire2;
                }
                return Actions.Fire2;
            }
        }
        else
        {
            return Actions.Blizzard2;
        }

        return actionID;
    }
}
