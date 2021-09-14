using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;

public class CommandHub : MonoBehaviour
{
    private static HubConnection connection;
    public string baseURL = "http://localhost:5001/command";
    void Start()
    {
        Debug.Log("Hello World!");

        transform.Find("Button").GetComponent<Button>()
            .onClick.AddListener(Login);
        elapsedTimesText = transform.Find("ElapsedTime").GetComponent<Text>();

        connection = new HubConnectionBuilder()
            .WithUrl(baseURL)
            .Build();
        connection.Closed += async (error) =>
        {
            await Task.Delay(1000);
            await connection.StartAsync();
        };

        Connect();
    }

    List<double> elapsedTimes = new List<double>();
    void OnReceiveMessage(Command command, string jsonStr)
    {
        double elapsedTime = (DateTime.Now - requstTime).TotalMilliseconds;
        elapsedTimes.Add(elapsedTime);
        lock (mainThreadFn)
        { 
            mainThreadFn.Add(() =>
            {
                OnReceiveCommand(command, jsonStr);
            });
        }
    }

    private void OnReceiveCommand(Command command, string jsonStr)
    {
        UpdateElapsedTimeUI();

        switch (command)
        {
            case Command.ResultLogin:                
                ResultLogin resultLogin = JsonConvert.DeserializeObject<ResultLogin>(jsonStr);
                print(resultLogin.userinfo.Gold);
                UserData.Instance.userinfo = resultLogin.userinfo;
                UserData.Instance.account = resultLogin.account;
                break;
            default:
                Debug.LogError($"{command}:���� �������� ���� �޽����Դϴ�");
                break;
        }
    }

    Text elapsedTimesText;
    bool completeDeleteFirstTime = false;
    private void UpdateElapsedTimeUI()
    {
        double lastTime = elapsedTimes.Last() * 0.0001;
        double average = elapsedTimes.Sum() / elapsedTimes.Count * 0.0001;
        elapsedTimesText.text = $"Last :{lastTime:0.0000}s, average:{average:0.0000}s";
        if (completeDeleteFirstTime == false)
        {
            elapsedTimes.RemoveAt(0); // ó���α��� �޽��� ������ �ٸ��ͺ��� ������������ ������ ������.
            completeDeleteFirstTime = true;
        }
    }

    private async void Connect()
    {
        connection.On<Command, string>("ClientReceiveMessage", OnReceiveMessage);
        await connection.StartAsync();

        Login();
    }
    public string message = "Hello!";
    public void Login()
    {
        // �α��� ���..
        RequestLogin request = new RequestLogin();
        request.deviceID = SystemInfo.deviceUniqueIdentifier;

        SendToServer(request);
    }

    DateTime requstTime;
    public void SendToServer(RequestMsg request)
    {
        requstTime = DateTime.Now;
        string json = JsonConvert.SerializeObject(request);
        connection.InvokeAsync("SeverReceiveMessage", request.command, json);
    }

    List<Action> mainThreadFn = new List<Action>();
    
    private void Update()
    {
        lock (mainThreadFn)
        {
            if (mainThreadFn.Count > 0)
            {
                foreach (var item in mainThreadFn)
                {
                    item();
                }
                mainThreadFn.Clear();
            }
        }
    }
}
