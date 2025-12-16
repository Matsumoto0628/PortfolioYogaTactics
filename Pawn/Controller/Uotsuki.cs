using Uotsuki;
using UnityEngine;
using System.Collections;

namespace Uotsuki
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, UotsukiController>
    {
        public State State => State.Attack;

        public void Enter(UotsukiController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            pawn.Self.GetComponent<TD_Enemy>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(UotsukiController pawn)
        {
            
        }

        public void Exit(UotsukiController pawn)
        {

        }
    }
    
    public class Move : IState<State, UotsukiController>
    {
        public State State => State.Move;

        public void Enter(UotsukiController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(UotsukiController pawn)
        {
            pawn.Move();
            
            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(UotsukiController pawn)
        {
            
        }
    }
}

public class UotsukiController : PawnController
{
    private FSM<State, UotsukiController> fsm;
    public FSM<State, UotsukiController> FSM => fsm;

    public UotsukiController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, UotsukiController>(this, new IState<State, UotsukiController>[]
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