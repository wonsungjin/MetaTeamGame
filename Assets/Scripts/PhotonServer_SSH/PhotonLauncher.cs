using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    byte maxPlayer = 2;

    [SerializeField] TextMeshProUGUI serverState = null;
    [SerializeField] TextMeshProUGUI playerCount = null;
    [SerializeField] TextMeshProUGUI roomName = null;
    [SerializeField] TextMeshProUGUI buttonState = null;

    [SerializeField] Button joinRoomButton = null;
    [SerializeField] Button leaveRoomButton = null;

    RoomOptions roomOptions = new RoomOptions();

    private void Awake()
    {
        // 룸 내의 로드된 레벨 동기화
        PhotonNetwork.AutomaticallySyncScene = true;

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
            buttonState.text = "Conneced Server";
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

        buttonState.text = "Join Room";

        joinRoomButton.enabled = true;
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
        photonView.RPC("SyncCurrentRoomPlayer", RpcTarget.Others, true);
        StatusServer();
        leaveRoomButton.gameObject.SetActive(true);
        buttonState.text = "Joined Room";
        joinRoomButton.enabled = false;
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
        leaveRoomButton.gameObject.SetActive(false);
        joinRoomButton.enabled = true;
    }

    // 서버 상태 표시
    public void StatusServer()
    {
        Debug.Log(PhotonNetwork.NetworkClientState);
        serverState.text = "Server State : " + PhotonNetwork.NetworkClientState.ToString();
        if (PhotonNetwork.InRoom == true)
        {
            playerCount.text = "Player Count : " + PhotonNetwork.CurrentRoom.PlayerCount;
            roomName.text = "Room Name : " + PhotonNetwork.CurrentRoom.Name.ToString();
        }
        else
        {
            playerCount.text = "Player Count : - ";
            roomName.text = "Room Name : - ";
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
        joinRoomButton.enabled = false;
        leaveRoomButton.gameObject.SetActive(false);
        buttonState.text = "Connecting Server";
    }

    // JoinRoom Button 클릭시 실행되는 함수
    public void OnClick_Join_Room()
    {
        PhotonNetwork.JoinRandomRoom(null, maxPlayer);
        if (PhotonNetwork.CurrentRoom.PlayerCount >= maxPlayer) // ?
        {
            Debug.Log("LeaveRoom");
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.CurrentRoom.IsOpen = false;
            Debug.Log("CurrentRoom : " + PhotonNetwork.CurrentRoom.IsOpen);
            PhotonNetwork.JoinRandomRoom(null, maxPlayer);
        }
        StatusServer();
    }

    // LeaveRoom Button 클릭시 실행되는 함수
    public void OnClick_Leave_Room()
    {
        photonView.RPC("SyncCurrentRoomPlayer", RpcTarget.Others, false);
        PhotonNetwork.LeaveRoom();   
    }

    // 현재의 플레이어 수 동기화
    [PunRPC]
    public void SyncCurrentRoomPlayer(bool roomState)
    {
        if (roomState)
        {
            playerCount.text = "Player Count : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        }
        else
        {
            playerCount.text = "Player Count : " + (PhotonNetwork.CurrentRoom.PlayerCount - 1).ToString();
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayer && PhotonNetwork.IsMasterClient == true)
        {
            PhotonNetwork.LoadLevel("MongoDBScene");            
        }
    }
    #endregion
}
