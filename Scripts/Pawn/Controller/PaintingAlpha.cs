using PaintingAlpha;
using UnityEngine;
using System.Collections;

namespace PaintingAlpha
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, PaintingAlphaController>
    {

        public State State => State.Attack;
       

        public void Enter(PaintingAlphaController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            SeManager.Instance.PlayOSSe(12);
            pawn.Self.GetComponent<TD_Unit>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(PaintingAlphaController pawn)
        {
            
        }

        public void Exit(PaintingAlphaController pawn)
        {

        }
    }
    
    public class Move : IState<State, PaintingAlphaController>
    {
        public State State => State.Move;

        public void Enter(PaintingAlphaController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(PaintingAlphaController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(PaintingAlphaController pawn)
        {
            
        }
    }
}

public class PaintingAlphaController : PawnController
{
    private FSM<State, PaintingAlphaController> fsm;
    public FSM<State, PaintingAlphaController> FSM => fsm;

    public PaintingAlphaController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, PaintingAlphaController>(this, new IState<State, PaintingAlphaController>[]
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