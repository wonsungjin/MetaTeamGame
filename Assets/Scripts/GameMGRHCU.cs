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
    public Dictionary<int, List<Card>> playerList = new Dictionary<int, List<Card>>();

    // 델리게이트 이벤트들의 총집합이라고 보면 되는 것입니다.
    public delegate void _callback_SkillTiming();
    public event _callback_SkillTiming callbackEvent_TurnStart;
    public event _callback_SkillTiming callbackEvent_TurnEnd;
    public event _callback_SkillTiming callbackEvent_Buy;
    public event _callback_SkillTiming callbackEvent_Sell;
    public event _callback_SkillTiming callbackEvent_Reroll;
    public event _callback_SkillTiming callbackEvent_BattleStart;
    public event _callback_SkillTiming callbackEvent_BeforeAttack;
    public event _callback_SkillTiming callbackEvent_AfterAttack;
    public event _callback_SkillTiming callbackEvent_Kill;
    public event _callback_SkillTiming callbackEvent_Hit;
    public event _callback_SkillTiming callbackEvent_HitEnemy;
    public event _callback_SkillTiming callbackEvent_Death;
    public event _callback_SkillTiming callbackEvent_Summon;



}
