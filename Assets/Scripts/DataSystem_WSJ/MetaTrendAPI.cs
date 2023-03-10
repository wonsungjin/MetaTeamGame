using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using TMPro;
using Unity.VisualScripting;
using System;
using Photon.Pun;
using UnityEngine.Assertions.Must;

public class MetaTrendAPI : MonoBehaviour
{
    [SerializeField] TMP_InputField txtInputField;
    [SerializeField] string selectedBettingID;

    [Header("[등록된 프로젝트에서 획득가능한 API 키]")]
    [SerializeField] string API_KEY = "";

    [Header("[Betting Backend Base URL]")]
    [SerializeField] string FullAppsProductionURL = "https://odin-api.browseosiris.com";
    [SerializeField] string FullAppsStagingURL = "https://odin-api-sat.browseosiris.com";

    DateTime APITime;
    DateTime ServerTime;
    TimeSpan spanTime;

    string APIProfilePort = "http://localhost:8546/api/getuserprofile";
    string APIIdPort = "http://localhost:8546/api/getsessionid";

    WaitForSeconds waitTime = new WaitForSeconds(1f);

    string timeDI;
    string totalGold;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            Debug.LogError("포트번호 변경");
            APIProfilePort = "http://localhost:8544/api/getuserprofile";
            APIIdPort = "http://localhost:8544/api/getsessionid";
        }
    }

    // 버튼 누를시 URL 연결
    public void ButtonGetURL()
    {
        Application.OpenURL("https://www.naver.com/");
    }



    string getBaseURL()
    {
        // 프로덕션 단계라면
        //return FullAppsProductionURL;

        // 스테이징 단계(개발)라면
        return FullAppsStagingURL;
    }

    public Res_UserProfile res_UserProfile = null;
    Res_UserSessionID res_UserSessionID = null;
    Res_BettingSetting res_BettingSetting = null;
    public Res_DummyPool res_DummyPool = null;
    //---------------
    // 유저 정보
    public void GetUserProfile()
    {
        StartCoroutine(processRequestGetUserInfo());
    }
    IEnumerator processRequestGetUserInfo()
    {
        // 유저 정보
        yield return requestGetUserInfo((response) =>
        {
            if (response != null)
            {
                Debug.Log("## " + response.ToString());
                res_UserProfile = response;
                Debug.Log("" + res_UserProfile.userProfile.username);
                wallet_address = res_UserProfile.userProfile.public_address;
                GameMGR.Instance.stayAPI[0] = true;
            }
        });
    }
    delegate void resCallback_GetUserInfo(Res_UserProfile response);
    IEnumerator requestGetUserInfo(resCallback_GetUserInfo callback)
    {
        // get user profile
        UnityWebRequest www = UnityWebRequest.Get(APIProfilePort);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        //txtInputField.text = www.downloadHandler.text;
        Res_UserProfile res_getUserProfile = JsonUtility.FromJson<Res_UserProfile>(www.downloadHandler.text);
        callback(res_getUserProfile);
    }


    //------------------------------

    public void GetDummyPool()
    {
        StartCoroutine(processRequestGetDummy());
    }
    //------------------------------

    public IEnumerator processRequestGetDummy()
    {
        while (true)
        {
            yield return requestGetDummy((response) =>
                 {
                     if (response != null)
                     {
                         APITime = DateTime.Parse(response.data.records[0].endTime);

                         // photon server time
                         // ServerTime = DateTimeOffset.FromUnixTimeSeconds(PhotonNetwork.ServerTimestamp).LocalDateTime
                         // spanTime = APITime - DateTime.Now;
                         spanTime = APITime - DateTime.Now;
                         timeDI = spanTime.ToString(@"dd\:hh\:mm\:ss");

                         if (APITime > DateTime.Now)
                         {
                             // Debug.LogError(spanTime.Days + " 일_" + spanTime.Hours + " 시간_" + spanTime.Minutes + " 분_" + spanTime.Seconds + " 초");
                         }

                         else if (APITime < DateTime.Now)
                         {
                             // Debug.LogError("경과 시간 : " + timeDI);
                         }
                     }
                 });

            yield return waitTime;

            if (GameMGR.Instance.uiManager.isLobby) { GameMGR.Instance.uiManager.tournamentText.text = timeDI; }
        }
    }
    //--------------------------------

    delegate void resCallback_GetDummy(Res_DummyPool response);
    IEnumerator requestGetDummy(resCallback_GetDummy callback)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://odin-api-uat.browseosiris.com/v1/dummy-tournament-pool/get-data");
        yield return www.SendWebRequest();

        Res_DummyPool res_DummyPool = JsonUtility.FromJson<Res_DummyPool>(www.downloadHandler.text);
       // Debug.Log(www.downloadHandler.text);
        callback(res_DummyPool);
    }

    //---------------
    // Session ID
    public void GetSessionID()
    {
        StartCoroutine(processRequestGetSessionID());
    }
    IEnumerator processRequestGetSessionID()
    {
        // 유저 정보
        yield return requestGetSessionID((response) =>
        {
            if (response != null)
            {
                Debug.Log("## " + response.ToString());
                res_UserSessionID = response;
                GameMGR.Instance.stayAPI[1] = true;
            }
        });
    }
    delegate void resCallback_GetSessionID(Res_UserSessionID response);
    IEnumerator requestGetSessionID(resCallback_GetSessionID callback)
    {
        // get session id
        UnityWebRequest www = UnityWebRequest.Get(APIIdPort);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        //txtInputField.text = www.downloadHandler.text;
        Res_UserSessionID res_getSessionID = JsonUtility.FromJson<Res_UserSessionID>(www.downloadHandler.text);
        callback(res_getSessionID);
    }
    string coin_name;
    string wallet_address;
    int amount_in_integers;
    int balance;

    string dummy_Id;
    string dummy_Title;
    string dummy_Amount;
    string dummy_TotalCollectedAmount;
    string dummy_StartTime;
    string dummy_EndTime;
    string dummy_CreatedAt;
    string dummy_UpdatedAt;


    public float GetZera() { return balance; }
    //해당 코인을 얻습니다.
    public void GetCoin(int coinValue)
    {
        coin_name = "zera";
        amount_in_integers = coinValue;
        Debug.Log(coin_name);
        Debug.Log(wallet_address);
        Debug.Log(amount_in_integers);
        StartCoroutine(processRequestGetCoin());
    }
    IEnumerator processRequestGetCoin()
    {
        // 유저 정보
        yield return requestGetCoin((response) =>
        {
            if (response != null)
            {
                Debug.Log("## " + response.ToString());
                res_UserSessionID = response;
            }
        });
    }
    delegate void resCallback_GetCoin(Res_UserSessionID response);
    IEnumerator requestGetCoin(resCallback_GetCoin callback)
    {
        // get session id
        UnityWebRequest www = UnityWebRequest.Get($"https://dappx-api-sat.dappstore.me/users/{coin_name}/faucet?wallet={wallet_address}&amount={amount_in_integers}");
        ZeraBalance();
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
    }

    //---------------
    // 베팅관련 셋팅 정보를 얻어오기
    public void Settings()
    {
        StartCoroutine(processRequestSettings());
    }
    IEnumerator processRequestSettings()
    {
        yield return requestSettings((response) =>
        {
            if (response != null)
            {
                Debug.Log("## Settings : " + response.ToString());
                res_BettingSetting = response;
                Debug.Log(res_BettingSetting);
            }
        });
    }
    delegate void resCallback_Settings(Res_BettingSetting response);
    IEnumerator requestSettings(resCallback_Settings callback)
    {
        string url = getBaseURL() + "/v1/betting/settings";


        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("api-key", API_KEY);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        txtInputField.text = www.downloadHandler.text;
        Res_BettingSetting res = JsonUtility.FromJson<Res_BettingSetting>(www.downloadHandler.text);
        callback(res);
        //UnityWebRequest www = new UnityWebRequest(URL);
    }

    //---------------
    // Zera 잔고 확인
    public void ZeraBalance()
    {
        StartCoroutine(processRequestZeraBalance());
    }
    IEnumerator processRequestZeraBalance()
    {
        yield return requestZeraBalance(res_UserSessionID.sessionId, (response) =>
        {
            if (response != null)
            {
                Debug.Log("## Response Zera Balance : " + response.ToString());
            }
        });
    }
    delegate void resCallback_BalanceInfo(Res_ZeraBalance response);
    IEnumerator requestZeraBalance(string sessionID, resCallback_BalanceInfo callback)
    {
        string url = getBaseURL() + ("/v1/betting/" + "zera" + "/balance/" + sessionID);

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("api-key", API_KEY);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        //txtInputField.text = www.downloadHandler.text;
        Res_ZeraBalance res = JsonUtility.FromJson<Res_ZeraBalance>(www.downloadHandler.text);
        callback(res);
        string[] Value = res.ToString().Split("message : success Balance : ");
        balance = int.Parse(Value[1]);
        //UnityWebRequest www = new UnityWebRequest(URL);
    }

    //---------------
    // ZERA 베팅
    public void Betting_Zera()
    {
        StartCoroutine(processRequestBetting_Zera());
    }
    IEnumerator processRequestBetting_Zera()
    {
        Res_Initialize resBettingPlaceBet = null;
        Req_Initialize reqBettingPlaceBet = new Req_Initialize();
        reqBettingPlaceBet.players_session_id = new string[] { res_UserSessionID.sessionId };
        reqBettingPlaceBet.bet_id = selectedBettingID;// resSettigns.data.bets[0]._id;
        yield return requestCoinPlaceBet(reqBettingPlaceBet, (response) =>
        {
            if (response != null)
            {
                Debug.Log("## CoinPlaceBet : " + response.message);
                resBettingPlaceBet = response;
            }
        });
    }
    delegate void resCallback_BettingPlaceBet(Res_Initialize response);
    IEnumerator requestCoinPlaceBet(Req_Initialize req, resCallback_BettingPlaceBet callback)
    {
        string url = getBaseURL() + "/v1/betting/" + "zera" + "/place-bet";

        string reqJsonData = JsonUtility.ToJson(req);
        Debug.Log(reqJsonData);


        UnityWebRequest www = UnityWebRequest.Post(url, reqJsonData);
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
        www.uploadHandler = new UploadHandlerRaw(buff);


        www.SetRequestHeader("api-key", API_KEY);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        txtInputField.text = www.downloadHandler.text;
        Res_Initialize res = JsonUtility.FromJson<Res_Initialize>(www.downloadHandler.text);
        callback(res);
    }

    //---------------
    // ZERA 베팅-승자
    public void Betting_Zera_DeclareWinner()
    {
        StartCoroutine(processRequestBetting_Zera_DeclareWinner());
    }
    IEnumerator processRequestBetting_Zera_DeclareWinner()
    {
        Res_BettingWinner resBettingDeclareWinner = null;
        Req_BettingWinner reqBettingDeclareWinner = new Req_BettingWinner();
        reqBettingDeclareWinner.betting_id = selectedBettingID;// resSettigns.data.bets[0]._id;
        reqBettingDeclareWinner.winner_player_id = res_UserProfile.userProfile._id;
        yield return requestCoinDeclareWinner(reqBettingDeclareWinner, (response) =>
        {
            if (response != null)
            {
                Debug.Log("## CoinDeclareWinner : " + response.message);
                resBettingDeclareWinner = response;
            }
        });
    }
    delegate void resCallback_BettingDeclareWinner(Res_BettingWinner response);
    IEnumerator requestCoinDeclareWinner(Req_BettingWinner req, resCallback_BettingDeclareWinner callback)
    {
        string url = getBaseURL() + "/v1/betting/" + "zera" + "/declare-winner";

        string reqJsonData = JsonUtility.ToJson(req);
        Debug.Log(reqJsonData);


        UnityWebRequest www = UnityWebRequest.Post(url, reqJsonData);
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
        www.uploadHandler = new UploadHandlerRaw(buff);
        www.SetRequestHeader("api-key", API_KEY);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        txtInputField.text = www.downloadHandler.text;
        Res_BettingWinner res = JsonUtility.FromJson<Res_BettingWinner>(www.downloadHandler.text);
        callback(res);
    }

    //---------------
    // 베팅금액 반환
    public void Betting_Zera_Disconnect()
    {
        StartCoroutine(processRequestBetting_Zera_Disconnect());
    }
    IEnumerator processRequestBetting_Zera_Disconnect()
    {
        Res_BettingDisconnect resBettingDisconnect = null;
        Req_BettingDisconnect reqBettingDisconnect = new Req_BettingDisconnect();
        reqBettingDisconnect.betting_id = selectedBettingID;// resSettigns.data.bets[1]._id;
        yield return requestCoinDisconnect(reqBettingDisconnect, (response) =>
        {
            if (response != null)
            {
                Debug.Log("## CoinDisconnect : " + response.message);
                resBettingDisconnect = response;
            }
        });
    }
    delegate void resCallback_BettingDisconnect(Res_BettingDisconnect response);
    IEnumerator requestCoinDisconnect(Req_BettingDisconnect req, resCallback_BettingDisconnect callback)
    {
        string url = getBaseURL() + "/v1/betting/" + "zera" + "/disconnect";

        string reqJsonData = JsonUtility.ToJson(req);
        Debug.Log(reqJsonData);


        UnityWebRequest www = UnityWebRequest.Post(url, reqJsonData);
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
        www.uploadHandler = new UploadHandlerRaw(buff);
        www.SetRequestHeader("api-key", API_KEY);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        txtInputField.text = www.downloadHandler.text;
        Res_BettingDisconnect res = JsonUtility.FromJson<Res_BettingDisconnect>(www.downloadHandler.text);
        callback(res);
    }
}
