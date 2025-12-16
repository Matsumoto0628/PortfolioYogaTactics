using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class TD_PawnManager : MonoBehaviour
{
    [SerializeField] private TD_ScriptableobjectRef _scriptableObjectRef;
    [SerializeField] private TD_UnitUI[] unitUI;
    [SerializeField] private Sprite[] unitRef;
    [SerializeField] private GameObject[] poseRef;

    private Dictionary<int, PawnID> poseNumToPawnID = new Dictionary<int, PawnID>();
    public Dictionary<int, PawnID> PoseNumToPawnID => poseNumToPawnID;
    private PawnSO[] pawnSOs;
    private Dictionary<int, PawnSO> poseNumToPawnSO = new Dictionary<int, PawnSO>();
    public Dictionary<int, PawnSO> PoseNumToPawnSO => poseNumToPawnSO;
    private Dictionary<PawnID, Sprite> pawnIDToSprite = new Dictionary<PawnID, Sprite>();
    public Dictionary<PawnID, Sprite> PawnIDToSprite => pawnIDToSprite;

    private void Awake()
    {
        pawnSOs = Resources.LoadAll<PawnSO>("PawnSO");

        StageSO currentStageSO = _scriptableObjectRef.stageSOList[GameManager.Instance.SelectedStageIndex - 1];
        poseNumToPawnID.Add(0, currentStageSO.topPawnID);
        poseNumToPawnID.Add(1, currentStageSO.midPawnID);
        poseNumToPawnID.Add(2, currentStageSO.botPawnID);

        foreach (int poseNum in poseNumToPawnID.Keys)
        {
            poseNumToPawnSO.Add(poseNum, GetPawnSO(poseNumToPawnID[poseNum]));
        }

        pawnIDToSprite.Add(PawnID.Butterfly, unitRef[0]);
        pawnIDToSprite.Add(PawnID.Cannon, unitRef[1]);
        pawnIDToSprite.Add(PawnID.Flamingo, unitRef[2]);
        pawnIDToSprite.Add(PawnID.Golem, unitRef[3]);
        pawnIDToSprite.Add(PawnID.Knight, unitRef[4]);
        pawnIDToSprite.Add(PawnID.Painting, unitRef[5]);

        for (int i = 0; i < unitUI.Length; i++)
        {
            Sprite sprite = pawnIDToSprite[poseNumToPawnID[i]];
            string label = poseNumToPawnSO[i].label;
            PoseType poseType = poseNumToPawnSO[i].poseType;
            GameObject pose = GeneratePose(poseType);
            unitUI[i].Initialize(sprite, label, pose);
        }
    }

    private PawnSO GetPawnSO(PawnID pawnID)
    {
        string name = Enum.GetName(typeof(PawnID), pawnID);
        foreach (PawnSO pawnSO in pawnSOs)
        {
            if (name == pawnSO.name)
            {
                return pawnSO;
            }
        }

        Debug.LogError($"PawnID:{name}が見つかりません");
        return null;
    }

    private GameObject GeneratePose(PoseType poseType)
    {
        switch (poseType)
        {
            case PoseType.Dancer:
                return Instantiate(poseRef[0]);
            case PoseType.CrescentMoon:
                return Instantiate(poseRef[1]);
            case PoseType.Fighter:
                return Instantiate(poseRef[2]);
            case PoseType.Cannon:
                return Instantiate(poseRef[3]);
            case PoseType.Wall:
                return Instantiate(poseRef[4]);
            case PoseType.Painting:
                return Instantiate(poseRef[5]);
            default:
                Debug.LogError("ポーズプレファブが見つかりません(TD_PawnManager.cs)");
                return null;
        }
    }

    public void OnCursorEnter(int poseNum)
    {
        unitUI[poseNum].OnCursorEnter();
    }

    public void OnCursorExit(int poseNum)
    {
                
        unitUI[poseNum].OnCursorExit();
    }
}