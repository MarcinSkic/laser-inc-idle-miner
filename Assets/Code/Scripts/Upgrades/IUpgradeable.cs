using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeable<T>
{
    public T Data { get; set; }
    public abstract void SetDataReference(T Data);
    public abstract void Upgrade();
}
