using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunAnimation : MonoBehaviour
{

    [SerializeField] private new Renderer renderer;

    [SerializeField] private float offsetY;
    [SerializeField] private float maxOffsetLength = 0.1f;
    [SerializeField] private float timeMultiplier = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        offsetY = Mathf.Repeat(Time.time*timeMultiplier, maxOffsetLength);
        renderer.material.mainTextureOffset = new Vector2(0f, offsetY);
    }
}
