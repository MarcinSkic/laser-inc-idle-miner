using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBall : BasicBall
{
    void OnCollisionEnter(Collision collision)
    {
        for (int i=0; i<gameController._dynamic_blocks.childCount; i++)
        {
            BasicBlock block = gameController._dynamic_blocks.GetChild(i).GetComponent<BasicBlock>();
            if (Vector3.Distance(block.GetComponent<BoxCollider>().ClosestPoint(this.transform.position), this.transform.position) < 2)
            {
                block.TakeDamage(data.GetBulletDamage());
            }
        }
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
