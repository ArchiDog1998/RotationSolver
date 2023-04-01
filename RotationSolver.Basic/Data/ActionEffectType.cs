namespace RotationSolver.Basic.Data;

public enum ActionEffectType : byte
{
    Nothing = 0,
    Miss = 1,
    FullResist = 2,
    Damage = 3,
    Heal = 4,
    BlockedDamage = 5,
    ParriedDamage = 6,
    Invulnerable = 7,
    NoEffectText = 8,
    FailMissingStatus = 9,
    MpLoss = 10, // 0x0A
    MpGain = 11, // 0x0B
    TpLoss = 12, // 0x0C
    TpGain = 13, // 0x0D
    ApplyStatusEffectTarget = 14, // 0x0E - dissector calls this "GpGain"
    ApplyStatusEffectSource = 15, // 0x0F
    RecoveredFromStatusEffect = 16, // 0x10
    LoseStatusEffectTarget = 17, // 0x11
    LoseStatusEffectSource = 18, // 0x12
    Unknown_13 = 19, // 0x13 - sometimes part of pvp Purify & Empyrean Rain spells, related to afflictions removal?..
    StatusNoEffect = 20, // 0x14
    ThreatPosition = 24, // 0x18
    EnmityAmountUp = 25, // 0x19
    EnmityAmountDown = 26, // 0x1A
    StartActionCombo = 27, // 0x1B
    Retaliation = 29, // 0x1D - 'vengeance' has value = 7, 'arms length' has value = 0
    Knockback = 32, // 0x20
    Attract1 = 33, // 0x21
    Attract2 = 34, // 0x22
    AttractCustom1 = 35, // 0x23
    AttractCustom2 = 36, // 0x24
    AttractCustom3 = 37, // 0x25
    Unknown_27 = 39, // 0x27
    Mount = 40, // 0x28
    unknown_30 = 48, // 0x30
    unknown_31 = 49, // 0x31
    Unknown_32 = 50, // 0x32
    Unknown_33 = 51, // 0x33
    FullResistStatus = 52, // 0x34
    Unknown_37 = 55, // 0x37 - 'arms length' has value = 9 on source, is this 'attack speed slow'?
    Unknown_38 = 56, // 0x38
    Unknown_39 = 57, // 0x39
    VFX = 59, // 0x3B
    Gauge = 60, // 0x3C
    Resource = 61, // 0x3D - value 0x34 = gain war gauge (amount == hitSeverity)
    Unknown_40 = 64, // 0x40
    Unknown_42 = 66, // 0x42
    Unknown_46 = 70, // 0x46
    Unknown_47 = 71, // 0x47
    SetModelState = 72, // 0x48 - value == model state
    SetHP = 73, // 0x49 - e.g. zodiark's kokytos
    Partial_Invulnerable = 74, // 0x4A
    Interrupt = 75, // 0x4B
}
