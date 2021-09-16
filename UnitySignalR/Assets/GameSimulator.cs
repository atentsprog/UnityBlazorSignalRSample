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

        commandInfos[Command.ResultError] = new CommandInfo("에러 결과", null, ResultError);
        commandInfos[Command.ResultLogin] = new CommandInfo("로그인", RequestLogin, ResultLogin);
        commandInfos[Command.ResultReward] = new CommandInfo("보상", RequestReward, ResultReward);
        commandInfos[Command.ResultChangeNickname] = new CommandInfo("닉네임 변경", RequestChangeNickname, ResultChangeNickname);

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

        commandHub.onReceiveCommand = OnReceiveCommand;
    }
    public void SendToServer(RequestMsg request)
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

    /// <summary>
    /// 에러가 있다면 에러코드를 표시하고 true리턴
    /// </summary>
    /// <param name="result"></param>
    /// <returns>에러 있으면 true</returns>
    public bool ReturnIfErrorExist(ErrorCode result)
    {
        if (result != ErrorCode.Succeed)
        {
            Debug.LogError(result);
            return true;
        }

        return false;
    }

    #region 로그인
    public void RequestLogin()
    {
        // 로그인 명령..
        RequestLogin request = new RequestLogin();
        request.deviceID = SystemInfo.deviceUniqueIdentifier;

        SendToServer(request);
    }
    public void ResultError(string errorMessage)
    {
        Debug.LogError($"서버에서 에러를 받았습니다. 에러 내용 : {errorMessage}");
    }
    public void ResultLogin(string jsonStr)
    {
        ResultLogin result = JsonConvert.DeserializeObject<ResultLogin>(jsonStr);

        if (ReturnIfErrorExist(result.result))
            return;

        print(result.userinfo.gold);
        UserData.Instance.SetUserinfo(result.userinfo);
        UserData.Instance.account = result.account;
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
