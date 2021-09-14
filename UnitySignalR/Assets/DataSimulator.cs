using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DataSimulator : MonoBehaviour
{
    //로그인
    //간단한 UI로 서버랑 통신할 로직만 구현해서 테스트 하자!
    //골드, 최고 점수,
    //아이템 저장(로그인시 서버에 있는 값 읽기)
    //스테이지 클리어 보상 획득(서버 보상 지급 - 보상 그룹에서 랜덤으로 지급)
    //시간에 따라 회복하는 스테미나 로직(클라이언트및 서버검증)

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
