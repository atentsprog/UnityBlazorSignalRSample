using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameSimulator : MonoBehaviour
{
    class CommandInfo
    {
        public string name;
        public Command requestCommand;
        public Command resultCommand;
        public UnityAction requestFn;
        public UnityAction<string> resultFn;


        public CommandInfo(string name, Command requestCommand, Command resultCommand, UnityAction requestFn, UnityAction<string> resultFn)
        {
            this.name = name;
            this.requestCommand = requestCommand;
            this.resultCommand = resultCommand;
            this.requestFn = requestFn;
            this.resultFn = resultFn;
        }
    }
    CommandHub commandHub;
    public Button baseButton;
    List<CommandInfo> commandInfos = new List<CommandInfo>();
    void Awake()
    {
        commandHub = GetComponent<CommandHub>();

        commandInfos.AddRange(new CommandInfo[]{
            new CommandInfo("로그인", Command.RequestLogin, Command.ResultLogin, RequestLogin, ResultLogin),
            new CommandInfo("보상", Command.RequestReward, Command.ResultReward, RequestReward, ResultReward),
            });

        foreach(var item in commandInfos)
        {
            var newButton = Instantiate(baseButton, baseButton.transform.parent);
            newButton.GetComponentInChildren<Text>().text = item.name;
            newButton.onClick.AddListener(item.requestFn);
        }
        baseButton.gameObject.SetActive(false);
    }
    private void SendToServer(RequestMsg request)
    {
        commandHub.SendToServer(request);
    }

    #region 로그인
    public void RequestLogin()
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
