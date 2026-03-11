using KnightAlpha;
using UnityEngine;
using System.Collections;

namespace KnightAlpha
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, KnightAlphaController>
    {
        public State State => State.Attack;

        public void Enter(KnightAlphaController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            SeManager.Instance.PlayOSSe(12);
            pawn.Self.GetComponent<TD_Unit>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(KnightAlphaController pawn)
        {
            
        }

        public void Exit(KnightAlphaController pawn)
        {

        }
    }
    
    public class Move : IState<State, KnightAlphaController>
    {
        public State State => State.Move;

        public void Enter(KnightAlphaController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(KnightAlphaController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(KnightAlphaController pawn)
        {
            
        }
    }
}

public class KnightAlphaController : PawnController
{
    private FSM<State, KnightAlphaController> fsm;
    public FSM<State, KnightAlphaController> FSM => fsm;

    public KnightAlphaController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, KnightAlphaController>(this, new IState<State, KnightAlphaController>[]
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