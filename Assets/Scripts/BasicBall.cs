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

    // Start is called before the first frame update
    void Start()
    {
        laserRotationDirection = Random.Range(0, 2) == 1 ? 1 : -1;
        laserRotationSpeedDegrees = Mathf.Abs(laserRotationSpeedDegrees) * laserRotationDirection;
        rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * (float)data.GetSpd();

        TrySetLaserColor();
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(0).GetChild(0).Rotate(new Vector3(0, 0, 1), Time.deltaTime*laserRotationSpeedDegrees);
        /*if (rb.velocity.magnitude == 0)
        {
            rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * (float)data.GetSpd();
        }*/
    }

    void OnCollisionEnter(Collision collision)
    {
        applyConstantVelocity();
        TryDealDamage(collision);
        laserRotationDirection = rb.angularVelocity.z >= 0 ? 1 : -1;
        rb.angularVelocity = new Vector3(0, 0, 0);
        laserRotationSpeedDegrees = Mathf.Abs(laserRotationSpeedDegrees) * laserRotationDirection;
    }

    protected virtual void TrySetLaserColor()
    {
        SpriteRenderer circle = transform.GetChild(0).GetChild(1).GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        circle.color = laserColor;
        for (int i=0; i<3; i++)
        {
            SpriteRenderer arm = transform.GetChild(0).GetChild(0).GetChild(i).GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            arm.color = laserColor;
        }
        TrailRenderer tr = GetComponent(typeof(TrailRenderer)) as TrailRenderer;
        tr.startColor = new Color(laserColor.r, laserColor.g, laserColor.b, 0.3f);
    }

    protected virtual void TryDealDamage(Collision collision){
        if(collision.gameObject.TryGetComponent<BasicBlock>(out var block)){
            block.TakeDamage(data.GetBulletDamage());
        }
    }

    void applyConstantVelocity(){
        rb.velocity = rb.velocity.normalized * (float)data.GetSpd();
    }

}
