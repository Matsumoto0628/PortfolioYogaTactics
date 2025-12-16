using Golem;
using UnityEngine;
using System.Collections;

namespace Golem
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, GolemController>
    {
        public State State => State.Attack;

        public void Enter(GolemController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            pawn.Self.GetComponent<TD_Unit>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(GolemController pawn)
        {
            
        }

        public void Exit(GolemController pawn)
        {

        }
    }
    
    public class Move : IState<State, GolemController>
    {
        public State State => State.Move;

        public void Enter(GolemController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(GolemController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(GolemController pawn)
        {
            
        }
    }
}

public class GolemController : PawnController
{
    private FSM<State, GolemController> fsm;
    public FSM<State, GolemController> FSM => fsm;

    public GolemController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, GolemController>(this, new IState<State, GolemController>[]
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