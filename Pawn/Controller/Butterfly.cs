using System.Collections;
using Butterfly;
using UnityEngine;

namespace Butterfly
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, ButterflyController>
    {
        public State State => State.Attack;

        public void Enter(ButterflyController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            pawn.Self.GetComponent<TD_Unit>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(ButterflyController pawn)
        {
        }

        public void Exit(ButterflyController pawn)
        {

        }
    }
    
    public class Move : IState<State, ButterflyController>
    {
        public State State => State.Move;

        public void Enter(ButterflyController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(ButterflyController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(ButterflyController pawn)
        {
            
        }
    }
}

public class ButterflyController : PawnController
{
    private FSM<State, ButterflyController> fsm;
    public FSM<State, ButterflyController> FSM => fsm;

    public ButterflyController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, ButterflyController>(this, new IState<State, ButterflyController>[]
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