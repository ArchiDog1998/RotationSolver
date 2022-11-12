using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class RDMCombo_Base<TCmd> : JobGaugeCombo<RDMGauge, TCmd> where TCmd : Enum
{

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.RedMage };
    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;
    //¿´¿´ÏÖÔÚÓÐÃ»ÓÐ´Ù½ø

    private sealed protected override BaseAction Raise => Verraise;

    public static readonly BaseAction
        //³à¸´»î
        Verraise = new(7523, true),

        //Õðµ´
        Jolt = new(7503)
        {
            BuffsProvide = Swiftcast.BuffsProvide.Union(new[] { StatusID.Acceleration }).ToArray(),
        },

        //»Ø´Ì
        Riposte = new(7504)
        {
            OtherCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
        },

        //³àÉÁÀ×
        Verthunder = new(7505)
        {
            BuffsNeed = Jolt.BuffsProvide,
        },

        //¶Ì±øÏà½Ó
        CorpsAcorps = new(7506, shouldEndSpecial: true)
        {
            BuffsProvide = new[]
            {
                 StatusID.Bind1,
                 StatusID.Bind2,
            }
        },

        //³à¼²·ç
        Veraero = new(7507)
        {
            BuffsNeed = Jolt.BuffsProvide,
        },

        //É¢Ëé
        Scatter = new(7509)
        {
            BuffsNeed = Jolt.BuffsProvide,
        },

        //³àÕðÀ×
        Verthunder2 = new(16524u)
        {
            BuffsProvide = Jolt.BuffsProvide,
        },

        //³àÁÒ·ç
        Veraero2 = new(16525u)
        {
            BuffsProvide = Jolt.BuffsProvide,
        },

        //³à»ðÑ×
        Verfire = new(7510)
        {
            BuffsNeed = new[] { StatusID.VerfireReady },
            BuffsProvide = Jolt.BuffsProvide,
        },

        //³à·ÉÊ¯
        Verstone = new(7511)
        {
            BuffsNeed = new[] { StatusID.VerstoneReady },
            BuffsProvide = Jolt.BuffsProvide,
        },

        //½»»÷Õ¶
        Zwerchhau = new(7512)
        {
            OtherCheck = b => JobGauge.BlackMana >= 15 && JobGauge.WhiteMana >= 15,
        },

        //½»½£
        Engagement = new(16527),

        //·É½£
        Fleche = new(7517),

        //Á¬¹¥
        Redoublement = new(7516)
        {
            OtherCheck = b => JobGauge.BlackMana >= 15 && JobGauge.WhiteMana >= 15,
        },


        //´Ù½ø
        Acceleration = new(7518)
        {
            BuffsProvide = new[] { StatusID.Acceleration },
        },

        //»®Ô²Õ¶
        Moulinet = new(7513)
        {
            OtherCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
        },

        //³àÖÎÁÆ
        Vercure = new(7514, true)
        {
            BuffsProvide = Swiftcast.BuffsProvide.Union(Acceleration.BuffsProvide).ToArray(),
        },

        //Áù·Ö·´»÷
        ContreSixte = new(7519u),

        //¹ÄÀø
        Embolden = new(7520, true),

        //ÐøÕ¶
        Reprise = new(16529),

        //¿¹ËÀ
        MagickBarrier = new(25857),

        //³àºË±¬
        Verflare = new(7525),

        //³àÉñÊ¥
        Verholy = new(7526),

        //½¹ÈÈ
        Scorch = new(16530)
        {
            OtherIDsCombo = new uint[] { Verholy.ID },
        },

        //¾ö¶Ï
        Resolution = new(25858),

        //Ä§Ôª»¯
        Manafication = new(7521)
        {
            OtherCheck = b => JobGauge.WhiteMana <= 50 && JobGauge.BlackMana <= 50 && InCombat && JobGauge.ManaStacks == 0,
            OtherIDsNot = new uint[] { Riposte.ID, Zwerchhau.ID, Scorch.ID, Verflare.ID, Verholy.ID },
        };
}
