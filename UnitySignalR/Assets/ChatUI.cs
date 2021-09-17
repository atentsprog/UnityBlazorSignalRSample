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
    void Start()
    {
        // 채팅 메시지 받으면 화면 출력하자.
        textBaseItem.gameObject.SetActive(false);

        gameSimulator = GetComponentInParent<GameSimulator>();
        var commandInfos = gameSimulator.commandInfos;
        commandInfos[Command.RequestSendMessage] = new CommandInfo("SendMessage", RequestSendMessage, ResultSendMessage);
        // 센드 버튼 누르면 서버에 메시지 전송.

        // 채널 변경 버튼 누르면 채널 변경.
    }

    private void RequestSendMessage()
    {
        throw new NotImplementedException();
    }

    private void ResultSendMessage(string arg0)
    {
        throw new NotImplementedException();
    }

    public string tempMessage = "aa";
    int testCount;

    [ContextMenu("메시지 추가 테스트")]
    void TestAddMessage()
    {
        Text newText = Instantiate(textBaseItem, textBaseItem.transform.parent);
        newText.text = tempMessage + testCount++;
        newText.gameObject.SetActive(true);

        // 위치가 정상적으로 갱신되지 않아서 추가함.
        ContentSizeFitter csf = newText.GetComponent<ContentSizeFitter>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(csf.GetComponent<RectTransform>());
    }
}

