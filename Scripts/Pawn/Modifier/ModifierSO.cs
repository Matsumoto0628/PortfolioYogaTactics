using UnityEngine;

[CreateAssetMenu(fileName = "ModifierSO", menuName = "Game/ModifierSO")]
public class ModifierSO : ScriptableObject
{    
    [Header("ボーナス時の強化個体の体力倍率")]
    [SerializeField, Range(0.5f, 5f)] public float BonusBoostHealth;
    [Header("ボーナス時の強化個体の攻撃力倍率")]
    [SerializeField, Range(0.5f, 5f)] public float BonusBoostAttack;
    [Header("ボーナス時の強化個体の速度倍率")]
    [SerializeField, Range(0.5f, 5f)] public float BonusBoostSpeed;
}