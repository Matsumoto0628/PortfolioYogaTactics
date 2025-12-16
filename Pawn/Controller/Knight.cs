using Knight;
using UnityEngine;
using System.Collections;

namespace Knight
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, KnightController>
    {
        public State State => State.Attack;

        public void Enter(KnightController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            pawn.Self.GetComponent<TD_Unit>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            },
            0.5f, 0.5f));
        }

        public void Update(KnightController pawn)
        {
            
        }

        public void Exit(KnightController pawn)
        {

        }
    }
    
    public class Move : IState<State, KnightController>
    {
        public State State => State.Move;

        public void Enter(KnightController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(KnightController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(KnightController pawn)
        {
            
        }
    }
}

public class KnightController : PawnController
{
    private FSM<State, KnightController> fsm;
    public FSM<State, KnightController> FSM => fsm;

    public KnightController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, KnightController>(this, new IState<State, KnightController>[]
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