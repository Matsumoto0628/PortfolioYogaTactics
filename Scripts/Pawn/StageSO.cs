using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "StageSO", menuName = "Game/StageSO")]
public class StageSO : ScriptableObject
{
    /// <summary>
    // 必用な物
    // ステージの背景情報
    // キャラクターの生成ディレイ
    // 登場させる敵キャラクター
    /// </summary>
    [Header("ステージの背景情報")]
    public Sprite sprite;
    public List<SpawnDataSO> spawnData;

    [Header("ユニットの情報")]
    public PawnID topPawnID;
    public PawnID midPawnID;
    public PawnID botPawnID;
    
    [Header("バットが出現するかどうか")]
    public bool isBat;

    [Header("拠点のHP")]
    [Range(1, 1000)] public int castleHp = 150;

    [Header("敵の拠点のHP")]
    [Range(1, 1000)] public int castleEnemyHp = 200;
}
