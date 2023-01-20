using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Hashtable = ExitGames.Client.Photon.Hashtable; // 이게 구버전 한정인지 필수인지는 나도 모른다는 것이 학계의 점심

namespace hcu
{
    public class TurnSystem : MonoBehaviourPunCallbacks
    {
        private bool isGameOver;

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

        // 덱 수 카운트 ( 각 플레이어들의 덱 수를 가져온다 )
        public int[] deckCount;

        // 게임 도중 나가버린 플레이어 낙인
        public List<int> exitPlayerList;

        #region 포톤 콜백 함수들

        public void SetName()
        {
            string myName = null;
            PhotonNetwork.NickName = myName;
        }

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
        public override void OnJoinedLobby()
        {
            Debug.Log("로비에 입장 성공");
            PhotonNetwork.JoinRoom("test");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            // 룸옵션에 대한 정보
            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 20 };
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            PhotonNetwork.CreateRoom("test", roomOptions);
            Debug.Log("test룸 생성");

            photonView.RPC("FailedJoinRoom", RpcTarget.All, PhotonNetwork.NickName);
        }

        [PunRPC]
        public void FailedJoinRoom(string name)
        {
            Debug.Log(name + "가 방 들어오는데 실패했다");
        }

        [PunRPC]
        public void FailedConnectMaster(string name)
        {
            Debug.Log(name + "가 마스터서버 연결에 실패했다");
        }

        private void OnFailedToConnectToMasterServer()
        {
            photonView.RPC("FailedConnectMaster", RpcTarget.All, PhotonNetwork.NickName);
        }
        //Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;
        public override void OnJoinedRoom() // 자기 자신만
        {
            Debug.Log("test룸 입장");
            players = PhotonNetwork.PlayerList;
            Debug.Log("방 접속자 수 : " + PhotonNetwork.PlayerList.Length);
            Debug.Log(players[0].ActorNumber);
            for (int i = 0; i < players.Length; i++)
            {
                PhotonNetwork.PlayerList[i].NickName = (PhotonNetwork.PlayerList[i].ActorNumber - 1).ToString();
                if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.NickName)
                {
                    ExitGames.Client.Photon.Hashtable myCustomProperty = new ExitGames.Client.Photon.Hashtable();
                    myCustomProperty = PhotonNetwork.LocalPlayer.CustomProperties;
                    PhotonNetwork.PlayerList[i].CustomProperties["Number"] = i;
                    PhotonNetwork.PlayerList[i].CustomProperties["Life"] = startLife;
                    PhotonNetwork.PlayerList[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Number", $"{i}" } });
                    PhotonNetwork.PlayerList[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Life", $"{startLife}" } });
                    PhotonNetwork.PlayerList[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", -1 } });
                    userID = (int)PhotonNetwork.PlayerList[i].CustomProperties["Number"];  // 고유닉네임이 일치하면 고유번호를 지정한다.
                    Debug.Log($" 번호 값 잘 들어갔니 {PhotonNetwork.PlayerList[i].CustomProperties["Number"]}");
                    Debug.Log($" 라이프 잘 들어갔니 {PhotonNetwork.PlayerList[i].CustomProperties["Life"]}");
                    PhotonNetwork.SetPlayerCustomProperties(myCustomProperty);
                }
                Debug.Log($"내 지정번호는 {(int)PhotonNetwork.PlayerList[i].CustomProperties["Number"]}");
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) // 다른 사람 입장시
        {
            Debug.Log(newPlayer.NickName + " 입장");
            Debug.Log("방 접속자 수 : " + PhotonNetwork.PlayerList.Length);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // 플레이어가 나갈 때 나간 플레이어의 고유값을 상대 리스트에서도 제거해야한다. 나간 플레이어의 커스텀프로퍼티를 찾으려고할 때 NULL
            //matchingList.Remove((int)otherPlayer.CustomProperties["Number"]);
            //exitPlayerList.Add((int)otherPlayer.CustomProperties["Nunber"]);
            //if (PhotonNetwork.PlayerList.Length == 2) 
            {
                cloneOpponent = -1;
                cloneOpponentsOpponent = -1;
            }

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

        private void Awake()
        {
            PhotonNetwork.ConnectUsingSettings(); // 포톤 기본세팅 사용 및 마스터서버 연결
        }

        private void Start()
        {
            GameLoop();
        }

        private void GameLoop()
        {
            //BattleOrder(); // 순서대로 전투

            if (!isGameOver)
                GoShop(); // 상점으로 가!
        }

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
            // 플레이어 라이프 지정 (배열 변경시 연동 위함)
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
        }

        [SerializeField] List<int> matchingList = new List<int>();
        [SerializeField] List<int> matchingListReal = new List<int>();
        [SerializeField] int cloneOpponent = -1;
        [SerializeField] int cloneOpponentsOpponent = -1;
        int c = 0;

        [PunRPC]
        public void MatchingSetting()
        {
            //공격 랜덤값 지정 - 매 스테이지마다 랜덤 값을 받아온다
            for (int i = 0; i < setRandom.Length; i++)
            {
                setRandom[i] = UnityEngine.Random.Range(0, 3);
            }

            int n = 0;

            matchingList.Clear(); //  플레이어 리스트 한번 초기화 싹 해준다.
            matchingListReal.Clear();

            // 중복매칭 방지 스크립트
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (cloneOpponent != -1)
                {
                    matchingList.Add(cloneOpponent);
                    for(int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
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
                    n = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length);

                    if (matchingList.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]) || matchingListReal.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"])) { i--; continue; } // 중복 제외 처리
                }
                if (curRound != 0 && PhotonNetwork.PlayerList.Length > 2)
                {
                    //Debug.Log("현재 " + i);

                    if (matchingList.Count != 0)
                    {
                        if (matchingList.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Opponent"]) || cloneOpponentsOpponent == (int)PhotonNetwork.PlayerList[n].CustomProperties["Number"])
                        {
                            Debug.Log($"{(int)PhotonNetwork.PlayerList[n].CustomProperties["Opponent"]}가 들어있음");
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

            photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum, isClone);
        }


        [PunRPC] // 각 플레이어들의 매칭이 정상적으로 되었는지 확인하기 위한 RPC 함수
        public void ShowDebug(string a)
        {
            Debug.Log(a);
        }

        [PunRPC]
        public void Matching(int[] random, int[] num, bool clone)
        {
            Debug.Log(num.Length);
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
                            string a = "나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i + 1];
                            // 내가 선공

                            GameMGR.Instance.matching[0] = num[i];
                            GameMGR.Instance.matching[1] = num[i + 1];
                            GameMGR.Instance.randomValue = random;
                            //SceneManager.LoadScene(1);

                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);

                        }
                        else if (i % 2 != 0 && i < num.Length)
                        {
                            // 내 상대는 = matchMan[i-1]
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i - 1] } });
                            Debug.Log("나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i - 1]);
                            string a = "나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i - 1];
                            // 내가 후공

                            GameMGR.Instance.matching[0] = num[i];
                            GameMGR.Instance.matching[1] = num[i - 1];
                            GameMGR.Instance.randomValue = random;
                            //SceneManager.LoadScene(1);

                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
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
                            string a = "나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i + 1];
                            // 내가 선공

                            // 내가 짝수 상대 +1 홀수

                            GameMGR.Instance.matching[0] = num[i];
                            GameMGR.Instance.matching[1] = num[i + 1];
                            GameMGR.Instance.randomValue = random;
                            //SceneManager.LoadScene(1);

                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);

                        }
                        else if (i % 2 != 0 && i < num.Length)
                        {
                            // 내 상대는 = matchMan[i-1]
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i - 1] } });
                            Debug.Log("나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i - 1]);
                            string a = "나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i - 1];
                            // 내가 후공

                            // 내가 홀수 상대 -1짝수
                            GameMGR.Instance.matching[0] = num[i];
                            GameMGR.Instance.matching[1] = num[i - 1];
                            GameMGR.Instance.randomValue = random;
                            //SceneManager.LoadScene(1); // 전투 씬으로 이동

                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                        }
                    }
                }
            }

        }

        private void GoShop()
        {
            // 상점 씬으로 이동
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


        private void Update()
        {
            //if(PhotonNetwork.IsMasterClient)
            if (Input.GetKeyDown(KeyCode.S))
            {
                photonView.RPC("MatchingSetting", RpcTarget.MasterClient);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {

            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                photonView.RPC("StartSetting", RpcTarget.MasterClient);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                // photonView.RPC("LifeManager", RpcTarget.MasterClient);
                LifeManager();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                isWin = true;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                isWin = false;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                LifeDown();
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                LifeUp();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    Debug.Log($"{PhotonNetwork.PlayerList[i].NickName}");
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

}

