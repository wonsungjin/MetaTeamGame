using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSpwaner : MonoBehaviourPun
{
    bool isMyunit = true;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        // 나의 상점에서 받아온 유닛 배치 정보
        for (int i = 0; i < 6; i++)
        {
            GameMGR.Instance.batch.CreateBatch(0, i, 0 == (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        }

        // 매칭된 상대방의 상점에서 받아온 유닛 배치 정보
        for (int i = 0; i < 6; i++)
        {
            GameMGR.Instance.batch.CreateBatch(0, i, 0 == (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        }
    }
}
