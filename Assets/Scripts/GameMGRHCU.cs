using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameMGR : Singleton<GameMGR>
{
    // 나와 상대방의 매칭 정보를 확인할 수 있는 정수형 배열인 것이다. 0은 나고 1은 상대다.
    public int[] matching = new int[2];

    // 서로가 동일한 랜덤값을 가지기 위한 것이다.
    public int[] randomValue = new int[100];
}
