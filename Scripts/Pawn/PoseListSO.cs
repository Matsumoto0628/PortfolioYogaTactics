using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoseListSO", menuName = "Game/PoseListSO")]
public class PoseListSO : ScriptableObject
{
    [Header("対応するポーズタイプ"), SerializeField]
    public PoseType TargetPose;

    [Header("ミスしたパーツ"), SerializeField]
    private string[] missPartsName;

    [Header("対応する音源"), SerializeField]
    private AudioClip[] voiceClip;
    private Dictionary<string, AudioClip> voiceClipsDictionary;

    public Dictionary<string, AudioClip> GetPoseVoiceDictionary()
    {
        if (voiceClipsDictionary != null) return voiceClipsDictionary;
        voiceClipsDictionary = new Dictionary<string, AudioClip>();

        int count = Mathf.Min(missPartsName.Length, voiceClip.Length);

        for (int i = 0; i < count; i++)
        {
            if (!string.IsNullOrEmpty(missPartsName[i]) && !voiceClipsDictionary.ContainsKey(missPartsName[i]))
            {
                voiceClipsDictionary.Add(missPartsName[i], voiceClip[i]);
            }
            else
            {
                Debug.LogWarning($"{name}: 重複または空の名前をスキップしました: {missPartsName[i]}");
            }
        }
        return voiceClipsDictionary;
    }
}
