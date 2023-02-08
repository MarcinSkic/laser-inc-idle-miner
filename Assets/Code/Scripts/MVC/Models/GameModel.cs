using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.Events;

public class GameModel : MonoBehaviour
{
    [SerializeField] private double depth;

    public UnityAction<double> onDepthChange;
    public double Depth
    {
        get
        {
            return depth;
        }
        set
        {
            depth = value;
            onDepthChange?.Invoke(depth);
        }
    }

    private void OnValidate()
    {
        Depth = depth;
    }
}
