using UnityEngine;
using System.Collections;
using System;

public abstract class PawnController
{
    [SerializeField] private TD_PawnManager pawnManager;

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
    private const float HIT_DURATION_BASIC = 0.25f;
    private const float HIT_DURATION_KNOCKBACK = 1f;
    private float hitDuration = HIT_DURATION_BASIC;
    private const float HIT_STRENGTH = 1f;
    private bool isEnemy;
    public bool IsEnemy => isEnemy;
    private Vector2 hitPos;
    private bool isKnockback;

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
            SeManager.Instance.PlayOSSe(13);
            UpdateDeath();
        }
        else if (isHit) // 被弾時
        {
            UpdateHit();
        }
        else // それ以外はUpdate処理(FSM更新)
        {
            UpdateFSM();
        }

        UpdateAnimationSpeed(); // アニメーションの速度を常に更新
    }

    public void Hit(float hitStrength = 1f, float hitDuration = HIT_DURATION_KNOCKBACK)
    {
        if (isHit)
        {
            return;
        }

        isHit = true;
        Animator.SetTrigger("damage");
        SetFPS(damageFPS);
        hitPos = self.transform.position;
        status.TakeStun(hitStrength);

        if (status.IsStun())
        {
            isKnockback = true;
            this.hitDuration = hitDuration * UnityEngine.Random.Range(0.75f, 1.25f);
        }
        else
        {
            this.hitDuration = HIT_DURATION_BASIC;
        }
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

    private void UpdateHit()
    {
        if (isKnockback)
        {
            UpdateKnockback();
        }

        // 時間処理
        hitTimer += Time.deltaTime * TD_GameManager.Instance.GameSpeed;
        if (hitTimer >= hitDuration)
        {
            isHit = false;
            hitTimer = 0;
            ResetFSM();

            if (isKnockback)
            {
                isKnockback = false;
                status.ResetStun();
                self.transform.position = new Vector3(
                    self.transform.position.x,
                    hitPos.y,
                    self.transform.position.z
                );
            }
        }
    }

    private void UpdateKnockback()
    {
        // ノックバック(横)
        Vector2 dir = isEnemy ? Vector2.right : Vector2.left;
        float timeDecay = 1 - (hitTimer / hitDuration);
        rigidbody.AddForce(dir * TD_GameManager.Instance.GameSpeed * timeDecay, ForceMode2D.Force);

        // ノックバック(縦)
        float halfDuration = hitDuration / 2;
        if (hitTimer >= halfDuration)
        {
            float posY = Mathf.Lerp(self.transform.position.y, hitPos.y, (hitTimer - halfDuration) / halfDuration);
            self.transform.position = new Vector3(
                self.transform.position.x,
                posY,
                self.transform.position.z
            );
        }
        else
        {
            float posY = Mathf.Lerp(self.transform.position.y, hitPos.y + 0.1f, hitTimer / halfDuration);
            self.transform.position = new Vector3(
                self.transform.position.x,
                posY,
                self.transform.position.z
            );
        }
    }

    private void UpdateDeath()
    {
        // 時間処理
        deathTimer += Time.deltaTime * TD_GameManager.Instance.GameSpeed;
        if (deathTimer >= DEATH_DURATION)
        {
            isDestroy = true;
        }
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

    public IEnumerator AttackCoroutine(Action onComplete)
    {
        animator.SetTrigger("attack");

        float wait = status.attackDurStart;
        float elapsed = 0;
        while (elapsed < wait)
        {
            elapsed += Time.deltaTime * TD_GameManager.Instance.GameSpeed;
            yield return null;
        }

        Attack(status.attackRange);

        wait = status.attackDurEnd;
        elapsed = 0;
        while (elapsed < wait)
        {
            elapsed += Time.deltaTime * TD_GameManager.Instance.GameSpeed;
            yield return null;
        }
        
        onComplete();
    }
    
    private void Attack(float range)
    {
        if (isEnemy) // 敵は左向きにUnitとPlayerを検知
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(self.transform.position, Vector2.left, range);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("Unit"))
                {
                    Poolable effect = PoolManager.Instance.GetPoolable(PoolType.Spark1);
                    if (effect != null)
                    {
                        effect.transform.position = hit.collider.transform.position;
                    }
                    hit.collider.gameObject.GetComponent<TD_Unit>().PawnController.Hit(status.hitStrength);
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
            RaycastHit2D[] hits = Physics2D.RaycastAll(self.transform.position, Vector2.right, range);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    Poolable effect = PoolManager.Instance.GetPoolable(PoolType.Spark1);
                    if (effect != null)
                    {
                        effect.transform.position = hit.collider.transform.position;
                    }
                    hit.collider.gameObject.GetComponent<TD_Enemy>().PawnController.Hit(status.hitStrength);
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