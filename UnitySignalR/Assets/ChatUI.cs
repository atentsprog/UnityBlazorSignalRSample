using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    public Text textBaseItem;
    public Text currentChannel;
    public InputField chatInputFiled;
    public InputField newChannelInputFiled;
    public Button sendButton;
    public Button changeChannelButton;

    GameSimulator gameSimulator;
    void Awake()
    {
        textBaseItem.gameObject.SetActive(false);

        gameSimulator = GetComponentInParent<GameSimulator>();
        var commandInfos = gameSimulator.commandInfos;

        // 센드 버튼 누르면 서버에 메시지 전송. 채팅 메시지 받으면 화면 출력하자.
        // 채널 변경 버튼 누르면 채널 변경.
        commandInfos[Command.ResultSendMessage] = new CommandInfo("채팅 보내기", RequestSendMessage, ResultSendMessage);
        commandInfos[Command.ResultChangeChannel] = new CommandInfo("채널 변경", RequestChangeChannel, ResultChangeChannel);
    }

    private void SendToServer(RequestMsg request)
    {
        gameSimulator.SendToServer(request);
    }
    private bool ReturnIfErrorExist(ErrorCode result)
    {
        return gameSimulator.ReturnIfErrorExist(result);
    }

    #region 메시지 보내고 받기
    private void RequestSendMessage()
    {
        RequestSendMessage request = new RequestSendMessage();
        request.message = chatInputFiled.text;
        chatInputFiled.text = string.Empty;
        SendToServer(request);
    }
    private void ResultSendMessage(string jsonStr)
    {
        ResultSendMessage result = JsonConvert.DeserializeObject<ResultSendMessage>(jsonStr);

        if (ReturnIfErrorExist(result.result))
            return;
        string sendName = result.senderName;
        string message = result.message;
        TestAddMessage($"{sendName}: {message}");
    }
    #endregion 메시지 보내고 받기

    private void RequestChangeChannel()
    {
        throw new NotImplementedException();
    }
    private void ResultChangeChannel(string jsonStr)
    {
        throw new NotImplementedException();
    }

    void TestAddMessage(string message)
    {
        Text newText = Instantiate(textBaseItem, textBaseItem.transform.parent);
        newText.text = message;
        newText.gameObject.SetActive(true);

        // 위치가 정상적으로 갱신되지 않아서 추가함.
        ContentSizeFitter csf = newText.GetComponent<ContentSizeFitter>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(csf.GetComponent<RectTransform>());
    }
}

