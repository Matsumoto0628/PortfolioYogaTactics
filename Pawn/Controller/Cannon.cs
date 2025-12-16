using Cannon;
using UnityEngine;
using System.Collections;
using System;

namespace Cannon
{
    public enum State
    {
        Attack,
        Move
    }

    public class Attack : IState<State, CannonController>
    {
        public State State => State.Attack;

        public void Enter(CannonController pawn)
        {
            pawn.SetFPS(pawn.AttackFPS);
            pawn.Self.GetComponent<TD_Unit>().StartCoroutine(ShootCoroutine(pawn, () =>
            {
                pawn.FSM.Transit(State.Move);
            }));
        }

        public void Update(CannonController pawn)
        {
            
        }

        public void Exit(CannonController pawn)
        {

        }

        public IEnumerator ShootCoroutine(CannonController pawn, Action onComplete)
        {
            const float START_SEC = 0.5f;
            const float END_SEC = 1f;

            pawn.Animator.SetTrigger("attack");

            yield return new WaitForSeconds(START_SEC / TD_GameManager.Instance.GameSpeed);

            Shoot(pawn);

            yield return new WaitForSeconds(END_SEC / TD_GameManager.Instance.GameSpeed);

            onComplete();
        }

        private void Shoot(CannonController pawn)
        {
            Bullet bullet = UnityEngine.Object.Instantiate(pawn.BulletPrefab, pawn.Self.transform.position, Quaternion.identity);
            bullet.Initialize(pawn.IsEnemy, pawn.BulletFPS, pawn.Status.attack);
        }
    }
    
    public class Move : IState<State, CannonController>
    {
        public State State => State.Move;
        private const float RANGE_ATTACK = 2f;
        private const float SPAN_ATTACK = 5f;
        private float timer = 0;
        public void Enter(CannonController pawn)
        {
            timer = 0;
        }

        public void Update(CannonController pawn)
        {
            if (pawn.IsAttackRange(RANGE_ATTACK))
            {
                pawn.SetFPS(0);
                timer += Time.deltaTime;
                if (timer >= SPAN_ATTACK)
                {
                    pawn.FSM.Transit(State.Attack);
                }
            }
            else
            {
                pawn.SetFPS(pawn.MoveFPS);
                pawn.Move();
            }
        }

        public void Exit(CannonController pawn)
        {
            
        }
    }
}

public class CannonController : PawnController
{
    private FSM<State, CannonController> fsm;
    public FSM<State, CannonController> FSM => fsm;

    private int attack2FPS;
    private int bulletFPS;
    public int Attack2FPS => attack2FPS;
    public int BulletFPS => bulletFPS;
    private Bullet bulletPrefab;
    public Bullet BulletPrefab => bulletPrefab;

    public CannonController(GameObject self) : base(self) { }

    public override void SetupFSM()
    {
        fsm = new FSM<State, CannonController>(this, new IState<State, CannonController>[]
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

    public override PawnController SetDataFromSO(PawnSO pawnSO)
    {
        CannonSO cannonSO = (CannonSO)pawnSO;
        moveFPS = (int)cannonSO.moveFPS;
        attackFPS = (int)cannonSO.attackFPS;
        damageFPS = (int)cannonSO.damageFPS;
        deathFPS = (int)cannonSO.deathFPS;
        attack2FPS = (int)cannonSO.attack2FPS;
        bulletFPS = (int)cannonSO.ammoAnimFPS;
        bulletPrefab = cannonSO.bullet;

        return this;
    }
}