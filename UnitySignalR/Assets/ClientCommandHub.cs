using Microsoft.AspNetCore.SignalR.Client;
using ShareClientAndServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientCommandHub : MonoBehaviour
{
    private static HubConnection connection;
    public string baseURL = "http://localhost:5001/command";

    void Start()
    {
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

    private async void Connect()
    {
        ServerMessage serverMessage = GetComponent<ServerMessage>();
        //connection.On<string, string>("broadcastMessage", (name, message) =>
        connection.On<Command, string>("ClientReceiveCommand", serverMessage.OnReceiveCommand);

        try
        {
            await connection.StartAsync();

            Debug.Log("Connection started");
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public async Task SendToServer(RequestMsg sendObject)
    {
        try
        {
            string json = JsonUtility.ToJson(sendObject);
            await connection.InvokeAsync("SeverReceiveCommand", sendObject.command, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
}
