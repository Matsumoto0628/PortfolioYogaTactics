using System.Collections;
using UnityEngine;

public class Effect : Poolable
{
    [SerializeField] private PoolType poolType;
    [SerializeField] private Sprite[] sprites;
    [SerializeField, Range(1, 300)] private int fps = 1;
    private Coroutine spriteAnimation;
    private SpriteRenderer spriteRenderer;

    public override void OnSetup()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnGet()
    {
        Play();
    }

    public override void OnReturn()
    {
        if (spriteAnimation != null)
        {
            StopCoroutine(spriteAnimation);
        }
    }

    private void Play()
    {
        if (spriteAnimation != null)
        {
            StopCoroutine(spriteAnimation);
        }
        spriteAnimation = StartCoroutine(SpriteAnimation());
    }

    private IEnumerator SpriteAnimation()
    {
        foreach (Sprite sprite in sprites)
        {
            spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds((1f / fps) / TD_GameManager.Instance.GameSpeed);
        }

        if (gameObject.activeSelf)
        {
            PoolManager.Instance.Return(poolType, this);
        }
    }
}