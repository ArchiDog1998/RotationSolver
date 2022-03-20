using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.BLM
{
    internal abstract class BLMCombo : CustomCombo
    {
        public sealed override string JobName => "黑魔法师";

        internal sealed override uint JobID => 25;

        private BLMGauge _gauge;

        public BLMGauge JobGauge
        {
            get
            {
                if( _gauge == null)
                {
                    _gauge = Service.JobGauges.Get<BLMGauge>();
                }
                return _gauge;
            }
        }

        protected bool HaveEnoughMP => LocalPlayer.CurrentMp > 9000;

        protected sealed override JobGaugeBase GetJobGaugeBase()
        {
            return Service.JobGauges.Get<BLMGauge>();
        }

        protected bool CanAddAbility(byte level, out uint action)
        {
            action = 0;
            if (SpecialForTri(level, out uint tri))
            {
                action = tri;
                return true;
            }


            if (CanInsertAbility)
            {
                //加个即刻
                if (GetCooldown(GenActions.Swiftcast).CooldownRemaining == 0 && level > GenLevels.Swiftcast)
                    if (BuffDuration(Buffs.Triplecast) == 0 && JobGauge.InAstralFire && LocalPlayer.CurrentMp > 800)
                    {
                        action = GenActions.Swiftcast;
                        return true;
                    }

                //加个通晓
                if (GetCooldown(Actions.Amplifier).CooldownRemaining == 0 && level > Levels.Amplifier)
                    if (JobGauge.PolyglotStacks < 2)
                    {
                        action = Actions.Amplifier;
                        return true;
                    }

                //加个魔泉
                if (GetCooldown(Actions.Manafont).CooldownRemaining == 0 && level > Levels.Manafont)
                    if (JobGauge.InAstralFire && LocalPlayer.CurrentMp == 0)
                    {
                        action = Actions.Manafont;
                        return true;
                    }

                //加个激情
                if (GetCooldown(Actions.Sharpcast).CooldownRemaining == 0 && level > Levels.Sharpcast)
                {
                    action = Actions.Sharpcast;
                    return true;
                }
                //加个混乱
                if (GetCooldown(GenActions.Addle).CooldownRemaining == 0 && level > GenLevels.Addle)
                {
                    action = GenActions.Addle;
                    return true;
                }

                //加个魔罩
                if (GetCooldown(Actions.Manaward).CooldownRemaining == 0 && level > Levels.Manafont)
                {
                    action = Actions.Manaward;
                    return true;
                }

                //加个醒梦
                if (GetCooldown(GenActions.LucidDreaming).CooldownRemaining == 0 && level > GenLevels.LucidDreaming)
                {
                    action = GenActions.LucidDreaming;
                    return true;
                }
            }
            return false;
        }

        protected bool SpecialForTri(byte level, out uint action)
        {
            action=0;
            if(level < Levels.Triplecast) return false;

            if (!GetCooldown(Actions.Triplecast).IsCooldown && JobGauge.InAstralFire)
            {
                action = Actions.Triplecast;
                return true;
            }

            return false;
        }

        public struct Buffs
        {
            public const ushort

                Thundercloud = 164,

                LeyLines = 737,

                Triplecast = 1211,

                Firestarter = 165;
        }

        public struct Debuffs
        {
            public const ushort 

                Thunder = 161,

                Thunder2 = 162,

                Thunder3 = 163,

                Thunder4 = 1210;
        }

        public struct Levels
        {
            public const byte

                Fire2 = 18,

                Thunder2 = 26,

                Manafont = 30,

                Fire3 = 35,

                Blizzard3 = 35,

                Freeze = 40,

                Thunder3 = 45,

                Flare = 50,

                Blizzard4 = 58,

                Fire4 = 60,

                Sharpcast = 54,

                BetweenTheLines = 62,

                Thunder4 = 64,

                Triplecast = 66,

                Foul = 70,

                Despair = 72,

                UmbralSoul = 76,

                Xenoglossy = 80,

                HighFire2 = 82,

                HighBlizzard2 = 82,

                Amplifier = 86,

                Paradox = 90;
        }
        public struct Actions
        {
            public const uint

                Fire = 141u,

                Blizzard = 142u,

                Thunder = 144u,

                Blizzard2 = 25793u,

                Fire2 = 147u,

                Transpose = 149u,

                Fire3 = 152u,

                Thunder3 = 153u,

                Blizzard3 = 154u,

                Scathe = 156u,

                Manaward = 157u,

                Manafont = 158u,

                Freeze = 159u,

                Flare = 162u,

                LeyLines = 3573u,

                Sharpcast = 3574u,

                Blizzard4 = 3576u,

                Fire4 = 3577u,

                BetweenTheLines = 7419u,

                Thunder4 = 7420u,

                Triplecast = 7421u,

                Foul = 7422u,

                Thunder2 = 7447u,

                Despair = 16505u,

                UmbralSoul = 16506u,

                Xenoglossy = 16507u,

                HighFire2 = 25794u,

                HighBlizzard2 = 25795u,

                Amplifier = 25796u,

                Paradox = 25797u;
        }
    }
}
