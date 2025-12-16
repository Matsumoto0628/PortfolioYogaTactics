using UnityEngine;

[CreateAssetMenu(fileName = "CannonSO", menuName = "Game/CannonSO")]
public class CannonSO : PawnSO
{
    [Header("攻撃アニメーション2")]
    public AnimationClip attack2Anim;
    public float attack2FPS;
    
    [Header("弾丸")]
    public Bullet bullet;

    [Header("アニメーション")]
    public AnimationClip ammoAnim;
    public float ammoAnimFPS;

    [Header("アニメーションコントローラ")]
    public RuntimeAnimatorController ammoAnimController;
}