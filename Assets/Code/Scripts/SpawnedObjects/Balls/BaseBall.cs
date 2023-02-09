using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using MyBox;

public class BaseBall<T> : MonoBehaviour, IPoolable<BaseBall<T>>, IUpgradeable<T> where T : BallData
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Color laserColor;
    [SerializeField] protected float laserRotationSpeedDegrees;
    public T Data { get; set; }
    public ObjectPool<BaseBall<T>> Pool { get ; set; }
    public Rigidbody _rb => rb;

    private float laserRotationDirection;
    private float ballOneDimensionCorectionThreshold = 0.1f;
    private float ballSpeedPercentageCorrectionThreshold = 0.1f;
    private float velocityAppliedToBallStuckInOneDimension = 2f;
    private float velocityCorrectionDelay = 0.01f;

    public virtual void InitBall()
    {
        SetLaserColor();
        InitLaserRotation();
        SetInitialVelocity();
    }

    public void Upgrade(Upgrade upgrade)
    {
        SetVelocity();
    }

    public virtual void Upgrade(string value)
    {
        SetVelocity();
    }

    public virtual void SetVariables()
    {

    }

    public virtual void SetDataReference(T Data)
    {
        this.Data = Data;
    }

    void Update()
    {
        RotateLaser();
        Debug.Log(rb.velocity);
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
        rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * (float)Data.values[UpgradeableValues.Speed];
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

        transform.GetChild(0).GetChild(2).GetComponent<Light>().color = new Color(laserColor.r, laserColor.g, laserColor.b);
    }

    private void RotateLaser()
    {
        transform.GetChild(0).GetChild(0).Rotate(new Vector3(0, 0, 1), Time.deltaTime * laserRotationSpeedDegrees);
    }

    protected virtual void TryDealDamage(Collision collision){
        if(collision.gameObject.TryGetComponent<BasicBlock>(out var block)){
            block.TakeDamage(Data.values[UpgradeableValues.Damage]);
        }
    }

    IEnumerator ProtectVelocity(bool recursive=false)
    {
        bool attemptedCorrection = false;
        bool correctionApplied = false;

        if (recursive)
        {
            yield return new WaitForSeconds(velocityCorrectionDelay);
        } else
        {
            yield return new WaitForSeconds(velocityCorrectionDelay * 5);
        }
        
        if (rb.velocity.magnitude <= (float)Data.values[UpgradeableValues.Speed]* ballSpeedPercentageCorrectionThreshold)
        {
            rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * (float)Data.values[UpgradeableValues.Speed];
            NormalizeVelocity();
            Debug.Log("Ball has been unstuck",this);
            correctionApplied = true;
        }
        else if(Mathf.Abs(rb.velocity.x) <= ballOneDimensionCorectionThreshold || Mathf.Abs(rb.velocity.y) <= ballOneDimensionCorectionThreshold)
        {
            if (Mathf.Abs(rb.velocity.x) <= ballOneDimensionCorectionThreshold && recursive)
            {
                rb.velocity += new Vector3(transform.position.x < 0 ? velocityAppliedToBallStuckInOneDimension : -velocityAppliedToBallStuckInOneDimension, 0, 0);
                NormalizeVelocity();
                Debug.Log("Ball has been pushed horizontally", this);
                correctionApplied = true;
            }
            else if (Mathf.Abs(rb.velocity.y) <= ballOneDimensionCorectionThreshold && recursive)
            {
                rb.velocity += new Vector3(0, -velocityAppliedToBallStuckInOneDimension, 0);
                NormalizeVelocity();
                Debug.Log("Ball has been pushed vertically", this);
                correctionApplied = true;
            }

            attemptedCorrection = true;
        }

        attemptedCorrection |= correctionApplied;

        if (attemptedCorrection)
        {
            if (correctionApplied)
            {
                SettingsModel.Instance.ballsAppliedCorrections++;
            }
            SettingsModel.Instance.ballsAttemptedCorrections++;
            yield return new WaitForSeconds(velocityCorrectionDelay);
            StartCoroutine(ProtectVelocity(true));
        }
    }

    protected void NormalizeVelocity()
    {
        rb.velocity = rb.velocity.normalized * (float)Data.values[UpgradeableValues.Speed];
    }

    protected void SetVelocity(){
        NormalizeVelocity();
        StartCoroutine(ProtectVelocity());
    }

    private void UpdateLaserRotation()
    {
        laserRotationDirection = rb.angularVelocity.z >= 0 ? 1 : -1;
        rb.angularVelocity = new Vector3(0, 0, 0);
        laserRotationSpeedDegrees = Mathf.Abs(laserRotationSpeedDegrees) * laserRotationDirection;
    }
}
