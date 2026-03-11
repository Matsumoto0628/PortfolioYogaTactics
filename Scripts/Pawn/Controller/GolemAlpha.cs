using GolemAlpha;
using UnityEngine;
using System.Collections;

namespace GolemAlpha
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, GolemAlphaController>
    {

        public State State => State.Attack;

        public void Enter(GolemAlphaController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            SeManager.Instance.PlayOSSe(12);
            pawn.Self.GetComponent<TD_Unit>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(GolemAlphaController pawn)
        {
            
        }

        public void Exit(GolemAlphaController pawn)
        {

        }
    }
    
    public class Move : IState<State, GolemAlphaController>
    {
        public State State => State.Move;

        public void Enter(GolemAlphaController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(GolemAlphaController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(GolemAlphaController pawn)
        {
            
        }
    }
}

public class GolemAlphaController : PawnController
{
    private FSM<State, GolemAlphaController> fsm;
    public FSM<State, GolemAlphaController> FSM => fsm;

    public GolemAlphaController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, GolemAlphaController>(this, new IState<State, GolemAlphaController>[]
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