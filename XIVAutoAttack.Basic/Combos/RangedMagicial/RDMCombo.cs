using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.RangedMagicial;

public abstract class RDMCombo<TCmd> : JobGaugeCombo<RDMGauge, TCmd> where TCmd : Enum
{

    public sealed override uint[] JobIDs => new uint[] { 35 };
    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;
    //¿´¿´ÏÖÔÚÓÐÃ»ÓÐ´Ù½ø

    private protected override BaseAction Raise => Verraise;

    protected static bool StartLong = false;

    public class RDMAction : BaseAction
    {
        public override ushort[] BuffsNeed 
        {
            get => NeedBuffNotCast ? base.BuffsNeed : null;
            set => base.BuffsNeed = value; 
        }
        public bool NeedBuffNotCast => !StartLong || InCombat;

        internal RDMAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false) : base(actionID, isFriendly, shouldEndSpecial)
        {
            BuffsNeed = Swiftcast.BuffsProvide.Union(new[] { ObjectStatus.Acceleration }).ToArray();
        }
    }
    public static readonly BaseAction
        //³à¸´»î
        Verraise = new(7523, true),

        //Õðµ´
        Jolt = new(7503)
        {
            BuffsProvide = Swiftcast.BuffsProvide.Union(new[] { ObjectStatus.Acceleration }).ToArray(),
        },

        //»Ø´Ì
        Riposte = new(7504)
        {
            OtherCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
        },

        //³àÉÁÀ×
        Verthunder = new RDMAction(7505),

        //¶Ì±øÏà½Ó
        CorpsAcorps = new(7506, shouldEndSpecial: true)
        {
            BuffsProvide = new[]
            {
                    ObjectStatus.Bind1,
                    ObjectStatus.Bind2,
            }
        },

        //³à¼²·ç
        Veraero = new RDMAction(7507),

        //É¢Ëé
        Scatter = new RDMAction(7509),

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
            BuffsNeed = new[] { ObjectStatus.VerfireReady },
            BuffsProvide = Jolt.BuffsProvide,
        },

        //³à·ÉÊ¯
        Verstone = new(7511)
        {
            BuffsNeed = new[] { ObjectStatus.VerstoneReady },
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
            BuffsProvide = new[] { ObjectStatus.Acceleration },
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
