using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool isEnemy;
    private int hitFPS;
    private int attack;
    private float currentFPS;
    private const float BASIC_FPS = 12;
    private bool isHit;
    private const float LIFE_TIME = 0.5f;

    public void Initialize(bool isEnemy, int hitFPS, int attack)
    {
        this.isEnemy = isEnemy;
        this.hitFPS = hitFPS;
        this.attack = attack;
    }

    private void Update()
    {
        if (!isHit)
        {
            Move();
        }

        UpdateAnimationSpeed();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEnemy) // 敵は左向きにUnitとPlayerを検知
        {
            if (collision.gameObject.CompareTag("Unit"))
            {
                Poolable effect = PoolManager.Instance.GetPoolable(PoolType.Spark1);
                effect.transform.position = collision.transform.position;
                collision.gameObject.GetComponent<TD_Unit>().PawnController.Hit(0.05f);
                collision.gameObject.GetComponent<TD_Unit>().PawnController.Status.TakeDamage(attack);

                Hit();
            }

            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<TD_PlayerCastle>().Hit(attack);

                Hit();
            }
        }
        else // 味方は右向きにEnemyとEnemyCastleを検知
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Poolable effect = PoolManager.Instance.GetPoolable(PoolType.Spark1);
                effect.transform.position = collision.transform.position;
                collision.gameObject.GetComponent<TD_Enemy>().PawnController.Hit(0.05f);
                collision.gameObject.GetComponent<TD_Enemy>().PawnController.Status.TakeDamage(attack);

                Hit();
            }

            if (collision.gameObject.CompareTag("EnemyCastle"))
            {
                collision.gameObject.GetComponent<TD_EnemyCastle>().Hit(attack);

                Hit();
            }
        }
    }

    private void UpdateAnimationSpeed()
    {
        animator.SetFloat("speed", ((float)currentFPS / BASIC_FPS) * TD_GameManager.Instance.GameSpeed);
    }

    private void Move()
    {
        const float SPEED = 10f;
        const float MAX_POS_X_UNIT = 120f;
        const float MIN_POS_X_ENEMY = 80f;

        if (isEnemy) // 敵は左に移動
        {
            if(transform.position.x >= MIN_POS_X_ENEMY)
            {
                transform.position += Vector3.left * SPEED * Time.deltaTime * TD_GameManager.Instance.GameSpeed;            
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else // 味方は右に移動
        {
            if(transform.position.x <= MAX_POS_X_UNIT)
            {
                transform.position += Vector3.right * SPEED * Time.deltaTime * TD_GameManager.Instance.GameSpeed;            
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void Hit()
    {
        currentFPS = hitFPS;
        isHit = true;   
        Destroy(gameObject, LIFE_TIME);
    }
}
