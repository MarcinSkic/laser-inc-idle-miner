using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class BasicBlock : MonoBehaviour, IPoolable<BasicBlock>
{
    [SerializeField]
    private BoxCollider boxCollider;
    public BoxCollider BoxCollider { get => boxCollider; }

    public ObjectPool<BasicBlock> Pool { get; set; }

    protected double hp;
    protected double maxHp;
    protected double reward;
    protected double poisonPerSecond = 0;

    public void FixedUpdate()
    {
        if (poisonPerSecond>0)
        {
            // TODO - can be optimized
            TakeDamage(poisonPerSecond * Time.deltaTime, true);
        }
    }

    private FloatingText floatingRepeatedText = null;
    private double repeatedTotalValue=0;

/*    void OnMouseOver()
    {
        //if (Input.GetMouseButtonDown(0)){
            TakeDamage(1);
        //}
    }
*/
    public void InitBlock(double baseHp, double hpMultiplier, double rewardMultiplier)
    {
        maxHp = baseHp*hpMultiplier;
        hp = maxHp;
        reward = maxHp * rewardMultiplier;
    }

    public void AssignEvents(UnityAction<double> onBlockDestroyed)
    {
        this.onBlockDestroyed = onBlockDestroyed;
    }

    public void DisplayDamageTaken(double damage, bool repeating = false)
    {
        if (repeating) {
            if (floatingRepeatedText) {
                Destroy(floatingRepeatedText.gameObject);
                repeatedTotalValue += damage;
            } else {
                repeatedTotalValue = damage;
            }
            floatingRepeatedText = FloatingTextController.Instance.CreateFloatingText(repeatedTotalValue.ToString("F2"), transform);
        } else {
            FloatingTextController.Instance.CreateFloatingText(damage.ToString("F2"), transform);
        }
    }

    public void TakeDamage(double damage, bool repeating=false)
    {
        hp -= damage;

        if (hp <= 0 && gameObject.activeSelf)
        {
            RemoveBlock();
        }

        if (SettingsModel.Instance.DisplayFloatingText)   
        {
            DisplayDamageTaken(damage, repeating);
        }
        
    }

    public void TakePoison(double damagePerSecond)
    {
        poisonPerSecond += damagePerSecond;
    }

    /// <summary>
    /// Double is maxHp that is used to money calculations
    /// </summary>
    private UnityAction<double> onBlockDestroyed;  //TODO-FUTURE: Maybe change it to transfer data packet if it will be used for upgrades
    private void RemoveBlock()
    {
        onBlockDestroyed?.Invoke(reward);
        Pool.Release(this);
    }
}
