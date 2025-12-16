using Empty;
using UnityEngine;

namespace Empty
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, EmptyController>
    {
        public State State => State.Attack;

        public void Enter(EmptyController pawn)
        {

        }

        public void Update(EmptyController pawn)
        {

        }

        public void Exit(EmptyController pawn)
        {

        }
    }
    
    public class Move : IState<State, EmptyController>
    {
        public State State => State.Move;

        public void Enter(EmptyController pawn)
        {

        }

        public void Update(EmptyController pawn)
        {
            
        }

        public void Exit(EmptyController pawn)
        {
            
        }
    }
}

public class EmptyController : PawnController
{
    private FSM<State, EmptyController> fsm;
    public FSM<State, EmptyController> FSM => fsm;

    public EmptyController(GameObject self) : base(self) { }

   public override void SetupFSM()
    {
        fsm = new FSM<State, EmptyController>(this, new IState<State, EmptyController>[]
        {
            new Attack(),
            new Move()
        });
        fsm.Transit(State.Move);
    }

    public override void UpdateFSM()
    {
        fsm.Update();
    }

    public override void ResetFSM()
    {
        fsm.Transit(State.Move);
    }
}