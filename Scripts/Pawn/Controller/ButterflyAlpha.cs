using System.Collections;
using ButterflyAlpha;
using UnityEngine;

namespace ButterflyAlpha
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, ButterflyAlphaController>
    {
        public State State => State.Attack;

        public void Enter(ButterflyAlphaController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            SeManager.Instance.PlayOSSe(12);
            pawn.Self.GetComponent<TD_Unit>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(ButterflyAlphaController pawn)
        {
        }

        public void Exit(ButterflyAlphaController pawn)
        {

        }
    }
    
    public class Move : IState<State, ButterflyAlphaController>
    {
        public State State => State.Move;

        public void Enter(ButterflyAlphaController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(ButterflyAlphaController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(ButterflyAlphaController pawn)
        {
            
        }
    }
}

public class ButterflyAlphaController : PawnController
{
    private FSM<State, ButterflyAlphaController> fsm;
    public FSM<State, ButterflyAlphaController> FSM => fsm;

    public ButterflyAlphaController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, ButterflyAlphaController>(this, new IState<State, ButterflyAlphaController>[]
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