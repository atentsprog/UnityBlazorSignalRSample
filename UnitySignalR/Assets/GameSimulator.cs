using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class GameSimulator : MonoBehaviour
{
    // 닉네임 변경
    // ID, PW 넣고 로그인.

    // 골드 추가(IAP, 광고보상)
    // 운영툴에서 아이템 추가 -> 보통 엑셀파일에서 작업한걸 서버와 클라이언트에서 사용.

    // Item구입
    // 우편 보내기

    // 스테이지 엔터(스테미나 소모, 10분에 스테미나 1개씩 회복)
    // 스테이지 클리어(랜덤 아이템 보상 지급 -> 우편함으로 지급)



    public Button buttonBaseItem;
    CommandHub commandHub;
    public InputField input1;
    void Start()
    {
        commandHub = GetComponent<CommandHub>();
        commandHub.onReceiveCommand += CommandHub_onReceiveCommand;

        input1 = transform.Find("Input1").GetComponent<InputField>();

        //List<Tuple<string, Action>> commands = new List<Tuple<string, Action>>();
        //commands.Add(new Tuple<string, Action>("로그인", RequestLogin));
        //commands.Add(new Tuple<string, Action>("닉네임 설정", RequestChangeNickname));
        //commands.Add(new Tuple<string, Action>("아이템 구입", RequestBuyItem));
        //commands.Add(new Tuple<string, Action>("아이템 판매", RequestSellItem));

        commands.AddRange(new CommandInfo[]
            {
                new CommandInfo("로그인", Command.RequestLogin, Command.ResultLogin, RequestLogin, ResultLogin),
                new CommandInfo("닉네임 설정", Command.RequestChangeNickname, Command.ResultChangeNickname, RequestChangeNickname, ResultChangeNickname),
            });

        //버튼 만들기.
        foreach (var item in commands)
        {
            var newbutton = Instantiate(buttonBaseItem, buttonBaseItem.transform.parent);
            newbutton.onClick.AddListener(item.requestFn);
            newbutton.GetComponentInChildren<Text>().text = item.name;
        }
    }

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
    List<CommandInfo> commands = new List<CommandInfo>();

    private void CommandHub_onReceiveCommand(Command command, string jsonStr)
    {
        CommandInfo commandInfo = commands.Find(x => x.resultCommand == command);

        if(commandInfo != null)
            commandInfo.resultFn(jsonStr);
        else
            Debug.LogError($"{command}:아직 구현하지 안은 메시지입니다");
    }


    /// <summary>
    /// 에러가 있으면 에러 코드를 표시하고 true를 반환한다.
    /// </summary>
    /// <param name="result"></param>
    /// <returns>에러가 있으면 true</returns>
    private bool ShowErrorCodeIfNotSucceed(ErrorCode result)
    {
        if (result != ErrorCode.Succeed)
        {
            Debug.LogWarning(result);
            return true;
        }
        return false;
    }

    void SendToServer(RequestMsg requestMsg)
    {
        commandHub.SendToServer(requestMsg);
    }
}
