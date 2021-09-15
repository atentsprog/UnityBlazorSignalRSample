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
    public event Action<Command, string> onReceiveCommand;
    private void OnReceiveCommand(Command command, string jsonStr)
    {
        UpdateElapsedTimeUI();
        print($"응답 받음({command}):{jsonStr}");
        onReceiveCommand(command, jsonStr);
    }

    Text elapsedTimesText;
    private void UpdateElapsedTimeUI()
    {
        double lastTime = elapsedTimes.Last() * 0.0001;
        double average = elapsedTimes.Sum() / elapsedTimes.Count * 0.0001;
        elapsedTimesText.text = $"Last :{lastTime:0.000}s, average:{average:0.000}s";
    }

    public float delayConnect = 0.5f;
    private async void Connect()
    {
        await Task.Delay((int)(delayConnect * 1000));
        connection.On<Command, string>("ClientReceiveMessage", OnReceiveMessage);
        await connection.StartAsync();

        Login();
    }
    public string message = "Hello!";
    public void Login()
    {
        // 로그인 명령..
        RequestLogin request = new RequestLogin();
        request.deviceID = SystemInfo.deviceUniqueIdentifier;

        SendToServer(request);
    }

    DateTime requstTime;
    public void SendToServer(RequestMsg request)
    {
        requstTime = DateTime.Now;
        string json = JsonConvert.SerializeObject(request);
        print("서버로 요청 : " + json);
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
