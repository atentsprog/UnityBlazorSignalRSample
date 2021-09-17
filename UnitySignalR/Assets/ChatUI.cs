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

        // ���� ��ư ������ ������ �޽��� ����. ä�� �޽��� ������ ȭ�� �������.
        // ä�� ���� ��ư ������ ä�� ����.
        commandInfos[Command.ResultSendMessage] = new CommandInfo("ä�� ������", RequestSendMessage, ResultSendMessage);
        commandInfos[Command.ResultChangeChannel] = new CommandInfo("ä�� ����", RequestChangeChannel, ResultChangeChannel);
    }

    private void SendToServer(RequestMsg request)
    {
        gameSimulator.SendToServer(request);
    }
    private bool ReturnIfErrorExist(ErrorCode result)
    {
        return gameSimulator.ReturnIfErrorExist(result);
    }

    #region �޽��� ������ �ޱ�
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
    #endregion �޽��� ������ �ޱ�

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

        // ��ġ�� ���������� ���ŵ��� �ʾƼ� �߰���.
        ContentSizeFitter csf = newText.GetComponent<ContentSizeFitter>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(csf.GetComponent<RectTransform>());
    }
}

