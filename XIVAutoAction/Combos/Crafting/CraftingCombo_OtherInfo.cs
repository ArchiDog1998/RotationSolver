using Dalamud.Game.ClientState.Objects.SubKinds;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Crafting
{
    internal abstract partial class CraftingCombo
    {
        protected static PlayerCharacter Player => Service.ClientState.LocalPlayer;

        protected static byte Level => Player?.Level ?? 0;
        protected static uint CurrentCP => Player?.CurrentCp ?? 0;
        protected static uint MaxCP => Player?.MaxCp ?? 0;

        protected static int CurrentProgress => SynthesisUpdater.CurrentProgress;
        protected static int MaxProgress => SynthesisUpdater.MaxProgress;
        protected static int CurrentQuality => SynthesisUpdater.CurrentQuality;
        protected static int MaxQuality => SynthesisUpdater.MaxQuality;
        protected static int CurrentDurability => SynthesisUpdater.CurrentDurability;
        protected static int StartingDurability => SynthesisUpdater.StartingDurability;
        protected static int StepNumber => SynthesisUpdater.StepNumber;
        protected static bool CanHQ => SynthesisUpdater.Recipe?.CanHq ?? false;
        protected static CraftCondition CraftCondition => SynthesisUpdater.CraftCondition;

        protected static byte WasteNotTime => Player?.StatusStack(false, StatusID.WasteNot, StatusID.WasteNot2) ?? 0;
        protected static bool HasInnovation => Player?.HasStatus(false, StatusID.Innovation) ?? false;
        protected static bool HasFinalAppraisal => Player?.HasStatus(false, StatusID.FinalAppraisal) ?? false;
        protected static bool HasGreatStrides => Player?.HasStatus(false, StatusID.GreatStrides) ?? false;
    }
}
