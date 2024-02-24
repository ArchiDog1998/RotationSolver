namespace RotationSolver.Basic.Rotations.Basic;

partial class BlueMageRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Intelligence;

    /// <summary>
    /// Tye ID card for Blu.
    /// </summary>
    public enum BLUID : byte
    {
        /// <summary>
        /// 
        /// </summary>
        Tank,

        /// <summary>
        /// 
        /// </summary>
        Healer,

        /// <summary>
        /// 
        /// </summary>
        DPS,
    }

    /// <summary>
    /// 
    /// </summary>
    protected BLUID BlueId { get; set; } = BLUID.DPS;

    private protected sealed override IBaseAction Raise => AngelWhisperPvE;

    static partial void ModifyFeatherRainPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
    }

    static partial void ModifyEruptionPvE(ref ActionSetting setting)
    {
        setting.CreateConfig = () => new()
        {
            AoeCount = 1,
        };
    }
}

///// <summary>
///// The BLU Action.
///// </summary>
//public interface IBLUAction : IBaseAction
//{
//    /// <summary>
//    /// Is on the slot.
//    /// </summary>
//    bool OnSlot { get; }

//    /// <summary>
//    /// Is right type.
//    /// </summary>
//    bool RightType { get; }
//}

///// <summary>
///// The base class about <see href="https://na.finalfantasyxiv.com/jobguide/bluemage/">Blue Mage</see>.
///// </summary>
//public abstract class BLU_Base : CustomRotation
//{

//    #region Magical Single

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction SongOfTorment { get; } = new BLUAction(ActionID.SongOfTorment, ActionOption.Dot)
//    {
//        TargetStatus = new[] { StatusID.Bleeding }
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction BloodDrain { get; } = new BLUAction(ActionID.BloodDrain)
//    {
//        ActionCheck = (b, m) => Player.CurrentMp <= 9500,
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction SonicBoom { get; } = new BLUAction(ActionID.SonicBoom);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction PerpetualRay { get; } = new BLUAction(ActionID.PerpetualRay);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Reflux { get; } = new BLUAction(ActionID.Reflux);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Devour { get; } = new BLUAction(ActionID.Devour, ActionOption.Heal);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction TheRoseOfDestruction { get; } = new BLUAction(ActionID.TheRoseOfDestruction);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction MatraMagic { get; } = new BLUAction(ActionID.MatraMagic);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction WhiteDeath { get; } = new BLUAction(ActionID.WhiteDeath)
//    {
//        ActionCheck = (b, m) => Player.HasStatus(true, StatusID.TouchOfFrost)
//    };
//    #endregion

//    #region Magical Area
//    /// <summary>
//    /// <see href="https://ffxiv.consolegameswiki.com/wiki/Flame_Thrower"/>
//    /// </summary>
//    public static IBLUAction FlameThrower { get; } = new BLUAction(ActionID.FlameThrower);

//    /// <summary>
//    /// <see href="https://ffxiv.consolegameswiki.com/wiki/Aqua_Breath"/>
//    /// </summary>
//    public static IBLUAction AquaBreath { get; } = new BLUAction(ActionID.AquaBreath, ActionOption.Dot);

//    /// <summary>
//    /// <see href="https://ffxiv.consolegameswiki.com/wiki/High_Voltage"/>
//    /// </summary>
//    public static IBLUAction HighVoltage { get; } = new BLUAction(ActionID.HighVoltage);

//    /// <summary>
//    /// <see href="https://ffxiv.consolegameswiki.com/wiki/Glower"/>
//    /// </summary>
//    public static IBLUAction Glower { get; } = new BLUAction(ActionID.Glower);

//    /// <summary>
//    /// <see href="https://ffxiv.consolegameswiki.com/wiki/Plaincracker"/>
//    /// </summary>
//    public static IBLUAction PlainCracker { get; } = new BLUAction(ActionID.Plaincracker);

//    /// <summary>
//    /// <see href="https://ffxiv.consolegameswiki.com/wiki/The_Look"/>
//    /// </summary>
//    public static IBLUAction TheLook { get; } = new BLUAction(ActionID.TheLook);

//    /// <summary>
//    /// <see href="https://ffxiv.consolegameswiki.com/wiki/The_Ram%27s_Voice"/>
//    /// </summary>
//    public static IBLUAction TheRamVoice { get; } = new BLUAction(ActionID.TheRamsVoice);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction TheDragonVoice { get; } = new BLUAction(ActionID.TheDragonsVoice);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction InkJet { get; } = new BLUAction(ActionID.InkJet);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction FireAngon { get; } = new BLUAction(ActionID.FireAngon);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction MindBlast { get; } = new BLUAction(ActionID.MindBlast);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction FeatherRain { get; } = new BLUAction(ActionID.FeatherRain);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Eruption { get; } = new BLUAction(ActionID.Eruption);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction MountainBuster { get; } = new BLUAction(ActionID.MountainBuster);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction ShockStrike { get; } = new BLUAction(ActionID.ShockStrike);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction GlassDance { get; } = new BLUAction(ActionID.GlassDance);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction AlpineDraft { get; } = new BLUAction(ActionID.AlpineDraft);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction ProteanWave { get; } = new BLUAction(ActionID.ProteanWave);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Northerlies { get; } = new BLUAction(ActionID.Northerlies);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Electrogenesis { get; } = new BLUAction(ActionID.Electrogenesis);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction MagicHammer { get; } = new BLUAction(ActionID.MagicHammer, ActionOption.Defense);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction WhiteKnightsTour { get; } = new BLUAction(ActionID.WhiteKnightsTour);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction BlackKnightsTour { get; } = new BLUAction(ActionID.BlackKnightsTour);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Surpanakha { get; } = new BLUAction(ActionID.Surpanakha);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Quasar { get; } = new BLUAction(ActionID.Quasar);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Tingle { get; } = new BLUAction(ActionID.Tingle)
//    {
//        StatusProvide = new StatusID[] { StatusID.Tingling },
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Tatamigaeshi { get; } = new BLUAction(ActionID.Tatamigaeshi);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction SaintlyBeam { get; } = new BLUAction(ActionID.SaintlyBeam);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction FeculentFlood { get; } = new BLUAction(ActionID.FeculentFlood);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Blaze { get; } = new BLUAction(ActionID.Blaze);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction MustardBomb { get; } = new BLUAction(ActionID.MustardBomb);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction AetherialSpark { get; } = new BLUAction(ActionID.AetherialSpark, ActionOption.Dot);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction HydroPull { get; } = new BLUAction(ActionID.HydroPull);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction MaledictionOfWater { get; } = new BLUAction(ActionID.MaledictionOfWater);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction ChocoMeteor { get; } = new BLUAction(ActionID.ChocoMeteor);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction NightBloom { get; } = new BLUAction(ActionID.Nightbloom);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction DivineCataract { get; } = new BLUAction(ActionID.DivineCataract)
//    {
//        ActionCheck = (b, m) => Player.HasStatus(true, StatusID.AuspiciousTrance)
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction PhantomFlurry2 { get; } = new BLUAction(ActionID.PhantomFlurry_23289)
//    {
//        ActionCheck = (b, m) => Player.HasStatus(true, StatusID.PhantomFlurry)
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction PeatPelt { get; } = new BLUAction(ActionID.PeatPelt);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction MortalFlame { get; } = new BLUAction(ActionID.MortalFlame, ActionOption.Dot);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction SeaShanty { get; } = new BLUAction(ActionID.SeaShanty);

//    #endregion

//    #region Physical Single
//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction FinalSting { get; } = new BLUAction(ActionID.FinalSting)
//    {
//        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.BrushWithDeath),
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction SharpenedKnife { get; } = new BLUAction(ActionID.SharpenedKnife);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction FlyingSardine { get; } = new BLUAction(ActionID.FlyingSardine)
//    {
//        FilterForHostiles = b => b.Where(ObjectHelper.CanInterrupt),
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction AbyssalTransfixion { get; } = new BLUAction(ActionID.AbyssalTransfixion);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction TripleTrident { get; } = new BLUAction(ActionID.TripleTrident);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction RevengeBlast { get; } = new BLUAction(ActionID.RevengeBlast)
//    {
//        ActionCheck = (b, m) => b.GetHealthRatio() < 0.2f,
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction GoblinPunch { get; } = new BLUAction(ActionID.GoblinPunch);

//    #endregion

//    #region Physical Area
//    /// <summary>
//    /// <see href="https://ffxiv.consolegameswiki.com/wiki/Flying_Frenzy"/>
//    /// </summary>
//    public static IBLUAction FlyingFrenzy { get; } = new BLUAction(ActionID.FlyingFrenzy);

//    /// <summary>
//    /// <see href="https://ffxiv.consolegameswiki.com/wiki/Drill_Cannons"/>
//    /// </summary>
//    public static IBLUAction DrillCannons { get; } = new BLUAction(ActionID.DrillCannons);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Weight4Tonze { get; } = new BLUAction(ActionID._4TonzeWeight);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Needles1000 { get; } = new BLUAction(ActionID._1000Needles);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Kaltstrahl { get; } = new BLUAction(ActionID.Kaltstrahl);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction JKick { get; } = new BLUAction(ActionID.JKick);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction PeripheralSynthesis { get; } = new BLUAction(ActionID.PeripheralSynthesis);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction BothEnds { get; } = new BLUAction(ActionID.BothEnds);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction RightRound { get; } = new BLUAction(ActionID.RightRound);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction WildRage { get; } = new BLUAction(ActionID.WildRage);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction DeepClean { get; } = new BLUAction(ActionID.DeepClean);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction RubyDynamics { get; } = new BLUAction(ActionID.RubyDynamics);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction WingedReprobation { get; } = new BLUAction(ActionID.WingedReprobation);
//    #endregion

//    #region Other Single
//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction StickyTongue { get; } = new BLUAction(ActionID.StickyTongue);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Missile { get; } = new BLUAction(ActionID.Missile);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction TailScrew { get; } = new BLUAction(ActionID.TailScrew);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Doom { get; } = new BLUAction(ActionID.Doom);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction EerieSoundwave { get; } = new BLUAction(ActionID.EerieSoundwave);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction CondensedLibra { get; } = new BLUAction(ActionID.CondensedLibra);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Schiltron { get; } = new BLUAction(ActionID.Schiltron);
//    #endregion

//    #region Other Area
//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Level5Petrify { get; } = new BLUAction(ActionID.Level5Petrify);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction AcornBomb { get; } = new BLUAction(ActionID.AcornBomb);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction BombToss { get; } = new BLUAction(ActionID.BombToss);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction SelfDestruct { get; } = new BLUAction(ActionID.Selfdestruct)
//    {
//        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.BrushWithDeath),
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Faze { get; } = new BLUAction(ActionID.Faze);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Snort { get; } = new BLUAction(ActionID.Snort);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction BadBreath { get; } = new BLUAction(ActionID.BadBreath, ActionOption.Defense);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Chirp { get; } = new BLUAction(ActionID.Chirp);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction FrogLegs { get; } = new BLUAction(ActionID.FrogLegs);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Level5Death { get; } = new BLUAction(ActionID.Level5Death);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Launcher { get; } = new BLUAction(ActionID.Launcher);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction UltraVibration { get; } = new BLUAction(ActionID.Ultravibration);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction PhantomFlurry { get; } = new BLUAction(ActionID.PhantomFlurry);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction BreathOfMagic { get; } = new BLUAction(ActionID.BreathOfMagic);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction DivinationRune { get; } = new BLUAction(ActionID.DivinationRune);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction DimensionalShift { get; } = new BLUAction(ActionID.DimensionalShift);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction ConvictionMarcato { get; } = new BLUAction(ActionID.ConvictionMarcato);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction LaserEye { get; } = new BLUAction(ActionID.LaserEye);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction CandyCane { get; } = new BLUAction(ActionID.CandyCane);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Apokalypsis { get; } = new BLUAction(ActionID.Apokalypsis);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction BeingMortal { get; } = new BLUAction(ActionID.BeingMortal);
//    #endregion

//    #region Defense
//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction IceSpikes { get; } = new BLUAction(ActionID.IceSpikes, ActionOption.Defense);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction VeilOfTheWhorl { get; } = new BLUAction(ActionID.VeilOfTheWhorl, ActionOption.Defense);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Diamondback { get; } = new BLUAction(ActionID.Diamondback, ActionOption.Defense)
//    {
//        StatusProvide = Rampart.StatusProvide,
//        ActionCheck = BaseAction.TankDefenseSelf,
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Gobskin { get; } = new BLUAction(ActionID.Gobskin, ActionOption.Defense)
//    {
//        StatusProvide = Rampart.StatusProvide,
//        ActionCheck = BaseAction.TankDefenseSelf,
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Cactguard { get; } = new BLUAction(ActionID.Cactguard, ActionOption.Defense)
//    {
//        StatusProvide = Rampart.StatusProvide,
//        ActionCheck = BaseAction.TankDefenseSelf,
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction ChelonianGate { get; } = new BLUAction(ActionID.ChelonianGate, ActionOption.Defense)
//    {
//        StatusProvide = Rampart.StatusProvide,
//        ActionCheck = BaseAction.TankDefenseSelf,
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction DragonForce { get; } = new BLUAction(ActionID.DragonForce, ActionOption.Defense)
//    {
//        StatusProvide = Rampart.StatusProvide,
//        ActionCheck = BaseAction.TankDefenseSelf,
//    };
//    #endregion

//    #region Support
//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction ToadOil { get; } = new BLUAction(ActionID.ToadOil, ActionOption.Buff);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Bristle { get; } = new BLUAction(ActionID.Bristle, ActionOption.Buff)
//    {
//        StatusProvide = new StatusID[] { StatusID.Boost, StatusID.Harmonized },
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction OffGuard { get; } = new BLUAction(ActionID.Offguard, ActionOption.Buff);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction MightyGuard { get; } = new BLUAction(ActionID.MightyGuard, ActionOption.Buff)
//    {
//        StatusProvide = new StatusID[]
//        {
//            StatusID.MightyGuard,
//        },
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction MoonFlute { get; } = new BLUAction(ActionID.MoonFlute, ActionOption.Buff)
//    {
//        StatusProvide = new StatusID[] { StatusID.WaxingNocturne },
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction PeculiarLight { get; } = new BLUAction(ActionID.PeculiarLight, ActionOption.Buff);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Avail { get; } = new BLUAction(ActionID.Avail, ActionOption.Buff);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Whistle { get; } = new BLUAction(ActionID.Whistle, ActionOption.Buff)
//    {
//        StatusProvide = new StatusID[] { StatusID.Boost, StatusID.Harmonized },
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction ColdFog { get; } = new BLUAction(ActionID.ColdFog, ActionOption.Buff);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction ForceField { get; } = new BLUAction(ActionID.ForceField, ActionOption.Buff);
//    #endregion

//    #region Heal
//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction WhiteWind { get; } = new BLUAction(ActionID.WhiteWind, ActionOption.Heal)
//    {
//        ActionCheck = (b, m) => Player.GetHealthRatio() is > 0.3f and < 0.5f,
//    };

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Stotram { get; } = new BLUAction(ActionID.Stotram, ActionOption.Heal);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction PomCure { get; } = new BLUAction(ActionID.PomCure, ActionOption.Heal);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction AngelWhisper { get; } = new BLUAction(ActionID.AngelWhisper, ActionOption.Heal);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Exuviation { get; } = new BLUAction(ActionID.Exuviation, ActionOption.Heal);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction AngelsSnack { get; } = new BLUAction(ActionID.AngelsSnack, ActionOption.Heal);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction Rehydration { get; } = new BLUAction(ActionID.Rehydration, ActionOption.Heal);
//    #endregion

//    #region Others
//    /// <summary>
//    /// 
//    /// </summary>
//    private static IBLUAction Loom { get; } = new BLUAction(ActionID.Loom, ActionOption.EndSpecial);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBLUAction BasicInstinct { get; } = new BLUAction(ActionID.BasicInstinct)
//    {
//        StatusProvide = new StatusID[] { StatusID.BasicInstinct },
//        ActionCheck = (b, m) => Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty56] && DataCenter.PartyMembers.Count(p => p.GetHealthRatio() > 0) == 1,
//    };

//    static IBaseAction AethericMimicry { get; } = new BaseAction(ActionID.AethericMimicry, ActionOption.Friendly)
//    {
//        ChoiceTarget = (charas, mustUse) =>
//        {
//            switch (BlueId)
//            {
//                case BLUID.DPS:
//                    if (!Player.HasStatus(true, StatusID.AethericMimicryDps))
//                    {
//                        return charas.GetJobCategory(JobRole.Melee, JobRole.RangedMagical, JobRole.RangedPhysical).FirstOrDefault();
//                    }
//                    break;

//                case BLUID.Tank:
//                    if (!Player.HasStatus(true, StatusID.AethericMimicryTank))
//                    {
//                        return charas.GetJobCategory(JobRole.Tank).FirstOrDefault();
//                    }
//                    break;

//                case BLUID.Healer:
//                    if (!Player.HasStatus(true, StatusID.AethericMimicryHealer))
//                    {
//                        return charas.GetJobCategory(JobRole.Healer).FirstOrDefault();
//                    }
//                    break;
//            }
//            return null;
//        },
//    };

//    #endregion

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="act"></param>
//    /// <returns></returns>
//    protected sealed override bool MoveForwardGCD(out IAction act)
//    {
//        if (JKick.CanUse(out act, CanUseOption.MustUse)) return true;
//        if (Loom.CanUse(out act)) return true;
//        return base.MoveForwardGCD(out act);
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="act"></param>
//    /// <returns></returns>
//    protected override bool EmergencyGCD(out IAction act)
//    {
//        if (AethericMimicry.CanUse(out act)) return true;
//        if (BlueId == BLUID.Healer)
//        {
//            //Esuna
//            if (DataCenter.IsEsunaStanceNorth && DataCenter.WeakenPeople.Any() || DataCenter.DyingPeople.Any())
//            {
//                if (Exuviation.CanUse(out act, CanUseOption.MustUse)) return true;
//            }
//        }
//        if (BasicInstinct.CanUse(out _))
//        {
//            if (MightyGuard.CanUse(out act)) return true;
//            act = BasicInstinct;
//            return true;
//        }

//        return base.EmergencyGCD(out act);
//    }

//    /// <summary>
//    /// All these actions are on slots.
//    /// </summary>
//    /// <param name="actions"></param>
//    /// <returns></returns>
//    protected static bool AllOnSlot(params IBLUAction[] actions) => actions.All(a => a.OnSlot);

//    /// <summary>
//    /// How many actions are on slots.
//    /// </summary>
//    /// <param name="actions"></param>
//    /// <returns></returns>
//    protected static uint OnSlotCount(params IBLUAction[] actions) => (uint)actions.Count(a => a.OnSlot);

//    /// <summary>
//    /// All base actions.
//    /// </summary>
//    public override IBaseAction[] AllBaseActions => base.AllBaseActions
//        .Where(a => a is not IBLUAction b || b.OnSlot).ToArray();

//    /// <summary>
//    /// Configurations.
//    /// </summary>
//    /// <returns></returns>
//    protected override IRotationConfigSet CreateConfiguration()
//    {
//        return base.CreateConfiguration()
//            .SetCombo(CombatType.PvE, "BlueId", 2, "Role", "Tank", "Healer", "DPS")
//            .SetCombo(CombatType.PvE, "AttackType", 2, "Type", "Both", "Magic", "Physic");
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    protected override void UpdateInfo()
//    {
//        BlueId = (BLUID)Configs.GetCombo("BlueId");
//        BluAttackType = (BLUAttackType)Configs.GetCombo("AttackType");
//        base.UpdateInfo();
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="act"></param>
//    /// <returns></returns>
//    protected override bool HealSingleGCD(out IAction act)
//    {
//        if (BlueId == BLUID.Healer)
//        {
//            if (PomCure.CanUse(out act)) return true;
//        }
//        if (WhiteWind.CanUse(out act, CanUseOption.MustUse)) return true;
//        return base.HealSingleGCD(out act);
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="act"></param>
//    /// <returns></returns>
//    protected override bool HealAreaGCD(out IAction act)
//    {
//        if (BlueId == BLUID.Healer)
//        {
//            if (AngelsSnack.CanUse(out act)) return true;
//            if (Stotram.CanUse(out act)) return true;
//        }

//        if (WhiteWind.CanUse(out act, CanUseOption.MustUse)) return true;
//        return base.HealAreaGCD(out act);
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public override void DisplayStatus()
//    {
//        ImGui.TextWrapped(BlueId.ToString());
//        base.DisplayStatus();
//    }
//}
