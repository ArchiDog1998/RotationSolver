namespace RotationSolver.Vfx;

public class ActorVfx(string path, IGameObject caster, IGameObject target) 
    : NRender.Vfx.ActorVfx(path, caster, target), IDisposable
{
}
