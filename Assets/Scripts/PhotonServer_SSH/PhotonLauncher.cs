using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    byte maxPlayer = 3;

    [SerializeField] TextMeshProUGUI playerCount = null;

    [SerializeField] Button joinRoomButton = null;
    [SerializeField] Button leaveRoomButton = null;
    GameObject matchingPannel;

    RoomOptions roomOptions = new RoomOptions();

    private void Awake()
    {
        // 룸 내의 로드된 레벨 동기화
        PhotonNetwork.AutomaticallySyncScene = true;
        playerCount= GameObject.Find("PlayerCount").GetComponent<TextMeshProUGUI>();
       // joinRoomButton = GameObject.Find("StartButton").GetComponent<Button>();
        leaveRoomButton = GameObject.Find("LeaveButton").GetComponent<Button>();
        matchingPannel = GameObject.Find("MatchingPannel");
        matchingPannel.SetActive(false);

        // 룸 생성 옵션 : MaxPlayer
        roomOptions.MaxPlayers = maxPlayer;

        // Object 초기화
        Init();
    }

    private void Start()
    {
        // 서버 연결
        SetPhotonNetWork();

        // 서버 상태 출력
        StatusServer();
        Debug.Log(PhotonNetwork.ServerAddress);

        // PhotonNetwork.CreateRoom(null, roomOptions);
    }

    #region Photon Setting
    // Server Connect
    public void SetPhotonNetWork()
    {
        // PhotonServer 연결
        PhotonNetwork.ConnectUsingSettings();

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("서버 연결 완료");
        }

        else
        {
            Debug.Log("서버 연결 실패");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // MasterClient 연결 시 호출되는 Callback 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnMaster");

      //  joinRoomButton.enabled = true;
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공");
        StatusServer();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 생성 실패");
    }

    // 방 참가 시 호출되는 CallBack 함수
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공");
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        photonView.RPC("SyncCurrentRoomPlayer", RpcTarget.All, true);
        StatusServer();
        leaveRoomButton.gameObject.SetActive(true);
       // joinRoomButton.enabled = false;
        Debug.Log("test룸 입장");
        Debug.Log("방 접속자 수 : " + PhotonNetwork.PlayerList.Length);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            PhotonNetwork.PlayerList[i].NickName = (PhotonNetwork.PlayerList[i].ActorNumber - 1).ToString();
            if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.NickName)
            {
                ExitGames.Client.Photon.Hashtable myCustomProperty = new ExitGames.Client.Photon.Hashtable();
                myCustomProperty = PhotonNetwork.LocalPlayer.CustomProperties;
                PhotonNetwork.PlayerList[i].CustomProperties["Number"] = i;
                PhotonNetwork.PlayerList[i].CustomProperties["Life"] = 20;
                PhotonNetwork.PlayerList[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Number", $"{i}" } });
                PhotonNetwork.PlayerList[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Life", $"{20}" } });
                PhotonNetwork.PlayerList[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Opponent", -1 } });                


                PhotonNetwork.SetPlayerCustomProperties(myCustomProperty);
            }
            Debug.Log($"내 지정번호는 {(int)PhotonNetwork.PlayerList[i].CustomProperties["Number"]}");
        }
    }

    // 방 입장 실패 시 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방 입장 실패");
        Debug.Log(message);
        Debug.Log("Returncode : " + returnCode);

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayer });
    }

    // 방에서 나갈때 호출되는 콜백함수
    public override void OnLeftRoom()
    {
        StatusServer();
       // joinRoomButton.enabled = true;
    }

    // 서버 상태 표시
    public void StatusServer()
    {
        Debug.Log(PhotonNetwork.NetworkClientState);
        if (PhotonNetwork.InRoom == true)
        {
            playerCount.text =  PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayer;
        }
        else
        {
            playerCount.text = "-";
        }
    }

    // 서버와 연결이 끊겼을 때 호출되는 콜백 함수
    public void OnDisconnectedFromServer(DisconnectCause Cause)
    {
        Debug.LogError("서버 연결 끊김");
    }

    // 오브젝트 초기화
    private void Init()
    {
        StatusServer();
       // joinRoomButton.enabled = false;
    }

    // JoinRoom Button 클릭시 실행되는 함수
    public void OnClick_Join_Room()
    {
        PhotonNetwork.JoinRandomRoom(null, maxPlayer);
        StatusServer();
        matchingPannel.gameObject.SetActive(true);
    }

    // LeaveRoom Button 클릭시 실행되는 함수
    public void OnClick_Leave_Room()
    {
        photonView.RPC("SyncCurrentRoomPlayer", RpcTarget.All, false);
        matchingPannel.SetActive(false);
        PhotonNetwork.LeaveRoom();   
    }


    // 현재의 플레이어 수 동기화
    [PunRPC]
    public void SyncCurrentRoomPlayer(bool roomState)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= maxPlayer)
        {
            GameMGR.Instance.uiManager.Faid(GameMGR.Instance.uiManager.blackUI, faidType.In, 0.03f);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                StartCoroutine(COR_SceneDelay());
            }
            else PhotonNetwork.CurrentRoom.IsOpen = true;
        }
        if (roomState)
        {
            playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString()+"/"+maxPlayer;
        }

        else
        {
            playerCount.text = (PhotonNetwork.CurrentRoom.PlayerCount - 1).ToString() + "/" + maxPlayer;
        }

    }
    #endregion
    IEnumerator COR_SceneDelay()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("내가 이동");
        PhotonNetwork.LoadLevel("StoreScene");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) { Debug.Log("내가 이동");  PhotonNetwork.LoadLevel("StoreScene"); }
    }
}
