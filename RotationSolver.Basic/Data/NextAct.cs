using RotationSolver.Basic.Actions;

namespace RotationSolver.Basic.Data;

public record NextAct(IBaseAction act, DateTime deadTime);
