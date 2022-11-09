using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBall : MonoBehaviour
{
    public Data data;
    public GameController gameController;
    public Rigidbody rb;
    public float laserRotationSpeedDegrees;
    private float laserRotationDirection;

    public Color laserColor;

    void Start()
    {
        TrySetLaserColor(); //TODO @Filip Why 'Try', there is no case in which it will not set color (even if it is still white)
        InitLaserRotation();

        SetInitialVelocity();
    }

    void Update()
    {
        RotateLaser();
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        SetVelocity();
        UpdateLaserRotation();
        TryDealDamage(collision);
    }

    private void InitLaserRotation()
    {
        laserRotationDirection = Random.Range(0, 2) == 1 ? 1 : -1;
        laserRotationSpeedDegrees = Mathf.Abs(laserRotationSpeedDegrees) * laserRotationDirection;
    }

    private void SetInitialVelocity()
    {
        rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * (float)data.GetSpd();
    }

    protected virtual void TrySetLaserColor()
    {
        SpriteRenderer circle = transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();   //TODO unnecessary getComponent
        circle.color = laserColor;

        var arms = transform.GetChild(0).GetChild(0).GetComponentsInChildren<SpriteRenderer>();
        foreach (var arm in arms)
        {
            arm.color = laserColor;
        }

        TrailRenderer tr = GetComponent<TrailRenderer>();    //TODO unnecessary getComponent
        tr.startColor = new Color(laserColor.r, laserColor.g, laserColor.b, 0.3f);
    }

    private void RotateLaser()
    {
        transform.GetChild(0).GetChild(0).Rotate(new Vector3(0, 0, 1), Time.deltaTime * laserRotationSpeedDegrees);
    }

    protected virtual void TryDealDamage(Collision collision){
        if(collision.gameObject.TryGetComponent<BasicBlock>(out var block)){
            block.TakeDamage(data.GetBulletDamage());
        }
    }

    protected void SetVelocity(){
        if(rb.velocity.magnitude <= 0.000001f)
        {
            rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * (float)data.GetSpd();
            //Debug.Log("Ball has been unstuck");
        }
        rb.velocity = rb.velocity.normalized * (float)data.GetSpd();
    }

    private void UpdateLaserRotation()
    {
        laserRotationDirection = rb.angularVelocity.z >= 0 ? 1 : -1;
        rb.angularVelocity = new Vector3(0, 0, 0);
        laserRotationSpeedDegrees = Mathf.Abs(laserRotationSpeedDegrees) * laserRotationDirection;
    }
}
