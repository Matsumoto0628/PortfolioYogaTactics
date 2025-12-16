using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private Pool spark1;
    [SerializeField] private Pool explosion1;
    private Dictionary<PoolType, Pool> typeToPool = new Dictionary<PoolType, Pool>();

    public static PoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        typeToPool.Add(PoolType.Spark1, spark1);
        typeToPool.Add(PoolType.Explosion1, explosion1);

        foreach (PoolType poolType in typeToPool.Keys)
        {
            Setup(poolType);
        }
    }

    public void Setup(PoolType poolType)
    {
        typeToPool[poolType].Setup();
    }

    public Poolable GetPoolable(PoolType poolType)
    {
        return typeToPool[poolType].GetPoolable();
    }

    public void Return(PoolType poolType, Poolable poolable)
    {
        typeToPool[poolType].Return(poolable);
    }
}