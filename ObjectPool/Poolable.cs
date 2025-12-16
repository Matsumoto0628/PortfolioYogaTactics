using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    public abstract void OnSetup();
    public abstract void OnGet();
    public abstract void OnReturn();
}
