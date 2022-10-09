using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.Disciplines
{
    internal class FSHCombo : OtherCombo
    {
        internal override uint JobID => 18;
        internal struct Actions
        {
            public static readonly BaseAction
                //抛竿
                Cast = new (289),

                //提钩
                Hook = new (296)
                {
                    AfterUse = () =>
                    {
                        Service.ToastGui.ShowQuest(Hook.Action.Name, new Dalamud.Game.Gui.Toast.QuestToastOptions()
                        {
                            IconId = Hook.Action.Icon,
                        });
                        TargetHelper.Fish = FishType.None;

#if DEBUG
                        Service.ChatGui.Print(Hook.Action.Name);
#endif
                    },
                },

                //精准提钩
                PrecisionHookset = new (4179)
                {
                    AfterUse = () =>
                    {
                        Service.ToastGui.ShowQuest(PrecisionHookset.Action.Name, new Dalamud.Game.Gui.Toast.QuestToastOptions()
                        {
                            IconId = PrecisionHookset.Action.Icon,
                        });
                        TargetHelper.Fish = FishType.None;
#if DEBUG
                        Service.ChatGui.Print(PrecisionHookset.Action.Name);
#endif
                    },
                },
                //强力提钩
                PowerfulHookset = new (4103)
                {
                    AfterUse = () =>
                    {
                        Service.ToastGui.ShowQuest(PowerfulHookset.Action.Name, new Dalamud.Game.Gui.Toast.QuestToastOptions()
                        {
                            IconId = PowerfulHookset.Action.Icon,
                        });
                        TargetHelper.Fish = FishType.None;

#if DEBUG
                        Service.ChatGui.Print(PowerfulHookset.Action.Name);
#endif
                    },
                },

                //沙利亚克的恩宠
                ThaliaksFavor = new (26804),

                //耐心
                Patience = new (4102)
                {
                    OtherCheck = b =>
                    {
                        foreach (var item in Service.ClientState.LocalPlayer.StatusList)
                        {
                            if (item.StatusId == ObjectStatus.Patience) return false;
                        }
                        return true;
                    },
                },

                //钓组
                Snagging = new (4100)
                {
                    BuffsProvide = new ushort[] { ObjectStatus.Snagging },
                },

                //以小钓大
                Mooch = new (297)
                {
                    OtherCheck = b => TargetHelper.Fish == FishType.Mooch,
                    AfterUse = () =>
                    {
                        TargetHelper.Fish = FishType.None;
#if DEBUG
                        Service.ChatGui.Print("成功了！");
#endif
                    }

                },

                //撒饵
                Chum = new (4104)
                {
                    OtherCheck = b => TargetHelper.Fish != FishType.Mooch,
                    AfterUse = () =>
                    {
                        TargetHelper.Fish = FishType.None;
                    },
                    BuffsProvide = new ushort[] {ObjectStatus.Chum},
                };

            public static readonly BaseItem
                //强心剂
                Strong = new (6141, 65535),
                //高级强心剂
                HighStrong = new (12669, 65535);
        }
        private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
        {
            act = null;
            return false;
        }

        private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
        {
            var maxgp = Service.ClientState.LocalPlayer.MaxGp;
            var gp = Service.ClientState.LocalPlayer.CurrentGp;
            bool fishing = Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Fishing];

            //钓鱼
            if (fishing && TargetHelper.Fish != FishType.None && TargetHelper._fisherTimer.ElapsedMilliseconds > Config.GetDoubleByName("CastTime") * 1000)
            {
                if(StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Patience))
                {
                    switch (TargetHelper.Fish)
                    {
                        case FishType.Small:

                            if (Config.GetBoolByName("OnlyLargeFish"))
                            {
                                TargetHelper.Fish = FishType.None;
                                act = null;
                                return false;
                            }
                            else
                            {
                                if (Actions.PrecisionHookset.ShouldUseAction(out act)) return true;
                            }
                            break;
                        case FishType.Medium:
                            if (Config.GetBoolByName("OnlyLargeFish"))
                            {
                                TargetHelper.Fish = FishType.None;
                                act = null;
                                return false;
                            }
                            else
                            {
                                if (Actions.PowerfulHookset.ShouldUseAction(out act)) return true;
                            }
                            break;
                        case FishType.Large:
                            if (Config.GetBoolByName("UsePowerfulHookset") && Actions.PowerfulHookset.ShouldUseAction(out act)) return true;
                            if (Actions.PrecisionHookset.ShouldUseAction(out act)) return true;
                            break;
                    }
                }
                else if (Actions.Hook.ShouldUseAction(out act)) return true;
            }

            //非钓鱼
            if (!fishing)
            {
                //以小掉大
                if (Actions.Mooch.ShouldUseAction(out act)) return true;

                var status = Service.ClientState.LocalPlayer.StatusList.Where(s => s.StatusId == ObjectStatus.AnglersArt);
                byte stack = 0;
                if (status != null && status.Count() > 0)
                {
                    stack = status.First().StackCount;
                }

                if (Config.GetBoolByName("UseSnagging") && Actions.Snagging.ShouldUseAction(out act)) return true;

                //补充GP
                if (stack > 2 && maxgp - gp >= 150)
                {
                    if (Actions.ThaliaksFavor.ShouldUseAction(out act)) return true;
                }
                if (maxgp - gp >= 400)
                {
                    if (Actions.HighStrong.ShoudUseItem(out act)) return true;
                }
                if (maxgp - gp >= 350)
                {
                    if (Actions.Strong.ShoudUseItem(out act)) return true;
                }
                if (gp >= 300)
                {
                    if (Actions.Patience.ShouldUseAction(out act)) return true;
                }

                if (gp >= 350 && Actions.Chum.ShouldUseAction(out act)) return true;
                if (TargetHelper._unfishingTimer.ElapsedMilliseconds > 300 && Actions.Cast.ShouldUseAction(out act)) return true;
            }

            act = null;
            return false;
        }

        private protected override ActionConfiguration CreateConfiguration()
        {
            return base.CreateConfiguration()
                .SetBool("UsePowerfulHookset", true, "三个感叹号用强力提钩")
                .SetFloat("CastTime", 0.6f, des:"反应提钩的速度")
                .SetBool("UseSnagging", true, "使用钓组")
                .SetBool("OnlyLargeFish", false, "只钓三个感叹号的鱼");
        }
    }
}
