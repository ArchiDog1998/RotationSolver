using RotationSolver.Basic.Actions;

namespace RotationSolver.Basic.Data;

public record NextAct(IAction act, DateTime deadTime);
