using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DataSimulator : MonoBehaviour
{
    //�α���
    //������ UI�� ������ ����� ������ �����ؼ� �׽�Ʈ ����!
    //���, �ְ� ����,
    //������ ����(�α��ν� ������ �ִ� �� �б�)
    //�������� Ŭ���� ���� ȹ��(���� ���� ���� - ���� �׷쿡�� �������� ����)
    //�ð��� ���� ȸ���ϴ� ���׹̳� ����(Ŭ���̾�Ʈ�� ��������)

    public Text infoText;
    public Button buttonBase;
    public InputFieldButton inputFiledButtonBase;
    ServerMessage serverMessage;
    void Start()
    {
        InitComponent();

        UpdateInfo();
    }

    private void RequestLogin()
    {
        _ = serverMessage.SendLoginRequest();
    }

    public bool showDeviceID = true;
    private void UpdateInfo()
    {
        StringBuilder sb = new StringBuilder();
        if (showDeviceID)
            sb.AppendLine($"ID: {UserInfo.DeviceID}");
        infoText.text = sb.ToString();
    }

    private void InitComponent()
    {
        serverMessage = GetComponent<ServerMessage>();

        List<Tuple<string, Action>> commands = new List<Tuple<string, Action>>();
        commands.Add(new Tuple<string, Action>("Login", RequestLogin));

        foreach (var item in commands)
        {
            var button = Instantiate(buttonBase, buttonBase.transform.parent);
            button.onClick.AddListener(() => { item.Item2.Invoke(); });
            button.GetComponentInChildren<Text>().text = item.Item1;
        }
        buttonBase.gameObject.SetActive(false);
    }
}
