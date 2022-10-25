using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;

namespace XIVAutoAttack
{
    internal unsafe class MacroItem
    {
        private GameObject _lastTarget;
        public GameObject Target { get; }
        public bool IsRunning { get; private set; }
        public RaptureMacroModule.Macro* Macro { get; }

        public MacroItem(GameObject target, RaptureMacroModule.Macro* macro)
        {
            Macro = macro;
            Target = target;
        }

        public bool StartUseMacro()
        {
            if (RaptureShellModule.Instance->MacroCurrentLine > -1) return false;

            _lastTarget = Service.TargetManager.Target;
            Service.TargetManager.SetTarget(Target);
            RaptureShellModule.Instance->ExecuteMacro(Macro);

            IsRunning = true;
            return true;
        }

        public bool EndUseMacro()
        {
            if (RaptureShellModule.Instance->MacroCurrentLine > -1) return false;

            Service.TargetManager.SetTarget(_lastTarget);

            IsRunning = false;
            return true;
        }
    }
}
