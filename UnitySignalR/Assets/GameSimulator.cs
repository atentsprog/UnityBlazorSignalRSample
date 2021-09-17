using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CommandInfo
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
public class GameSimulator : MonoBehaviour
{
    CommandHub commandHub;
    public Button baseButton;
    public Dictionary<Command, CommandInfo> commandInfos = new Dictionary<Command, CommandInfo>();
    void Awake()
    {
        commandHub = GetComponent<CommandHub>();

        commandInfos[Command.ResultError] = new CommandInfo("����", null, ResultError);

        commandInfos[Command.ResultLogin] = new CommandInfo("�α���", RequestLogin, ResultLogin);
        commandInfos[Command.ResultReward] = new CommandInfo("����", RequestReward, ResultReward);
        commandInfos[Command.ResultChangeNickname] = new CommandInfo("�г��� ����", RequestChangeNickname, ResultChangeNickname);
    }

    private void Start()
    {   
        foreach (var item in commandInfos.Values)
        {
            if (item.requestFn == null)
                continue;

            var newButton = Instantiate(baseButton, baseButton.transform.parent);
            newButton.GetComponentInChildren<Text>().text = item.name;
            newButton.onClick.AddListener(item.requestFn);
        }
        baseButton.gameObject.SetActive(false);
    }

    private void ResultError(string errorDiscription)
    {
        Debug.LogError($"�������� ���� ���� ����: {errorDiscription}");
    }

    public void SendToServer(RequestMsg request)
    {
        commandHub.SendToServer(request);
    }

    public void OnReceiveCommand(Command resultCommand, string jsonStr)
    {
        if (commandInfos.TryGetValue(resultCommand, out CommandInfo commandInfo))
        {
            commandInfo.resultFn(jsonStr);
        }
        else
        {
            Debug.LogError($"{resultCommand}:���� �������� ���� �޽����Դϴ�");
        }
    }

    /// <summary>
    /// ������ �ִٸ� �����ڵ带 ǥ���ϰ� true����
    /// </summary>
    /// <param name="result"></param>
    /// <returns>���� ������ true</returns>
    public bool ReturnIfErrorExist(ErrorCode result)
    {
        if (result != ErrorCode.Succeed)
        {
            Debug.LogError(result);
            return true;
        }

        return false;
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

        print(result.userinfo.gold);
        UserData.Instance.userinfo = result.userinfo;
        GetComponentInChildren<ChatUI>().currentChannel.text = result.userinfo.lastChatGroup;

        UserData.Instance.account = result.account;
    }

    #endregion �α���
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

        if (ReturnIfErrorExist(result.result))
            return;

        print(result.rewardGold);
        print(result.currentGold);
        UserData.Instance.userinfo.gold = result.currentGold;
    }
    #endregion
    #region �г��� ����
    private void RequestChangeNickname()
    {
        RequestChangeNickname request = new RequestChangeNickname();
        request.newNickname = UserData.Instance.userinfo.nickname;
        SendToServer(request);
    }
    private void ResultChangeNickname(string jsonStr)
    {
        ResultChangeNickname result = JsonConvert.DeserializeObject<ResultChangeNickname>(jsonStr);

        if (ReturnIfErrorExist(result.result))
            return;

        Debug.Log($"<color=green>{result.resultNickname}</color> �г��� �����");
    }

    #endregion �г��� ����
}
