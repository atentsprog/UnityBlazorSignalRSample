using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

public class CommandHub : MonoBehaviour
{
    private static HubConnection connection;
    public string baseURL = "http://localhost:5001/command";
    void Start()
    {
        Debug.Log("Hello World!");
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
        Debug.Log($"{message}");
    }

    private async void Connect()
    {
        connection.On<string>("ClientReceiveMessage", OnReceiveMessage);
        await connection.StartAsync();
    }
    public string message = "Hello!";
    private async void Send()
    {
        await connection.InvokeAsync("SeverReceiveMessage", message);
    }

    List<Action> mainThreadFn = new List<Action>();
    private void Update()
    {
        lock (mainThreadFn)
        {
            foreach (var item in mainThreadFn)
                item();
            mainThreadFn.Clear();
        }
    }
}
