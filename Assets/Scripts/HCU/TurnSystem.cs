using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using ExitGames.Client.Photon;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.Experimental.AI;
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

        public int[] setRandom = new int[500]; // 공격할 대상을 랜덤으로 지정한다. 

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

        // 

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
            matchingList.Remove((int)otherPlayer.CustomProperties["Number"]);
            Debug.Log(otherPlayer + "가 나갔다");
            if (otherPlayer.IsInactive) Debug.Log(" IsInactive");
            else Debug.Log("NotInactive");

            Debug.Log("나의 커스텀프로퍼티 번호" + (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
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
            Debug.Log($"{targetPlayer} 의 {changedProps}가 변경되었다");
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
        [SerializeField] List<int> matchingList2 = new List<int>();
        [SerializeField] List<int> matchingList3 = new List<int>();

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
            // 중복매칭 방지 스크립트
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Debug.Log("현재 " + i);
                n = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length);

                if(matchingList.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"])|| matchingList2.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"])) { i--; continue; } // 중복 제외 처리

                if (curRound != 0 && PhotonNetwork.PlayerList.Length>2)
                {
                    if (i % 2 != 0)
                    {
                        if (matchingList.Contains((int)PhotonNetwork.PlayerList[n].CustomProperties["Opponent"]))
                        {
                            Debug.Log($"{(int)PhotonNetwork.PlayerList[n].CustomProperties["Opponent"]}가 들어있음");

                            i--;
                            continue;
                        }
                        else
                        {
                            Debug.Log("3 " + matchingList[0]);
                            matchingList.Add((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
                            Debug.Log("4 " + matchingList[1]);
                            matchingList2.Add(matchingList[0]);
                            matchingList2.Add(matchingList[1]);
                            matchingList.Clear();
                            if (matchingList2.Count >= PhotonNetwork.PlayerList.Length)
                            {
                                if (matchingList2.Count % 2 != 0)
                                {
                                    matchingList3=matchingList2; // 깍두기를 제외한 나머지 랜덤 범위를 지정하기 위한 일종의 매칭리스트 업데이트 베타 테스트 알파 메가 버젼 3

                                    matchingList3.Remove(matchingList3[matchingList3.Count - 1]);
                                    int a = UnityEngine.Random.Range(0, matchingList3.Count);
                                    int clone = matchingList3[a];
                                    matchingList2.Add(clone);
                                    for(int j = 0; j < matchNum.Length; j++)
                                    {
                                        matchNum[j] = matchingList2[j];
                                    }
                                    matchingList2.Clear();
                                    matchingList3.Clear();
                                    photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum, true); // Null이 뜨는 이유?}
                                    return;

                                }
                                else 
                                {
                                    matchNum = new int[matchingList2.Count];
                                    Debug.Log(matchNum.Length);
                                    for (int j = 0; j < matchNum.Length; j++)
                                    {
                                        matchNum[j] = matchingList2[j];
                                    }
                                    matchingList2.Clear();
                                    photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum, false); // Null이 뜨는 이유?}
                                    return;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("디버그 " + (int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
                        matchingList.Add((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
                    }
                }
                else
                {
                    matchingList.Add((int)PhotonNetwork.PlayerList[n].CustomProperties["Number"]);
                }
            }

            curRound++;

            // 플레이어들을 큐에 다 집어넣으면 배열에 순서대로 넣는다.
            matchNum = new int[matchingList.Count];
            Debug.Log(matchNum.Length);
            for (int i = 0; i < matchNum.Length; i++)
            {
                matchNum[i] = matchingList[i];
            }

            photonView.RPC("Matching", RpcTarget.All, setRandom, matchNum); // Null이 뜨는 이유?
        }

        [PunRPC] // 각 플레이어들의 매칭이 정상적으로 되었는지 확인하기 위한 RPC 함수
        public void ShowDebug(string a)
        {
            Debug.Log(a);
        }

        [PunRPC]
        public void Matching(int[] random, int[] num,bool clone)
        {

            if (clone) // 클론 있을때
            {
                Debug.Log("1:1 & 1:1 시작");
                for (int i = 0; i < num.Length; i++)
                {
                    if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Number"] == num[i] && i < num.Length - 1) // 내 번호랑 일치하는지와 배열 번호가 마지막(클론) 이 아닌 경우에만
                    {
                        if (i == 0 || i % 2 == 0)
                        {
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i+1] } });
                            Debug.Log("나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i+1] );
                            string a = "나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i + 1];
                            // 내가 선공
                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                            
                        }
                        else if ( i % 2 != 0 && i < num.Length )
                        {
                            // 내 상대는 = matchMan[i-1]
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", num[i - 1] } });
                            Debug.Log("나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i - 1]);
                            string a = "나는 " + userID + " 닉네임 : " + PhotonNetwork.LocalPlayer.CustomProperties["Number"] + " 상대 : " + num[i - 1];
                            // 내가 후공
                            photonView.RPC("ShowDebug", RpcTarget.MasterClient, a);
                        }
                    }
                    else if(i < num.Length - 1)
                    {

                    }
                }
            }

            else if (!clone) // 클론 없을때 01 23 45
            {
                Debug.Log("1:1 & 1:1 시작");
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

                            
                            SceneManager.LoadScene(1);


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
/*                if(PhotonNetwork.IsMasterClient)
                {
                    MatchingSetting();
                }*/
                photonView.RPC("MatchingSetting", RpcTarget.MasterClient);
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

