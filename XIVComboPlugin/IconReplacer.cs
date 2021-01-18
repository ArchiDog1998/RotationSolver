using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game;
using Dalamud.Game.Chat;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Structs.JobGauge;
using Dalamud.Hooking;
using Dalamud.Plugin;

namespace XIVComboPlugin
{
    public class IconReplacer
    {
        public delegate ulong OnCheckIsIconReplaceableDelegate(uint actionID);

        public delegate ulong OnGetIconDelegate(byte param1, uint param2);

        private IntPtr activeBuffArray = IntPtr.Zero;

        private readonly IconReplacerAddressResolver Address;
        private readonly Hook<OnCheckIsIconReplaceableDelegate> checkerHook;
        private readonly ClientState clientState;

        private readonly XIVComboConfiguration Configuration;

        private readonly HashSet<uint> customIds = new HashSet<uint>();

        private readonly Hook<OnGetIconDelegate> iconHook;

        private unsafe delegate int* getArray(long* address);

        private readonly CancellationTokenSource ShutdownSource = new CancellationTokenSource();

        public IconReplacer(SigScanner scanner, ClientState clientState, XIVComboConfiguration configuration)
        {
            Configuration = configuration;
            this.clientState = clientState;

            Address = new IconReplacerAddressResolver();
            Address.Setup(scanner);

            UpdateEnabledActionIDs();

            PluginLog.Verbose("===== H O T B A R S =====");
            PluginLog.Verbose($"IsIconReplaceable address {Address.IsIconReplaceable.ToInt64():X}");
            PluginLog.Verbose($"GetIcon address {Address.GetIcon.ToInt64():X}");
            PluginLog.Verbose($"ComboTimer address {Address.ComboTimer.ToInt64():X}");
            PluginLog.Verbose($"LastComboMove address {Address.LastComboMove.ToInt64():X}");

            iconHook = new Hook<OnGetIconDelegate>(Address.GetIcon,
                new OnGetIconDelegate(GetIconDetour), this);

            checkerHook = new Hook<OnCheckIsIconReplaceableDelegate>(Address.IsIconReplaceable,
                new OnCheckIsIconReplaceableDelegate(IsIconReplaceableDetour), this);

            Task.Run(() => BuffTask(ShutdownSource.Token));
        }

        public void Enable()
        {
            iconHook.Enable();
            checkerHook.Enable();
        }

        public void Dispose()
        {
            ShutdownSource.Cancel();
            iconHook.Dispose();
            checkerHook.Dispose();
        }

        /// <summary>
        /// Maps to <see cref="XIVComboConfiguration.EnabledActions"/>, these actions can potentially update their icon per the user configuration.
        /// </summary>
        public void UpdateEnabledActionIDs()
        {
            var actionIDs = Enum
                .GetValues(typeof(CustomComboPreset))
                .Cast<CustomComboPreset>()
                .Select(preset => preset.GetAttribute<CustomComboInfoAttribute>())
                .OfType<CustomComboInfoAttribute>()  // filters null
                .SelectMany(comboInfo => comboInfo.Abilities)
                .ToHashSet();
            customIds.Clear();
            customIds.UnionWith(actionIDs);
        }

        private ulong IsIconReplaceableDetour(uint actionID) => 1;

        /// <summary>
        ///     Replace an ability with another ability
        ///     actionID is the original ability to be "used"
        ///     Return either actionID (itself) or a new Action table ID as the
        ///     ability to take its place.
        ///     I tend to make the "combo chain" button be the last move in the combo
        ///     For example, Souleater combo on DRK happens by dragging Souleater
        ///     onto your bar and mashing it.
        /// </summary>
        private ulong GetIconDetour(byte self, uint actionID)
        {
            if (clientState.LocalPlayer == null)
                return iconHook.Original(self, actionID);

            var job = clientState.LocalPlayer.ClassJob.Id;

            if (!customIds.Contains(actionID))
                return iconHook.Original(self, actionID);

            if (activeBuffArray == IntPtr.Zero)
                return iconHook.Original(self, actionID);

            // Don't clutter the spaghetti any worse than it already is.
            var lastMove = Marshal.ReadInt32(Address.LastComboMove);
            var comboTime = Marshal.PtrToStructure<float>(Address.ComboTimer);
            var level = clientState.LocalPlayer.Level;

            // ====================================================================================
            #region DRAGOON

            // Change Jump/High Jump into Mirage Dive when Dive Ready
            if (Configuration.IsEnabled(CustomComboPreset.DragoonJumpFeature))
            {
                if (actionID == DRG.Jump)
                {
                    UpdateBuffAddress();
                    if (HasBuff(DRG.Buffs.DiveReady))
                        return DRG.MirageDive;
                    if (level >= DRG.Levels.HighJump)
                        return DRG.HighJump;
                    return DRG.Jump;
                }
            }

            // Change Blood of the Dragon into Stardiver when in Life of the Dragon
            if (Configuration.IsEnabled(CustomComboPreset.DragoonBOTDFeature))
            {
                if (actionID == DRG.BloodOfTheDragon)
                {
                    if (level >= DRG.Levels.Stardiver)
                    {
                        var gauge = clientState.JobGauges.Get<DRGGauge>();
                        if (gauge.BOTDState == BOTDState.LOTD)
                            return DRG.Stardiver;
                    }
                    return DRG.BloodOfTheDragon;
                }
            }

            // Replace Coerthan Torment with Coerthan Torment combo chain
            if (Configuration.IsEnabled(CustomComboPreset.DragoonCoerthanTormentCombo))
            {
                if (actionID == DRG.CoerthanTorment)
                {
                    if (comboTime > 0)
                    {
                        if (lastMove == DRG.DoomSpike && level >= DRG.Levels.SonicThrust)
                            return DRG.SonicThrust;
                        if (lastMove == DRG.SonicThrust && level >= DRG.Levels.CoerthanTorment)
                            return DRG.CoerthanTorment;
                    }
                    return DRG.DoomSpike;
                }
            }

            // Replace Chaos Thrust with the Chaos Thrust combo chain
            if (Configuration.IsEnabled(CustomComboPreset.DragoonChaosThrustCombo))
            {
                if (actionID == DRG.ChaosThrust)
                {
                    if (comboTime > 0)
                    {
                        if ((lastMove == DRG.TrueThrust || lastMove == DRG.RaidenThrust) && level >= DRG.Levels.Disembowel)
                            return DRG.Disembowel;
                        if (lastMove == DRG.Disembowel && level >= DRG.Levels.ChaosThrust)
                            return DRG.ChaosThrust;
                    }
                    UpdateBuffAddress();
                    if (HasBuff(DRG.Buffs.SharperFangAndClaw) && level >= DRG.Levels.FangAndClaw)
                        return DRG.FangAndClaw;
                    if (HasBuff(DRG.Buffs.EnhancedWheelingThrust) && level >= DRG.Levels.WheelingThrust)
                        return DRG.WheelingThrust;
                    if (HasBuff(DRG.Buffs.RaidenThrustReady) && level >= DRG.Levels.RaidenThrust)
                        return DRG.RaidenThrust;
                    return DRG.TrueThrust;
                }
            }

            // Replace Full Thrust with the Full Thrust combo chain
            if (Configuration.IsEnabled(CustomComboPreset.DragoonFullThrustCombo))
            {
                if (actionID == DRG.FullThrust)
                {
                    if (comboTime > 0)
                    {
                        if ((lastMove == DRG.TrueThrust || lastMove == DRG.RaidenThrust)
                            && level >= DRG.Levels.VorpalThrust)
                            return DRG.VorpalThrust;
                        if (lastMove == DRG.VorpalThrust && level >= DRG.Levels.FullThrust)
                            return DRG.FullThrust;
                    }
                    UpdateBuffAddress();
                    if (HasBuff(DRG.Buffs.SharperFangAndClaw) && level >= DRG.Levels.FangAndClaw)
                        return DRG.FangAndClaw;
                    if (HasBuff(DRG.Buffs.EnhancedWheelingThrust) && level >= DRG.Levels.WheelingThrust)
                        return DRG.WheelingThrust;
                    if (HasBuff(DRG.Buffs.RaidenThrustReady) && level >= DRG.Levels.RaidenThrust)
                        return DRG.RaidenThrust;
                    return DRG.TrueThrust;
                }
            }

            #endregion
            // ====================================================================================
            #region DARK KNIGHT

            // Replace Souleater with Souleater combo chain
            if (Configuration.IsEnabled(CustomComboPreset.DarkSouleaterCombo))
            {
                if (actionID == DRK.Souleater)
                {
                    if (Configuration.IsEnabled(CustomComboPreset.DeliriumFeature))
                        if (level >= DRK.Levels.Bloodpiller && level >= DRK.Levels.Delirium && HasBuff(DRK.Buffs.Delirium))
                            return DRK.Bloodspiller;

                    if (comboTime > 0)
                    {
                        if (lastMove == DRK.HardSlash && level >= DRK.Levels.SyphonStrike)
                            return DRK.SyphonStrike;
                        if (lastMove == DRK.SyphonStrike && level >= DRK.Levels.Souleater)
                            return DRK.Souleater;
                    }
                    return DRK.HardSlash;
                }
            }

            // Replace Stalwart Soul with Stalwart Soul combo chain
            if (Configuration.IsEnabled(CustomComboPreset.DarkStalwartSoulCombo))
            {
                if (actionID == DRK.StalwartSoul)
                {
                    if (Configuration.IsEnabled(CustomComboPreset.DeliriumFeature))
                        if (level >= DRK.Levels.Quietus && level >= DRK.Levels.Delirium && HasBuff(DRK.Buffs.Delirium))
                            return DRK.Quietus;

                    if (comboTime > 0)
                        if (lastMove == DRK.Unleash && level >= DRK.Levels.StalwartSoul)
                            return DRK.StalwartSoul;

                    return DRK.Unleash;
                }
            }

            #endregion
            // ====================================================================================
            #region PALADIN

            // Replace Goring Blade with Goring Blade combo
            if (Configuration.IsEnabled(CustomComboPreset.PaladinGoringBladeCombo))
            {
                if (actionID == PLD.GoringBlade)
                {
                    if (comboTime > 0)
                    {
                        if (lastMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                            return PLD.RiotBlade;
                        if (lastMove == PLD.RiotBlade && level >= PLD.Levels.GoringBlade)
                            return PLD.GoringBlade;
                    }
                    return PLD.FastBlade;
                }
            }

            // Replace Royal Authority with Royal Authority combo
            if (Configuration.IsEnabled(CustomComboPreset.PaladinRoyalAuthorityCombo))
            {
                if (actionID == PLD.RoyalAuthority || actionID == PLD.RageOfHalone)
                {
                    if (comboTime > 0)
                    {
                        if (lastMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                            return PLD.RiotBlade;
                        if (lastMove == PLD.RiotBlade)
                        {
                            if (level >= PLD.Levels.RoyalAuthority)
                                return PLD.RoyalAuthority;
                            if (level >= PLD.Levels.RageOfHalone)
                                return PLD.RageOfHalone;
                        }
                    }
                    return PLD.FastBlade;
                }
            }

            // Replace Prominence with Prominence combo
            if (Configuration.IsEnabled(CustomComboPreset.PaladinProminenceCombo))
            {
                if (actionID == PLD.Prominence)
                {
                    if (comboTime > 0)
                        if (lastMove == PLD.TotalEclipse && level >= PLD.Levels.Prominence)
                            return PLD.Prominence;

                    return PLD.TotalEclipse;
                }
            }

            // Replace Requiescat with Confiteor when under the effect of Requiescat
            if (Configuration.IsEnabled(CustomComboPreset.PaladinRequiescatCombo))
            {
                if (actionID == PLD.Requiescat)
                {
                    if (HasBuff(PLD.Buffs.Requiescat) && level >= PLD.Levels.Confiteor)
                        return PLD.Confiteor;
                    return PLD.Requiescat;
                }
            }

            #endregion
            // ====================================================================================
            #region WARRIOR

            // Replace Storm's Path with Storm's Path combo
            if (Configuration.IsEnabled(CustomComboPreset.WarriorStormsPathCombo))
            {
                if (actionID == WAR.StormsPath)
                {
                    if (comboTime > 0)
                    {
                        if (lastMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                            return WAR.Maim;
                        if (lastMove == WAR.Maim && level >= WAR.Levels.StormsPath)
                            return WAR.StormsPath;
                    }
                    return WAR.HeavySwing;
                }
            }

            // Replace Storm's Eye with Storm's Eye combo
            if (Configuration.IsEnabled(CustomComboPreset.WarriorStormsEyeCombo))
            {
                if (actionID == WAR.StormsEye)
                {
                    if (comboTime > 0)
                    {
                        if (lastMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                            return WAR.Maim;
                        if (lastMove == WAR.Maim && level >= WAR.Levels.StormsEye)
                            return WAR.StormsEye;
                    }
                    return WAR.HeavySwing;
                }
            }

            // Replace Mythril Tempest with Mythril Tempest combo
            if (Configuration.IsEnabled(CustomComboPreset.WarriorMythrilTempestCombo))
            {
                if (actionID == WAR.MythrilTempest)
                {
                    if (comboTime > 0)
                        if (lastMove == WAR.Overpower && level >= WAR.Levels.MythrilTempest)
                            return WAR.MythrilTempest;
                    return WAR.Overpower;
                }
            }

            #endregion
            // ====================================================================================
            #region SAMURAI

            // Replace Yukikaze with Yukikaze combo
            if (Configuration.IsEnabled(CustomComboPreset.SamuraiYukikazeCombo))
            {
                if (actionID == SAM.Yukikaze)
                {
                    UpdateBuffAddress();
                    if (HasBuff(SAM.Buffs.MeikyoShisui))
                        return SAM.Yukikaze;
                    if (comboTime > 0)
                        if (lastMove == SAM.Hakaze && level >= SAM.Levels.Yukikaze)
                            return SAM.Yukikaze;
                    return SAM.Hakaze;
                }
            }

            // Replace Gekko with Gekko combo
            if (Configuration.IsEnabled(CustomComboPreset.SamuraiGekkoCombo))
            {
                if (actionID == SAM.Gekko)
                {
                    UpdateBuffAddress();
                    if (HasBuff(SAM.Buffs.MeikyoShisui))
                        return SAM.Gekko;
                    if (comboTime > 0)
                    {
                        if (lastMove == SAM.Hakaze && level >= SAM.Levels.Jinpu)
                            return SAM.Jinpu;
                        if (lastMove == SAM.Jinpu && level >= SAM.Levels.Gekko)
                            return SAM.Gekko;
                    }
                    return SAM.Hakaze;
                }
            }

            // Replace Kasha with Kasha combo
            if (Configuration.IsEnabled(CustomComboPreset.SamuraiKashaCombo))
            {
                if (actionID == SAM.Kasha)
                {
                    UpdateBuffAddress();
                    if (HasBuff(SAM.Buffs.MeikyoShisui))
                        return SAM.Kasha;
                    if (comboTime > 0)
                    {
                        if (lastMove == SAM.Hakaze && level >= SAM.Levels.Shifu)
                            return SAM.Shifu;
                        if (lastMove == SAM.Shifu && level >= SAM.Levels.Kasha)
                            return SAM.Kasha;
                    }
                    return SAM.Hakaze;
                }
            }

            // Replace Mangetsu with Mangetsu combo
            if (Configuration.IsEnabled(CustomComboPreset.SamuraiMangetsuCombo))
            {
                if (actionID == SAM.Mangetsu)
                {
                    UpdateBuffAddress();
                    if (HasBuff(SAM.Buffs.MeikyoShisui))
                        return SAM.Mangetsu;
                    if (comboTime > 0)
                        if (lastMove == SAM.Fuga && level >= SAM.Levels.Mangetsu)
                            return SAM.Mangetsu;
                    return SAM.Fuga;
                }
            }

            // Replace Oka with Oka combo
            if (Configuration.IsEnabled(CustomComboPreset.SamuraiOkaCombo))
            {
                if (actionID == SAM.Oka)
                {
                    UpdateBuffAddress();
                    if (HasBuff(SAM.Buffs.MeikyoShisui))
                        return SAM.Oka;
                    if (comboTime > 0)
                        if (lastMove == SAM.Fuga && level >= SAM.Levels.Oka)
                            return SAM.Oka;
                    return SAM.Fuga;
                }
            }

            // Turn Seigan into Third Eye when not procced
            if (Configuration.IsEnabled(CustomComboPreset.SamuraiThirdEyeFeature))
            {
                if (actionID == SAM.Seigan)
                {
                    UpdateBuffAddress();
                    if (HasBuff(SAM.Buffs.EyesOpen))
                        return SAM.Seigan;
                    return SAM.ThirdEye;
                }
            }

            #endregion
            // ====================================================================================
            #region NINJA

            // Replace Armor Crush with Armor Crush combo
            if (Configuration.IsEnabled(CustomComboPreset.NinjaArmorCrushCombo))
            {
                if (actionID == NIN.ArmorCrush)
                {
                    if (comboTime > 0)
                    {
                        if (lastMove == NIN.SpinningEdge && level >= NIN.Levels.GustSlash)
                            return NIN.GustSlash;
                        if (lastMove == NIN.GustSlash && level >= NIN.Levels.ArmorCrush)
                            return NIN.ArmorCrush;
                    }
                    return NIN.SpinningEdge;
                }
            }

            // Replace Aeolian Edge with Aeolian Edge combo
            if (Configuration.IsEnabled(CustomComboPreset.NinjaAeolianEdgeCombo))
            {
                if (actionID == NIN.AeolianEdge)
                {
                    if (comboTime > 0)
                    {
                        if (lastMove == NIN.SpinningEdge && level >= NIN.Levels.GustSlash)
                            return NIN.GustSlash;
                        if (lastMove == NIN.GustSlash && level >= NIN.Levels.AeolianEdge)
                            return NIN.AeolianEdge;
                    }
                    return NIN.SpinningEdge;
                }
            }

            // Replace Hakke Mujinsatsu with Hakke Mujinsatsu combo
            if (Configuration.IsEnabled(CustomComboPreset.NinjaHakkeMujinsatsuCombo))
            {
                if (actionID == NIN.HakkeMujinsatsu)
                {
                    if (comboTime > 0)
                        if (lastMove == NIN.DeathBlossom && level >= NIN.Levels.HakkeMujinsatsu)
                            return NIN.HakkeMujinsatsu;
                    return NIN.DeathBlossom;
                }
            }

            //Replace Dream Within a Dream with Assassinate when Assassinate Ready
            if (Configuration.IsEnabled(CustomComboPreset.NinjaAssassinateFeature))
            {
                if (actionID == NIN.DreamWithinADream)
                {
                    UpdateBuffAddress();
                    if (HasBuff(NIN.Buffs.AssassinateReady))
                        return NIN.Assassinate;
                    return NIN.DreamWithinADream;
                }
            }

            #endregion
            // ====================================================================================
            #region GUNBREAKER

            // Replace Solid Barrel with Solid Barrel combo
            if (Configuration.IsEnabled(CustomComboPreset.GunbreakerSolidBarrelCombo))
            {
                if (actionID == GNB.SolidBarrel)
                {
                    if (comboTime > 0)
                    {
                        if (lastMove == GNB.KeenEdge && level >= GNB.Levels.BrutalShell)
                            return GNB.BrutalShell;
                        if (lastMove == GNB.BrutalShell && level >= GNB.Levels.SolidBarrel)
                            return GNB.SolidBarrel;
                    }
                    return GNB.KeenEdge;
                }
            }

            // Replace Wicked Talon with Gnashing Fang combo
            if (Configuration.IsEnabled(CustomComboPreset.GunbreakerGnashingFangCombo))
            {
                if (actionID == GNB.WickedTalon)
                {
                    if (Configuration.IsEnabled(CustomComboPreset.GunbreakerGnashingFangCont))
                    {
                        if (level >= GNB.Levels.Continuation)
                        {
                            UpdateBuffAddress();
                            if (HasBuff(GNB.Buffs.ReadyToRip))
                                return GNB.JugularRip;
                            if (HasBuff(GNB.Buffs.ReadyToTear))
                                return GNB.AbdomenTear;
                            if (HasBuff(GNB.Buffs.ReadyToGouge))
                                return GNB.EyeGouge;
                        }
                    }
                    var ammoComboState = clientState.JobGauges.Get<GNBGauge>().AmmoComboStepNumber;
                    switch (ammoComboState)
                    {
                        case 1:
                            return GNB.SavageClaw;
                        case 2:
                            return GNB.WickedTalon;
                        default:
                            return GNB.GnashingFang;
                    }
                }
            }

            // Replace Demon Slaughter with Demon Slaughter combo
            if (Configuration.IsEnabled(CustomComboPreset.GunbreakerDemonSlaughterCombo))
            {
                if (actionID == GNB.DemonSlaughter)
                {
                    if (comboTime > 0)
                        if (lastMove == GNB.DemonSlice && level >= GNB.Levels.DemonSlaughter)
                        {
                            if (Configuration.IsEnabled(CustomComboPreset.GunbreakerFatedCircleFeature))
                            {
                                var gauge = clientState.JobGauges.Get<GNBGauge>();
                                if (gauge.NumAmmo == 2 && level >= GNB.Levels.FatedCircle)
                                {
                                    return GNB.FatedCircle;
                                }
                            }
                            return GNB.DemonSlaughter;
                        }
                    return GNB.DemonSlice;
                }
            }

            #endregion
            // ====================================================================================
            #region MACHINIST

            // Replace Clean Shot with Heated Clean Shot combo
            // Or with Heat Blast when overheated.
            // For some reason the shots use their unheated IDs as combo moves
            if (Configuration.IsEnabled(CustomComboPreset.MachinistMainCombo))
            {
                if (actionID == MCH.CleanShot || actionID == MCH.HeatedCleanShot)
                {
                    if (comboTime > 0)
                    {
                        if (lastMove == MCH.SplitShot)
                        {
                            if (level >= MCH.Levels.HeatedSlugshot)
                                return MCH.HeatedSlugshot;
                            if (level >= MCH.Levels.SlugShot)
                                return MCH.SlugShot;
                        }
                        if (lastMove == MCH.SlugShot)
                        {
                            if (level >= MCH.Levels.HeatedCleanShot)
                                return MCH.HeatedCleanShot;
                            if (level >= MCH.Levels.CleanShot)
                                return MCH.CleanShot;
                        }
                    }
                    if (level >= MCH.Levels.HeatedSplitShot)
                        return MCH.HeatedSplitShot;
                    return MCH.SplitShot;
                }
            }

            // Replace Heat Blast and Auto crossbow with Hypercharge when not overheated
            if (Configuration.IsEnabled(CustomComboPreset.MachinistOverheatFeature))
            {
                if (actionID == MCH.HeatBlast || actionID == MCH.AutoCrossbow)
                {
                    var gauge = clientState.JobGauges.Get<MCHGauge>();
                    if (!gauge.IsOverheated() && level >= MCH.Levels.Hypercharge)
                        return MCH.Hypercharge;
                }
            }

            // Replace Spread Shot with Auto Crossbow when overheated.
            if (Configuration.IsEnabled(CustomComboPreset.MachinistSpreadShotFeature))
            {
                if (actionID == MCH.SpreadShot)
                {
                    if (clientState.JobGauges.Get<MCHGauge>().IsOverheated() && level >= MCH.Levels.AutoCrossbow)
                        return MCH.AutoCrossbow;
                }
            }

            // Replace Rook Turret and Automaton Queen with Overdrive while active.
            if (Configuration.IsEnabled(CustomComboPreset.MachinistOverdriveFeature))
            {
                if (actionID == MCH.RookAutoturret || actionID == MCH.AutomatonQueen)
                {
                    if (clientState.JobGauges.Get<MCHGauge>().IsRobotActive())
                    {
                        if (level >= MCH.Levels.QueenOverdrive)
                            return MCH.QueenOverdrive;
                        if (level >= MCH.Levels.RookOverdrive)
                            return MCH.RookOverdrive;
                    }
                }
            }

            #endregion
            // ====================================================================================
            #region BLACK MAGE

            // Enochian changes to B4 or F4 depending on stance.
            if (Configuration.IsEnabled(CustomComboPreset.BlackEnochianFeature))
            {
                if (actionID == BLM.Enochian)
                {
                    var gauge = clientState.JobGauges.Get<BLMGauge>();
                    if (gauge.IsEnoActive())
                    {
                        if (gauge.InUmbralIce() && level >= BLM.Levels.Blizzard4)
                            return BLM.Blizzard4;
                        if (level >= BLM.Levels.Fire4)
                            return BLM.Fire4;
                    }

                    return BLM.Enochian;
                }
            }

            // Umbral Soul and Transpose
            if (Configuration.IsEnabled(CustomComboPreset.BlackManaFeature))
            {
                if (actionID == BLM.Transpose)
                {
                    var gauge = clientState.JobGauges.Get<BLMGauge>();
                    if (gauge.InUmbralIce() && gauge.IsEnoActive() && level >= BLM.Levels.UmbralSoul)
                        return BLM.UmbralSoul;
                    return BLM.Transpose;
                }
            }

            // Ley Lines and BTL
            if (Configuration.IsEnabled(CustomComboPreset.BlackLeyLines))
            {
                if (actionID == BLM.LeyLines)
                {
                    if (HasBuff(BLM.Buffs.LeyLines) && level >= BLM.Levels.BetweenTheLines)
                        return BLM.BetweenTheLines;
                    return BLM.LeyLines;
                }
            }

            #endregion
            // ====================================================================================
            #region ASTROLOGIAN

            // Make cards on the same button as play
            if (Configuration.IsEnabled(CustomComboPreset.AstrologianCardsOnDrawFeature))
            {
                if (actionID == AST.Play)
                {
                    var gauge = clientState.JobGauges.Get<ASTGauge>();
                    switch (gauge.DrawnCard())
                    {
                        case CardType.BALANCE:
                            return AST.Balance;
                        case CardType.BOLE:
                            return AST.Bole;
                        case CardType.ARROW:
                            return AST.Arrow;
                        case CardType.SPEAR:
                            return AST.Spear;
                        case CardType.EWER:
                            return AST.Ewer;
                        case CardType.SPIRE:
                            return AST.Spire;
                        /*
                        case CardType.LORD:
                            return AST.LordOfCrowns;
                        case CardType.LADY:
                            return AST.LadyOfCrowns;
                        */
                        default:
                            return AST.Draw;
                    }
                }
            }

            #endregion
            // ====================================================================================
            #region SUMMONER

            // DWT changes. 
            // Now contains DWT, Deathflare, Summon Bahamut, Enkindle Bahamut, FBT, and Enkindle Phoenix.
            // What a monster of a button.
            /*
            if (Configuration.IsEnabled(CustomComboPreset.SummonerDwtCombo))
                if (actionID == 3581)
                {
                    var gauge = clientState.JobGauges.Get<SMNGauge>();
                    if (gauge.TimerRemaining > 0)
                    {
                        if (gauge.ReturnSummon > 0)
                        {
                            if (gauge.IsPhoenixReady()) return 16516;
                            return 7429;
                        }

                        if (level >= 60) return 3582;
                    }
                    else
                    {
                        if (gauge.IsBahamutReady()) return 7427;
                        if (gauge.IsPhoenixReady())
                        {
                            if (level == 80) return 16549;
                            return 16513;
                        }

                        return 3581;
                    }
                }
            */

            if (Configuration.IsEnabled(CustomComboPreset.SummonerDemiCombo))
            {

                // Replace Deathflare with demi enkindles
                if (actionID == SMN.Deathflare)
                {
                    var gauge = clientState.JobGauges.Get<SMNGauge>();
                    if (gauge.IsPhoenixReady())
                        return SMN.EnkindlePhoenix;
                    if (gauge.TimerRemaining > 0 && gauge.ReturnSummon != SummonPet.NONE)
                        return SMN.EnkindleBahamut;
                    return SMN.Deathflare;
                }

                //Replace DWT with demi summons
                if (actionID == SMN.DreadwyrmTrance)
                {
                    var gauge = clientState.JobGauges.Get<SMNGauge>();
                    if (gauge.IsBahamutReady())
                        return SMN.SummonBahamut;
                    if (gauge.IsPhoenixReady() ||
                        gauge.TimerRemaining > 0 && gauge.ReturnSummon != SummonPet.NONE)
                    {
                        if (level >= SMN.Levels.EnhancedFirebirdTrance)
                            return SMN.FirebirdTranceHigh;
                        return SMN.FirebirdTranceLow;
                    }
                    return SMN.DreadwyrmTrance;
                }
            }

            // Ruin 1 now upgrades to Brand of Purgatory in addition to Ruin 3 and Fountain of Fire
            if (Configuration.IsEnabled(CustomComboPreset.SummonerBoPCombo))
            {
                if (actionID == SMN.Ruin1 || actionID == SMN.Ruin3)
                {
                    var gauge = clientState.JobGauges.Get<SMNGauge>();
                    if (gauge.TimerRemaining > 0)
                        if (gauge.IsPhoenixReady())
                        {
                            UpdateBuffAddress();
                            if (HasBuff(SMN.Buffs.HellishConduit))
                                return SMN.BrandOfPurgatory;
                            return SMN.FountainOfFire;
                        }

                    if (level >= SMN.Levels.Ruin3)
                        return SMN.Ruin3;
                    return SMN.Ruin1;
                }
            }

            // Change Fester into Energy Drain
            if (Configuration.IsEnabled(CustomComboPreset.SummonerEDFesterCombo))
            {
                if (actionID == SMN.Fester)
                {
                    if (!clientState.JobGauges.Get<SMNGauge>().HasAetherflowStacks())
                        return SMN.EnergyDrain;
                    return SMN.Fester;
                }
            }

            //Change Painflare into Energy Syphon
            if (Configuration.IsEnabled(CustomComboPreset.SummonerESPainflareCombo))
            {
                if (actionID == SMN.Painflare)
                {
                    if (!clientState.JobGauges.Get<SMNGauge>().HasAetherflowStacks())
                        return SMN.EnergySyphon;
                    if (level >= SMN.Levels.Painflare)
                        return SMN.Painflare;
                    return SMN.EnergySyphon;
                }
            }

            #endregion
            // ====================================================================================
            #region SCHOLAR

            // Change Fey Blessing into Consolation when Seraph is out.
            if (Configuration.IsEnabled(CustomComboPreset.ScholarSeraphConsolationFeature))
            {
                if (actionID == SCH.FeyBless)
                {
                    if (clientState.JobGauges.Get<SCHGauge>().SeraphTimer > 0) return SCH.Consolation;
                    return SCH.FeyBless;
                }
            }

            // Change Energy Drain into Aetherflow when you have no more Aetherflow stacks.
            if (Configuration.IsEnabled(CustomComboPreset.ScholarEnergyDrainFeature))
            {
                if (actionID == SCH.EnergyDrain)
                {
                    if (clientState.JobGauges.Get<SCHGauge>().NumAetherflowStacks == 0) return SCH.Aetherflow;
                    return SCH.EnergyDrain;
                }
            }

            #endregion
            // ====================================================================================
            #region DANCER

            // AoE GCDs are split into two buttons, because priority matters
            // differently in different single-target moments. Thanks yoship.
            // Replaces each GCD with its procced version.
            /*
            if (Configuration.IsEnabled(CustomComboPreset.DancerAoeGcdFeature))
            {
                if (actionID == DNC.Bloodshower)
                {
                    UpdateBuffAddress();
                    if (SearchBuffArray(DNC.Buffs.FlourishingShower))
                        return DNC.Bloodshower;
                    return DNC.Bladeshower;
                }

                if (actionID == DNC.RisingWindmill)
                {
                    UpdateBuffAddress();
                    if (SearchBuffArray(DNC.Buffs.FlourishingWindmill))
                        return DNC.RisingWindmill;
                    return DNC.Windmill;
                }
            }
            */

            // Fan Dance changes into Fan Dance 3 while flourishing.
            if (Configuration.IsEnabled(CustomComboPreset.DancerFanDanceCombo))
            {
                if (actionID == DNC.FanDance1)
                {
                    UpdateBuffAddress();
                    if (HasBuff(DNC.Buffs.FlourishingFanDance))
                        return DNC.FanDance3;
                    return DNC.FanDance1;
                }

                // Fan Dance 2 changes into Fan Dance 3 while flourishing.
                if (actionID == DNC.FanDance2)
                {
                    UpdateBuffAddress();
                    if (HasBuff(DNC.Buffs.FlourishingFanDance))
                        return DNC.FanDance3;
                    return DNC.FanDance2;
                }
            }

            // Standard Step and Technical Steps turn into the movements while dancing.
            if (Configuration.IsEnabled(CustomComboPreset.DancerDanceStepCombo))
            {
                if (actionID == DNC.StandardStep)
                {
                    var gauge = clientState.JobGauges.Get<DNCGauge>();
                    if (gauge.IsDancing() && HasBuff(DNC.Buffs.StandardStep))
                        if (gauge.NumCompleteSteps < 2)
                            return gauge.NextStep();
                }
                if (actionID == DNC.TechnicalStep)
                {
                    var gauge = clientState.JobGauges.Get<DNCGauge>();
                    if (gauge.IsDancing() && HasBuff(DNC.Buffs.TechnicalStep))
                        if (gauge.NumCompleteSteps < 4)
                            return gauge.NextStep();
                }
            }

            // Before using Flourish, use any procs
            if (Configuration.IsEnabled(CustomComboPreset.DancerFlourishFeature))
            {
                if (actionID == DNC.Flourish)
                {
                    UpdateBuffAddress();
                    if (HasBuff(DNC.Buffs.FlourishingFountain))
                        return DNC.Fountainfall;
                    if (HasBuff(DNC.Buffs.FlourishingCascade))
                        return DNC.ReverseCascade;
                    if (HasBuff(DNC.Buffs.FlourishingShower))
                        return DNC.Bloodshower;
                    if (HasBuff(DNC.Buffs.FlourishingWindmill))
                        return DNC.RisingWindmill;
                    return DNC.Flourish;
                }
            }

            // Single target multibutton
            if (Configuration.IsEnabled(CustomComboPreset.DancerSingleTargetMultibutton))
            {
                if (actionID == DNC.Cascade)
                {
                    UpdateBuffAddress();
                    // From Fountain
                    if (HasBuff(DNC.Buffs.FlourishingFountain))
                        return DNC.Fountainfall;
                    // From Cascade
                    if (HasBuff(DNC.Buffs.FlourishingCascade))
                        return DNC.ReverseCascade;
                    // Cascade Combo
                    if (lastMove == DNC.Cascade && level >= DNC.Levels.Fountain)
                        return DNC.Fountain;
                    return DNC.Cascade;
                }
            }

            // AoE multibutton
            if (Configuration.IsEnabled(CustomComboPreset.DancerAoeMultibutton))
            {
                if (actionID == DNC.Windmill)
                {
                    UpdateBuffAddress();
                    // From Bladeshower
                    if (HasBuff(DNC.Buffs.FlourishingShower))
                        return DNC.Bloodshower;
                    // From Windmill
                    if (HasBuff(DNC.Buffs.FlourishingWindmill))
                        return DNC.RisingWindmill;
                    // Windmill Combo
                    if (lastMove == DNC.Windmill && level >= DNC.Levels.Bladeshower)
                        return DNC.Bladeshower;
                    return DNC.Windmill;
                }
            }

            #endregion
            // ====================================================================================
            #region WHITE MAGE

            // Replace Solace with Misery when full blood lily
            if (Configuration.IsEnabled(CustomComboPreset.WhiteMageSolaceMiseryFeature))
            {
                if (actionID == WHM.Solace)
                {
                    if (clientState.JobGauges.Get<WHMGauge>().NumBloodLily == 3)
                        return WHM.Misery;
                    return WHM.Solace;
                }
            }

            // Replace Solace with Misery when full blood lily
            if (Configuration.IsEnabled(CustomComboPreset.WhiteMageRaptureMiseryFeature))
            {
                if (actionID == WHM.Rapture)
                {
                    if (clientState.JobGauges.Get<WHMGauge>().NumBloodLily == 3)
                        return WHM.Misery;
                    return WHM.Rapture;
                }
            }

            #endregion
            // ====================================================================================
            #region BARD

            // Replace Wanderer's Minuet with PP when in WM.
            if (Configuration.IsEnabled(CustomComboPreset.BardWandererPPFeature))
            {
                if (actionID == BRD.WanderersMinuet)
                {
                    if (clientState.JobGauges.Get<BRDGauge>().ActiveSong == CurrentSong.WANDERER)
                        return BRD.PitchPerfect;
                    return BRD.WanderersMinuet;
                }
            }

            // Replace HS/BS with SS/RA when procced.
            if (Configuration.IsEnabled(CustomComboPreset.BardStraightShotUpgradeFeature))
            {
                if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot)
                {
                    UpdateBuffAddress();
                    if (HasBuff(BRD.Buffs.StraightShotReady))
                    {
                        if (level >= BRD.Levels.RefulgentArrow)
                            return BRD.RefulgentArrow;
                        return BRD.StraightShot;
                    }

                    if (level >= BRD.Levels.BurstShot)
                        return BRD.BurstShot;
                    return BRD.HeavyShot;
                }
            }

            #endregion
            // ====================================================================================
            #region MONK

            if (Configuration.IsEnabled(CustomComboPreset.MnkAoECombo))
            {
                if (actionID == MNK.Rockbreaker)
                {
                    UpdateBuffAddress();
                    if (HasBuff(MNK.Buffs.PerfectBalance) || HasBuff(MNK.Buffs.FormlessFist))
                        return MNK.Rockbreaker;
                    if (HasBuff(MNK.Buffs.OpoOpoForm))
                        return MNK.ArmOfTheDestroyer;
                    if (HasBuff(MNK.Buffs.RaptorForm) && level >= MNK.Levels.FourPointFury)
                        return MNK.FourPointFury;
                    if (HasBuff(MNK.Buffs.CoerlForm) && level >= MNK.Levels.Rockbreaker)
                        return MNK.Rockbreaker;
                    return MNK.ArmOfTheDestroyer;
                }
            }

            #endregion
            // ====================================================================================
            #region RED MAGE

            // Replace Veraero/thunder 2 with Impact when Dualcast is active
            if (Configuration.IsEnabled(CustomComboPreset.RedMageAoECombo))
            {
                if (actionID == RDM.Veraero2)
                {
                    UpdateBuffAddress();
                    if (HasBuff(DoM.Buffs.Swiftcast) || HasBuff(RDM.Buffs.Dualcast))
                    {
                        if (level >= RDM.Levels.Impact)
                            return RDM.Impact;
                        return RDM.Scatter;
                    }
                    return RDM.Veraero2;
                }

                if (actionID == RDM.Verthunder2)
                {
                    UpdateBuffAddress();
                    if (HasBuff(DoM.Buffs.Swiftcast) || HasBuff(RDM.Buffs.Dualcast))
                    {
                        if (level >= RDM.Levels.Impact)
                            return RDM.Impact;
                        return RDM.Scatter;
                    }
                    return RDM.Verthunder2;
                }
            }

            // Replace Redoublement with Redoublement combo, Enchanted if possible.
            if (Configuration.IsEnabled(CustomComboPreset.RedMageMeleeCombo))
            {
                if (actionID == RDM.Redoublement)
                {
                    var gauge = clientState.JobGauges.Get<RDMGauge>();
                    if ((lastMove == RDM.Riposte || lastMove == RDM.EnchantedRiposte) && level >= RDM.Levels.Zwerchhau)
                    {
                        if (gauge.BlackGauge >= 25 && gauge.WhiteGauge >= 25)
                            return RDM.EnchantedZwerchhau;
                        return RDM.Zwerchhau;
                    }

                    if (lastMove == RDM.Zwerchhau && level >= RDM.Levels.Redoublement)
                    {
                        if (gauge.BlackGauge >= 25 && gauge.WhiteGauge >= 25)
                            return RDM.EnchantedRedoublement;
                        return RDM.Redoublement;
                    }

                    if (gauge.BlackGauge >= 30 && gauge.WhiteGauge >= 30)
                        return RDM.EnchantedRiposte;
                    return RDM.Riposte;
                }
            }

            if (Configuration.IsEnabled(CustomComboPreset.RedMageVerprocCombo))
            {
                if (actionID == RDM.Verstone)
                {
                    if (level >= RDM.Levels.Scorch && (lastMove == RDM.Verflare || lastMove == RDM.Verholy))
                        return RDM.Scorch;
                    UpdateBuffAddress();
                    if (HasBuff(RDM.Buffs.VerstoneReady))
                        return RDM.Verstone;
                    if (level >= RDM.Levels.Jolt2)
                        return RDM.Jolt2;
                    return RDM.Jolt;
                }
                if (actionID == RDM.Verfire)
                {
                    if (level >= RDM.Levels.Scorch && (lastMove == RDM.Verflare || lastMove == RDM.Verholy))
                        return RDM.Scorch;
                    UpdateBuffAddress();
                    if (HasBuff(RDM.Buffs.VerfireReady))
                        return RDM.Verfire;
                    if (level >= RDM.Levels.Jolt2)
                        return RDM.Jolt2;
                    return RDM.Jolt;
                }
            }

            #endregion
            // ====================================================================================

            return iconHook.Original(self, actionID);
        }

        private bool HasBuff(short needle)
        {
            if (activeBuffArray == IntPtr.Zero) return false;
            for (var i = 0; i < 60; i++)
                if (Marshal.ReadInt16(activeBuffArray + (12 * i)) == needle)
                    return true;
            return false;
        }

        private async void BuffTask(CancellationToken shutdownToken)
        {
            try
            {
                while (!shutdownToken.IsCancellationRequested)
                {
                    UpdateBuffAddress();
                    await Task.Delay(1000, shutdownToken);
                }
            }
            catch (OperationCanceledException) { }
        }

        private void UpdateBuffAddress()
        {
            try
            {
                activeBuffArray = FindBuffAddress();
            }
            catch (Exception)
            {
                activeBuffArray = IntPtr.Zero;
            }
        }

        private unsafe IntPtr FindBuffAddress()
        {
            var num = Marshal.ReadIntPtr(Address.BuffVTableAddr);
            var step2 = (IntPtr)(Marshal.ReadInt64(num) + 0x280);
            var step3 = Marshal.ReadIntPtr(step2);
            var callback = Marshal.GetDelegateForFunctionPointer<getArray>(step3);
            return (IntPtr)callback((long*)num) + 8;
        }
    }
}
