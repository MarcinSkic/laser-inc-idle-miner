using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListWrapper<T> : ListWrapperBase
{
    public List<T> Value;

    public ListWrapper() { }
}

[System.Serializable]
public class ListWrapperBase
{
    public ListWrapperBase() { }
}