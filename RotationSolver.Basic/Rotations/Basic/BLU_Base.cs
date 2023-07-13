using Dalamud.Logging;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The BLU Action.
/// </summary>
public interface IBLUAction : IBaseAction
{
    /// <summary>
    /// Is on the slot.
    /// </summary>
    bool OnSlot { get; }

    /// <summary>
    /// Is right type.
    /// </summary>
    bool RightType { get; }
}

/// <summary>
/// The base class about blu.
/// </summary>
public abstract class BLU_Base : CustomRotation
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
    /// Attack Type.
    /// </summary>
    public enum BLUAttackType : byte
    {
        /// <summary>
        /// 
        /// </summary>
        Both,

        /// <summary>
        /// 
        /// </summary>
        Magical,

        /// <summary>
        /// 
        /// </summary>
        Physical,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum BLUActionType : byte
    {
        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        Magical,

        /// <summary>
        /// 
        /// </summary>
        Physical,
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new Job[] { Job.BLU };

    /// <summary>
    /// 
    /// </summary>
    protected static BLUAttackType AttackType { get; set; } = BLUAttackType.Both;

    /// <summary>
    /// 
    /// </summary>
    protected static BLUID BlueId { get; set; } = BLUID.DPS;

    private protected sealed override IBaseAction Raise => AngelWhisper;

    /// <summary>
    /// 
    /// </summary>
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

        readonly BLUActionType _type;

        /// <summary>
        /// 
        /// </summary>
        public bool RightType
        {
            get
            {
                if (_type == BLUActionType.None) return true;
                if (AttackType == BLUAttackType.Physical && _type == BLUActionType.Magical) return false;
                if (AttackType == BLUAttackType.Magical && _type == BLUActionType.Physical) return false;

                try
                {
                    if (Target.HasStatus(false, NoPhysic) && _type == BLUActionType.Physical) return false;
                    if (Target.HasStatus(false, NoMagic) && _type == BLUActionType.Magical) return false;
                }
                catch(Exception ex)
                {
                    PluginLog.Warning(ex, "Failed for checking target status.");
                }
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public unsafe bool OnSlot => DataCenter.BluSlots.Any(i => AdjustedID == Service.GetAdjustedActionId(i));

        internal BLUAction(ActionID actionID, BLUActionType type, ActionOption option = ActionOption.None)
            : base(actionID, option)
        {
            _type = type;
            ActionCheck = (t, m) => OnSlot && RightType;
        }

        /// <summary>
        /// Can this action be used.
        /// </summary>
        /// <param name="act"></param>
        /// <param name="option"></param>
        /// <param name="gcdCountForAbility"></param>
        /// <returns></returns>
        public override bool CanUse(out IAction act, CanUseOption option = CanUseOption.None, byte gcdCountForAbility = 0)
        {
            act = null;

            if (!OnSlot) return false;
            return base.CanUse(out act, option | CanUseOption.IgnoreClippingCheck, gcdCountForAbility);
        }
    }

    #region Magical Single
    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction WaterCannon { get; } = new BLUAction(ActionID.WaterCannon, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction SongOfTorment { get; } = new BLUAction(ActionID.SongOfTorment, BLUActionType.Magical, ActionOption.Dot)
    {
        TargetStatus = new[] { StatusID.Bleeding }
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction BloodDrain { get; } = new BLUAction(ActionID.BloodDrain, BLUActionType.Magical)
    {
        ActionCheck = (b, m) => Player.CurrentMp <= 9500,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction SonicBoom { get; } = new BLUAction(ActionID.SonicBoom, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction PerpetualRay { get; } = new BLUAction(ActionID.PerpetualRay, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Reflux { get; } = new BLUAction(ActionID.Reflux, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Devour { get; } = new BLUAction(ActionID.Devour, BLUActionType.Magical, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction TheRoseOfDestruction { get; } = new BLUAction(ActionID.TheRoseOfDestruction, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction MatraMagic { get; } = new BLUAction(ActionID.MatraMagic, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction WhiteDeath { get; } = new BLUAction(ActionID.WhiteDeath, BLUActionType.Magical)
    {
        ActionCheck = (b, m) => Player.HasStatus(true, StatusID.TouchOfFrost)
    };
    #endregion

    #region Magical Area
    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction FlameThrower { get; } = new BLUAction(ActionID.FlameThrower, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction AquaBreath { get; } = new BLUAction(ActionID.AquaBreath, BLUActionType.Magical, ActionOption.Dot);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction HighVoltage { get; } = new BLUAction(ActionID.HighVoltage, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Glower { get; } = new BLUAction(ActionID.Glower, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction PlainCracker { get; } = new BLUAction(ActionID.PlainCracker, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction TheLook { get; } = new BLUAction(ActionID.TheLook, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction TheRamVoice { get; } = new BLUAction(ActionID.TheRamVoice, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction TheDragonVoice { get; } = new BLUAction(ActionID.TheDragonVoice, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction InkJet { get; } = new BLUAction(ActionID.InkJet, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction FireAngon { get; } = new BLUAction(ActionID.FireAngon, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction MindBlast { get; } = new BLUAction(ActionID.MindBlast, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction FeatherRain { get; } = new BLUAction(ActionID.FeatherRain, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Eruption { get; } = new BLUAction(ActionID.Eruption, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction MountainBuster { get; } = new BLUAction(ActionID.MountainBusterBLU, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction ShockStrike { get; } = new BLUAction(ActionID.ShockStrike, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction GlassDance { get; } = new BLUAction(ActionID.GlassDance, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction AlpineDraft { get; } = new BLUAction(ActionID.AlpineDraft, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction ProteanWave { get; } = new BLUAction(ActionID.ProteanWave, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Northerlies { get; } = new BLUAction(ActionID.Northerlies, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Electrogenesis { get; } = new BLUAction(ActionID.Electrogenesis, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction MagicHammer { get; } = new BLUAction(ActionID.MagicHammer, BLUActionType.Magical, ActionOption.Defense);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction WhiteKnightsTour { get; } = new BLUAction(ActionID.WhiteKnightsTour, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction BlackKnightsTour { get; } = new BLUAction(ActionID.BlackKnightsTour, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Surpanakha { get; } = new BLUAction(ActionID.Surpanakha, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Quasar { get; } = new BLUAction(ActionID.Quasar, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Tingle { get; } = new BLUAction(ActionID.Tingle, BLUActionType.Magical)
    {
        StatusProvide = new StatusID[] { StatusID.Tingling },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Tatamigaeshi { get; } = new BLUAction(ActionID.Tatamigaeshi, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction SaintlyBeam { get; } = new BLUAction(ActionID.SaintlyBeam, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction FeculentFlood { get; } = new BLUAction(ActionID.FeculentFlood, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Blaze { get; } = new BLUAction(ActionID.Blaze, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction MustardBomb { get; } = new BLUAction(ActionID.MustardBomb, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction AetherialSpark { get; } = new BLUAction(ActionID.AetherialSpark, BLUActionType.Magical, ActionOption.Dot);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction HydroPull { get; } = new BLUAction(ActionID.HydroPull, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction MaledictionOfWater { get; } = new BLUAction(ActionID.MaledictionOfWater, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction ChocoMeteor { get; } = new BLUAction(ActionID.ChocoMeteor, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction NightBloom { get; } = new BLUAction(ActionID.NightBloom, BLUActionType.Magical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction DivineCataract { get; } = new BLUAction(ActionID.DivineCataract, BLUActionType.Magical)
    {
        ActionCheck = (b, m) => Player.HasStatus(true, StatusID.AuspiciousTrance)
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction PhantomFlurry2 { get; } = new BLUAction(ActionID.PhantomFlurry2, BLUActionType.Magical)
    {
        ActionCheck = (b, m) => Player.HasStatus(true, StatusID.PhantomFlurry)
    };
    #endregion

    #region Physical Single
    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction FinalSting { get; } = new BLUAction(ActionID.FinalSting, BLUActionType.Physical)
    {
        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.BrushWithDeath),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction SharpenedKnife { get; } = new BLUAction(ActionID.SharpenedKnife, BLUActionType.Physical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction FlyingSardine { get; } = new BLUAction(ActionID.FlyingSardine, BLUActionType.Physical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction AbyssalTransfixion { get; } = new BLUAction(ActionID.AbyssalTransfixion, BLUActionType.Physical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction TripleTrident { get; } = new BLUAction(ActionID.TripleTrident, BLUActionType.Physical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction RevengeBlast { get; } = new BLUAction(ActionID.RevengeBlast, BLUActionType.Physical)
    {
        ActionCheck = (b, m) => b.GetHealthRatio() < 0.2f,
    };
    #endregion

    #region Physical Area
    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction FlyingFrenzy { get; } = new BLUAction(ActionID.FlyingFrenzy, BLUActionType.Physical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction DrillCannons { get; } = new BLUAction(ActionID.DrillCannons, BLUActionType.Physical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Weight4Tonze { get; } = new BLUAction(ActionID.Weight4Tonze, BLUActionType.Physical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Needles1000 { get; } = new BLUAction(ActionID.Needles1000, BLUActionType.Physical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Kaltstrahl { get; } = new BLUAction(ActionID.Kaltstrahl, BLUActionType.Physical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction JKick { get; } = new BLUAction(ActionID.JKick, BLUActionType.Physical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction PeripheralSynthesis { get; } = new BLUAction(ActionID.PeripheralSynthesis, BLUActionType.Physical);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction BothEnds { get; } = new BLUAction(ActionID.BothEnds, BLUActionType.Physical);
    #endregion

    #region Other Single
    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction StickyTongue { get; } = new BLUAction(ActionID.StickyTongue, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Missile { get; } = new BLUAction(ActionID.Missile, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction TailScrew { get; } = new BLUAction(ActionID.TailScrew, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Doom { get; } = new BLUAction(ActionID.Doom, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction EerieSoundwave { get; } = new BLUAction(ActionID.EerieSoundwave, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction CondensedLibra { get; } = new BLUAction(ActionID.CondensedLibra, BLUActionType.None);
    #endregion

    #region Other Area
    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Level5Petrify { get; } = new BLUAction(ActionID.Level5Petrify, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction AcornBomb { get; } = new BLUAction(ActionID.AcornBomb, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction BombToss { get; } = new BLUAction(ActionID.BombToss, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction SelfDestruct { get; } = new BLUAction(ActionID.SelfDestruct, BLUActionType.None)
    {
        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.BrushWithDeath),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Faze { get; } = new BLUAction(ActionID.Faze, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Snort { get; } = new BLUAction(ActionID.Snort, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction BadBreath { get; } = new BLUAction(ActionID.BadBreath, BLUActionType.None, ActionOption.Defense);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Chirp { get; } = new BLUAction(ActionID.Chirp, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction FrogLegs { get; } = new BLUAction(ActionID.FrogLegs, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Level5Death { get; } = new BLUAction(ActionID.Level5Death, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Launcher { get; } = new BLUAction(ActionID.Launcher, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction UltraVibration { get; } = new BLUAction(ActionID.UltraVibration, BLUActionType.None);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction PhantomFlurry { get; } = new BLUAction(ActionID.PhantomFlurry, BLUActionType.None);
    #endregion

    #region Defense
    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction IceSpikes { get; } = new BLUAction(ActionID.IceSpikes, BLUActionType.None, ActionOption.Defense);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction VeilOfTheWhorl { get; } = new BLUAction(ActionID.VeilOfTheWhorl, BLUActionType.None, ActionOption.Defense);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Diamondback { get; } = new BLUAction(ActionID.Diamondback, BLUActionType.None, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Gobskin { get; } = new BLUAction(ActionID.Gobskin, BLUActionType.None, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Cactguard { get; } = new BLUAction(ActionID.CactGuard, BLUActionType.None, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction ChelonianGate { get; } = new BLUAction(ActionID.ChelonianGate, BLUActionType.None, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction DragonForce { get; } = new BLUAction(ActionID.DragonForce, BLUActionType.None, ActionOption.Defense)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };
    #endregion

    #region Support
    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction ToadOil { get; } = new BLUAction(ActionID.ToadOil, BLUActionType.None, ActionOption.Buff);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Bristle { get; } = new BLUAction(ActionID.Bristle, BLUActionType.Magical, ActionOption.Buff)
    {
        StatusProvide = new StatusID[] { StatusID.Boost, StatusID.Harmonized },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction OffGuard { get; } = new BLUAction(ActionID.OffGuard, BLUActionType.None, ActionOption.Buff);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction MightyGuard { get; } = new BLUAction(ActionID.MightyGuard, BLUActionType.None, ActionOption.Buff)
    {
        StatusProvide = new StatusID[]
        {
            StatusID.MightyGuard,
        },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction MoonFlute { get; } = new BLUAction(ActionID.MoonFlute, BLUActionType.None, ActionOption.Buff)
    {
        StatusProvide = new StatusID[] { StatusID.WaxingNocturne },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction PeculiarLight { get; } = new BLUAction(ActionID.PeculiarLight, BLUActionType.Magical, ActionOption.Buff);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Avail { get; } = new BLUAction(ActionID.Avail, BLUActionType.Magical, ActionOption.Buff);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Whistle { get; } = new BLUAction(ActionID.Whistle, BLUActionType.Physical, ActionOption.Buff)
    {
        StatusProvide = new StatusID[] { StatusID.Boost, StatusID.Harmonized },
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction ColdFog { get; } = new BLUAction(ActionID.ColdFog, BLUActionType.Magical, ActionOption.Buff);
    #endregion

    #region Heal
    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction WhiteWind { get; } = new BLUAction(ActionID.WhiteWind, BLUActionType.None, ActionOption.Heal)
    {
        ActionCheck = (b, m) => Player.GetHealthRatio() is > 0.3f and < 0.5f,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Stotram { get; } = new BLUAction(ActionID.Stotram, BLUActionType.Magical, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction PomCure { get; } = new BLUAction(ActionID.PomCure, BLUActionType.None, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction AngelWhisper { get; } = new BLUAction(ActionID.AngelWhisper, BLUActionType.None, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction Exuviation { get; } = new BLUAction(ActionID.Exuviation, BLUActionType.None, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction AngelsSnack { get; } = new BLUAction(ActionID.AngelsSnack, BLUActionType.None, ActionOption.Heal);
    #endregion

    #region Others
    /// <summary>
    /// 
    /// </summary>
    private static IBLUAction Loom { get; } = new BLUAction(ActionID.Loom, BLUActionType.None, ActionOption.EndSpecial);

    /// <summary>
    /// 
    /// </summary>
    public static IBLUAction BasicInstinct { get; } = new BLUAction(ActionID.BasicInstinct, BLUActionType.None)
    {
        StatusProvide = new StatusID[] { StatusID.BasicInstinct },
        ActionCheck = (b, m) =>  Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty56] && DataCenter.PartyMembers.Count(p => p.GetHealthRatio() > 0) == 1,
    };

    static IBLUAction AethericMimicry { get; } = new BLUAction(ActionID.AethericMimicry, BLUActionType.None, ActionOption.Friendly)
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

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait Learning { get; } = new BaseTrait(219);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait MaimAndMend { get; } = new BaseTrait(220);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait MaimAndMend2 { get; } = new BaseTrait(221);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait MaimAndMend3 { get; } = new BaseTrait(222);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait MaimAndMend4 { get; } = new BaseTrait(223);

    /// <summary>
    /// 
    /// </summary>
    protected static IBaseTrait MaimAndMend5 { get; } = new BaseTrait(224);
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected sealed override bool MoveForwardGCD(out IAction act)
    {
        if (JKick.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Loom.CanUse(out act)) return true;
        return base.MoveForwardGCD(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
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

    /// <summary>
    /// All these actions are on slots.
    /// </summary>
    /// <param name="actions"></param>
    /// <returns></returns>
    protected static bool AllOnSlot(params IBLUAction[] actions) => actions.All(a => a.OnSlot);

    /// <summary>
    /// How many actions are on slots.
    /// </summary>
    /// <param name="actions"></param>
    /// <returns></returns>
    protected static uint OnSlotCount(params IBLUAction[] actions) => (uint)actions.Count(a => a.OnSlot);

    /// <summary>
    /// All base actions.
    /// </summary>
    public override IBaseAction[] AllBaseActions => base.AllBaseActions
        .Where(a => a is IBLUAction b && b.OnSlot).ToArray();

    /// <summary>
    /// Configurations.
    /// </summary>
    /// <returns></returns>
    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetCombo("BlueId", 2, "Role", "Tank", "Healer", "DPS")
            .SetCombo("AttackType", 2, "Type", "Magic", "Physic", "Both");
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void UpdateInfo()
    {
        BlueId = (BLUID)Configs.GetCombo("BlueId");
        AttackType = (BLUAttackType)Configs.GetCombo("AttackType");
        base.UpdateInfo();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected override bool HealSingleGCD(out IAction act)
    {
        if (BlueId == BLUID.Healer)
        {
            if (PomCure.CanUse(out act)) return true;
        }
        if (WhiteWind.CanUse(out act, CanUseOption.MustUse)) return true;
        return base.HealSingleGCD(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
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
