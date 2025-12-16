using System;
using System.Collections.Generic;

public interface IState<EState, TPawn> where EState : Enum
{
    public EState State { get; }
    public void Enter(TPawn pawn);
    public void Update(TPawn pawn);
    public void Exit(TPawn pawn);
}

public class FSM<EState, TPawn> where EState : Enum
{
    private TPawn pawn;
    public TPawn Pawn => pawn;
    private EState current;
    public EState Current => current;
    private IState<EState, TPawn>[] states;

    public FSM(TPawn pawn, IEnumerable<IState<EState, TPawn>> stateObjects)
    {
        this.pawn = pawn;

        states = new IState<EState, TPawn>[Enum.GetValues(typeof(EState)).Length];
        foreach (IState<EState, TPawn> state in stateObjects)
        {
            states[Convert.ToInt32(state.State)] = state;
        }
    }

    public void Update()
    {
        states[Convert.ToInt32(current)].Update(pawn);
    }

    public void Transit(EState next)
    {
        states[Convert.ToInt32(current)].Exit(pawn);
        states[Convert.ToInt32(next)].Enter(pawn);
        current = next;
    }
}