using System;
using UnityEngine;

public class TD_Enemy : MonoBehaviour, IPawn
{
    private TD_EnemyGenerator owner;

    public void SetOwner(TD_EnemyGenerator generator)
    {
        owner = generator;
    }

    private void OnDestroy()
    {
        owner?.RemoveUnit(this);
    }

    [SerializeField] private AudioClip hitClip;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Action<TD_Enemy> onDestroy;
    private PawnController pawnController;
    public PawnController PawnController => pawnController;

    public void Initialize(Action<TD_Enemy> onDestroy, PawnSO pawnSO, PawnController pawnController)
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        Animator animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = pawnSO.runtimeAnimController;
        rb.drag = 3f;
        sr.sprite = pawnSO.icon;
        transform.localScale *= pawnSO.size;
        this.onDestroy = onDestroy;
        this.pawnController = pawnController;
        this.pawnController.Initialize(new PawnStatus(pawnSO), animator, rb)
            .SetDataFromSO(pawnSO)
            .SetIsEnemy()
            .SetupFSM();
    }

    private void Update()
    {
        if (pawnController.IsDestroy)
        {
            Destroy(gameObject);
            onDestroy(this);
            StopAllCoroutines();
        }

        pawnController.Update();
    }
}
