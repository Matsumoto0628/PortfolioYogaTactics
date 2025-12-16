using UnityEngine;
using System.Collections;
using System;

public abstract class PawnController
{
    private GameObject self;
    public GameObject Self => self;

    private PawnStatus status;
    public PawnStatus Status => status;

    private Animator animator;
    public Animator Animator => animator;

    private Rigidbody2D rigidbody;
    public Rigidbody2D Rigidbody => rigidbody;

    protected int deathFPS;
    protected int moveFPS;
    protected int attackFPS;
    protected int damageFPS;
    public int DeathFPS => deathFPS;
    public int MoveFPS => moveFPS;
    public int AttackFPS => attackFPS;
    public int DamageFPS => damageFPS;
    private const int BASIC_FPS = 12;
    private int currentFPS = BASIC_FPS;

    private bool isDeath = false;
    private bool isHit = false;
    private bool isDestroy = false;
    public bool IsDeath => isDeath;
    public bool IsHit => isHit;
    public bool IsDestroy => isDestroy;
    private float deathTimer = 0;
    private const float DEATH_DURATION = 0.5f;
    private float hitTimer = 0;
    private const float HIT_DURATION_BASIC = 1f;
    private float hitDuration = HIT_DURATION_BASIC;
    private bool isEnemy;
    public bool IsEnemy => isEnemy;

    public PawnController(GameObject self)
    {
        this.self = self;
    }

    public PawnController Initialize(PawnStatus status, Animator animator, Rigidbody2D rigidbody)
    {
        this.status = status;
        this.animator = animator;
        this.rigidbody = rigidbody;

        return this;
    }

    public abstract void SetupFSM();
    public abstract void UpdateFSM();
    public abstract void ResetFSM();

    public void Update()
    {
        if (status.hp <= 0 && !isDeath)
        {
            Death();
        }

        if (isDeath) // 死亡時
        {
            deathTimer += Time.deltaTime * TD_GameManager.Instance.GameSpeed;
            if (deathTimer >= DEATH_DURATION)
            {
                isDestroy = true;
            }
        }
        else if (isHit) // 被弾時
        {
            Vector2 dir = isEnemy ? Vector2.right : Vector2.left;
            float timeDecay = 1 - (hitTimer / hitDuration);
            rigidbody.AddForce(dir * TD_GameManager.Instance.GameSpeed * timeDecay, ForceMode2D.Force);

            hitTimer += Time.deltaTime * TD_GameManager.Instance.GameSpeed;
            if (hitTimer >= hitDuration)
            {
                isHit = false;
                hitTimer = 0;
                ResetFSM();
            }
        }
        else // それ以外はUpdate処理(FSM更新)
        {
            UpdateFSM();
        }

        UpdateAnimationSpeed(); // アニメーションの速度を常に更新
    }

    public void Hit(float hitDuration = HIT_DURATION_BASIC)
    {
        if (isHit)
        {
            return;
        }

        isHit = true;
        Animator.SetTrigger("damage");
        SetFPS(damageFPS);
        this.hitDuration = hitDuration * UnityEngine.Random.Range(0.75f, 1.25f);
    }

    public void Death()
    {
        if (isDeath)
        {
            return;
        }

        isDeath = true;
        animator.SetBool("death", true);
        SetFPS(deathFPS);
    }

    public virtual PawnController SetDataFromSO(PawnSO pawnSO)
    {
        moveFPS = (int)pawnSO.moveFPS;
        attackFPS = (int)pawnSO.attackFPS;
        damageFPS = (int)pawnSO.damageFPS;
        deathFPS = (int)pawnSO.deathFPS;

        return this;
    }

    private void UpdateAnimationSpeed()
    {
        animator.SetFloat("speed", ((float)currentFPS / BASIC_FPS) * TD_GameManager.Instance.GameSpeed);
    }

    public void SetFPS(int fps)
    {
        currentFPS = fps;
    }

    public PawnController SetIsEnemy()
    {
        isEnemy = true;
        return this;
    }

    public void Move()
    {
        const float MAX_POS_X_UNIT = 103f;
        const float MIN_POS_X_ENEMY = 91f;

        if (isEnemy) // 敵は左に移動
        {
            if(self.transform.position.x >= MIN_POS_X_ENEMY)
            {
                self.transform.position += Vector3.left * status.speed * Time.deltaTime * TD_GameManager.Instance.GameSpeed;            
            }
        }
        else // 味方は右に移動
        {
            if(self.transform.position.x <= MAX_POS_X_UNIT)
            {
                self.transform.position += Vector3.right * status.speed * Time.deltaTime * TD_GameManager.Instance.GameSpeed;            
            }
        }
    }

    public bool IsAttackRange(float range = 0.5f)
    {
        if (isEnemy) // 敵は左向きにUnitとPlayerを検知
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(self.transform.position, Vector2.left, range);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("Unit") || hit.collider.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }

            return false;
        }
        else // 味方は右向きにEnemyとEnemyCastleを検知
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(self.transform.position, Vector2.right, range);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("Enemy") || hit.collider.gameObject.CompareTag("EnemyCastle"))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public IEnumerator AttackCoroutine(Action onComplete, float startDur = 0.5f, float endDur = 1f)
    {
        animator.SetTrigger("attack");

        yield return new WaitForSeconds(startDur / TD_GameManager.Instance.GameSpeed);

        Attack();

        yield return new WaitForSeconds(endDur / TD_GameManager.Instance.GameSpeed);

        onComplete();
    }

    private void Attack()
    {
        const float RANGE_ATTACK = 0.5f;

        if (isEnemy) // 敵は左向きにUnitとPlayerを検知
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(self.transform.position, Vector2.left, RANGE_ATTACK);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("Unit"))
                {
                    Poolable effect = PoolManager.Instance.GetPoolable(PoolType.Spark1);
                    effect.transform.position = hit.collider.transform.position;
                    hit.collider.gameObject.GetComponent<TD_Unit>().PawnController.Hit();
                    hit.collider.gameObject.GetComponent<TD_Unit>().PawnController.Status.TakeDamage(status.attack);
                }

                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    hit.collider.gameObject.GetComponent<TD_PlayerCastle>().Hit(status.attack);
                }
            }
        }
        else // 味方は右向きにEnemyとEnemyCastleを検知
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(self.transform.position, Vector2.right, RANGE_ATTACK);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    Poolable effect = PoolManager.Instance.GetPoolable(PoolType.Spark1);
                    effect.transform.position = hit.collider.transform.position;
                    hit.collider.gameObject.GetComponent<TD_Enemy>().PawnController.Hit();
                    hit.collider.gameObject.GetComponent<TD_Enemy>().PawnController.Status.TakeDamage(status.attack);
                }

                if (hit.collider.gameObject.CompareTag("EnemyCastle"))
                {
                    hit.collider.gameObject.GetComponent<TD_EnemyCastle>().Hit(status.attack);
                }
            }
        }
    }
}