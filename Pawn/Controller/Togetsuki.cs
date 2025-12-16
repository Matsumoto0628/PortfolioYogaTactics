using Togetsuki;
using UnityEngine;
using System.Collections;

namespace Togetsuki
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, TogetsukiController>
    {
        public State State => State.Attack;

        public void Enter(TogetsukiController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            pawn.Self.GetComponent<TD_Enemy>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(TogetsukiController pawn)
        {
            
        }

        public void Exit(TogetsukiController pawn)
        {

        }
    }
    
    public class Move : IState<State, TogetsukiController>
    {
        public State State => State.Move;

        public void Enter(TogetsukiController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(TogetsukiController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(TogetsukiController pawn)
        {
            
        }
    }
}

public class TogetsukiController : PawnController
{
    private FSM<State, TogetsukiController> fsm;
    public FSM<State, TogetsukiController> FSM => fsm;

    public TogetsukiController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, TogetsukiController>(this, new IState<State, TogetsukiController>[]
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