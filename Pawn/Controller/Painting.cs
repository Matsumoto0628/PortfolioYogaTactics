using Painting;
using UnityEngine;
using System.Collections;

namespace Painting
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, PaintingController>
    {
        public State State => State.Attack;

        public void Enter(PaintingController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            pawn.Self.GetComponent<TD_Unit>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(PaintingController pawn)
        {
            
        }

        public void Exit(PaintingController pawn)
        {

        }
    }
    
    public class Move : IState<State, PaintingController>
    {
        public State State => State.Move;

        public void Enter(PaintingController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(PaintingController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(PaintingController pawn)
        {
            
        }
    }
}

public class PaintingController : PawnController
{
    private FSM<State, PaintingController> fsm;
    public FSM<State, PaintingController> FSM => fsm;

    public PaintingController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, PaintingController>(this, new IState<State, PaintingController>[]
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