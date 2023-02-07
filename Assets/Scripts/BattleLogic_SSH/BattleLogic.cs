using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Unity.VisualScripting;
using System.Data;

public partial class BattleLogic : MonoBehaviourPunCallbacks
{
    public GameObject[] playerForwardUnits = null; // player ����
    public GameObject[] playerBackwardUnits = null; // player �Ŀ�
    public GameObject[] enemyForwardUnits = null; // enemy ����
    public GameObject[] enemyBackwardUnits = null; // enemy �Ŀ�

    public GameObject[] playerAttackArray = new GameObject[6]; // player attack unit
    public GameObject[] enemyAttackArray = new GameObject[6]; // enemy atack unit

    private bool isPlayerPreemptiveAlive = true; // player ���� ���� ����
    private bool isEnemyPreemptiveAlive = true; // enemy ���� ���� ����
    public bool isFirstAttack = true; // ���� �İ��� ���� bool ���� => true : Player ����
    private bool isResurrection = true; // ��ȯ Ư���� ���� bool ����

    private int playerTurnCount = 0; // Player Turn Count
    private int enemyTurnCount = 0; // Enemy Turn Count

    private int randomArrayNum = 0;
    private int isPlayerAliveCount = 0;
    private int isEnemyAliveCount = 0;


    // ���� master client�� gamemananger���� ������ ���� �迭�� ��ü ���� (�� ���� ���� �� ����)
    private int[] exArray = new int[100];

    private int playerCurRound = 0;
    private int enemyCurRound = 0;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        playerForwardUnits = new GameObject[3];
        playerBackwardUnits = new GameObject[3];
        enemyForwardUnits = new GameObject[3];
        enemyBackwardUnits = new GameObject[3];
    }

    #region PlayerList �ʱ�ȭ
    public void InitPlayerList()
    {
        // player ���ݸ���Ʈ �߰�
        for (int i = 0; i < playerForwardUnits.Length; i++) { playerAttackArray[i] = playerForwardUnits[i]; }
        for (int i = 0; i < playerBackwardUnits.Length; i++) { playerAttackArray[i + 3] = playerBackwardUnits[i]; }
    }
    #endregion

    #region EnemyList �ʱ�ȭ
    public void InitEnemyList()
    {
        // enemy ���ݸ���Ʈ �߰�
        for (int i = 0; i < enemyForwardUnits.Length; i++) { enemyAttackArray[i] = enemyForwardUnits[i]; }
        for (int i = 0; i < enemyBackwardUnits.Length; i++) { enemyAttackArray[i + 3] = enemyBackwardUnits[i]; }
    }
    #endregion

    #region ���� ���� 
    // �ΰ��� ���� ����
    public void AttackLogic()
    {
        // ���� ������ ���
        if (isFirstAttack) { PreemptiveAttack(); }

        // ������ ������ ���
        else if (!isFirstAttack) { SubordinatedAttack(); }
        else { Debug.Log("���� �İ��� �������� ����"); }
    }
    #endregion

    public void AliveUnit()
    {
        // player forward unit�� ���� ���
        for (int i = 0; i < playerForwardUnits.Length; i++)
        {
            if (playerForwardUnits[i] == null) { isPlayerAliveCount++; }
        }

        if (isPlayerAliveCount == playerForwardUnits.Length) { isPlayerPreemptiveAlive = false; }

        // enemy forward unit�� ���� ���
        for (int i = 0; i < enemyForwardUnits.Length; i++)
        {
            if (enemyForwardUnits[i] == null) { isEnemyAliveCount++; }
        }

        if (isEnemyAliveCount == enemyForwardUnits.Length) { isEnemyPreemptiveAlive = false; }

        isPlayerAliveCount = 0;
        isEnemyAliveCount = 0;
    }


    #region Player PreemptiveAttack
    // Player Attack
    public void PreemptiveAttack()
    {
        // ���� ��ġ���� Ȯ��
        AliveUnit();

        for (int i = 0; i < GameMGR.Instance.randomValue.Length; i++) exArray[i] = GameMGR.Instance.randomValue[i];
        Debug.Log("player PreemptiveAttack");

        while (true)
        {
            Debug.Log("Player�� ���� ����");
            // ���� ���� ������ �ִ� �迭 1���� ������ �� 0��°�� �ʱ�ȭ
            if (randomArrayNum == exArray.Length) { randomArrayNum = 0; }
            // [Player -> Enemy Attack] ���� ������ ����ִ� ���
            if (isEnemyPreemptiveAlive)
            {
                isPlayerAliveCount = 0;
                isEnemyAliveCount = 0;

                // �ǰ� ������ ���� ���ö����� randomArray ��ȸ
                while (enemyForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                    isEnemyAliveCount = 0;

                    for (int i = 0; i < enemyForwardUnits.Length; i++)
                    {
                        if (enemyForwardUnits[i] == null) { isEnemyAliveCount++; }
                        if (isEnemyAliveCount == enemyForwardUnits.Length) 
                        {
                            isEnemyAliveCount = 0;
                            isEnemyPreemptiveAlive = false; 
                        }
                    }
                    isEnemyAliveCount = 0;
                    if (isEnemyPreemptiveAlive == false) { break; }
                }

                if (playerAttackArray.Length <= playerTurnCount) { playerTurnCount = 0; }

                // ���� ������ �÷��̾ ���ö� ���� playerturnCount ����
                while (playerAttackArray[playerTurnCount] == null)
                {
                    playerTurnCount++;
                    isPlayerAliveCount = 0;

                    if (playerAttackArray.Length <= playerTurnCount) { playerTurnCount = 0; }

                    for (int i = 0; i < playerAttackArray.Length; i++)
                    {
                        if (playerAttackArray[i] == null) { isPlayerAliveCount++; }
                    }

                    if (isPlayerAliveCount == playerAttackArray.Length)
                    {
                        Debug.Log("���ݰ����� player unit�� ����");
                        break;
                    }
                }

                Debug.Log("Player Attack Unit name : " + playerAttackArray[playerTurnCount].name);
                Debug.Log("Enemy forward hit unit : " + enemyForwardUnits[exArray[randomArrayNum]].name);
                // �÷��̾� ������ �� ���� ���� ���� ����
                playerAttackArray[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ �迭���� ����
                for (int i = 0; i < enemyAttackArray.Length; i++)
                {
                    if (enemyAttackArray[i] == enemyForwardUnits[exArray[randomArrayNum]])
                    {
                        Debug.Log("Hit enemy unit (attackArray) : " + enemyAttackArray[i].name);
                        Debug.Log("Hit enemy unit (forwardUnit) : " + enemyForwardUnits[exArray[randomArrayNum]].name);

                        enemyAttackArray[i] = null;
                        enemyForwardUnits[exArray[randomArrayNum]] = null;

                        break;
                    }
                    else { Debug.Log("enemyAttackArray Ž����"); }
                }

                // ���� ���� ���� �Ǵ�
                for (int i = 0; i < enemyForwardUnits.Length; i++)
                {
                    if (enemyForwardUnits[i] == null) { isEnemyAliveCount++; }
                }

                if (enemyForwardUnits.Length == isEnemyAliveCount)
                {
                    isEnemyAliveCount = 0;
                    isEnemyPreemptiveAlive = false;
                }

                isEnemyAliveCount = 0;

                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // 1�� ���ῡ ���� �� ���� ���� 
                playerTurnCount++;
            }

            // ���� ������ ������ ���
            // �Ŀ��� ���� ������ ���·� ����
            else if (!isEnemyPreemptiveAlive)
            {
                isPlayerAliveCount = 0;
                isEnemyAliveCount = 0;

                while (enemyBackwardUnits[exArray[randomArrayNum]] == null) { randomArrayNum++; }

                if (playerAttackArray.Length <= playerTurnCount) { playerTurnCount = 0; }

                // ���� ������ �÷��̾ ���ö� ���� playerTurnCount ����
                while (playerAttackArray[playerTurnCount] == null)
                {
                    playerTurnCount++;
                    isPlayerAliveCount = 0;

                    if (playerAttackArray.Length <= playerTurnCount) { playerTurnCount = 0; }

                    for (int i = 0; i < playerAttackArray.Length; i++)
                    {
                        if (playerAttackArray[i] == null) { isPlayerAliveCount++; }
                    }

                    if (isPlayerAliveCount == playerAttackArray.Length)
                    {
                        isPlayerAliveCount = 0;
                        Debug.Log("���ݰ����� player unit�� ����");
                        break;
                    }
                }

                Debug.Log("playerTurnCount : " + playerTurnCount);
                Debug.Log("player attack unit : " + playerAttackArray[playerTurnCount].name);
                Debug.Log("enemy backward hit unit : " + enemyBackwardUnits[exArray[randomArrayNum]].name);

                // �÷��̾� ������ �� �Ŀ� ���� ���� ����
                playerAttackArray[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyBackwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ �迭���� ����
                for (int i = 0; i < enemyAttackArray.Length; i++)
                {
                    if (enemyAttackArray[i] == enemyBackwardUnits[exArray[randomArrayNum]])
                    {
                        Debug.Log("Hit enemy unit (attackArray) : " + enemyAttackArray[i].name);
                        Debug.Log("Hit enemy unit (forwardUnit) : " + enemyBackwardUnits[exArray[randomArrayNum]].name);

                        enemyAttackArray[i] = null;
                        enemyBackwardUnits[exArray[randomArrayNum]] = null;
                        break;
                    }
                    else { Debug.Log("enemyAttackArray Ž����"); }
                }

                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // 1�� ���ῡ ���� �� ���� ���� 
                playerTurnCount++;
            }

            // ���� ������ ���
            for (int i = 0; i < enemyAttackArray.Length; i++)
            {
                if (enemyAttackArray[i] == null) { isEnemyAliveCount++; }
            }
            if (isEnemyAliveCount == enemyAttackArray.Length)
            {
                Debug.Log("2. isEnemyAliveCount : " + isEnemyAliveCount);
                isEnemyAliveCount = 0;
                PlayerBattleWin();
                break;
            }

            isEnemyAliveCount = 0;

            // [Enemy -> Player Attack] �÷��̾��� ������ ����ִ� ���
            if (isPlayerPreemptiveAlive)
            {
                isPlayerAliveCount = 0;
                isEnemyAliveCount = 0;

                // �ǰ� ������ Player�� ���ö����� ���� �� �� ����
                while (playerForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                    isPlayerAliveCount = 0;

                    for (int i = 0; i < playerForwardUnits.Length; i++)
                    {
                        if (playerForwardUnits[i] == null) { isPlayerAliveCount++; }
                    }

                    if (isPlayerAliveCount == playerForwardUnits.Length)
                    {
                        isPlayerAliveCount = 0;
                        isPlayerPreemptiveAlive = false;
                    }

                    if (isPlayerPreemptiveAlive == false) { break; }
                }

                isPlayerAliveCount = 0;

                if (enemyAttackArray.Length <= enemyTurnCount) { enemyTurnCount = 0; }

                while (enemyAttackArray[enemyTurnCount] == null)
                {
                    enemyTurnCount++;
                    isEnemyAliveCount = 0;

                    if (enemyAttackArray.Length <= enemyTurnCount) { enemyTurnCount = 0; }

                    for (int i = 0; i < enemyAttackArray.Length; i++)
                    {
                        if (enemyAttackArray[i] == null) { isEnemyAliveCount++; }
                    }

                    if (isEnemyAliveCount == enemyAttackArray.Length)
                    {
                        isEnemyAliveCount = 0;
                        Debug.Log("���ݰ����� Enemy uint�� ����");
                        break;
                    }
                }

                Debug.Log("enemy attack unit : " + enemyAttackArray[enemyTurnCount].name);
                Debug.Log("player forward hit Unit : " + playerForwardUnits[exArray[randomArrayNum]].name);

                // �� ������ �÷��̾� ���� �� ������ �÷��̾� ����
                enemyAttackArray[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerForwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ ���� ����Ʈ���� ����
                for (int i = 0; i < playerAttackArray.Length; i++)
                {
                    if (playerAttackArray[i] == playerForwardUnits[exArray[randomArrayNum]])
                    {
                        Debug.Log("Hit player unit (attackArray) : " + playerAttackArray[i].name);
                        Debug.Log("Hit player unit (forwardUnit) : " + playerForwardUnits[exArray[randomArrayNum]].name);

                        playerAttackArray[i] = null;
                        playerForwardUnits[exArray[randomArrayNum]] = null;
                        break;
                    }
                    else { Debug.Log("playerForwardUnits Ž����"); }
                }

                // ���� ���� ���� �Ǵ�
                for (int i = 0; i < playerForwardUnits.Length; i++)
                {
                    if (playerForwardUnits[i] == null) { isPlayerAliveCount++; }
                }

                if (playerForwardUnits.Length == isPlayerAliveCount)
                {
                    isPlayerAliveCount = 0;
                    isPlayerPreemptiveAlive = false;
                }

                isPlayerAliveCount = 0;
                
                // ���ο� random num �ο�
                // ex) ���� �÷��̾��� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // 1�� ���ῡ ���� �� ���� ����
                enemyTurnCount++;
            }

            // �÷��̾��� ������ ������ ���
            // �Ŀ��� ���� ������ ���·� ����
            else if (!isPlayerPreemptiveAlive)
            {
                isPlayerAliveCount = 0;
                isEnemyAliveCount = 0;

                while (playerBackwardUnits[exArray[randomArrayNum]] == null) { randomArrayNum++; }

                if (enemyAttackArray.Length <= enemyTurnCount) { enemyTurnCount = 0; }

                while (enemyAttackArray[enemyTurnCount] == null)
                {
                    enemyTurnCount++;

                    if (enemyAttackArray.Length <= enemyTurnCount) { enemyTurnCount = 0; }
                }

                Debug.Log("enemy attack unit : " + enemyAttackArray[enemyTurnCount].name);
                Debug.Log("player backward hit unit : " + playerBackwardUnits[exArray[randomArrayNum]].name);

                // �� ������ �÷��̾� �Ŀ� ���� ���� ����
                enemyAttackArray[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerBackwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� �÷��̾� ������ �迭���� ����
                for (int i = 0; i < playerAttackArray.Length; i++)
                {
                    if (playerAttackArray[i] == playerBackwardUnits[exArray[randomArrayNum]])
                    {
                        Debug.Log("Hit player unit (attackArray) : " + playerAttackArray[i].name);
                        Debug.Log("Hit player unit (forwardUnit) : " + playerBackwardUnits[exArray[randomArrayNum]].name);

                        playerAttackArray[i] = null;
                        playerBackwardUnits[exArray[randomArrayNum]] = null;
                        break;
                    }
                    else { Debug.Log("playerAttackArray Ž����"); }
                }


                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                isPlayerAliveCount = 0;

                // 1�� ���ῡ ���� �� ���� ���� 
                enemyTurnCount++;
                // enemy ���� ���� �ʱ�ȭ
                enemyTurnCount = 0;
            }
            isPlayerAliveCount = 0;

            // �÷��̾� ������ ������ ���
            for (int i = 0; i < playerAttackArray.Length; i++)
            {
                if (playerAttackArray[i] == null) { isPlayerAliveCount++; }
                if (isPlayerAliveCount == playerAttackArray.Length)
                {
                    isPlayerAliveCount = 0;
                    PlayerBattleLose();
                    break;
                }
            }
        }
    }
    #endregion

    #region Enemy PreemptiveAttack
    // Enemy Attack
    public void SubordinatedAttack()
    {
        // ���� ��ġ���� Ȯ��
        AliveUnit();

        for (int i = 0; i < GameMGR.Instance.randomValue.Length; i++) exArray[i] = GameMGR.Instance.randomValue[i];

        while (true)
        {
            Debug.Log("Enemy�� ���� ����");

            // ���� ���� ������ �ִ� �迭 1���� ������ �� 0��°�� �ʱ�ȭ
            if (randomArrayNum == exArray.Length) { randomArrayNum = 0; }

            // [Enemy -> Player Attack] �÷��̾��� ������ ����ִ� ���
            if (isPlayerPreemptiveAlive)
            {
                isPlayerAliveCount = 0;
                isEnemyAliveCount = 0;

                // �ǰ� ������ Player�� ���ö����� ���� �� �� ����
                while (playerForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                    isPlayerAliveCount = 0;

                    for (int i = 0; i < playerForwardUnits.Length; i++)
                    {
                        if (playerForwardUnits[i] == null) { isPlayerAliveCount++; }
                    }

                    if (isPlayerAliveCount == playerForwardUnits.Length)
                    {
                        isPlayerAliveCount = 0;
                        isPlayerPreemptiveAlive = false;
                    }

                    if (isPlayerPreemptiveAlive == false) { break; }
                }

                isPlayerAliveCount = 0;

                if (enemyAttackArray.Length <= enemyTurnCount) { enemyTurnCount = 0; }

                while (enemyAttackArray[enemyTurnCount] == null)
                {
                    enemyTurnCount++;
                    isEnemyAliveCount = 0;

                    if (enemyAttackArray.Length <= enemyTurnCount) { enemyTurnCount = 0; }

                    for (int i = 0; i < enemyAttackArray.Length; i++)
                    {
                        if (enemyAttackArray[i] == null) { isEnemyAliveCount++; }
                    }

                    if (isEnemyAliveCount == enemyAttackArray.Length)
                    {
                        isEnemyAliveCount = 0;
                        Debug.Log("���ݰ����� Enemy uint�� ����");
                        break;
                    }
                }

                Debug.Log("enemy attack unit : " + enemyAttackArray[enemyTurnCount].name);
                Debug.Log("player forward hit Unit : " + playerForwardUnits[exArray[randomArrayNum]].name);

                // �� ������ �÷��̾� ���� �� ������ �÷��̾� ����
                enemyAttackArray[enemyTurnCount].GetComponentInChildren<AttackLogic>().UnitAttack(playerForwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ ���� ����Ʈ���� ����
                for (int i = 0; i < playerAttackArray.Length; i++)
                {
                    if (playerAttackArray[i] == playerForwardUnits[exArray[randomArrayNum]])
                    {
                        Debug.Log("Hit player unit (attackArray) : " + playerAttackArray[i].name);
                        Debug.Log("Hit player unit (forwardUnit) : " + playerForwardUnits[exArray[randomArrayNum]].name);

                        playerAttackArray[i] = null;
                        playerForwardUnits[exArray[randomArrayNum]] = null;
                        break;
                    }
                    else { Debug.Log("playerForwardUnits Ž����"); }
                }

                // ���� ���� ���� �Ǵ�
                for (int i = 0; i < playerForwardUnits.Length; i++)
                {
                    if (playerForwardUnits[i] == null) { isPlayerAliveCount++; }
                }

                if (playerForwardUnits.Length == isPlayerAliveCount)
                {
                    isPlayerAliveCount = 0;
                    isPlayerPreemptiveAlive = false;
                }

                isPlayerAliveCount = 0;

                // ���ο� random num �ο�
                // ex) ���� �÷��̾��� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // 1�� ���ῡ ���� �� ���� ����
                enemyTurnCount++;
            }

            // �÷��̾��� ������ ������ ���
            // �Ŀ��� ���� ������ ���·� ����
            else if (!isPlayerPreemptiveAlive)
            {
                isPlayerAliveCount = 0;
                isEnemyAliveCount = 0;

                while (playerBackwardUnits[exArray[randomArrayNum]] == null) { randomArrayNum++; }

                if (enemyAttackArray.Length <= enemyTurnCount) { enemyTurnCount = 0; }

                while (enemyAttackArray[enemyTurnCount] == null)
                {
                    enemyTurnCount++;

                    if (enemyAttackArray.Length <= enemyTurnCount) { enemyTurnCount = 0; }
                }

                Debug.Log("enemy attack unit : " + enemyAttackArray[enemyTurnCount].name);
                Debug.Log("player backward hit unit : " + playerBackwardUnits[exArray[randomArrayNum]].name);

                // �� ������ �÷��̾� �Ŀ� ���� ���� ����
                enemyAttackArray[enemyTurnCount].GetComponentInChildren<AttackLogic>().UnitAttack(playerBackwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� �÷��̾� ������ �迭���� ����
                for (int i = 0; i < playerAttackArray.Length; i++)
                {
                    if (playerAttackArray[i] == playerBackwardUnits[exArray[randomArrayNum]])
                    {
                        Debug.Log("Hit player unit (attackArray) : " + playerAttackArray[i].name);
                        Debug.Log("Hit player unit (forwardUnit) : " + playerBackwardUnits[exArray[randomArrayNum]].name);

                        playerAttackArray[i] = null;
                        playerBackwardUnits[exArray[randomArrayNum]] = null;
                        break;
                    }
                    else { Debug.Log("playerAttackArray Ž����"); }
                }


                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                isPlayerAliveCount = 0;

                // 1�� ���ῡ ���� �� ���� ���� 
                enemyTurnCount++;
                // enemy ���� ���� �ʱ�ȭ
                enemyTurnCount = 0;
            }
            isPlayerAliveCount = 0;

            // �÷��̾� ������ ������ ���
            for (int i = 0; i < playerAttackArray.Length; i++)
            {
                if (playerAttackArray[i] == null) { isPlayerAliveCount++; }
                if (isPlayerAliveCount == playerAttackArray.Length)
                {
                    isPlayerAliveCount = 0;
                    PlayerBattleLose();
                    break;
                }
            }

            isPlayerAliveCount = 0;

            // [Player -> Enemy Attack] ���� ������ ����ִ� ���
            if (isEnemyPreemptiveAlive)
            {
                isPlayerAliveCount = 0;
                isEnemyAliveCount = 0;

                // �ǰ� ������ ���� ���ö����� randomArray ��ȸ
                while (enemyForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                    isEnemyAliveCount = 0;

                    for (int i = 0; i < enemyForwardUnits.Length; i++)
                    {
                        if (enemyForwardUnits[i] == null) { isEnemyAliveCount++; }
                        if (isEnemyAliveCount == enemyForwardUnits.Length)
                        {
                            isEnemyAliveCount = 0;
                            isEnemyPreemptiveAlive = false;
                        }
                    }
                    isEnemyAliveCount = 0;
                    if (isEnemyPreemptiveAlive == false) { break; }
                }

                if (playerAttackArray.Length <= playerTurnCount) { playerTurnCount = 0; }

                // ���� ������ �÷��̾ ���ö� ���� playerturnCount ����
                while (playerAttackArray[playerTurnCount] == null)
                {
                    playerTurnCount++;
                    isPlayerAliveCount = 0;

                    if (playerAttackArray.Length <= playerTurnCount) { playerTurnCount = 0; }

                    for (int i = 0; i < playerAttackArray.Length; i++)
                    {
                        if (playerAttackArray[i] == null) { isPlayerAliveCount++; }
                    }

                    if (isPlayerAliveCount == playerAttackArray.Length)
                    {
                        Debug.Log("���ݰ����� player unit�� ����");
                        break;
                    }
                }

                Debug.Log("Player Attack Unit name : " + playerAttackArray[playerTurnCount].name);
                Debug.Log("Enemy forward hit unit : " + enemyForwardUnits[exArray[randomArrayNum]].name);
                // �÷��̾� ������ �� ���� ���� ���� ����
                playerAttackArray[playerTurnCount].GetComponentInChildren<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ �迭���� ����
                for (int i = 0; i < enemyAttackArray.Length; i++)
                {
                    if (enemyAttackArray[i] == enemyForwardUnits[exArray[randomArrayNum]])
                    {
                        Debug.Log("Hit enemy unit (attackArray) : " + enemyAttackArray[i].name);
                        Debug.Log("Hit enemy unit (forwardUnit) : " + enemyForwardUnits[exArray[randomArrayNum]].name);

                        enemyAttackArray[i] = null;
                        enemyForwardUnits[exArray[randomArrayNum]] = null;

                        break;
                    }
                    else { Debug.Log("enemyAttackArray Ž����"); }
                }

                // ���� ���� ���� �Ǵ�
                for (int i = 0; i < enemyForwardUnits.Length; i++)
                {
                    if (enemyForwardUnits[i] == null) { isEnemyAliveCount++; }
                }

                if (enemyForwardUnits.Length == isEnemyAliveCount)
                {
                    isEnemyAliveCount = 0;
                    isEnemyPreemptiveAlive = false;
                }

                isEnemyAliveCount = 0;

                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // 1�� ���ῡ ���� �� ���� ���� 
                playerTurnCount++;
            }

            // ���� ������ ������ ���
            // �Ŀ��� ���� ������ ���·� ����
            else if (!isEnemyPreemptiveAlive)
            {
                isPlayerAliveCount = 0;
                isEnemyAliveCount = 0;

                while (enemyBackwardUnits[exArray[randomArrayNum]] == null) { randomArrayNum++; }

                if (playerAttackArray.Length <= playerTurnCount) { playerTurnCount = 0; }

                // ���� ������ �÷��̾ ���ö� ���� playerTurnCount ����
                while (playerAttackArray[playerTurnCount] == null)
                {
                    playerTurnCount++;
                    isPlayerAliveCount = 0;

                    if (playerAttackArray.Length <= playerTurnCount) { playerTurnCount = 0; }

                    for (int i = 0; i < playerAttackArray.Length; i++)
                    {
                        if (playerAttackArray[i] == null) { isPlayerAliveCount++; }
                    }

                    if (isPlayerAliveCount == playerAttackArray.Length)
                    {
                        isPlayerAliveCount = 0;
                        Debug.Log("���ݰ����� player unit�� ����");
                        break;
                    }
                }

                Debug.Log("playerTurnCount : " + playerTurnCount);
                Debug.Log("player attack unit : " + playerAttackArray[playerTurnCount].name);
                Debug.Log("enemy backward hit unit : " + enemyBackwardUnits[exArray[randomArrayNum]].name);

                // �÷��̾� ������ �� �Ŀ� ���� ���� ����
                playerAttackArray[playerTurnCount].GetComponentInChildren<AttackLogic>().UnitAttack(enemyBackwardUnits[exArray[randomArrayNum]]);

                // �ǰ� ���� ������ �迭���� ����
                for (int i = 0; i < enemyAttackArray.Length; i++)
                {
                    if (enemyAttackArray[i] == enemyBackwardUnits[exArray[randomArrayNum]])
                    {
                        Debug.Log("Hit enemy unit (attackArray) : " + enemyAttackArray[i].name);
                        Debug.Log("Hit enemy unit (forwardUnit) : " + enemyBackwardUnits[exArray[randomArrayNum]].name);

                        enemyAttackArray[i] = null;
                        enemyBackwardUnits[exArray[randomArrayNum]] = null;
                        break;
                    }
                    else { Debug.Log("enemyAttackArray Ž����"); }
                }

                // ���ο� random num �ο�
                // ex) �÷��̾ ���� 2��°�� �������� �� ���� �÷��̾��� 2��°�� �����ϱ� ������ �ٸ� random num �ο�
                randomArrayNum++;

                // 1�� ���ῡ ���� �� ���� ���� 
                playerTurnCount++;
            }

            // ���� ������ ���
            for (int i = 0; i < enemyAttackArray.Length; i++)
            {
                if (enemyAttackArray[i] == null) { isEnemyAliveCount++; }
            }
            if (isEnemyAliveCount == enemyAttackArray.Length)
            {
                Debug.Log("2. isEnemyAliveCount : " + isEnemyAliveCount);
                isEnemyAliveCount = 0;
                PlayerBattleWin();
                break;
            }
            isEnemyAliveCount = 0;
        }
    }
    #endregion

    // �¸� ��
    private void PlayerBattleWin()
    {
        Debug.Log("Player Win");
        // �¸� ���� �߰�
    }

    // �й� ��
    private void PlayerBattleLose()
    {
        Debug.Log("Player Lose");
        // �й� ���� �߰�
    }
}
