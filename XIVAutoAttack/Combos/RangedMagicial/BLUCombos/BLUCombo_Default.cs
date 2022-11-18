using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedMagicial.BLUCombos.BLUCombo_Default;

namespace XIVAutoAttack.Combos.RangedMagicial.BLUCombos
{
    internal sealed class BLUCombo_Default : BLUCombo_Base<CommandType>
    {
        public override string Author => "秋水";

        internal enum CommandType : byte
        {
            None,
        }

        protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
        {
            //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
        };

        protected override bool CanHealAreaSpell => base.CanHealAreaSpell && BlueId == BLUID.Healer;
        protected override bool CanHealSingleSpell => base.CanHealSingleSpell && BlueId == BLUID.Healer;

        private protected override ActionConfiguration CreateConfiguration()
        {
            return base.CreateConfiguration()
                .SetCombo("BlueId", 2, "职能", "防护", "治疗", "进攻")
                .SetCombo("AttackType", 2, "攻击方式", "魔法", "物理", "我全都要")
                .SetBool("MoonFluteBreak", false, "D青月笛爆发")
                .SetBool("UseFinalSting", false, "终极针收尾")
                .SetFloat("FinalStingHP", 0, "开始使用终极针的Hp");
        }

        private bool MoonFluteBreak => Config.GetBoolByName("MoonFluteBreak");
        private bool UseFinalSting => Config.GetBoolByName("UseFinalSting");
        private float FinalStingHP => Config.GetFloatByName("FinalStingHP");
        /// <summary>
        /// 0-70练级,快速练级,滑舌拉怪
        /// </summary>
        private bool QuickLevel => false;
        /// <summary>
        /// 赌几率秒杀
        /// </summary>
        private bool GamblerKill => false;
        /// <summary>
        /// 单体时是否释放高伤害AOE
        /// </summary>
        private bool SingleAOE => true;

        private protected override void UpdateInfo()
        {
            BlueId = (BLUID)Config.GetComboByName("BlueId");
            AttackType = (BLUAttackType)Config.GetComboByName("AttackType");
        }

        private protected override bool AttackAbility(byte abilityRemain, out IAction act)
        {
            act = null;
            return false;
        }

        private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
        {
            if(nextGCD.IsAnySameAction(false, Selfdestruct, FinalSting))
            {
                if (Swiftcast.ShouldUse(out act)) return true;
            }
            return base.EmergencyAbility(abilityRemain, nextGCD, out act);
        }

        private protected override bool MoveGCD(out IAction act)
        {
            //正义飞踢
            if (JKick.ShouldUse(out act, mustUse: true)) return true;
            return base.MoveGCD(out act);
        }

        private protected override bool GeneralGCD(out IAction act)
        {
            act=null;
            //狂战士副作用期间
            if (Player.HasStatus(true, StatusID.WaningNocturne)) return false;
            //鬼宿脚
            if (IsLastAction(false, PhantomFlurry) || Player.HasStatus(true, StatusID.PhantomFlurry))
            {
                //if (Player.WillStatusEnd(1, true, StatusID.PhantomFlurry) && PhantomFlurry2.ShouldUse(out act, mustUse: true)) return true;
                return false;
            } 
            //穿甲散弹
            if (Player.HasStatus(true, StatusID.SurpanakhaFury))
            {
                if (Surpanakha.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;
            }

            //终极针组合
            if (UseFinalSting && CanUseFinalSting(out act)) return true;

            //爆发
            if (MoonFluteBreak && DBlueBreak(out act)) return true;

            //高伤害
            if (PrimalSpell(out act)) return true;
            //群体
            if (AreaGCD(out act)) return true;
            //单体填充
            if (SingleGCD(out act)) return true;
            
            
            act = null;
            return false;
        }

        private protected override bool HealAreaGCD(out IAction act)
        {
            if (BlueId == BLUID.Healer)
            {
                //有某些非常危险的状态。
                if (CommandController.EsunaOrShield && TargetUpdater.WeakenPeople.Length > 0 || TargetUpdater.DyingPeople.Length > 0)
                {
                    if (Exuviation.ShouldUse(out act, mustUse: true)) return true;
                }
                if (AngelsSnack.ShouldUse(out act)) return true;
                if (Stotram.ShouldUse(out act)) return true;
                if (PomCure.ShouldUse(out act)) return true;
            }
            else
            {
                if (WhiteWind.ShouldUse(out act)) return true; 
            }

            return base.HealAreaGCD(out act);
        }

        /// <summary>
        /// D青爆发
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool DBlueBreak(out IAction act)
        {
            act = null;
            if (BlueId == BLUID.Healer && BlueId == BLUID.Tank) return false;

            if (!AllOnSlot(MoonFlute)) return false;

            if (AllOnSlot(Whistle, Tingle, TripleTrident) && !Nightbloom.IsCoolDown && !TripleTrident.IsCoolDown)
            {
                //口笛
                if (Whistle.ShouldUse(out act)) return true;
                //哔哩哔哩
                if (!Player.HasStatus(true, StatusID.Tingling) && Player.HasStatus(true, StatusID.Harmonized) && Tingle.ShouldUse(out act, mustUse: true)) return true;
                //鱼叉
                if (Player.HasStatus(true, StatusID.WaxingNocturne) && TripleTrident.ShouldUse(out act, mustUse: true)) return true;
            }

            if (AllOnSlot(Whistle, FinalSting, BasicInstinct) && UseFinalSting)
            {
                if (HaveHostilesInRange && Whistle.ShouldUse(out act)) return true;
                //破防
                if (Player.HasStatus(true, StatusID.WaxingNocturne) && Offguard.ShouldUse(out act)) return true;
                //哔哩哔哩
                if (Player.HasStatus(true, StatusID.WaxingNocturne) && Tingle.ShouldUse(out act)) return true;
            }

            //月笛
            if (CanUseMoonFlute(out act)) return true;

            if (!Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

            //月下彼岸花
            if (Nightbloom.ShouldUse(out act, mustUse: true)) return true;
            //地火喷发
            if (Eruption.ShouldUse(out act, mustUse: true)) return true;
            //马特拉魔术
            if (MatraMagic.ShouldUse(out act, mustUse: true)) return true;
            //正义飞踢
            if (JKick.ShouldUse(out act, mustUse: true)) return true;
            //捕食
            if (Devour.ShouldUse(out act, mustUse: true)) return true;
            //轰雷
            if (ShockStrike.ShouldUse(out act, mustUse: true)) return true;
            //冰雪乱舞
            if (GlassDance.ShouldUse(out act, mustUse: true)) return true;
            //魔法锤
            if (MagicHammer.ShouldUse(out act, mustUse: true)) return true;
            //穿甲散弹
            if (Surpanakha.CurrentCharges >= 3 && Surpanakha.ShouldUse(out act, mustUse: true, emptyOrSkipCombo: true)) return true;
            //鬼宿脚
            if (PhantomFlurry.ShouldUse(out act, mustUse: true)) return true;

            //音爆
            if (SonicBoom.ShouldUse(out act)) return true;

            return false;
        }


        /// <summary>
        /// 月笛条件
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseMoonFlute(out IAction act)
        {
            if (!MoonFlute.ShouldUse(out act) && !HaveHostilesInRange) return false;

            if (Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

            if (Player.HasStatus(true, StatusID.Harmonized)) return true;

            return false;
        }

        /// <summary>
        /// 终极针组合
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool CanUseFinalSting(out IAction act)
        { 
            act = null;
            if (!UseFinalSting) return false;
            if (!FinalSting.ShouldUse(out _)) return false;

            var useFinalSting = Player.HasStatus(true, StatusID.WaxingNocturne, StatusID.Harmonized);

            if (AllOnSlot(Whistle, MoonFlute, FinalSting) && !AllOnSlot(BasicInstinct))
            {
                if ((float)Target.CurrentHp / Target.MaxHp > FinalStingHP) return false;

                if (Whistle.ShouldUse(out act)) return true;
                if (MoonFlute.ShouldUse(out act)) return true;
                if (useFinalSting && FinalSting.ShouldUse(out act)) return true;
            }

            if (AllOnSlot(Whistle, MoonFlute, FinalSting, BasicInstinct))
            {

                //破防
                if (Player.HasStatus(true, StatusID.WaxingNocturne) && Offguard.ShouldUse(out act)) return true;

                if ((float)Target.CurrentHp / Target.MaxHp > FinalStingHP) return false;
                if (Whistle.ShouldUse(out act)) return true;
                if (MoonFlute.ShouldUse(out act)) return true;
                if (useFinalSting && FinalSting.ShouldUse(out act)) return true;
            }

            return false;
        }

        /// <summary>
        /// 单体GCD填充
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool SingleGCD(out IAction act)
        {
            act = null;
            if (Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

            //滑舌 +眩晕 0-70练级用
            if (QuickLevel && StickyTongue.ShouldUse(out act)) return true;

            //苦闷之歌
            if (AllOnSlot(Bristle, SongofTorment) && SongofTorment.ShouldUse(out _))
            {
                //怒发冲冠
                if (Bristle.ShouldUse(out act)) return true;
                if (SongofTorment.ShouldUse(out act)) return true;
            }
            if (SongofTorment.ShouldUse(out act)) return true;

            //复仇冲击
            if (RevengeBlast.ShouldUse(out act)) return true;
            //赌徒行为
            if (GamblerKill)
            {
                //导弹
                if (Missile.ShouldUse(out act)) return true;
                //螺旋尾
                if (TailScrew.ShouldUse(out act)) return true;
                //死亡宣告
                if (Doom.ShouldUse(out act)) return true;
            }

            //锋利菜刀 近战 眩晕增伤
            if (SharpenedKnife.ShouldUse(out act)) return true;

            //吸血 回蓝
            if (Player.CurrentMp < 1000 && BloodDrain.ShouldUse(out act)) return true;
            //音爆
            if (SonicBoom.ShouldUse(out act)) return true;
            //永恒射线 无法 +眩晕1s
            if (PerpetualRay.ShouldUse(out act)) return true;
            //深渊贯穿 无物 +麻痹
            if (AbyssalTransfixion.ShouldUse(out act)) return true;
            //逆流 雷法 +加重
            if (Reflux.ShouldUse(out act)) return true;
            //水炮
            if (WaterCannon.ShouldUse(out act)) return true;

            //小侦测
            if (CondensedLibra.ShouldUse(out act)) return true;

            //滑舌 +眩晕
            if (StickyTongue.ShouldUse(out act)) return true;

            //投掷沙丁鱼(打断)
            if (FlyingSardine.ShouldUse(out act)) return true;

            return false;
        }

        /// <summary>
        /// 范围GCD填充
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool AreaGCD(out IAction act)
        {
            act = null;
            if (Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

            //赌徒行为
            if (GamblerKill)
            {
                //火箭炮
                if (Launcher.ShouldUse(out act, mustUse: true)) return true;
                //5级即死
                if (Level5Death.ShouldUse(out act, mustUse: true)) return true;
            }

            if (false)
            {
                if (AcornBomb.ShouldUse(out act, mustUse: true)) return true;
                if (Faze.ShouldUse(out act, mustUse: true)) return true;
                if (Snort.ShouldUse(out act, mustUse: true)) return true;
                if (BadBreath.ShouldUse(out act, mustUse: true)) return true;
                if (Chirp.ShouldUse(out act, mustUse: true)) return true;
                if (Level5Petrify.ShouldUse(out act, mustUse: true)) return true;
            }

          

            //陆行鸟陨石
            if (ChocoMeteor.ShouldUse(out act)) return true;

            if (TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 6).Length < 3)
            {
                //水力吸引
                if (HydroPull.ShouldUse(out act)) return true;
            }

            //寒冰咆哮
            if (TheRamVoice.ShouldUse(out act)) return true;

            //超振动
            if (!IsMoving && Target.HasStatus(false, StatusID.DeepFreeze) && TheRamVoice.ShouldUse(out act)) return true;

            //雷电咆哮
            if (TheDragonVoice.ShouldUse(out act)) return true;

            //冰焰
            if (Blaze.ShouldUse(out act)) return true;
            if (FeculentFlood.ShouldUse(out act)) return true;
            //火炎放射
            if (FlameThrower.ShouldUse(out act)) return true;
            //水流吐息
            if (AquaBreath.ShouldUse(out act)) return true;
            //高压电流
            if (HighVoltage.ShouldUse(out act)) return true;
            //怒视
            if (Glower.ShouldUse(out act)) return true;
            //平原震裂
            if (Plaincracker.ShouldUse(out act)) return true;
            //诡异视线
            if (TheLook.ShouldUse(out act)) return true;
            //喷墨
            if (InkJet.ShouldUse(out act)) return true;
            if (FireAngon.ShouldUse(out act)) return true;
            if (MindBlast.ShouldUse(out act)) return true;      
            if (AlpineDraft.ShouldUse(out act)) return true;
            if (ProteanWave.ShouldUse(out act)) return true;
            if (Northerlies.ShouldUse(out act)) return true;
            if (Electrogenesis.ShouldUse(out act)) return true;
            if (WhiteKnightsTour.ShouldUse(out act)) return true;
            if (BlackKnightsTour.ShouldUse(out act)) return true;
            if (Tatamigaeshi.ShouldUse(out act)) return true;

            if (MustardBomb.ShouldUse(out act)) return true;
            if (AetherialSpark.ShouldUse(out act)) return true;
            if (MaledictionofWater.ShouldUse(out act)) return true;
            if (FlyingFrenzy.ShouldUse(out act)) return true;
            if (DrillCannons.ShouldUse(out act)) return true;
            if (Weight4tonze.ShouldUse(out act)) return true;
            if (Needles1000.ShouldUse(out act)) return true;
            if (Kaltstrahl.ShouldUse(out act)) return true;
            if (PeripheralSynthesis.ShouldUse(out act)) return true;
            if (FlameThrower.ShouldUse(out act)) return true;
            if (FlameThrower.ShouldUse(out act)) return true;
            if (SaintlyBeam.ShouldUse(out act)) return true;

            return false;
        }

        /// <summary>
        /// 有CD的技能
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private bool PrimalSpell(out IAction act)
        {
            act = null;
            if (Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

            //冰雾
            if (WhiteDeath.ShouldUse(out act)) return true;
            //玄天武水壁
            if (DivineCataract.ShouldUse(out act)) return true;

            //斗灵弹
            if (TheRoseofDestruction.ShouldUse(out act)) return true;

            //渔叉三段
            if (SettingBreak && !MoonFluteBreak && TripleTrident.ShouldUse(out act)) return true;
            //马特拉魔术
            if (SettingBreak && !MoonFluteBreak && MatraMagic.ShouldUse(out act)) return true;

            //捕食
            if (Devour.ShouldUse(out act)) return true;
            //魔法锤
            //if (MagicHammer.ShouldUse(out act)) return true;

            //月下彼岸花
            if (SettingBreak && !MoonFluteBreak && Nightbloom.ShouldUse(out act, mustUse: SingleAOE)) return true;
            //如意大旋风
            if (SettingBreak && !MoonFluteBreak && BothEnds.ShouldUse(out act, mustUse: SingleAOE)) return true;
            
            //穿甲散弹
            if (SettingBreak && !MoonFluteBreak && Surpanakha.CurrentCharges >= 3 && Surpanakha.ShouldUse(out act, mustUse: SingleAOE, emptyOrSkipCombo: true)) return true;

            //类星体
            if (Quasar.ShouldUse(out act, mustUse: SingleAOE)) return true;
            //正义飞踢
            if (!IsMoving && JKick.ShouldUse(out act, mustUse: SingleAOE)) return true;

            //地火喷发
            if (Eruption.ShouldUse(out act, mustUse: SingleAOE)) return true;
            //飞翎雨
            if (FeatherRain.ShouldUse(out act, mustUse: SingleAOE)) return true;

            //轰雷
            if (ShockStrike.ShouldUse(out act, mustUse: SingleAOE)) return true;
            //山崩
            if (MountainBuster.ShouldUse(out act, mustUse: SingleAOE)) return true;

            //冰雪乱舞
            if (MountainBuster.ShouldUse(out act, mustUse: SingleAOE)) return true;

            if (MountainBuster.ShouldUse(out act, mustUse: SingleAOE)) return true;


            return false;
        }
    }
}
