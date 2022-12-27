using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using MyBox;

public abstract class BaseSpawner<T> : MonoBehaviour where T : MonoBehaviour, IPoolable<T>
{
    [Header("BASE SPAWNER")]
    [SerializeField] protected T prefab;
    [SerializeField] protected Transform instantionsParent;

    protected ObjectPool<T> pool;

    [Header("Debug")]
    [ReadOnly]
    public int active;
    [ReadOnly]
    public int inactive;

    protected virtual void Awake()
    {
        pool = new ObjectPool<T>(Create, Get, Release);
    }

    private void RefreshObjectsCount()
    {
        active = pool.CountActive;
        inactive = pool.CountInactive;
    }

    public virtual void Spawn(out T spawnedObject)
    {    
        spawnedObject = pool.Get();
        RefreshObjectsCount();
    }

    protected virtual T Create()
    {
        T element = Instantiate(prefab, instantionsParent);
        element.Pool = pool;
        return element;
    }

    protected virtual void Get(T element)
    {
        element.gameObject.SetActive(true);
    }

    protected virtual void Release(T element)
    {
        element.gameObject.SetActive(false);
        RefreshObjectsCount();
    }
}
