using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.Events;

public class GameModel : MonoBehaviour
{
    #region Depth
    [SerializeField] private double depth;
    public double cheatDepth = 100;

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
    #endregion

    public float heightOfBottomBar;
    public Transform bottomBorder;
    

    private void OnValidate()
    {
        Depth = depth;
    }
}
