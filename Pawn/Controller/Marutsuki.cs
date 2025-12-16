using Marutsuki;
using UnityEngine;
using System.Collections;

namespace Marutsuki
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, MarutsukiController>
    {
        public State State => State.Attack;

        public void Enter(MarutsukiController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            pawn.Self.GetComponent<TD_Enemy>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(MarutsukiController pawn)
        {
            
        }

        public void Exit(MarutsukiController pawn)
        {

        }
    }
    
    public class Move : IState<State, MarutsukiController>
    {
        public State State => State.Move;

        public void Enter(MarutsukiController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(MarutsukiController pawn)
        {
            pawn.Move();
            
            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(MarutsukiController pawn)
        {
            
        }
    }
}

public class MarutsukiController : PawnController
{
    private FSM<State, MarutsukiController> fsm;
    public FSM<State, MarutsukiController> FSM => fsm;

    public MarutsukiController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, MarutsukiController>(this, new IState<State, MarutsukiController>[]
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