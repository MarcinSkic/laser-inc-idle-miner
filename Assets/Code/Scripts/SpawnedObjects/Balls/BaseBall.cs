using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BaseBall<T> : MonoBehaviour, IPoolable<BaseBall<T>>, IUpgradeable<T> where T : BaseBallData
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Color laserColor;
    [SerializeField] protected float laserRotationSpeedDegrees;
    public T Data { get; set; }
    public ObjectPool<BaseBall<T>> Pool { get ; set; }

    [Header("TEMP")]
    public GameController gameController;

    private float laserRotationDirection;

    public virtual void InitBall()
    {
        SetLaserColor();
        InitLaserRotation();
        SetInitialVelocity();
    }

    public virtual void Upgrade(Upgrade upgrade)
    {
        SetVelocity();
    }

    public virtual void SetDataReference(T Data)
    {
        this.Data = Data;
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
        rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * (float)Data.speed;
    }

    protected virtual void SetLaserColor()
    {
        SpriteRenderer circle = transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();   //TODO: unnecessary getComponent
        circle.color = laserColor;

        var arms = transform.GetChild(0).GetChild(0).GetComponentsInChildren<SpriteRenderer>();
        foreach (var arm in arms)
        {
            arm.color = laserColor;
        }

        TrailRenderer tr = GetComponent<TrailRenderer>();    //TODO: unnecessary getComponent
        tr.startColor = new Color(laserColor.r, laserColor.g, laserColor.b, 0.3f);
    }

    private void RotateLaser()
    {
        transform.GetChild(0).GetChild(0).Rotate(new Vector3(0, 0, 1), Time.deltaTime * laserRotationSpeedDegrees);
    }

    protected virtual void TryDealDamage(Collision collision){
        if(collision.gameObject.TryGetComponent<BasicBlock>(out var block)){
            block.TakeDamage(Data.damage);
        }
    }

    protected void SetVelocity(){
        if(rb.velocity.magnitude <= 0.000001f)
        {
            rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * (float)Data.speed;
            //Debug.Log("Ball has been unstuck");
        }
        rb.velocity = rb.velocity.normalized * (float)Data.speed;
    }

    private void UpdateLaserRotation()
    {
        laserRotationDirection = rb.angularVelocity.z >= 0 ? 1 : -1;
        rb.angularVelocity = new Vector3(0, 0, 0);
        laserRotationSpeedDegrees = Mathf.Abs(laserRotationSpeedDegrees) * laserRotationDirection;
    }
}

[System.Serializable]
public class BaseBallData
{
    public string name;
    public Sprite sprite;
    public UpgradeableData<double> speed;
    public UpgradeableData<double> damage;
}
