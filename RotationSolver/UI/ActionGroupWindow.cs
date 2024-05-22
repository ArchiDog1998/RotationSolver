using Dalamud.Interface.Utility.Raii;

namespace RotationSolver.UI;
internal class ActionGroupWindow : CtrlWindow
{
    public ActionGroupWindow() : base(nameof(ActionGroupWindow))
    {
        Size = new Vector2(300, 300f);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void Draw()
    {
        foreach (var item in Service.Config.ActionGroups)
        {
            if (!item.ShowInWindow) continue;

            using var color = ImRaii.PushColor(ImGuiCol.Text, item.Color);
            if(ImGui.Selectable(item.Name + "##" + item.GetHashCode(), item.Enable))
            {
                item.Enable = !item.Enable;
            }
        }
    }
}
