using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using Hashtable = ExitGames.Client.Photon.Hashtable; // 이게 구버전 한정인지 필수인지는 나도 모른다는 것이 학계의 점심


public class TurnSystem : MonoBehaviourPunCallbacks
{
    private bool isGameOver;
    private int []  readyCount = new int[2];

    [SerializeField] private GameObject Me;
    [SerializeField] private GameObject enemyDeck;

    [SerializeField] private GameObject[] playerCardPos;
    [SerializeField] private GameObject[] otherCardPos;

    public Player[] players = null;

    [SerializeField] public InputField inputField; // 닉네임 입력칸

    public int[] setRandom = new int[100]; // 공격할 대상을 랜덤으로 지정한다. 

    public Player[] savePlayers = null;

    public int matchingDone;


    public int userID = 0;

    // 플레이어 라이프 관련
    public int[] life = null;
    public int myLife = 10;
    [SerializeField] public int startLife = 10;

    // 전투 승패 관련
    public bool isWin = false;
    public bool isLose = false;

    // 전투 선공 카운트
    public int firstCount = 0;
    public int afterCount = 0;

    // 라운드 카운트
    public int curRound = 0;

    // 덱 수 카운트 ( 각 플레이어들의 덱 수를 가져온다. 배열의 인덱스 = 해당 플레이어 인덱스 )
    public int[] deckCount = new int[8];

    // 선공 선정 관련
    public bool[] isFirst = new bool[8];
    public int[] firstPoint = new int[8]; // 각 플레이어들의 선공 점수. 해당 점수가 높은 플레이어가 선공권을 가진다.

    // 게임 도중 나가버린 플레이어 낙인
    public List<int> leavePlayerList;


    // 마스터가 가지고 있는 이전 매칭 상대 기록 배열
    [SerializeField] public int[] prevOpponent = new int[8];

    #region 포톤 콜백 함수들

    public override void OnConnectedToMaster()
    {
        photonView.RPC("OnConnectMaster", RpcTarget.All, PhotonNetwork.NickName);
        PhotonNetwork.JoinLobby();
    }

    [PunRPC]
    public void OnConnectMaster(string name)
    {
        Debug.Log(name);
    }

    private void OnFailedToConnectToMasterServer()
    {
        photonView.RPC("FailedConnectMaster", RpcTarget.All, PhotonNetwork.NickName);
    }
    //Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 플레이어가 나갈 때 나간 플레이어의 고유값을 상대 리스트에서도 제거해야한다. 나간 플레이어의 커스텀프로퍼티를 찾으려고할 때 NULL
        //matchingList.Remove((int)otherPlayer.CustomProperties["Number"]);
        //exitPlayerList.Add((int)otherPlayer.CustomProperties["Nunber"]);
        //if (PhotonNetwork.PlayerList.Length == 2) 
        
            cloneOpponent = -1;
            cloneOpponentsOpponent = -1;
        Debug.Log(otherPlayer + "가 나갔다");
        if (otherPlayer.IsInactive) Debug.Log(" IsInactive");
        else Debug.Log("NotInactive");

        Debug.Log("나의 커스텀프로퍼티 번호" + (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);

        //방에서 나간 플레이어는 두번 다시 이 방에 발을 들이지 못할 것이다. 추후 조건 추가(게임 시작 이후에)

        //int a = (int)otherPlayer.CustomProperties["Number"]; // 리스트상에서 제거해줄 값.

        /*for (int i = 0; i < userName.Length; i++)
        {
            Debug.Log("유저번호" + userName[i]);
            if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"] == userName[i])
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Number", $"{userName[i]}" } });
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Life", $"{userLife[i]}" } });
                Debug.Log($"나는 {PhotonNetwork.LocalPlayer.NickName}, 나의 번호는 {PhotonNetwork.LocalPlayer.CustomProperties["Number"]}, 나의 체력은 {PhotonNetwork.LocalPlayer.CustomProperties["Life"]}");
            }
        }*/
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
    }
    #endregion

    //List<Player> matchMan = null;
    Player[] matchMan = new Player[8]; // 최대 8인이고 그 이상을 넘을 수는 없으니 일단 이 값으로 지정.
    int[] matchNum;
    int[] prevMatchNum = new int[8];
    int[] userName;
    int[] userLife;

    [PunRPC]
    public void StartSetting() // 게임 시작시 '최초' 한번만 실행되는 함수
    {
        userName = new int[PhotonNetwork.PlayerList.Length];
        userLife = new int[PhotonNetwork.PlayerList.Length];
        // 플레이어 고유번호 및 라이프 지정. 게임 시작하는 시점부터 각 플레이어리스트 배열의 인덱스에 고유번호 지정. 들락날락으로 인한 엑터넘버 불규칙 방지
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            PhotonNetwork.PlayerList[i].CustomProperties["Number"] = i;
            PhotonNetwork.PlayerList[i].CustomProperties["Life"] = startLife; // 서버 전체에서 동기화해줄 커스텀프로퍼티
            userName[i] = i;
            userLife[i] = startLife;
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log($"포톤 플레이어 {i} 번째 번호는 {PhotonNetwork.PlayerList[i].CustomProperties["Number"]}");
            Debug.Log($"포톤 플레이어 {i} 번째의 체력은 {PhotonNetwork.PlayerList[i].CustomProperties["Life"]}");
        }

        prevOpponent = new int[8];
    }

    [SerializeField] List<int> matchingList = new List<int>();
    [SerializeField] List<int> matchingListReal = new List<int>();
    [SerializeField] int cloneOpponent = -1;
    [SerializeField] int cloneOpponentsOpponent = -1;
    int c = 0; // 루프 체크 디버그용 임시 변수

    //=========================================================================================================================
    // 선공을 선정하는 함수
    public void SetFirstAttack(int me, int you)
    {
        int myDeckCount = GameMGR.Instance.batch.GetBatch(me).Count;
        int youDeckCount = GameMGR.Instance.batch.GetBatch(you).Count;
        // 대진이 정해지면 각 상대의 덱 수를 우선 비교 후
        if (myDeckCount > youDeckCount)
        {
            isFirst[me] = true;
            isFirst[you] = false;
        }

        // 서로 덱 수가 같다면
        else
        {
            int randomPoint = UnityEngine.Random.Range(0, 10);
            int randomPoint2 = UnityEngine.Random.Range(0, 10);
            firstPoint[me] += randomPoint;
            firstPoint[you] += randomPoint2;
        }
    }
    //=========================================================================================================================

    [PunRPC]
    public void MatchingSetting() // 마스터가 각 라운드마다 실행하는 대진 설정
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        //공격 랜덤값 지정 - 매 스테이지마다 랜덤 값을 받아온다
        for (int i = 0; i < setRandom.Length; i++)
        {
            setRandom[i] = UnityEngine.Random.Range(0, 3);
        }

        int n = -1; // 플레이어리스트 배열의 랜덤 인덱스 값

        matchingList.Clear(); //  플레이어 리스트 한번 초기화 싹 해준다.
        matchingListReal.Clear(); // 실제 플레이어 리스트도 초기화한다.

        // 중복매칭 방지 스크립트
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (cloneOpponent != -1)
            {
                matchingList.Add(cloneOpponent);
                for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
                {
                    if ((int)PhotonNetwork.PlayerList[j].CustomProperties["Number"] == cloneOpponent)
                    {
                        cloneOpponentsOpponent = (int)PhotonNetwork.PlayerList[j].CustomProperties["Opponent"];
                    }
                }
                cloneOpponent = -1;
                continue;
            }
            else
            {
                n = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length); // 랜덤 인덱스 값 추출

                if (matchingList.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]) || matchingListReal.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"])) { i--; continue; } // 중복 제외 처리
            }
            if (curRound != 0 && PhotonNetwork.PlayerList.Length > 2)
            {
                if (matchingList.Count != 0)
                {
                    if (matchingList.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Opponent"]) || cloneOpponentsOpponent == (int)PhotonNetwork.PlayerList[n].CustomProperties["Number"])
                    {
                        Debug.Log($"{(int)PhotonNetwork.PlayerList[n].CustomProperties["Opponent"]}가 들어있음");

                        // 조건식 6,7인일 경우
                        if ((PhotonNetwork.PlayerList.Length == 6 && matchingListReal.Count == 4) || (PhotonNetwork.PlayerList.Length == 7 && matchingListReal.Count == 4))  // 6명일 떄 랜덤 값 4개가 매칭 되었을 때
                        {
                            Debug.Log("6인 또는 7인이라 여기로 들어온다");
                            if (matchingListReal.Contains(prevOpponent[matchingListReal[0]]) && matchingListReal.Contains(prevOpponent[matchingListReal[1]]))
                            {
                                // 6명 들어왔는데 앞 4명이서 서로 교차로 만났다면 3,4번 매칭 제거
                                matchingListReal.RemoveRange(2, 2);
                            }
                        }

                        else if (PhotonNetwork.PlayerList.Length == 8 && matchingListReal.Count == 6)
                        {
                            if (matchingListReal.Contains(prevOpponent[matchingListReal[0]]) && matchingListReal.Contains(prevOpponent[matchingListReal[1]]) && matchingListReal.Contains(prevOpponent[matchingListReal[2]]) && matchingListReal.Contains(prevOpponent[matchingListReal[3]]) && matchingListReal.Contains(prevOpponent[matchingListReal[4]]) && matchingListReal.Contains(prevOpponent[matchingListReal[5]]))
                            {
                                matchingListReal.Clear(); // 6명 들어왔는데 6명 다 교차로 만났다면 리셋
                                matchingList.Clear();
                            }
                        }

                        c++;
                        if (c < 1000)
                        {
                            i--;
                            continue;
                        }
                        else
                        {
                            Debug.Log("무한반복함");
                            c = 0;
                        }
                    }
                    else
                    {
                        matchingList.Add((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);

                        Debug.Log(matchingList.Count);
                        matchingListReal.Add(matchingList[0]);
                        matchingListReal.Add(matchingList[1]);
                        matchingList.Clear();

                        if (matchingListReal.Count >= PhotonNetwork.PlayerList.Length)
                        {

                            matchNum = new int[matchingListReal.Count];
                            Debug.Log(matchNum.Length);
                            for (int j = 0; j < matchNum.Length; j++)
                            {
                                matchNum[j] = matchingListReal[j];
                            }

                            photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum, false); // Null이 뜨는 이유?}
                            curRound++;
                            return;
                        }
                        else
                        {
                            i = -1;
                            continue;
                        }
                    }
                }
                else
                {
                    Debug.Log("디버그 " + (int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
                    matchingList.Add((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
                    if (matchingListReal.Count >= PhotonNetwork.PlayerList.Length - 1 && PhotonNetwork.PlayerList.Length % 2 != 0) // 홀수 + 한명빼고 다 짝찾으면
                    {
                        int a = UnityEngine.Random.Range(0, matchingListReal.Count);

                        while ((int)PhotonNetwork.PlayerList[n].CustomProperties["Opponent"] == matchingListReal[a])
                        {
                            a = UnityEngine.Random.Range(0, matchingListReal.Count);
                        }
                        matchingList.Add(matchingListReal[a]); // 클론 생성 코드 by SJ @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                        cloneOpponent = matchingList[0];
                        matchingListReal.Add(matchingList[0]);
                        matchingListReal.Add(matchingList[1]);
                        matchingList.Clear();
                        matchNum = new int[matchingListReal.Count];
                        for (int j = 0; j < matchNum.Length; j++)
                        {
                            matchNum[j] = matchingListReal[j];
                        }
                        Debug.Log("matchList Cur Count :" + matchingList.Count);

                        photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum, true); // Null이 뜨는 이유?}
                        curRound++;
                        return;
                    }

                }
            }
            else // 첫 라운드에만 들어올 수 있는 부분
            {
                matchingList.Add((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
            }
        }

        if (PhotonNetwork.PlayerList.Length % 2 == 0)
        {
            matchingListInput(false);
        }
        else
        {
            for (int i = 0; i < matchingList.Count; i++)
            {
                matchingListReal.Add(matchingList[i]); // Debug 할 것
            }
            matchingListReal.RemoveAt(matchingList.Count - 1);
            cloneOpponent = matchingList[matchingList.Count - 1];
            matchingList.Add(matchingListReal[UnityEngine.Random.Range(0, matchingListReal.Count)]);

            matchingListInput(true);
        }
        curRound++;
    }
    //=========================================================================================================================

    public void matchingListInput(bool isClone)
    {
        matchNum = new int[matchingList.Count];
        for (int i = 0; i < matchNum.Length; i++)
        {

            matchNum[i] = matchingList[i];
        }

        Debug.Log("prevOpponent 길이 : " + prevOpponent.Length);

        photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum, isClone);
    }


    [PunRPC] // 각 플레이어들의 매칭이 정상적으로 되었는지 확인하기 위한 RPC 함수
    public void ShowDebug(string debugMatching)
    {
        Debug.Log(debugMatching);
    }

    [PunRPC] // 마스터 클라이언트가 지정한 대진표를 각 로컬들이 받아와서 대진 상 자신과 자신의 상대를 찾는 함수
    public void Matching(int[] random, int[] num, bool clone)
    {
        if (PhotonNetwork.IsMasterClient)   // 마스터 클라이언트는 대진 정보를 기록
        {
            Debug.Log("prevOpponent 길이 : " + prevOpponent.Length);
            for (int i = 0; i < num.Length; i++)
            {
                if (clone && i == num.Length - 1) break; // 만일 클론이 있다면 배열 마지막 값은 클론이므로 대진 정보를 기록하지 않는다.
                if (i % 2 == 0)
                {
                    prevOpponent[num[i]] = num[i + 1];
                    Debug.Log($"{prevOpponent[num[i]]} 의 상대는 {num[i + 1]}");
                }
                else
                {
                    prevOpponent[num[i]] = num[i - 1];
                    Debug.Log($"{prevOpponent[num[i]]} 의 상대는 {num[i - 1]}");
                }
            }
        }


        Debug.Log((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        Debug.Log(num.Length);
        Debug.Log(num[0]);
        Debug.Log(num[1]);
        if (clone) // 클론 있을때
        {
            Debug.Log("클론 있는 홀수");
            for (int i = 0; i < num.Length - 1; i++)
            {
                if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"] == num[i]) // 내 번호랑 일치하는지와 배열 번호가 마지막(클론) 이 아닌 경우에만
                {
                    if (i == 0 || i % 2 == 0)
                    {
                        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i + 1] } });
                        Debug.Log("나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i + 1]);
                        string matchingDebug = "나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i + 1];
                        // 내가 선공

                        GameMGR.Instance.matching[0] = num[i];
                        GameMGR.Instance.matching[1] = num[i + 1];
                        GameMGR.Instance.randomValue = random;
                        //SceneManager.LoadScene(1);

                        photonView.RPC("ShowDebug", RpcTarget.MasterClient, matchingDebug);

                    }
                    else if (i % 2 != 0 && i < num.Length)
                    {
                        // 내 상대는 = matchMan[i-1]
                        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i - 1] } });
                        Debug.Log("나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i - 1]);
                        string matchingDebug = "나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i - 1];
                        // 내가 후공

                        GameMGR.Instance.matching[0] = num[i];
                        GameMGR.Instance.matching[1] = num[i - 1];
                        GameMGR.Instance.randomValue = random;
                        //SceneManager.LoadScene(1);

                        photonView.RPC("ShowDebug", RpcTarget.MasterClient, matchingDebug);
                    }
                }
            }
        }

        else if (!clone) // 클론 없을때 01 23 45
        {
            Debug.Log("클론없는 짝수");
            for (int i = 0; i < num.Length; i++)
            {
                if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"] == num[i])
                {
                    if (i == 0 || i % 2 == 0)
                    {
                        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i + 1] } });
                        Debug.Log("나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i + 1]);
                        string matchingDebug = "나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i + 1];
                        // 내가 선공

                        // 내가 짝수 상대 +1 홀수

                        GameMGR.Instance.matching[0] = num[i];
                        GameMGR.Instance.matching[1] = num[i + 1];
                        GameMGR.Instance.randomValue = random;
                        //SceneManager.LoadScene(1);

                        photonView.RPC("ShowDebug", RpcTarget.MasterClient, matchingDebug);

                    }
                    else if (i % 2 != 0 && i < num.Length)
                    {
                        // 내 상대는 = matchMan[i-1]
                        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i - 1] } });
                        Debug.Log("나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i - 1]);
                        string matchingDebug = "나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i - 1];
                        // 내가 후공

                        // 내가 홀수 상대 -1짝수
                        GameMGR.Instance.matching[0] = num[i];
                        GameMGR.Instance.matching[1] = num[i - 1];
                        GameMGR.Instance.randomValue = random;
                        //SceneManager.LoadScene(1); // 전투 씬으로 이동

                        photonView.RPC("ShowDebug", RpcTarget.MasterClient, matchingDebug);
                    }
                }
            }

        }
        StartCoroutine(COR_DelayMove());

    }
    IEnumerator COR_DelayMove()
    {
        yield return new WaitForSeconds(1f);
        Camera.main.gameObject.transform.position = new Vector3(20, 0, -10);
        GameMGR.Instance.uiManager.storePannel.SetActive(false);
        GameMGR.Instance.batch.UnitPlacement();
        GameMGR.Instance.battleLogic.AttackLogic();
    }
    /*
    private void BattleOrder()
    {
        // 패시브(순서 상관없이 항상 발동) // 함수 X
        // 전투 시작
        // if 내가 선공이라면
        for (int i = 0; i < 6; i++)
        {
            // myDeck.transform.GetChild(i).gameObject.GetComponent<Card>().OnStart();
            // otherDeck.transform.GetChild(i).gameObject.GetComponent<Card>().OnStart();
        }
        // else 상대가 선공이라면
        for (int i = 0; i < 6; i++)
        {
            // otherDeck.transform.GetChild(i).gameObject.GetComponent<Card>().OnStart();
            // myDeck.transform.GetChild(i).gameObject.GetComponent<Card>().OnStart();
        }

        GoShop();
    }
    */
    [PunRPC]
    public void MatchingReady()
    {
        readyCount[0]++;
        if (PhotonNetwork.IsMasterClient)
        {
            if (readyCount[0] >= PhotonNetwork.PlayerList.Length)
            {
                photonView.RPC("MatchingSetting", RpcTarget.MasterClient);
            }
        }
    }

    public void LifeDown()
    {
        myLife--;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Life", $"{myLife}" } });
    }

    public void LifeUp()
    {
        myLife++;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Life", $"{myLife}" } });
    }

    //[PunRPC]
    public void LifeManager()
    {
        //if(!isWin)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log($"{i} 번호의 유저 닉네임은 {PhotonNetwork.PlayerList[i].NickName}"); // 1
                Debug.Log($"{i}의 커스텀 넘버값 : {PhotonNetwork.PlayerList[i].CustomProperties["Number"]}"); // 1
                Debug.Log($"{i}의 커스텀라이프 라이프값 : {PhotonNetwork.PlayerList[i].CustomProperties["Life"]}");
            }
        }
    }

}



