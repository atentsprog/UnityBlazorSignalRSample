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
        public UnityAction requestFn;
        public UnityAction<string> resultFn;
        public CommandInfo(string name, UnityAction requestFn, UnityAction<string> resultFn)
        {
            this.name = name;
            this.requestFn = requestFn;
            this.resultFn = resultFn;
        }
    }
    CommandHub commandHub;
    public Button baseButton;
    Dictionary<Command, CommandInfo> commandInfos = new Dictionary<Command, CommandInfo>();
    void Awake()
    {
        commandHub = GetComponent<CommandHub>();

        commandInfos[Command.ResultLogin] = new CommandInfo("�α���", RequestLogin, ResultLogin);
        commandInfos[Command.ResultReward] = new CommandInfo("����", RequestReward, ResultReward);


        foreach(var item in commandInfos.Values)
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

    #region �α���
    public void RequestLogin()
    {
        // �α��� ���..
        RequestLogin request = new RequestLogin();
        request.deviceID = SystemInfo.deviceUniqueIdentifier;

        SendToServer(request);
    }
    public void ResultLogin(string jsonStr)
    {
        ResultLogin result = JsonConvert.DeserializeObject<ResultLogin>(jsonStr);

        if (ReturnIfErrorExist(result.result))
            return;

        print(result.userinfo.Gold);
        UserData.Instance.userinfo = result.userinfo;
        UserData.Instance.account = result.account;
    }

    #endregion �α���
    /// <summary>
    /// ������ �ִٸ� �����ڵ带 ǥ���ϰ� true����
    /// </summary>
    /// <param name="result"></param>
    /// <returns>���� ������ true</returns>
    private bool ReturnIfErrorExist(ErrorCode result)
    {
        if (result != ErrorCode.Succeed)
        {
            Debug.LogError(result);
            return true;
        }

        return false;
    }
    #region ���� ��û
    public string rewardType = "100Gold";
    void RequestReward()
    {
        RequestReward request = new RequestReward();
        request.rewardType = rewardType;
        SendToServer(request);
    }
    public void ResultReward(string jsonStr)
    {
        ResultReward result = JsonConvert.DeserializeObject<ResultReward>(jsonStr);
        
        if(result.result != ErrorCode.Succeed)
        {
            Debug.LogError(result.result);
            return;
        }
         
        print(result.rewardGold);
        print(result.currentGold);
        UserData.Instance.userinfo.Gold = result.currentGold;
    }
    #endregion
}
