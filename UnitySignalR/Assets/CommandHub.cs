using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class CommandHub : MonoBehaviour
{
    private static HubConnection connection;
    public string baseURL = "http://localhost:5001/command";
    void Start()
    {
        Debug.Log("Hello World!");

        transform.Find("Button").GetComponent<Button>()
            .onClick.AddListener(Login);

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
    void OnReceiveMessage(string message)
    {
        lock(mainThreadFn)
        { 
            mainThreadFn.Add(() =>
            {
                Debug.Log($"{message} + !!!!!");
                Debug.Log(transform.name);
                Debug.Log(transform.position);
                Debug.Log($"{message} + !");
            });
        }
    }

    private async void Connect()
    {
        connection.On<string>("ClientReceiveMessage", OnReceiveMessage);
        await connection.StartAsync();

        Login();
    }
    public string message = "Hello!";
    public void Login()
    {
        // 로그인 명령..
        RequestLogin request = new RequestLogin();
        request.deviceID = SystemInfo.deviceUniqueIdentifier;
        string json = JsonUtility.ToJson(request);

        connection.InvokeAsync("SeverReceiveMessage", Command.RequestLogin, json);
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
