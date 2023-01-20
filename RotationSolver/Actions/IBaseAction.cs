using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Actions
{
    internal interface IBaseAction : IAction, IEnable
    {
        Func<BattleChara, bool> ActionCheck { get; set; }
        Func<BattleChara, bool> ComboCheck { get; set; }
        StatusID[] StatusProvide { get; set; }

        StatusID[] StatusNeed { get; set; }

        bool EnoughLevel { get; }

        bool ShouldUse(out IAction act, bool mustUse = false, bool emptyOrSkipCombo = false, bool skipDisable = false);

        #region CoolDown
        bool IsCoolDown { get; }

        ushort CurrentCharges { get; }
        ushort MaxCharges { get; }
        bool ElapsedAfterGCD(uint gcdCount = 0, uint abilityCount = 0);

        bool ElapsedAfter(float gcdelapsed);

        bool WillHaveOneChargeGCD(uint gcdCount = 0, uint abilityCount = 0);

        bool WillHaveOneCharge(float remain);
        #endregion

        #region Target
        StatusID[] TargetStatus { get; set; }

        BattleChara Target { get; }

        bool IsTargetBoss { get; }
        bool IsTargetDying { get; }
        #endregion
    }
}
