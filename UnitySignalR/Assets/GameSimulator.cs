using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class GameSimulator : MonoBehaviour
{
    // �г��� ����
    // ID, PW �ְ� �α���.

    // ��� �߰�(IAP, ������)
    // ������� ������ �߰� -> ���� �������Ͽ��� �۾��Ѱ� ������ Ŭ���̾�Ʈ���� ���.

    // Item����
    // ���� ������

    // �������� ����(���׹̳� �Ҹ�, 10�п� ���׹̳� 1���� ȸ��)
    // �������� Ŭ����(���� ������ ���� ���� -> ���������� ����)



    public Button buttonBaseItem;
    CommandHub commandHub;
    public InputField input1;
    void Start()
    {
        commandHub = GetComponent<CommandHub>();
        commandHub.onReceiveCommand += CommandHub_onReceiveCommand;

        input1 = transform.Find("Input1").GetComponent<InputField>();

        //List<Tuple<string, Action>> commands = new List<Tuple<string, Action>>();
        //commands.Add(new Tuple<string, Action>("�α���", RequestLogin));
        //commands.Add(new Tuple<string, Action>("�г��� ����", RequestChangeNickname));
        //commands.Add(new Tuple<string, Action>("������ ����", RequestBuyItem));
        //commands.Add(new Tuple<string, Action>("������ �Ǹ�", RequestSellItem));

        commands.AddRange(new CommandInfo[]
            {
                new CommandInfo("�α���", Command.RequestLogin, Command.ResultLogin, RequestLogin, ResultLogin),
                new CommandInfo("�г��� ����", Command.RequestChangeNickname, Command.ResultChangeNickname, RequestChangeNickname, ResultChangeNickname),
            });

        //��ư �����.
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
            Debug.LogError($"{command}:���� �������� ���� �޽����Դϴ�");
    }


    /// <summary>
    /// ������ ������ ���� �ڵ带 ǥ���ϰ� true�� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="result"></param>
    /// <returns>������ ������ true</returns>
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
