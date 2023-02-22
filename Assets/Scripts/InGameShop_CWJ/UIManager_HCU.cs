using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    public void ChangeShopLevelUpCost(int cost)
    {
        TextMeshProUGUI shopLevelUpText = levelUpButton.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        shopLevelUpText.text = cost.ToString();
    }
}