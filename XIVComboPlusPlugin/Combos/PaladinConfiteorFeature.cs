using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class PaladinConfiteorFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinConfiteorFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[2] { 7384u, 16458u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 7384 || actionID == 16458)
        {
            if (lastComboMove == 25749 && level >= 90)
            {
                return 25750u;
            }
            if (lastComboMove == 25748 && level >= 90)
            {
                return 25749u;
            }
            if (lastComboMove == 16459 && level >= 90)
            {
                return 25748u;
            }
            if (level >= 80)
            {
                Status val = FindEffect(1368);
                if (val != null)
                {
                    if (val.StackCount != 1)
                    {
                        PlayerCharacter localPlayer = LocalPlayer;
                        if (localPlayer == null || localPlayer.CurrentMp >= 2000)
                        {
                            goto IL_0089;
                        }
                    }
                    return 16459u;
                }
            }
        }
        goto IL_0089;
    IL_0089:
        return actionID;
    }
}
