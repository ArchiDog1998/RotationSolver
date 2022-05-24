using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class NINCombo : CustomComboJob<NINGauge>
{
    internal override uint JobID => 30;

    internal struct Actions
    {
        public static readonly BaseAction
            //双刃旋
            SpinningEdge = new BaseAction(2240),

            //残影
            ShadeShift = new BaseAction(2241),

            //绝风
            GustSlash = new BaseAction(2242),

            //飞刀
            ThrowingDagger = new BaseAction(2247),

            //夺取
            Mug = new BaseAction(2248)
            {
                OtherCheck = b => JobGauge.Ninki <= 50,
            },

            //攻其不备
            TrickAttack = new BaseAction(2258)
            {
                EnermyLocation = EnemyLocation.Back,
            },

            //旋风刃
            AeolianEdge = new BaseAction(2255)
            {
                EnermyLocation = EnemyLocation.Back,
            },

            //血雨飞花
            DeathBlossom = new BaseAction(2254),

            //天之印
            Ten = new BaseAction(2259),

            //忍术
            Ninjutsu = new BaseAction(2260),

            //地之印
            Chi = new BaseAction(2261),

            //缩地
            Shukuchi = new BaseAction(2262),

            //断绝
            Assassinate = new BaseAction(2246),

            //人之印
            Jin = new BaseAction(2263),

            //生杀予夺
            Kassatsu = new BaseAction(2264),

            //八卦无刃杀
            HakkeMujinsatsu = new BaseAction(16488),

            //强甲破点突
            ArmorCrush = new BaseAction(3563)
            {
                EnermyLocation = EnemyLocation.Side,
                OtherCheck = b => JobGauge.HutonTimer < 30000,
            },

            //通灵之术·大虾蟆
            HellfrogMedium = new BaseAction(7401),

            //六道轮回
            Bhavacakra = new BaseAction(7402),

            //分身之术
            Bunshin = new BaseAction(16493),

            //残影镰鼬
            PhantomKamaitachi = new BaseAction(25774)
            {
                BuffsNeed = new ushort[] { ObjectStatus.PhantomKamaitachiReady },
            },

            //月影雷兽牙
            FleetingRaiju = new BaseAction(25778)
            {
                BuffsNeed = new ushort[] { ObjectStatus.RaijuReady },
            },

            //月影雷兽爪
            ForkedRaiju = new BaseAction(25777)
            {
                BuffsNeed = new ushort[] { ObjectStatus.RaijuReady },
            },

            //风来刃
            Huraijin = new BaseAction(25876)
            {
                OtherCheck = b => JobGauge.HutonTimer == 0,
            };
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //大招
        if (Actions.FleetingRaiju.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.ForkedRaiju.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.PhantomKamaitachi.ShouldUseAction(out act, lastComboActionID)) return true;

        //续状态
        if (Actions.Huraijin.ShouldUseAction(out act, lastComboActionID)) return true;

        //AOE
        if (Actions.HakkeMujinsatsu.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.DeathBlossom.ShouldUseAction(out act, lastComboActionID)) return true;

        //Single
        if (Actions.ArmorCrush.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.AeolianEdge.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.GustSlash.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.SpinningEdge.ShouldUseAction(out act, lastComboActionID)) return true;

        //飞刀
        if (Actions.ThrowingDagger.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.ShadeShift.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        if (JobGauge.Ninki >= 50)
        {
            if (Actions.Bunshin.ShouldUseAction(out act)) return true;
            if (Actions.HellfrogMedium.ShouldUseAction(out act)) return true;
            if (Actions.Bhavacakra.ShouldUseAction(out act)) return true;
        }

        if (Actions.Mug.ShouldUseAction(out act)) return true;
        if (Actions.Assassinate.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Shukuchi.ShouldUseAction(out act)) return true;

        return false;
    }
    public static class Buffs
    {
        public const ushort Mudra = 496;

        public const ushort Kassatsu = 497;

        public const ushort Suiton = 507;

        public const ushort Hidden = 614;

        public const ushort Bunshin = 1954;

        public const ushort PhantomKamaitachiReady = 2723;

        public const ushort ForkedRaijuReady = 2690;

        public const ushort FleetingRaijuReady = 2691;
    }

    public static class Debuffs
    {
        public const ushort TrickAttack = 638;

        public const ushort Placeholder = 0;
    }

    public static class Levels
    {
        public const byte GustSlash = 4;

        public const byte Hide = 10;

        public const byte Mug = 15;

        public const byte AeolianEdge = 26;

        public const byte Ninjitsu = 30;

        public const byte Suiton = 45;

        public const byte HakkeMujinsatsu = 52;

        public const byte ArmorCrush = 54;

        public const byte Huraijin = 60;

        public const byte TenChiJin = 70;

        public const byte Meisui = 72;

        public const byte EnhancedKassatsu = 76;

        public const byte Bunshin = 80;

        public const byte PhantomKamaitachi = 82;

        public const byte ForkedRaiju = 90;
    }

    public const uint SpinningEdge = 2240u;

    public const uint GustSlash = 2242u;

    public const uint Hide = 2245u;

    public const uint Assassinate = 8814u;

    public const uint Mug = 2248u;

    public const uint DeathBlossom = 2254u;

    public const uint AeolianEdge = 2255u;

    public const uint TrickAttack = 2258u;

    public const uint Ninjutsu = 2260u;

    public const uint Chi = 2261u;

    public const uint Tian = 2259u;

    public const uint FenShen = 16493u;

    public const uint JinNormal = 2263u;

    public const uint Kassatsu = 2264u;

    public const uint ArmorCrush = 3563u;

    public const uint DreamWithinADream = 3566u;

    public const uint TenChiJin = 7403u;

    public const uint LiuDao = 7402u;

    public const uint HakkeMujinsatsu = 16488u;

    public const uint Meisui = 16489u;

    public const uint Jin = 18807u;

    public const uint 背刺 = 2258u;

    public const uint Bunshin = 16493u;

    public const uint Huraijin = 25876u;

    public const uint PhantomKamaitachi = 25774u;

    public const uint ForkedRaiju = 25777u;

    public const uint FleetingRaiju = 25778u;
}
