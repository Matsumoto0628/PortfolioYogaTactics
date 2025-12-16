using System;
using UnityEngine;

public class TD_Unit : MonoBehaviour, IPawn
{
    private TD_UnitGenerator owner; // ←追加

    public void SetOwner(TD_UnitGenerator generator)
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
    private Action<TD_Unit> onDestroy;
    private PawnController pawnController;
    public PawnController PawnController => pawnController;

    public void Initialize(Action<TD_Unit> onDestroy, PawnSO pawnSO, PawnController pawnController)
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
