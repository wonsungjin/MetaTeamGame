using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public enum OrderType
{
    Buy,
    Sell,
    Start,
    Attack,
    Hit,
    Kill,
    Die,
    Alive,
    End
}

public class TurnSystem : MonoBehaviourPunCallbacks
{
    private bool isGameOver;

    [SerializeField] private GameObject Me;
    [SerializeField] private GameObject enemyDeck;

    [SerializeField] private GameObject[] playerCardPos;
    [SerializeField] private GameObject[] otherCardPos;

    public Player[] players = null;

    [SerializeField] public InputField inputField; // 닉네임 입력칸

    public int[] setRandom = new int[500]; // 공격할 대상을 랜덤으로 지정한다. 

    public int userID = 0;
    /*    private TurnSystem instance = null;
        public TurnSystem Inst
        {
            get
            {
                if(instance == null)
                    instance = FindObjectOfType<TurnSystem>();
                return instance;
            }
        }*/

    #region 포톤 콜백 함수들

    public void SetName()
    {
        string myName = null;
        PhotonNetwork.NickName = myName;
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터서버에 연결 성공");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 입장 성공");


        PhotonNetwork.JoinRoom("test");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // 룸옵션에 대한 정보
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 20 };
        PhotonNetwork.CreateRoom("test", roomOptions);
        //PhotonNetwork.JoinRoom("test");
        Debug.Log("test룸 생성");
    }



    public override void OnJoinedRoom() // 자기 자신만
    {
        Debug.Log("test룸 입장");

        // 플레이어 고유번호 지정 ( 랜덤값으로 선공을 설정하기 위함 )
        players = PhotonNetwork.PlayerList;
        Debug.Log("방 접속자 수 : " + PhotonNetwork.PlayerList.Length);
        Debug.Log(players[0].ActorNumber);
        for (int i = 0; i < players.Length; i++)
        {
            PhotonNetwork.NickName = PhotonNetwork.PlayerList[i].ActorNumber.ToString();
            if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.NickName) userID = i;  // 고유닉네임이 일치하면 고유번호를 지정한다.
        }
        /*for(int i = 0; i < players.Length; i++)
        {
            // 고유 번호를 매긴다.
            if (players[i].ActorNumber == playerNum)
            {
                playerID = i;
                Debug.Log("내 ID는 " + i);
                playerNum++;
            }
        }*/

    }
    #endregion

    public override void OnPlayerEnteredRoom(Player newPlayer) // 다른 사람 입장시
    {
        Debug.Log(newPlayer.NickName + " 입장");
        Debug.Log("방 접속자 수 : " + PhotonNetwork.PlayerList.Length);
    }

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(); // 포톤 기본세팅 사용 및 마스터서버 연결
    }

    private void Start()
    {
        //playerCardPos = new GameObject[6];
        //otherCardPos = new GameObject[6];
        GameLoop();
    }

    private void GameLoop()
    {

        BattleOrder(); // 순서대로 전투

        if (!isGameOver)
            GoShop(); // 상점으로 가!
    }

    //List<Player> matchMan = null;
    Player[] matchMan = new Player[8]; // 최대 8인이고 그 이상을 넘을 수는 없으니 일단 이 값으로 지정.

    [PunRPC]
    public void MatchingSetting()
    {
        // 매칭은 마스터클라이언트 혼자 관리하여 전역으로 실행한다.

        Queue<Player> matchQueue = new Queue<Player>();

        for (int i = 0; i < setRandom.Length; i++)
        {
            setRandom[i] = Random.Range(0, 3);
        }
        int n = 0;
        // 큐에 플레이어들을 랜덤한 순서대로 집어넣는다.

        players = PhotonNetwork.PlayerList;
        Debug.Log("players.Length 의 길이는 :" + players.Length);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            n = Random.Range(0, PhotonNetwork.PlayerList.Length);
                if (matchQueue.Contains(players[n]))
                { i--; continue; }
                matchQueue.Enqueue(players[n]);
                Debug.Log("큐에 1개 추가");
        }
        Debug.Log(matchQueue.Count);
        // 플레이어들을 큐에 다 집어넣으면 배열에 순서대로 넣는다.
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            matchMan[i] = matchQueue.Dequeue();
            Debug.Log("큐에서 1개 꺼냄");
        }

        // 모든 세팅이 끝났다.
        Debug.Log(setRandom);
        Debug.Log(matchMan);
        photonView.RPC("Matching", RpcTarget.All, setRandom, matchMan);
        Debug.Log("RPC Matchig 호출");
        if(setRandom == null)
        {
            Debug.Log("setRandom이 널");
        }
        if(matchMan == null)
        {
            Debug.Log("matchman이 널");
        }
    }

    [PunRPC]
    public void Matching(int[] random, Player[] matchManAll)
    {
        if(random == null)
        Debug.Log("random 배열 넘겨받기 실패");
        if(matchManAll == null)
        Debug.Log("matchMan 배열 넘겨받기 실패");
        matchMan = matchManAll;
        //마스터 클라이언트가 대진 설정을 마치고 각 플레이어들을 1:1로 묶는 함수
        Debug.Log("Matching 함수 실행");
        if (PhotonNetwork.PlayerList.Length < 2)
        {
            // 나 혼자 남았으니 자동 승리
            Debug.Log("나 혼자 방에 남아버렸다");
        }

        else if (PhotonNetwork.PlayerList.Length == 2)
        {
            // 1 : 1 시작
            if (matchMan[0].NickName == PhotonNetwork.NickName)
            {
                // 내가 선공
                Debug.Log("나는 " + userID + " 닉네임 : " + matchMan[0].NickName + " 상대 : " + matchMan[1].NickName);
                // 내 상대는 배열1
            }
            else
            {
                // 내가 후공
                Debug.Log("나는 " + userID + " 닉네임 : " + matchMan[1].NickName + " 상대 : " + matchMan[0].NickName);
                // 내 상대는 배열
            }
        }

        // 플레이어가 짝수, 4명 이상이라면
        else if (PhotonNetwork.PlayerList.Length % 2 == 0 && PhotonNetwork.PlayerList.Length > 3)
        {
            for (int i = 0; i < players.Length;)
            {
                if (matchMan[i].NickName == PhotonNetwork.NickName)
                {
                    if (i == 0 || i % 2 == 0)
                    {
                        // 내 상대는  = matchMan[i+1] 
                        Debug.Log("나는 " + userID + " 닉네임 : " + matchMan[0].NickName + " 상대 : " + matchMan[1].NickName);
                        // 내가 선공
                    }
                    else
                    {
                        // 내 상대는 = matchMan[i-1]
                        Debug.Log("나는 " + userID + " 닉네임 : " + matchMan[1].NickName + " 상대 : " + matchMan[0].NickName);
                        // 내가 후공
                    }
                }
            }
        }

        // 플레이어가 홀수, 3명 이상이라면
        else if (PhotonNetwork.PlayerList.Length % 2 == 1 && PhotonNetwork.PlayerList.Length > 2)
        {
            for (int i = 0; i < players.Length;)
            {
                if (matchMan[i].NickName == PhotonNetwork.NickName)
                {
                    if (i == 0 || i % 2 == 0)
                    {
                        // 내 상대는  = matchMan[i+1]
                        Debug.Log("나는 " + userID + " 닉네임 : " + matchMan[0].NickName + " 상대 : " + matchMan[1].NickName);
                    }
                    else
                    {
                        // 내 상대는 = matchMan[i-1]
                        Debug.Log("나는 " + userID + " 닉네임 : " + matchMan[1].NickName + " 상대 : " + matchMan[0].NickName);
                    }
                }
            }
        }
    }

    private void GoShop()
    {
        // 상점 씬으로 이동

    }

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

        // 턴 종료 = 상점으로 이동
        GoShop();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            photonView.RPC("MatchingSetting", RpcTarget.MasterClient);
        }
    }


}
