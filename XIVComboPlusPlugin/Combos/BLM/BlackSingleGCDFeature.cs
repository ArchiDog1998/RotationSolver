using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackSingleGCDFeature : BLMCombo
{
    public override string ComboFancyName => "单个目标GCD";

    public override string Description => "替换火1为持续的GCD循环！";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Fire };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {

        if(CanAddAbility(level, out uint act)) return act;

        //冰状态
        if (JobGauge.InUmbralIce)
        {
            // 35级以下，{冰1打到满蓝} 星灵
            if (level < Levels.Fire3)
            {
                if (TargetBuffDuration(Debuffs.Thunder) < 10f)
                {
                    return Actions.Thunder;
                }
                else if (HaveEnoughMP)
                {
                    return Actions.Transpose;
                }
                return Actions.Blizzard;
            }
            // 60级以下，冰3 {蓝没满可以打冰1} 雷3/1
            else if (level < Levels.Fire4)
            {
                //补 Dot
                if (level < Levels.Thunder3)
                {
                    if (TargetBuffDuration(Debuffs.Thunder) < 10f && lastComboMove != Actions.Thunder)
                    {
                        return Actions.Thunder;
                    }
                }
                else
                {
                    if (TargetBuffDuration(Debuffs.Thunder3) < 10f && lastComboMove != Actions.Thunder3)
                    {
                        return Actions.Thunder3;
                    }
                }

                if (HaveEnoughMP)
                {
                    return Actions.Fire3;
                }


                if (level > Levels.Blizzard4)
                    return Actions.Blizzard4;

                return Actions.Blizzard;
            }
            // 89级以下，冰4 雷3 火3
            else if (level < Levels.Paradox)
            {
                if (TargetBuffDuration(Debuffs.Thunder3) < 10f && lastComboMove != Actions.Thunder3)
                {
                    return Actions.Thunder3;
                }

                if (HaveEnoughMP && JobGauge.UmbralHearts == 3)
                {
                    return Actions.Fire3;
                }


                return Actions.Blizzard4;
            }
            //90 级
            else
            {
                if (HaveEnoughMP)
                {
                    return Actions.Fire3;
                }

                if (JobGauge.UmbralHearts < 3)
                {
                    return Actions.Blizzard4;
                }

                if (JobGauge.IsParadoxActive)
                {
                    return Actions.Paradox;
                }

                return Actions.Thunder3;
            }
        }
        //火状态
        else if (JobGauge.InAstralFire)
        {
            // 35级以下，{火1打到没蓝} 星灵
            if (level < Levels.Fire3)
            {
                if (TargetBuffDuration(Debuffs.Thunder) < 10f)
                {
                    return Actions.Thunder;
                }
                else if (LocalPlayer.CurrentMp < 800)
                {
                    return Actions.Transpose;
                }
                return Actions.Fire;
            }
            // 60级以下，火3 {火1打到蓝量不够再打火1}
            else if (level < Levels.Fire4)
            {
                if (LocalPlayer.CurrentMp < 1600)
                {
                    return Actions.Blizzard3;
                }

                if (BuffDuration(Buffs.Firestarter) > 0)
                {
                    return Actions.Fire3;
                }

                return Actions.Fire;
            }
            // 89级以下，火4 x 3 火1 火4 x 3 冰3
            else if (level < Levels.Paradox)
            {
                //时间不够，赶紧火1
                if (JobGauge.ElementTimeRemaining < 4000)
                {
                    if (LocalPlayer.CurrentMp > 3000)
                        return Actions.Fire;

                    else if (level < Levels.Despair && LocalPlayer.CurrentMp < 800)
                        return Actions.Blizzard3;

                    else return Actions.Despair;
                }

                //如果通晓太多，就丢掉。
                switch (JobGauge.PolyglotStacks)
                {
                    case 1:
                        if(level < Levels.Xenoglossy)
                        {
                            return Actions.Foul;
                        }
                        break;
                    case 2:
                        return Actions.Xenoglossy;
                }

                if (LocalPlayer.CurrentMp < 1600)
                {
                    if (level >= Levels.Despair && LocalPlayer.CurrentMp >= 800)
                    {
                        return Actions.Despair;
                    }
                    return Actions.Blizzard3;
                }

                //如果没有雷了，就补上！
                if (TargetBuffDuration(Debuffs.Thunder3) < 10f && lastComboMove != Actions.Thunder3)
                {
                    return Actions.Thunder3;
                }

                return Actions.Fire4;
            }

            //90级别
            else
            {
                if (JobGauge.PolyglotStacks == 2)
                {
                    return Actions.Xenoglossy;
                }

                if (LocalPlayer.CurrentMp < 1600)
                {
                    if (level >= Levels.Despair)
                    {
                        return Actions.Despair;
                    }
                    return Actions.Blizzard3;
                }

                //时间不够，赶紧悖论或火1
                if (JobGauge.ElementTimeRemaining < 5000)
                {
                    if (JobGauge.IsParadoxActive)
                    {
                        return Actions.Paradox;
                    }
                    return Actions.Fire;
                }

                if (TargetBuffDuration(Debuffs.Thunder3) < 10f)
                {
                    return Actions.Thunder3;
                }

                return Actions.Fire4;
            }
        }

        if (level > Levels.Blizzard3)
        {
            return Actions.Blizzard3;
        }
        else
        {
            return Actions.Blizzard;
        }
        return actionID;
    }
}
