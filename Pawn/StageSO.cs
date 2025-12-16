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
}
