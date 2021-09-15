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
        public CommandInfo(string name,  UnityAction requestFn, UnityAction<string> resultFn)
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

        commandInfos[Command.ResultLogin] = new CommandInfo("로그인", RequestLogin, ResultLogin);
        commandInfos[Command.ResultReward] = new CommandInfo("보상", RequestReward, ResultReward);
        commandInfos[Command.ResultChangeNickname] = new CommandInfo("닉네임 변경", RequestChangeNickname, ResultChangeNickname);


        foreach (var item in commandInfos.Values)
        {
            var newButton = Instantiate(baseButton, baseButton.transform.parent);
            newButton.GetComponentInChildren<Text>().text = item.name;
            newButton.onClick.AddListener(item.requestFn);
        }
        baseButton.gameObject.SetActive(false);

        commandHub.onReceiveCommand = OnReceiveCommand;
    }
    private void SendToServer(RequestMsg request)
    {
        commandHub.SendToServer(request);
    }

    private void OnReceiveCommand(Command command, string jsonStr)
    {
        if (commandInfos.TryGetValue(command, out CommandInfo info))
        {
            info.resultFn(jsonStr);
        }
        else
        {
            Debug.LogError($"{command}:아직 구현하지 안은 메시지입니다");
        }
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
        ResultLogin result = JsonConvert.DeserializeObject<ResultLogin>(jsonStr);

        if (ReturnIfErrorExist(result.result))
            return;

        print(result.userinfo.gold);
        UserData.Instance.userinfo = result.userinfo;
        UserData.Instance.account = result.account;
    }

    #endregion 로그인
    /// <summary>
    /// 에러가 있다면 에러코드를 표시하고 true리턴
    /// </summary>
    /// <param name="result"></param>
    /// <returns>에러 있으면 true</returns>
    private bool ReturnIfErrorExist(ErrorCode result)
    {
        if (result != ErrorCode.Succeed)
        {
            Debug.LogError(result);
            return true;
        }

        return false;
    }
    #region 보상 요청
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

        if (result.result != ErrorCode.Succeed)
        {
            Debug.LogError(result.result);
            return;
        }

        print(result.rewardGold);
        print(result.currentGold);
        UserData.Instance.userinfo.gold = result.currentGold;
    }
    #endregion


    #region 닉네임 교체
    void RequestChangeNickname()
    {
        RequestChangeNickname request = new RequestChangeNickname();
        request.newNickname = UserData.Instance.userinfo.nickname;
        SendToServer(request);
    }

    public void ResultChangeNickname(string jsonStr)
    {
        ResultChangeNickname result = JsonConvert.DeserializeObject<ResultChangeNickname>(jsonStr);

        if (ReturnIfErrorExist(result.result))
            return;

        print($"[{result.newNickname}] 이름 변경이 완료되었습니다.");
    }
    #endregion 닉네임 교체
}
