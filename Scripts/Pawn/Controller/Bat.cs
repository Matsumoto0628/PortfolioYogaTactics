using Bat;
using UnityEngine;

namespace Bat
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, BatController>
    {
        public State State => State.Attack;

        public void Enter(BatController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            SeManager.Instance.PlayOSSe(12);
            pawn.Self.GetComponent<TD_Enemy>().StartCoroutine(pawn.AttackCoroutine(() =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(BatController pawn)
        {
        }

        public void Exit(BatController pawn)
        {

        }
    }
    
    public class Move : IState<State, BatController>
    {
        public State State => State.Move;

        public void Enter(BatController pawn)
        {
            pawn.SetFPS(pawn.MoveFPS);
        }

        public void Update(BatController pawn)
        {
            pawn.Move();

            if (pawn.IsAttackRange())
            {
                pawn.FSM.Transit(State.Attack);
            }
        }

        public void Exit(BatController pawn)
        {
            
        }
    }
}

public class BatController : PawnController
{
    private FSM<State, BatController> fsm;
    public FSM<State, BatController> FSM => fsm;

    public BatController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, BatController>(this, new IState<State, BatController>[]
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