using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public interface IPoolable<T> where T : MonoBehaviour
{
    public ObjectPool<T> Pool { get; set; }
}
