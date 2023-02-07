using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class  UIManager : MonoBehaviour
{
    GameObject BattleSceneUI = null;

    public void OffBattleUI()
    {
        BattleSceneUI = GameObject.Find("BattleScene");
        BattleSceneUI.SetActive(false);
    }

    public void OnBattleUI()
    {
        BattleSceneUI.SetActive(true);
    }
}
