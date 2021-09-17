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
        // ä�� �޽��� ������ ȭ�� �������.
        textBaseItem.gameObject.SetActive(false);

        gameSimulator = GetComponentInParent<GameSimulator>();
        var commandInfos = gameSimulator.commandInfos;
        commandInfos[Command.RequestSendMessage] = new CommandInfo("SendMessage", RequestSendMessage, ResultSendMessage);
        // ���� ��ư ������ ������ �޽��� ����.

        // ä�� ���� ��ư ������ ä�� ����.
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

    [ContextMenu("�޽��� �߰� �׽�Ʈ")]
    void TestAddMessage()
    {
        Text newText = Instantiate(textBaseItem, textBaseItem.transform.parent);
        newText.text = tempMessage + testCount++;
        newText.gameObject.SetActive(true);

        // ��ġ�� ���������� ���ŵ��� �ʾƼ� �߰���.
        ContentSizeFitter csf = newText.GetComponent<ContentSizeFitter>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(csf.GetComponent<RectTransform>());
    }
}

