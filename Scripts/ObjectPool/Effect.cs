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

            float wait = 1f / fps;
            float elapsed = 0;
            while (elapsed < wait)
            {
                elapsed += Time.deltaTime * TD_GameManager.Instance.GameSpeed;
                yield return null;
            }
        }

        if (gameObject.activeSelf)
        {
            PoolManager.Instance.Return(poolType, this);
        }
    }
}