using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackSingleGCDFeature : BLMCombo
{
    public override string ComboFancyName => "单个目标GCD";

    public override string Description => "这个很牛逼！";

    protected internal override uint[] ActionIDs { get; } = { (uint)Actions.Xenoglossy };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        //冰状态
        if (JobGauge.InUmbralIce)
        {
            // 35级以下，{冰1打到满蓝} 星灵
            if (level < (byte)Levels.Fire3)
            {
                if(TargetBuffDuration((ushort)Debuffs.Thunder) < 3f)
                {
                    return (uint)Actions.Thunder;
                }
                else  if(LocalPlayer.CurrentMp > 9000)
                {
                    return (uint)Actions.Transpose;
                }
                return (uint)Actions.Blizzard;
            }
            // 60级以下，冰3 {蓝没满可以打冰1} 雷3/1
            else if (level < (byte)Levels.Fire4)
            {
                if (LocalPlayer.CurrentMp > 9000)
                {
                    return (uint)Actions.Fire3;
                }

                //补 Dot
                if(level < (byte)Levels.Thunder3)
                {
                    if (TargetBuffDuration((ushort)Debuffs.Thunder) < 3f)
                    {
                        return (uint)Actions.Thunder;
                    }
                }
                else
                {
                    if (TargetBuffDuration((ushort)Debuffs.Thunder3) < 3f)
                    {
                        return (uint)Actions.Thunder3;
                    }
                }

                if(level > (byte)Levels.Blizzard4)
                    return (uint)Actions.Blizzard4;

                return (uint)Actions.Blizzard;
            }
            // 89级以下，冰4 雷3 火3
            else if (level < (byte)Levels.Paradox)
            {
                if(lastComboMove == (uint)Actions.Thunder3)
                {
                    return (uint)Actions.Fire3;
                }
                if(lastComboMove == (uint)Actions.Blizzard4)
                {
                    return (uint)Actions.Thunder3;
                }

                return (uint)Actions.Blizzard4;
            }
            //90 级
            else
            {
                if (LocalPlayer.CurrentMp == LocalPlayer.MaxMp)
                {
                    return (uint)Actions.Fire3;
                }

                if (JobGauge.UmbralHearts < 3)
                {
                    return (uint)Actions.Blizzard4;
                }

                if (JobGauge.IsParadoxActive)
                {
                    return (uint)Actions.Paradox;
                }

                return (uint)Actions.Thunder3;
            }
        }
        //火状态
        else if (JobGauge.InAstralFire)
        {
            // 35级以下，{火1打到没蓝} 星灵
            if (level < (byte)Levels.Fire3)
            {
                if (TargetBuffDuration((ushort)Debuffs.Thunder) < 3f)
                {
                    return (uint)Actions.Thunder;
                }
                else  if (LocalPlayer.CurrentMp < 800)
                {
                    return (uint)Actions.Transpose;
                }
                return (uint)Actions.Fire;
            }
            // 60级以下，火3 {火1打到蓝量不够再打火1}
            else if (level < (byte)Levels.Fire4)
            {
                if (LocalPlayer.CurrentMp < 1600)
                {
                    return (uint)Actions.Blizzard3;
                }

                if (BuffDuration((ushort)Buffs.火苗).HasValue)
                {
                    return (uint)Actions.Fire3;
                }

                return (uint)Actions.Fire;
            }
            // 89级以下，火4 x 3 火1 火4 x 3 冰3
            else if (level < (byte)Levels.Paradox)
            {
                if (JobGauge.ElementTimeRemaining < 5000)
                {
                    return (uint)Actions.Fire;
                }

                if (JobGauge.PolyglotStacks != 0)
                {
                    if (level >= (byte)Levels.Xenoglossy)
                    {
                        return (uint)Actions.Xenoglossy;
                    }
                    return (uint)Actions.Foul;
                }

                if (LocalPlayer.CurrentMp < 1600)
                {
                    if(level >= (byte)Levels.Despair && LocalPlayer.CurrentMp >= 800)
                    {
                        return (uint)Actions.Despair;
                    }
                    return (uint)Actions.Blizzard3;
                }

                return(uint)Actions.Fire4;
            }

            //90级别
            else
            {
                if (JobGauge.PolyglotStacks != 0)
                {
                    return (uint)Actions.Xenoglossy;
                }

                if (LocalPlayer.CurrentMp < 1600)
                {
                    if (level >= (byte)Levels.Despair)
                    {
                        return (uint)Actions.Despair;
                    }
                    return (uint)Actions.Blizzard3;
                }

                if (JobGauge.ElementTimeRemaining < 10)
                {
                    if (JobGauge.IsParadoxActive)
                    {
                        return (uint)Actions.Paradox;
                    }
                    return (uint)Actions.Fire;
                }

                if (TargetBuffDuration((ushort)Debuffs.Thunder3) < 3f)
                {
                    return (uint)Actions.Thunder3;
                }

                return (uint)Actions.Fire4;
            }
        }

        if(level > (byte)Levels.Fire3)
        {
            if(LocalPlayer.CurrentMp < 5000)
            {
                return (uint)Actions.Blizzard3;
            }
            return (uint)Actions.Fire3;
        }
        else
        {
            if (LocalPlayer.CurrentMp < 5000)
            {
                return (uint)Actions.Blizzard;
            }
            return (uint)Actions.Fire;
        }
        return actionID;
    }
}
