using FlamingoAlpha;
using UnityEngine;
using System.Collections;

namespace FlamingoAlpha
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, FlamingoAlphaController>
    {
        public State State => State.Attack;

        public void Enter(FlamingoAlphaController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            SeManager.Instance.PlayOSSe(12);
            pawn.Self.GetComponent<TD_Unit>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));

        }

        public void Update(FlamingoAlphaController pawn)
        {
        }

        public void Exit(FlamingoAlphaController pawn)
        {

        }
    }
    
    public class Move : IState<State, FlamingoAlphaController>
    {
        public State State => State.Move;

        public void Enter(FlamingoAlphaController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(FlamingoAlphaController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(FlamingoAlphaController pawn)
        {
            
        }
    }
}

public class FlamingoAlphaController : PawnController
{
    private FSM<State, FlamingoAlphaController> fsm;
    public FSM<State, FlamingoAlphaController> FSM => fsm;

    public FlamingoAlphaController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, FlamingoAlphaController>(this, new IState<State, FlamingoAlphaController>[]
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