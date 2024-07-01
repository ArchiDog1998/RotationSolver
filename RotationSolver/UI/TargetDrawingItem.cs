using XIVConfigUI;
using XIVDrawer.Element3D;

namespace RotationSolver.UI;
internal abstract class TargetDrawingItem
{
    public abstract bool Show { get; }
    public abstract IGameObject[] Targets { get; }

    readonly Drawing3DImage[] Drawings = new Drawing3DImage[64];

    public abstract float Height { get; }
    public abstract float Size { get; }

    public TargetDrawingItem()
    {
        for (int i = 0; i < Drawings.Length; i++)
        {
            Drawings[i] = new Drawing3DImage(null, Vector3.Zero)
            {
                Enable = false,
                MustInViewRange = true,
            };
        }
    }

    protected abstract uint GetIcon(IGameObject IGameObject);

    public void Update()
    {
        foreach (var item in Drawings)
        {
            item.Enable = false;
        }

        if (!Show)
        {
            return;
        }

        var targets = Targets;

        for (int i = 0; i < Math.Min(Drawings.Length, targets.Length); i++)
        {
            var item = Drawings[i];
            var obj = targets[i];

            var iconId = GetIcon(obj);
            if (!ImageLoader.GetTexture(iconId, out var icon)) continue;

            item.Enable = true;
            item.Image = icon;
            item.Position = obj.Position + new Vector3(0, Height, 0);
            item.Size = Size;
        }
    }
}
