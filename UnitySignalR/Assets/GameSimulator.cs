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
    private void SendToServer(RequestLogin request)
    {
        commandHub.SendToServer(request);
    }

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

}
