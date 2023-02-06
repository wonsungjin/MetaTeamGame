using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public partial class BattleLogic : MonoBehaviourPunCallbacks
{
    
    // 스킬 능력 함수들 모음집
    public void ThrowMissile(int value)
    {
       if (value < 0)
        enemyForwardUnits[exArray[randomArrayNum]].GetComponent<Card>().curHP -= value;
    }

 
}
