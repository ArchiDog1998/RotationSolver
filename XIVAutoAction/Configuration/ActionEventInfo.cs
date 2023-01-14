namespace XIVAutoAction.Configuration
{
    public class ActionEventInfo
    {
        public string Name { get; set; }
        public int MacroIndex { get; set; }
        public bool IsShared { get; set; }

        public ActionEventInfo()
        {
            Name = "";
            MacroIndex = -1;
            IsShared = false;
        }
    }
}
