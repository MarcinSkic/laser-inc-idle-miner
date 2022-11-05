using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBuyUpgrade : MonoBehaviour
{
    public UpgradeBuyingBar upgradeBar;
    public GameController gameController;
    public void BuyUpgrade() => gameController.BuyUpgrade(upgradeBar.upgradeName);
}
