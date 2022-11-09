using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBlock : MonoBehaviour
{
    public double hp;
    public double maxHp;
    public GameController gameController;
    public Data data;

    [SerializeField]    
    private BoxCollider boxCollider;
    public BoxCollider BoxCollider { get => boxCollider;}

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)){
            TakeDamage(1);
        }
    }

    public void TakeDamage(double damage)
    {
        hp -= damage;
        if (data.displayFloatingText)
        {
            FloatingTextController.CreateFloatingText(damage.ToString(), transform);
        }
    }

    private void Update()
    {
        if (hp <= 0)
        {
            gameController.AddMoney(maxHp);
            Destroy(this.gameObject);
        }
    }
}
