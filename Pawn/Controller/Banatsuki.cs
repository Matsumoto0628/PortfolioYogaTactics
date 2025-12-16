using Banatsuki;
using UnityEngine;
using System.Collections;

namespace Banatsuki
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, BanatsukiController>
    {
        public State State => State.Attack;

        public void Enter(BanatsukiController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            pawn.Self.GetComponent<TD_Enemy>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(BanatsukiController pawn)
        {
        }

        public void Exit(BanatsukiController pawn)
        {

        }
    }
    
    public class Move : IState<State, BanatsukiController>
    {
        public State State => State.Move;

        public void Enter(BanatsukiController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(BanatsukiController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(BanatsukiController pawn)
        {
            
        }
    }
}

public class BanatsukiController : PawnController
{
    private FSM<State, BanatsukiController> fsm;
    public FSM<State, BanatsukiController> FSM => fsm;

    public BanatsukiController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, BanatsukiController>(this, new IState<State, BanatsukiController>[]
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