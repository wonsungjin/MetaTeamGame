using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSpwaner : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            GameMGR.Instance.batch.CreateBatch(0, i, 0== (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
