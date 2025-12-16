using UnityEngine;

[CreateAssetMenu(fileName = "PawnSO", menuName = "Game/PawnSO")]
public class PawnSO : ScriptableObject
{
    [Header("関連するヨガのポーズ"), SerializeField] 
    private string relatedYogaPose;
    public PoseType poseType;
    
    [Header("基本ステータス")]
    public string label = "NULL";
    public int cost;
    public Sprite icon;
    public float size;
    public int hp;
    public float speed;
    public int attack;
    public int level1;
    public int level2;
    public int level3;
    public RuntimeAnimatorController runtimeAnimController;

    [Header("移動アニメーション")]
    public AnimationClip moveAnim;

    [Header("攻撃アニメーション")]
    public AnimationClip attackAnim;

    [Header("被ダメージアニメーション")]
    public AnimationClip damageAnim;

    [Header("死亡アニメーション")]
    public AnimationClip deathAnim;

    [Header("アニメーションフレーム")]
    public float moveFPS;
    public float attackFPS;
    public float damageFPS;
    public float deathFPS;

    [Header("生成周り")]
    public float posY;
    public float offset;
    public bool isRandom;
    
    [Header("出撃準備完了")]
    public Sprite[] readyAnimSprites;
    public float readyFPS = 12f;
    public Vector2 readyOffset = Vector2.zero;
    public Vector2 readyScale = Vector2.one;
}