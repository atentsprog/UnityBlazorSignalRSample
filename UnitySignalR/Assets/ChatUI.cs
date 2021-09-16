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
    public InputField input;
    GameSimulator gameSimulator;
    public Text baseChatItem;
    void Awake()
    {
        gameSimulator = GetComponentInParent<GameSimulator>();
        gameSimulator.commandInfos[Command.ResultSendChat] = new CommandInfo("채팅 메시지 보내기", RequestSendChat, ResultSendChat);

        baseChatItem.gameObject.SetActive(false);

        transform.Find("MessageSend/Button").GetComponent<Button>()
            .onClick.AddListener(RequestSendChat);
    }

    bool allowEnter;
    void Update()
    {
        if (allowEnter && (input.text.Length > 0) && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
        {
            RequestSendChat();
            allowEnter = false;
        }
        else
            allowEnter = input.isFocused || input.isFocused;
    }

    private void AddChatMessage(string senderName, string message)
    {
        string cleanMessage = GetCleanString(message);
        var newChat = Instantiate(baseChatItem, baseChatItem.transform.parent);
        newChat.text = $"{senderName} : {cleanMessage}";
        newChat.gameObject.SetActive(true);
    }
    string GetCleanString(string input)
    {
        return BadWord_Regex.Replace(input, "*");
    }
    public string[] badWords = new string[]{
        "광고", "홍보", "심한욕" 
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
    #region 래핑함수
    private bool ReturnIfErrorExist(ErrorCode result)
    {
        return gameSimulator.ReturnIfErrorExist(result);
    }
    private void SendToServer(RequestSendChat request)
    {
        gameSimulator.SendToServer(request);
    }
    #endregion 래핑함수

    #region 채팅 메시지 보내기
    void RequestSendChat()
    {
        RequestSendChat request = new RequestSendChat();
        request.message = input.text;
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
    #endregion 채팅 메시지 보내기
}
