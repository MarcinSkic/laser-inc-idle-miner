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
    [Tooltip("Equals to bats per ~166s with current (2023/08/18) settings")]
    public int batsPer10000FixedUpdates = 1;
    public bool batFrenzyActive = false;


    private void OnValidate()
    {
        Depth = depth;
    }
}
