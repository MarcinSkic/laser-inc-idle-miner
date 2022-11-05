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
        rb.velocity = rb.velocity.normalized * (float)data.GetSpd();
        if (collision.gameObject.tag == "block")
        {
            collision.gameObject.GetComponent<BasicBlock>().TakeDamage(data.GetBulletDamage());
        }
        if (collision.gameObject.tag == "block" || collision.gameObject.tag == "border")
        {
            // rb.velocity = speed * Vector3.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
        }

    }

}
