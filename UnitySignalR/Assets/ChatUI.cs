using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    public InputField chatMessageInput;
    public InputField newChannelInput;
    GameSimulator gameSimulator;
    public Text baseChatItem;
    public Text currentChannel;
    void Awake()
    {
        gameSimulator = GetComponentInParent<GameSimulator>();
        gameSimulator.commandInfos[Command.ResultSendChat] = new CommandInfo("ä�� �޽��� ������", null, ResultSendChat);
        gameSimulator.commandInfos[Command.ResultChangeChatChannel] = new CommandInfo("ä�� ����", null, ResultChangeChatChannel);

        baseChatItem.gameObject.SetActive(false);

        transform.Find("MessageSend/Button").GetComponent<Button>()
            .onClick.AddListener(RequestSendChat);
        currentChannel = transform.Find("Channel/ChannelText").GetComponent<Text>();

        transform.Find("Channel/Button").GetComponent<Button>()
            .onClick.AddListener(RequestChangeChatChannel);
    }

    private void Start()
    {
        UserData.Instance.onInitUserinfo += Instance_onInitUserInfo;
    }

    private void Instance_onInitUserInfo(Userinfo userinfo)
    {
        currentChannel.text = $"{userinfo.lastChatGroup} ä��";
    }

    bool allowSendEnter;
    bool allowChangeChannelEnter;
    void Update()
    {
        if (allowSendEnter && (chatMessageInput.text.Length > 0) && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
        {
            RequestSendChat();
            allowSendEnter = false;
        }
        else
            allowSendEnter = chatMessageInput.isFocused || chatMessageInput.isFocused;


        if (allowChangeChannelEnter && (newChannelInput.text.Length > 0) && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
        {
            RequestChangeChatChannel();
            allowChangeChannelEnter = false;
        }
        else
            allowChangeChannelEnter = newChannelInput.isFocused || newChannelInput.isFocused;
    }

    private void AddChatMessage(string senderName, string message)
    {
        string cleanMessage = GetCleanString(message);
        var newChat = Instantiate(baseChatItem, baseChatItem.transform.parent);
        newChat.text = $"{senderName} : {cleanMessage}";
        newChat.gameObject.SetActive(true);
        StartCoroutine(DelayActiveCo(newChat.gameObject));
    }

    private IEnumerator DelayActiveCo(GameObject go)
    {
        yield return null;
        go.SetActive(false);
        yield return null;
        go.SetActive(true);
    }

    string GetCleanString(string input)
    {
        return BadWord_Regex.Replace(input, "*");
    }
    public string[] badWords = new string[]{
        "����", "ȫ��", "���ѿ�" 
    };
    Regex _BadWord_Regex;
    public Regex BadWord_Regex
    {
        get
        {
            if (_BadWord_Regex == null)
                _BadWord_Regex = CreateRegex(badWords, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            return _BadWord_Regex;

            Regex CreateRegex(string[] filters, RegexOptions options)
            {
                StringBuilder ft = new StringBuilder();

                for (int i = 0; i < filters.Length; ++i)
                {
                    if (i >= filters.Length - 1)
                        ft.AppendFormat("{0}", filters[i]);
                    else
                        ft.AppendFormat("{0} | ", filters[i]);
                }

                return new Regex(ft.ToString(), options);
            }
        }
    }
    #region �����Լ�
    private bool ReturnIfErrorExist(ErrorCode result)
    {
        return gameSimulator.ReturnIfErrorExist(result);
    }
    private void SendToServer(RequestMsg request)
    {
        gameSimulator.SendToServer(request);
    }
    #endregion �����Լ�

    #region ä�� �޽��� ������
    void RequestSendChat()
    {
        RequestSendChat request = new RequestSendChat();
        request.message = chatMessageInput.text;
        chatMessageInput.text = string.Empty;
        SendToServer(request);
    }


    public void ResultSendChat(string jsonStr)
    {
        ResultSendChat result = JsonConvert.DeserializeObject<ResultSendChat>(jsonStr);

        if (ReturnIfErrorExist(result.result))
            return;

        string senderName = result.senderName;
        string message = result.message;
        AddChatMessage(senderName, message);
    }
    #endregion ä�� �޽��� ������

    #region ä�� ä�� ����

    private void RequestChangeChatChannel()
    {
        RequestChangeChatChannel request = new RequestChangeChatChannel();
        request.newChannel = newChannelInput.text;
        newChannelInput.text = string.Empty;
        SendToServer(request);
    }


    public void ResultChangeChatChannel(string jsonStr)
    {
        ResultChangeChatChannel result = JsonConvert.DeserializeObject<ResultChangeChatChannel>(jsonStr);

        if (ReturnIfErrorExist(result.result))
            return;

        AddChatMessage("ä��", $"<color=green>{result.newChannel}</color> �� �̵��Ǿ����ϴ�");
    }
    #endregion ä�� ä�� ����

}
