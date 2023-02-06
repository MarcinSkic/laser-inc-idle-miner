using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "ScriptableObjects/Achievement", order = 1)]
public class AchievementScriptable : ScriptableObject
{
    [SerializeField]
    private Achievement achievement;
    public Achievement Achievement
    {
        get
        {
            return Functions.GetObjectCopy(achievement);
        }
        set => achievement = value;
    }
}