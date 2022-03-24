namespace XIVComboPlus.Combos;

internal abstract class BRDCombo : CustomCombo
{
    internal sealed override string JobName => "吟游诗人";
    internal sealed override uint JobID => 23;

    protected struct Actions
    {
        public static readonly BaseAction
            //强力射击
            HeavyShoot = new BaseAction(97u),

            //直线射击
            StraitShoot = new BaseAction(98u) { BuffsNeed = new ushort[] { 122 } };
    }

    public static class Buffs
    {
        public const ushort StraightShotReady = 122;

        public const ushort BlastShotReady = 2692;

        public const ushort ShadowbiteReady = 3002;
    }

    public static class Debuffs
    {
        public const ushort VenomousBite = 124;

        public const ushort Windbite = 129;

        public const ushort CausticBite = 1200;

        public const ushort Stormbite = 1201;
    }

    public static class Levels
    {
        public const byte StraightShot = 2;

        public const byte VenomousBite = 6;

        public const byte Bloodletter = 12;

        public const byte Windbite = 30;

        public const byte RainOfDeath = 45;

        public const byte PitchPerfect = 52;

        public const byte EmpyrealArrow = 54;

        public const byte IronJaws = 56;

        public const byte Sidewinder = 60;

        public const byte BiteUpgrade = 64;

        public const byte RefulgentArrow = 70;

        public const byte Shadowbite = 72;

        public const byte BurstShot = 76;

        public const byte ApexArrow = 80;

        public const byte Ladonsbite = 82;

        public const byte BlastShot = 86;
    }


    public const uint HeavyShot = 97u;

    public const uint StraightShot = 98u;

    public const uint VenomousBite = 100u;

    public const uint QuickNock = 106u;

    public const uint Bloodletter = 110u;

    public const uint Windbite = 113u;

    public const uint RainOfDeath = 117u;

    public const uint EmpyrealArrow = 3558u;

    public const uint WanderersMinuet = 3559u;

    public const uint IronJaws = 3560u;

    public const uint Sidewinder = 3562u;

    public const uint PitchPerfect = 7404u;

    public const uint CausticBite = 7406u;

    public const uint Stormbite = 7407u;

    public const uint RefulgentArrow = 7409u;

    public const uint BurstShot = 16495u;

    public const uint ApexArrow = 16496u;

    public const uint Shadowbite = 16494u;

    public const uint Ladonsbite = 25783u;

    public const uint BlastArrow = 25784u;
}
