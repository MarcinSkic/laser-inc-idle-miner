using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBall : BasicBall
{
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "border" && gameController._dynamic_blocks.childCount > 0)
        {
            Transform target = gameController._dynamic_blocks.GetChild(0);
            for (int i = 0; i < gameController._dynamic_blocks.childCount; i++)
            {
                BasicBlock block = gameController._dynamic_blocks.GetChild(i).GetComponent<BasicBlock>();
                if (Vector3.Distance(block.GetComponent<BoxCollider>().ClosestPoint(this.transform.position), this.transform.position) < 
                    (Vector3.Distance((target.position), this.transform.position)))
                {
                    target = gameController._dynamic_blocks.GetChild(i);
                }
            }
            rb.velocity = (target.position - transform.position).normalized * (float)data.GetSpd() * 2f;
        } else {
            rb.velocity = rb.velocity.normalized * (float)data.GetSpd();
        }

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
