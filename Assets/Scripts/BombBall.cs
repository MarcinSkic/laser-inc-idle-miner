using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBall : BasicBall
{
    protected override void TryDealDamage(Collision collision)
    {
        //TODO - omówić czy chcemy żeby bomba biła tylko przy trafieniu bloku czy też przy trafieniu obramówki
        if(collision.gameObject.TryGetComponent<BasicBlock>(out _)){
            var blocks = gameController._dynamic_blocks.GetComponentsInChildren<BasicBlock>();

            foreach (var block in blocks) {
                if(Vector3.Distance(block.BoxCollider.ClosestPoint(transform.position),transform.position) < data.explosionSize)
                {
                    block.TakeDamage(data.GetBulletDamage()); //TODO better damage pick
                } 
            }
        }
    }
}
