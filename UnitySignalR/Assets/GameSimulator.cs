using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSimulator : MonoBehaviour
{
    CommandHub commandHub;
    void Awake()
    {
        commandHub = GetComponent<CommandHub>();
    }
    private void SendToServer(RequestMsg request)
    {
        commandHub.SendToServer(request);
    }

    #region 로그인
    public void RequstLogin()
    {
        // 로그인 명령..
        RequestLogin request = new RequestLogin();
        request.deviceID = SystemInfo.deviceUniqueIdentifier;

        SendToServer(request);
    }
    public void ResultLogin(string jsonStr)
    {
        ResultLogin resultLogin = JsonConvert.DeserializeObject<ResultLogin>(jsonStr);
        print(resultLogin.userinfo.Gold);
        UserData.Instance.userinfo = resultLogin.userinfo;
        UserData.Instance.account = resultLogin.account;
    }
    #endregion 로그인

    #region 보상 요청
    public string rewardType = "100Gold";
    void RequestReward()
    {
        RequestReward request = new RequestReward();
        request.rewardType = rewardType;
        SendToServer(request);
    }
    void ResultReward(string jsonStr)
    {
        ResultReward result = JsonConvert.DeserializeObject<ResultReward>(jsonStr);
        print(result.rewardGold);
        print(result.currentGold);
        UserData.Instance.userinfo.Gold = result.currentGold;
    }
    #endregion
}
