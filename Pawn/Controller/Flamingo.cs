using Flamingo;
using UnityEngine;
using System.Collections;

namespace Flamingo
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, FlamingoController>
    {
        public State State => State.Attack;

        private const float START_DURATION = 0.5f;
        private const float END_DURATION = 0.5f;

        public void Enter(FlamingoController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            pawn.Self.GetComponent<TD_Unit>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            },
            START_DURATION, END_DURATION));
        }

        public void Update(FlamingoController pawn)
        {
        }

        public void Exit(FlamingoController pawn)
        {

        }
    }
    
    public class Move : IState<State, FlamingoController>
    {
        public State State => State.Move;

        public void Enter(FlamingoController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(FlamingoController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(FlamingoController pawn)
        {
            
        }
    }
}

public class FlamingoController : PawnController
{
    private FSM<State, FlamingoController> fsm;
    public FSM<State, FlamingoController> FSM => fsm;

    public FlamingoController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, FlamingoController>(this, new IState<State, FlamingoController>[]
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