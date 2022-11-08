using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBall : BasicBall
{
    protected override void TryDealDamage(Collision collision)
    {
        //TODO - omówić czy chcemy żeby bomba biła tylko przy trafieniu bloku czy też przy trafieniu obramówki
        if(collision.gameObject.TryGetComponent<BasicBlock>(out var block)){
            for (int i=0; i<gameController._dynamic_blocks.childCount; i++)
            {
                //TODO - cast sphere a nie szukać wszystkich childów!
                BasicBlock foundBlock = gameController._dynamic_blocks.GetChild(i).GetComponent<BasicBlock>();
                if (Vector3.Distance(foundBlock.GetComponent<BoxCollider>().ClosestPoint(this.transform.position), this.transform.position) < data.explosionSize)
                {
                    foundBlock.TakeDamage(data.GetBulletDamage());
                }
            }
        }
    }
}
