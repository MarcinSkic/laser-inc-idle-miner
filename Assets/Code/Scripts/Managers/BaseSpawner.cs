using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BaseSpawner<T> : MonoBehaviour where T : MonoBehaviour, IPoolable<T>
{
    [Header("BASE SPAWNER")]
    [SerializeField] private T prefab;
    [SerializeField] private Transform instantionsParent;

    private ObjectPool<T> pool;

    [Header("Debug")]
    public int active;
    public int inactive;

    private void Awake()
    {
        pool = new ObjectPool<T>(Create, Get, Release);
    }

    private void Update()
    {
        //TODO: Add if isDebugging parameter, it should be in Settings.Instance in future
        active = pool.CountActive;
        inactive = pool.CountInactive;
    }

    public T Spawn()
    {    
        return pool.Get();
    }

    private T Create()
    {
        T element = Instantiate(prefab, instantionsParent);
        element.Pool = pool;
        return element;
    }

    private void Get(T element)
    {
        element.gameObject.SetActive(true);
    }

    private void Release(T element)
    {
        element.gameObject.SetActive(false);
    }
}
