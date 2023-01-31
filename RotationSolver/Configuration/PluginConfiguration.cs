using Dalamud.Configuration;
using RotationSolver.Data;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace RotationSolver.Configuration;

[Serializable]
public class PluginConfiguration : IPluginConfiguration
{
    public int Version { get; set; } = 6;

    public int VoiceVolume = 100;
    public SortedSet<string> DisabledCombos { get; private set; } = new SortedSet<string>();
    public SortedSet<uint> DiabledActions { get; private set; } = new SortedSet<uint>();
    public List<ActionEventInfo> Events { get; private set; } = new List<ActionEventInfo>();
    public Dictionary<uint, Dictionary<string, Dictionary<string, string>>> RotationsConfigurations { get; private set; }
        = new Dictionary<uint, Dictionary<string, Dictionary<string, string>>>();
    public Dictionary<uint, string> ComboChoices { get; private set; } = new Dictionary<uint, string>();
    public Dictionary<uint, byte> TargetToHostileTypes { get; set; } =
        new Dictionary<uint, byte>();
    public int AddDotGCDCount = 2;

    public int TimelineIndex = 0;
    public bool AutoBurst = true;
    public bool AutoOffBetweenArea = true;
    public bool UseAbility = true;
    public bool UseDefenceAbility = true;
    public bool NeverReplaceIcon = false;
    public bool AutoProvokeForTank = true;
    public bool AutoUseTrueNorth = true;
    public bool ChangeTargetForFate = true;
    public bool MoveTowardsScreenCenter = true;

    public bool SayOutStateChanged = true;

    public bool ShowInfoOnDtr = true;

    public bool SayPotional = true;

    public bool FlytextPositional = true;
    public bool HealOutOfCombat = false;
    public bool ShowInfoOnToast = true;
    public bool RaiseAll = false;
    public bool CastingDisplay = true;
    public bool PoslockCasting = false;
    public int PoslockModifier = 0;
    public bool RaisePlayerByCasting = true;
    public bool RaisePlayerBySwift = true;
    public bool RaiseBrinkofDeath = true;
    public int LessMPNoRaise = 0;
    public bool AutoShield = true;
    public bool AddEnemyListToHostile = true;
    public bool UseAOEWhenManual = false;
    public bool UseItem = false;
    public bool PositionalFeedback = true;
    public bool ShowMoveTarget = true;
    public bool ShowTarget = true;
    public bool ChooseAttackMark = true;
    public bool CanAttackMarkAOE = true;
    public bool FilterStopMark = true;
    public bool UseOverlayWindow = true;
    public bool TeachingMode = true;
    public Vector3 TeachingModeColor = new(0f, 1f, 0.8f);
    public Vector3 MovingTargetColor = new(0f, 1f, 0.8f);
    public Vector3 TargetColor = new(1f, 0.2f, 0f);
    public Vector3 SubTargetColor = new(1f, 0.9f, 0f);
    public bool KeyBoardNoise = true;
    public bool UseGroundBeneficialAbility = true;
    public bool MoveAreaActionFarthest = true;
    public bool StartOnCountdown = true;
    public bool NoNewHostiles = false;
    public bool UseHealWhenNotAHealer = true;
    public float ObjectMinRadius = 0f;
    public float HealthDifference = 0.25f;
    public Dictionary<ClassJobID, float> HealingOfTimeSubtractSingles { get; set; } = new Dictionary<ClassJobID, float>();

    public Dictionary<ClassJobID, float> HealingOfTimeSubtractAreas { get; set; } = new Dictionary<ClassJobID, float>();
    public Dictionary<ClassJobID, float> HealthAreaAbilities { get; set; } = new Dictionary<ClassJobID, float>();
    public float HealthAreaAbility = 0.75f;

    public Dictionary<ClassJobID, float> HealthAreafSpells { get; set; } = new Dictionary<ClassJobID, float>();
    public float HealthAreafSpell = 0.65f;

    public Dictionary<ClassJobID, float> HealthSingleAbilities { get; set; } = new Dictionary<ClassJobID, float>();
    public float HealthSingleAbility = 0.7f;

    public Dictionary<ClassJobID, float> HealthSingleSpells { get; set; } = new Dictionary<ClassJobID, float>();
    public float HealthSingleSpell = 0.55f;

    public Dictionary<ClassJobID, float> HealthForDyingTanks { get; set; } = new Dictionary<ClassJobID, float>();

    public bool InterruptibleMoreCheck = true;
    public float SpecialDuration = 3;
    public float WeaponInterval = 0.67f;
    public float WeaponFaster = 0.08f;

    public float WeaponDelayMin = 0;
    public float WeaponDelayMax = 0;

    public float DeathDelayMin = 0;
    public float DeathDelayMax = 0;

    public float WeakenDelayMin = 0;
    public float WeakenDelayMax = 0;

    public float HostileDelayMin = 0;
    public float HostileDelayMax = 0;

    public float HealDelayMin = 0;
    public float HealDelayMax = 0;

    public float InterruptDelayMin = 0.5f;
    public float InterruptDelayMax = 1;

    public string PositionalErrorText = string.Empty;

    public int MoveTargetAngle = 24;
    public List<TargetingType> TargetingTypes { get; set; } = new List<TargetingType>();
    public int TargetingIndex { get; set; } = 0;
    public void Save()
    {
        Service.Interface.SavePluginConfig(this);
    }
}
