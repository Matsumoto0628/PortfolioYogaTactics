using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField] private uint initPoolSize;
    [SerializeField] private Poolable prefab;

    // コレクション内のプールされたオブジェクトを格納する
    private Stack<Poolable> stack;
    
    // プールを作成する（ラグが目立たないときに呼び出す）
    public void Setup()
    {
        stack = new Stack<Poolable>();
        for (int i = 0; i < initPoolSize; i++)
        {
            Poolable instance = Instantiate(prefab, transform);
            instance.OnSetup();
            instance.gameObject.SetActive(false);
            stack.Push(instance);
        }
    }

    public Poolable GetPoolable()
    {
        // プールの大きさが十分でない場合は、新しい Poolables をインスタンス化する
        if (stack.Count == 0)
        {
            return Instantiate(prefab, transform);
        }
        
        // それ以外の場合は、リストから次のものをグラブする
        Poolable instance = stack.Pop();
        instance.gameObject.SetActive(true);
        instance.OnGet();
        return instance;
    }

    public void Return(Poolable Poolable)
    {
        Poolable.OnReturn();
        Poolable.gameObject.SetActive(false);
        stack.Push(Poolable);
    }
}
