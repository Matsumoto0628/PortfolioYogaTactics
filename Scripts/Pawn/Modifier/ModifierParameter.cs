using UnityEngine;

public class ModifierParameter : MonoBehaviour
{
    public static ModifierParameter Instance { get; private set; }

    private ModifierSO modifierSO;

    private void Awake()
    {
        Instance = this;

        modifierSO = Resources.Load<ModifierSO>("ModifierSO/ModifierSO");
    }

    public ModifierSO GetModifierSO()
    {
        return modifierSO;
    }
}