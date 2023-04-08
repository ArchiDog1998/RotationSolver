namespace RotationSolver.Basic.Rotations.Basic;

public interface IBLUAction : IBaseAction
{
    bool OnSlot { get; }
    bool RightType { get; }
}
public abstract class BLU_Base : CustomRotation
{
    public override MedicineType MedicineType => MedicineType.Intelligence;

    public enum BLUID : byte
    {
        Tank,
        Healer,
        DPS,
    }

    public enum BLUAttackType : byte
    {
        Magical,
        Physical,
        Both,
    }

    public enum BLUActionType : byte
    {
        None,
        Magical,
        Physical,
    }

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.BlueMage };

    protected static BLUAttackType AttackType { get; set; } = BLUAttackType.Both;

    protected static BLUID BlueId { get; set; } = BLUID.DPS;

    private protected sealed override IBaseAction Raise => AngelWhisper;

    public class BLUAction : BaseAction, IBLUAction
    {
        static readonly StatusID[] NoPhysic = new StatusID[]
        {
            StatusID.IceSpikes,
        };

        static readonly StatusID[] NoMagic = new StatusID[]
        {
            StatusID.RespellingSpray,
            StatusID.Magitek,
        };

        private BLUActionType _type;
        public bool RightType
        {
            get
            {
                if (_type == BLUActionType.None) return true;
                if (AttackType == BLUAttackType.Physical && _type == BLUActionType.Magical) return false;
                if (AttackType == BLUAttackType.Magical && _type == BLUActionType.Physical) return false;

                if (Target.HasStatus(false, NoPhysic) && _type == BLUActionType.Physical) return false;
                if (Target.HasStatus(false, NoMagic) && _type == BLUActionType.Magical) return false;
                return true;
            }
        }

        public unsafe bool OnSlot => DataCenter.BluSlots.Any(i => AdjustedID == Service.GetAdjustedActionId(i));

        internal BLUAction(ActionID actionID, BLUActionType type, bool isFriendly = false, bool shouldEndSpecial = false, bool isEot = false, bool isTimeline = false)
            : base(actionID, isFriendly, shouldEndSpecial, isEot, isTimeline)
        {
            _type = type;
            ActionCheck = t => OnSlot && RightType;
        }

        public override bool CanUse(out IAction act, CanUseOption option = CanUseOption.None, byte gcdCountForAbility = 0)
        {
            act = null;

            if (!OnSlot) return false;
            return base.CanUse(out act, option, gcdCountForAbility);
        }
    }

    #region Magical Single

    public static IBLUAction WaterCannon { get; } = new BLUAction(ActionID.WaterCannon, BLUActionType.Magical);

    public static IBLUAction SongOfTorment { get; } = new BLUAction(ActionID.SongOfTorment, BLUActionType.Magical, isEot: true)
    {
        TargetStatus = new[] { StatusID.Bleeding }
    };

    public static IBLUAction BloodDrain { get; } = new BLUAction(ActionID.BloodDrain, BLUActionType.Magical)
    {
        ActionCheck = b => Player.CurrentMp <= 9500,
    };

    public static IBLUAction SonicBoom { get; } = new BLUAction(ActionID.SonicBoom, BLUActionType.Magical);

    public static IBLUAction PerpetualRay { get; } = new BLUAction(ActionID.PerpetualRay, BLUActionType.Magical);

    public static IBLUAction Reflux { get; } = new BLUAction(ActionID.Reflux, BLUActionType.Magical);

    public static IBLUAction Devour { get; } = new BLUAction(ActionID.Devour, BLUActionType.Magical, isTimeline: true);

    public static IBLUAction TheRoseOfDestruction { get; } = new BLUAction(ActionID.TheRoseOfDestruction, BLUActionType.Magical);

    public static IBLUAction MatraMagic { get; } = new BLUAction(ActionID.MatraMagic, BLUActionType.Magical);

    public static IBLUAction WhiteDeath { get; } = new BLUAction(ActionID.WhiteDeath, BLUActionType.Magical)
    {
        ActionCheck = b => Player.HasStatus(true, StatusID.TouchOfFrost)
    };
    #endregion

    #region Magical Area
    public static IBLUAction FlameThrower { get; } = new BLUAction(ActionID.FlameThrower, BLUActionType.Magical);

    public static IBLUAction AquaBreath { get; } = new BLUAction(ActionID.AquaBreath, BLUActionType.Magical, isEot: true);

    public static IBLUAction HighVoltage { get; } = new BLUAction(ActionID.HighVoltage, BLUActionType.Magical);

    public static IBLUAction Glower { get; } = new BLUAction(ActionID.Glower, BLUActionType.Magical);

    public static IBLUAction PlainCracker { get; } = new BLUAction(ActionID.PlainCracker, BLUActionType.Magical);

    public static IBLUAction TheLook { get; } = new BLUAction(ActionID.TheLook, BLUActionType.Magical);

    public static IBLUAction TheRamVoice { get; } = new BLUAction(ActionID.TheRamVoice, BLUActionType.Magical);

    public static IBLUAction TheDragonVoice { get; } = new BLUAction(ActionID.TheDragonVoice, BLUActionType.Magical);

    public static IBLUAction InkJet { get; } = new BLUAction(ActionID.InkJet, BLUActionType.Magical);

    public static IBLUAction FireAngon { get; } = new BLUAction(ActionID.FireAngon, BLUActionType.Magical);

    public static IBLUAction MindBlast { get; } = new BLUAction(ActionID.MindBlast, BLUActionType.Magical);

    public static IBLUAction FeatherRain { get; } = new BLUAction(ActionID.FeatherRain, BLUActionType.Magical);

    public static IBLUAction Eruption { get; } = new BLUAction(ActionID.Eruption, BLUActionType.Magical);

    public static IBLUAction MountainBuster { get; } = new BLUAction(ActionID.MountainBusterBLU, BLUActionType.Magical);

    public static IBLUAction ShockStrike { get; } = new BLUAction(ActionID.ShockStrike, BLUActionType.Magical);

    public static IBLUAction GlassDance { get; } = new BLUAction(ActionID.GlassDance, BLUActionType.Magical);

    public static IBLUAction AlpineDraft { get; } = new BLUAction(ActionID.AlpineDraft, BLUActionType.Magical);

    public static IBLUAction ProteanWave { get; } = new BLUAction(ActionID.ProteanWave, BLUActionType.Magical);

    public static IBLUAction Northerlies { get; } = new BLUAction(ActionID.Northerlies, BLUActionType.Magical);

    public static IBLUAction Electrogenesis { get; } = new BLUAction(ActionID.Electrogenesis, BLUActionType.Magical);

    public static IBLUAction MagicHammer { get; } = new BLUAction(ActionID.MagicHammer, BLUActionType.Magical, isTimeline: true);

    public static IBLUAction WhiteKnightsTour { get; } = new BLUAction(ActionID.WhiteKnightsTour, BLUActionType.Magical);

    public static IBLUAction BlackKnightsTour { get; } = new BLUAction(ActionID.BlackKnightsTour, BLUActionType.Magical);

    public static IBLUAction Surpanakha { get; } = new BLUAction(ActionID.Surpanakha, BLUActionType.Magical);

    public static IBLUAction Quasar { get; } = new BLUAction(ActionID.Quasar, BLUActionType.Magical);

    public static IBLUAction Tingle { get; } = new BLUAction(ActionID.Tingle, BLUActionType.Magical)
    {
        StatusProvide = new StatusID[] { StatusID.Tingling },
    };

    public static IBLUAction Tatamigaeshi { get; } = new BLUAction(ActionID.Tatamigaeshi, BLUActionType.Magical);

    public static IBLUAction SaintlyBeam { get; } = new BLUAction(ActionID.SaintlyBeam, BLUActionType.Magical);

    public static IBLUAction FeculentFlood { get; } = new BLUAction(ActionID.FeculentFlood, BLUActionType.Magical);

    public static IBLUAction Blaze { get; } = new BLUAction(ActionID.Blaze, BLUActionType.Magical);

    public static IBLUAction MustardBomb { get; } = new BLUAction(ActionID.MustardBomb, BLUActionType.Magical);

    public static IBLUAction AetherialSpark { get; } = new BLUAction(ActionID.AetherialSpark, BLUActionType.Magical, isEot: true);

    public static IBLUAction HydroPull { get; } = new BLUAction(ActionID.HydroPull, BLUActionType.Magical);

    public static IBLUAction MaledictionOfWater { get; } = new BLUAction(ActionID.MaledictionOfWater, BLUActionType.Magical);

    public static IBLUAction ChocoMeteor { get; } = new BLUAction(ActionID.ChocoMeteor, BLUActionType.Magical);

    public static IBLUAction NightBloom { get; } = new BLUAction(ActionID.NightBloom, BLUActionType.Magical);

    public static IBLUAction DivineCataract { get; } = new BLUAction(ActionID.DivineCataract, BLUActionType.Magical)
    {
        ActionCheck = b => Player.HasStatus(true, StatusID.AuspiciousTrance)
    };

    public static IBLUAction PhantomFlurry2 { get; } = new BLUAction(ActionID.PhantomFlurry2, BLUActionType.Magical)
    {
        ActionCheck = b => Player.HasStatus(true, StatusID.PhantomFlurry)
    };
    #endregion

    #region Physical Single
    public static IBLUAction FinalSting { get; } = new BLUAction(ActionID.FinalSting, BLUActionType.Physical)
    {
        ActionCheck = b => !Player.HasStatus(true, StatusID.BrushWithDeath),
    };

    public static IBLUAction SharpenedKnife { get; } = new BLUAction(ActionID.SharpenedKnife, BLUActionType.Physical);

    public static IBLUAction FlyingSardine { get; } = new BLUAction(ActionID.FlyingSardine, BLUActionType.Physical);

    public static IBLUAction AbyssalTransfixion { get; } = new BLUAction(ActionID.AbyssalTransfixion, BLUActionType.Physical);

    public static IBLUAction TripleTrident { get; } = new BLUAction(ActionID.TripleTrident, BLUActionType.Physical);

    public static IBLUAction RevengeBlast { get; } = new BLUAction(ActionID.RevengeBlast, BLUActionType.Physical)
    {
        ActionCheck = b => b.GetHealthRatio() < 0.2f,
    };
    #endregion

    #region Physical Area
    public static IBLUAction FlyingFrenzy { get; } = new BLUAction(ActionID.FlyingFrenzy, BLUActionType.Physical);

    public static IBLUAction DrillCannons { get; } = new BLUAction(ActionID.DrillCannons, BLUActionType.Physical);

    public static IBLUAction Weight4Tonze { get; } = new BLUAction(ActionID.Weight4Tonze, BLUActionType.Physical);

    public static IBLUAction Needles1000 { get; } = new BLUAction(ActionID.Needles1000, BLUActionType.Physical);

    public static IBLUAction Kaltstrahl { get; } = new BLUAction(ActionID.Kaltstrahl, BLUActionType.Physical);

    public static IBLUAction JKick { get; } = new BLUAction(ActionID.JKick, BLUActionType.Physical);

    public static IBLUAction PeripheralSynthesis { get; } = new BLUAction(ActionID.PeripheralSynthesis, BLUActionType.Physical);

    public static IBLUAction BothEnds { get; } = new BLUAction(ActionID.BothEnds, BLUActionType.Physical);
    #endregion

    #region Other Single
    public static IBLUAction StickyTongue { get; } = new BLUAction(ActionID.StickyTongue, BLUActionType.None);

    public static IBLUAction Missile { get; } = new BLUAction(ActionID.Missile, BLUActionType.None);

    public static IBLUAction TailScrew { get; } = new BLUAction(ActionID.TailScrew, BLUActionType.None);

    public static IBLUAction Doom { get; } = new BLUAction(ActionID.Doom, BLUActionType.None);

    public static IBLUAction EerieSoundwave { get; } = new BLUAction(ActionID.EerieSoundwave, BLUActionType.None);

    public static IBLUAction CondensedLibra { get; } = new BLUAction(ActionID.CondensedLibra, BLUActionType.None);
    #endregion

    #region Other Area
    public static IBLUAction Level5Petrify { get; } = new BLUAction(ActionID.Level5Petrify, BLUActionType.None);

    public static IBLUAction AcornBomb { get; } = new BLUAction(ActionID.AcornBomb, BLUActionType.None);

    public static IBLUAction BombToss { get; } = new BLUAction(ActionID.BombToss, BLUActionType.None);

    public static IBLUAction SelfDestruct { get; } = new BLUAction(ActionID.SelfDestruct, BLUActionType.None)
    {
        ActionCheck = b => !Player.HasStatus(true, StatusID.BrushWithDeath),
    };

    public static IBLUAction Faze { get; } = new BLUAction(ActionID.Faze, BLUActionType.None);

    public static IBLUAction Snort { get; } = new BLUAction(ActionID.Snort, BLUActionType.None);

    public static IBLUAction BadBreath { get; } = new BLUAction(ActionID.BadBreath, BLUActionType.None, isTimeline: true);

    public static IBLUAction Chirp { get; } = new BLUAction(ActionID.Chirp, BLUActionType.None);

    public static IBLUAction FrogLegs { get; } = new BLUAction(ActionID.FrogLegs, BLUActionType.None);

    public static IBLUAction Level5Death { get; } = new BLUAction(ActionID.Level5Death, BLUActionType.None);

    public static IBLUAction Launcher { get; } = new BLUAction(ActionID.Launcher, BLUActionType.None);

    public static IBLUAction UltraVibration { get; } = new BLUAction(ActionID.UltraVibration, BLUActionType.None);

    public static IBLUAction PhantomFlurry { get; } = new BLUAction(ActionID.PhantomFlurry, BLUActionType.None);
    #endregion

    #region Defense
    public static IBLUAction IceSpikes { get; } = new BLUAction(ActionID.IceSpikes, BLUActionType.None, true);

    public static IBLUAction VeilOfTheWhorl { get; } = new BLUAction(ActionID.VeilOfTheWhorl, BLUActionType.None, true);

    public static IBLUAction Diamondback { get; } = new BLUAction(ActionID.Diamondback, BLUActionType.None, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBLUAction Gobskin { get; } = new BLUAction(ActionID.Gobskin, BLUActionType.None, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBLUAction Cactguard { get; } = new BLUAction(ActionID.CactGuard, BLUActionType.None, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBLUAction ChelonianGate { get; } = new BLUAction(ActionID.ChelonianGate, BLUActionType.None, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    public static IBLUAction DragonForce { get; } = new BLUAction(ActionID.DragonForce, BLUActionType.None, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };
    #endregion

    #region Support
    public static IBLUAction ToadOil { get; } = new BLUAction(ActionID.ToadOil, BLUActionType.None, true);

    public static IBLUAction Bristle { get; } = new BLUAction(ActionID.Bristle, BLUActionType.Magical, true)
    {
        StatusProvide = new StatusID[] { StatusID.Boost, StatusID.Harmonized },
    };

    public static IBLUAction OffGuard { get; } = new BLUAction(ActionID.OffGuard, BLUActionType.None, true);

    public static IBLUAction MightyGuard { get; } = new BLUAction(ActionID.MightyGuard, BLUActionType.None, true)
    {
        StatusProvide = new StatusID[]
        {
            StatusID.MightyGuard,
        },
    };

    public static IBLUAction MoonFlute { get; } = new BLUAction(ActionID.MoonFlute, BLUActionType.None, true)
    {
        StatusProvide = new StatusID[] { StatusID.WaxingNocturne },
    };

    public static IBLUAction PeculiarLight { get; } = new BLUAction(ActionID.PeculiarLight, BLUActionType.Magical);

    public static IBLUAction Avail { get; } = new BLUAction(ActionID.Avail, BLUActionType.Magical);

    public static IBLUAction Whistle { get; } = new BLUAction(ActionID.Whistle, BLUActionType.Physical, true)
    {
        StatusProvide = new StatusID[] { StatusID.Boost, StatusID.Harmonized },
    };

    public static IBLUAction ColdFog { get; } = new BLUAction(ActionID.ColdFog, BLUActionType.Magical, true);
    #endregion

    #region Heal
    public static IBLUAction WhiteWind { get; } = new BLUAction(ActionID.WhiteWind, BLUActionType.None, true)
    {
        ActionCheck = b => Player.GetHealthRatio() is > 0.3f and < 0.5f,
    };

    public static IBLUAction Stotram { get; } = new BLUAction(ActionID.Stotram, BLUActionType.Magical, true);

    public static IBLUAction PomCure { get; } = new BLUAction(ActionID.PomCure, BLUActionType.None, true);

    public static IBLUAction AngelWhisper { get; } = new BLUAction(ActionID.AngelWhisper, BLUActionType.None, true);

    public static IBLUAction Exuviation { get; } = new BLUAction(ActionID.Exuviation, BLUActionType.None, true);

    public static IBLUAction AngelsSnack { get; } = new BLUAction(ActionID.AngelsSnack, BLUActionType.None, true);
    #endregion

    #region Others
    private static IBLUAction Loom { get; } = new BLUAction(ActionID.Loom, BLUActionType.None, shouldEndSpecial: true);

    public static IBLUAction BasicInstinct { get; } = new BLUAction(ActionID.BasicInstinct, BLUActionType.None)
    {
        StatusProvide = new StatusID[] { StatusID.BasicInstinct },
        ActionCheck = b =>
        {
            //TODO: 还需要判断是否为多人本
            return Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty56]
                && DataCenter.PartyMembers.Count(p => p.GetHealthRatio() > 0) == 1;
        },
    };

    private static IBLUAction AethericMimicry { get; } = new BLUAction(ActionID.AethericMimicry, BLUActionType.None, true)
    {
        ChoiceTarget = (charas, mustUse) =>
        {
            switch (BlueId)
            {
                case BLUID.DPS:
                    if (!Player.HasStatus(true, StatusID.AethericMimicryDPS))
                    {
                        return charas.GetJobCategory(JobRole.Melee, JobRole.RangedMagical, JobRole.RangedPhysical).FirstOrDefault();
                    }
                    break;

                case BLUID.Tank:
                    if (!Player.HasStatus(true, StatusID.AethericMimicryTank))
                    {
                        return charas.GetJobCategory(JobRole.Tank).FirstOrDefault();
                    }
                    break;

                case BLUID.Healer:
                    if (!Player.HasStatus(true, StatusID.AethericMimicryHealer))
                    {
                        return charas.GetJobCategory(JobRole.Healer).FirstOrDefault();
                    }
                    break;
            }
            return null;
        },
    };

    #endregion

    protected override bool MoveForwardGCD(out IAction act)
    {
        if (Loom.CanUse(out act)) return true;
        return base.MoveForwardGCD(out act);
    }


    protected override bool EmergencyGCD(out IAction act)
    {
        if (AethericMimicry.CanUse(out act)) return true;
        if (BlueId == BLUID.Healer)
        {
            //Esuna
            if (DataCenter.SpecialType == SpecialCommandType.EsunaStanceNorth && DataCenter.WeakenPeople.Any() || DataCenter.DyingPeople.Any())
            {
                if (Exuviation.CanUse(out act, CanUseOption.MustUse)) return true;
            }
        }
        if (BasicInstinct.CanUse(out _))
        {
            if (MightyGuard.CanUse(out act)) return true;
            act = BasicInstinct;
            return true;
        }

        return base.EmergencyGCD(out act);
    }

    protected static bool AllOnSlot(params IBLUAction[] actions) => actions.All(a => a.OnSlot);
    protected static uint OnSlotCount(params IBLUAction[] actions) => (uint)actions.Count(a => a.OnSlot);

    public override IBaseAction[] AllBaseActions => base.AllBaseActions.Where(a =>
    {
        if (a is not BLUAction b) return false;
        return b.OnSlot;
    }).ToArray();

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetCombo("BlueId", 2, "Role", "Tank", "Healer", "DPS")
            .SetCombo("AttackType", 2, "Type", "Magic", "Physic", "Both");
    }

    protected override void UpdateInfo()
    {
        BlueId = (BLUID)Configs.GetCombo("BlueId");
        AttackType = (BLUAttackType)Configs.GetCombo("AttackType");
        base.UpdateInfo();
    }

    protected override bool HealSingleGCD(out IAction act)
    {
        if (BlueId == BLUID.Healer)
        {
            if (PomCure.CanUse(out act)) return true;
        }
        if (WhiteWind.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.HealSingleGCD(out act);
    }

    protected override bool HealAreaGCD(out IAction act)
    {
        if (BlueId == BLUID.Healer)
        {
            if (AngelsSnack.CanUse(out act)) return true;
            if (Stotram.CanUse(out act)) return true;
        }

        if (WhiteWind.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.HealAreaGCD(out act);
    }
}
