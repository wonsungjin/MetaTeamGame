using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Node[] monsterTrans;
    [Header("몬스터 프리팹")]
    [SerializeField] GameObject[] monsters = null;

    [Header("상점 위치")]
    public int createdPlace = 3;
    public int stage = 1;
    public bool isFreeze = false;
    public int randomTrans = 0;

    int randomNum;
    bool isFirstStart = false;

    private void Start()
    {
        // 처음에 카드 생성
        if (isFirstStart == false)
        {
            for (int i = 0; i < createdPlace; i++)
            {
                randomNum = Random.Range(0, 3);
                GameObject mon = Instantiate(monsters[randomNum], monsterTrans[randomTrans].transform.position, Quaternion.identity);

                randomTrans++;

                isFirstStart = true;
            }
            randomTrans = 0;
        }
    }

    private void Update()
    {
        // TEST
        if (Input.GetKeyDown(KeyCode.Q))
        {
            stage++;
            UIManager.Instance.goldCount = 10;
            ChooseRandomCard();
        }
       
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject[] monster = GameObject.FindGameObjectsWithTag("Monster");
            for (int i = 0; i < monster.Length; i++)
            {
                Destroy(monster[i]); 
            }
        }
    }
    
    // 리셋 
    public void OnClick_Reset_Monster()
    {
        if(UIManager.Instance.goldCount > 0)
        {
            GameObject[] monster = GameObject.FindGameObjectsWithTag("Monster");
            UIManager.Instance.goldCount--;

            for (int i = 0; i < monster.Length; i++)
            {
                Destroy(monster[i]);
            }
            ChooseRandomCard();
        }
        else
        {
            StartCoroutine(UIManager.Instance.COR_NotGold());
        }
    }

    // 랜덤으로 카드 선택 함수
    // 스테이지 마다 퍼센트율을 정해 카드들 소환
    void ChooseRandomCard()
    {
        if (stage > 5)
        {
            for (int i = 0; i < 6; i++)
            {
                // 랜덤 퍼센트율
                int randomCard;
                if (monsterTrans[i].isNotSpawn == false)
                {
                    randomCard = Random.Range(0, 100);
                    // 1티어
                    if (randomCard < 10)
                    {
                        SummonMonster(0, 8);
                    }
                    // 2티어
                    else if (randomCard > 10 && randomCard < 30)
                    {
                        SummonMonster(8, 16);
                    }
                    // 3티어
                    else if (randomCard > 30 && randomCard < 55)
                    {
                        SummonMonster(16, 24);
                    }
                    else if (randomCard > 55 && randomCard < 75)
                    {
                        SummonMonster(24, 32);
                    }
                    else if (randomCard > 75 && randomCard < 90)
                    {
                        SummonMonster(32 , 40);
                    }
                    else
                    {
                        SummonMonster(40, 48);
                    }
                }
                else
                {
                    randomTrans++;
                }
            }
            randomTrans = 0;
        }
      

        if (stage == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                if (monsterTrans[i].isNotSpawn == false)
                {

                    SummonMonster(0, 8);
                }
                else
                {
                    randomTrans++;
                }
            }
            randomTrans = 0;
            createdPlace++;
        }

        if (stage == 2)
        {
            for (int i = 0; i < 3; i++)
            {
                // 랜덤 퍼센트율
                int randomCard;
                randomCard = Random.Range(0, 10);
                if (monsterTrans[i].isNotSpawn == false)
                {
                    if (randomCard < 7)
                    {
                        SummonMonster(0, 8);
                    }
                    else
                    {
                        SummonMonster(8, 16);
                    }
                }
                else
                {
                    randomTrans++;
                }
            }
            randomTrans = 0;
            createdPlace++;
        }
        if (stage == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                // 랜덤 퍼센트율
                int randomCard;
                randomCard = Random.Range(0, 10);
                if (monsterTrans[i].isNotSpawn == false)
                {
                    // 1티어
                    if (randomCard < 6)
                    {
                        SummonMonster(0, 8);
                    }
                    // 2티어
                    else if (randomCard > 6 && randomCard < 9)
                    {
                        SummonMonster(8, 16);
                    }
                    // 3티어
                    else
                    {
                        SummonMonster(16, 24);
                    }
                }
                else
                {
                    randomTrans++;
                }
            }
            randomTrans = 0;
            createdPlace++;
        }
        if (stage == 4)
        {
            for (int i = 0; i < 4; i++)
            {
                // 랜덤 퍼센트율
                int randomCard;
                randomCard = Random.Range(0, 100);
                if (monsterTrans[i].isNotSpawn == false)
                {
                    // 1티어
                    if (randomCard < 40)
                    {
                        SummonMonster(0, 8);
                    }
                    // 2티어
                    else if (randomCard > 40 && randomCard < 65)
                    {
                        SummonMonster(8, 16);
                    }
                    // 3티어
                    else if (randomCard > 65 && randomCard < 85)
                    {
                        SummonMonster(16, 24);
                    }
                    else
                    {
                        SummonMonster(24, 32);
                    }
                }
                else
                {
                    randomTrans++;
                }
            }
            randomTrans = 0;
            createdPlace++;
        }
        if (stage == 5)
        {
            for (int i = 0; i < 5; i++)
            {
                // 랜덤 퍼센트율
                int randomCard;
                randomCard = Random.Range(0, 100);
                if (monsterTrans[i].isNotSpawn == false)
                {
                    // 1티어
                    if (randomCard < 25)
                    {
                        SummonMonster(0, 8);
                    }
                    // 2티어
                    else if (randomCard > 25 && randomCard < 45)
                    {
                        SummonMonster(8, 16);
                    }
                    // 3티어
                    else if (randomCard > 45 && randomCard < 70)
                    {
                        SummonMonster(16, 24);
                    }
                    else if (randomCard > 70 && randomCard < 90)
                    {
                        SummonMonster(24, 32);
                    }
                    else
                    {
                        SummonMonster(32, 40);
                    }
                }
                else
                {
                    randomTrans++;
                }
            }
            randomTrans = 0;
        }
        
    }

    void SummonMonster(int a, int b)
    {
        randomNum = Random.Range(a, b);

        GameObject mon = Instantiate(monsters[randomNum], monsterTrans[randomTrans].transform.position, Quaternion.identity);

        randomTrans++;
    }
}
