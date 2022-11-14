using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BasicBall : MonoBehaviour
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Color laserColor;
    [SerializeField] protected float laserRotationSpeedDegrees;
    public ObjectPool<BasicBall> Pool {protected get; set;}

#region Upgradeable
    protected double speed;
    protected double damage;
#endregion

    [Header("TEMP")]
    public GameController gameController;

    private float laserRotationDirection;

    public virtual void InitBall(double speed, double damage)
    {
        this.speed = speed;
        this.damage = damage;

        SetLaserColor();
        InitLaserRotation();
        SetInitialVelocity();
    }

    public void UpgradeBall(double speed, double damage)    //TODO-HOTFIX
    {
        this.speed = speed;
        this.damage = damage;
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
        rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * (float)speed;
    }

    protected virtual void SetLaserColor()
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
            block.TakeDamage(damage);
        }
    }

    protected void SetVelocity(){
        if(rb.velocity.magnitude <= 0.000001f)
        {
            rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * (float)speed;
            //Debug.Log("Ball has been unstuck");
        }
        rb.velocity = rb.velocity.normalized * (float)speed;
    }

    private void UpdateLaserRotation()
    {
        laserRotationDirection = rb.angularVelocity.z >= 0 ? 1 : -1;
        rb.angularVelocity = new Vector3(0, 0, 0);
        laserRotationSpeedDegrees = Mathf.Abs(laserRotationSpeedDegrees) * laserRotationDirection;
    }
}
