using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBall : MonoBehaviour
{
    public Data data;
    public GameController gameController;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * (float)data.GetSpd();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        applyConstantVelocity();
        TryDealDamage(collision);
    }

    protected virtual void TryDealDamage(Collision collision){
        collision.gameObject.TryGetComponent<BasicBlock>(out var block);
        block.TakeDamage(data.GetBulletDamage());
    }

    void applyConstantVelocity(){
        rb.velocity = rb.velocity.normalized * (float)data.GetSpd();
    }

}
