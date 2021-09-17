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
        sendButton.onClick.AddListener(RequestSendMessage);
        changeChannelButton.onClick.AddListener(RequestChangeChannel);

        var commandInfos = gameSimulator.commandInfos;

        // ���� ��ư ������ ������ �޽��� ����. ä�� �޽��� ������ ȭ�� �������.
        // ä�� ���� ��ư ������ ä�� ����.
        commandInfos[Command.ResultSendMessage] = new CommandInfo("ä�� ������", null, ResultSendMessage);
        commandInfos[Command.ResultChangeChannel] = new CommandInfo("ä�� ����", null, ResultChangeChannel);
    }

    public bool allowSendEnter;
    private void Update()
    {
        if (allowSendEnter && (chatInputFiled.text.Length > 0) && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
        {
            RequestSendMessage();
            allowSendEnter = false;
        }
        else
            allowSendEnter = chatInputFiled.isFocused;
    }

    private void SendToServer(RequestMsg request)
    {
        gameSimulator.SendToServer(request);
    }
    private bool ReturnIfErrorExist(ErrorCode result)
    {
        return gameSimulator.ReturnIfErrorExist(result);
    }

    #region ä�� �޽��� ������ �ޱ�
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
    #endregion ä�� �޽��� ������ �ޱ�

    #region ä�� ä�� �ٲٱ�
    private void RequestChangeChannel()
    {
        RequestChangeChannel request = new RequestChangeChannel();
        request.newChannelName = newChannelInputFiled.text;
        newChannelInputFiled.text = string.Empty;
        SendToServer(request);
    }
    private void ResultChangeChannel(string jsonStr)
    {
        ResultChangeChannel result = JsonConvert.DeserializeObject<ResultChangeChannel>(jsonStr);
        if (ReturnIfErrorExist(result.result))
            return;
        string newChannelName = result.newChannelName;
        currentChannel.text = newChannelName;
    }
    #endregion ä�� ä�� �ٲٱ�

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

