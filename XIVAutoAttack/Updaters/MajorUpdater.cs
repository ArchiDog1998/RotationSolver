using Dalamud.Game;

namespace XIVAutoAttack.Updaters
{
    internal partial class MajorUpdater
    {
#if DEBUG
        private static readonly Dictionary<int, bool> _valus = new Dictionary<int, bool>();
#endif
        internal unsafe static void Framework_Update(Framework framework)
        {
            if (!Service.Conditions.Any()) return;

            PreviewUpdater.UpdateCastBar();
#if DEBUG
            //Get changed condition.
            string[] enumNames = Enum.GetNames(typeof(Dalamud.Game.ClientState.Conditions.ConditionFlag));
            int[] indexs = (int[])Enum.GetValues(typeof(Dalamud.Game.ClientState.Conditions.ConditionFlag));
            if (enumNames.Length == indexs.Length)
            {
                for (int i = 0; i < enumNames.Length; i++)
                {
                    string key = enumNames[i];
                    bool newValue = Service.Conditions[(Dalamud.Game.ClientState.Conditions.ConditionFlag)indexs[i]];
                    if (_valus.ContainsKey(i) && _valus[i] != newValue && indexs[i] != 48 && indexs[i] != 27)
                    {
                        Service.ToastGui.ShowQuest(indexs[i].ToString() + " " + key + ": " + newValue.ToString());
                    }
                    _valus[i] = newValue;
                }
            }

            //for (int i = 0; i < 100; i++)
            //{
            //    bool newValue = Service.Conditions[i];
            //    if (_valus.ContainsKey(i) && _valus[i] != newValue)
            //    {
            //        Service.ToastGui.ShowQuest(i.ToString());
            //    }
            //    _valus[i] = newValue;
            //}
#endif

            //Update State.
            PreviewUpdater.UpdateEntry();

            if (Service.ClientState.LocalPlayer == null) return;

            if (Service.ClientState.LocalPlayer.CurrentHp == 0
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas51]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.RolePlaying])
                IconReplacer.AutoAttack = false;


            ActionUpdater.UpdateWeaponTime();

            TargetUpdater.UpdateHostileTargets();
            TargetUpdater.UpdateFriends();

            ActionUpdater.DoAction();
            MacroUpdater.UpdateMacro();
        }

        public static void Dispose()
        {
            ActionUpdater.Dispose();
            PreviewUpdater.Dispose();
        }
    }
}
