using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public string userName = "UserSSS";
    private static HubConnection connection;
    public string baseURL = "http://localhost:59890/chat";
    void Start()
    {
        //SetPosition(5);
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
    void OnReceiveMessage(string name, string message)
    {
        Debug.Log($"{name}: {message}");

        string[] posString = message.Split(',');
        if (posString.Length != 2)
            return;

        float.TryParse(posString[0], out float x);
        float.TryParse(posString[1], out float y);
        mainThreadFn.Add(
                () => SetPosition(x, y)
            );
    }
    private async void Connect()
    {
        //connection.On<string, string>("broadcastMessage", (name, message) =>
        connection.On<string, string>("ClientReceiveMessage", OnReceiveMessage);

        try
        {
            await connection.StartAsync();

            Debug.Log("Connection started");
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    void OnMouseDrag()
    {
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        objPosition.z = transform.position.z;
        //transform.position = objPosition;
        Send($"{objPosition.x},{objPosition.y}");
    }

    private async void Send(string msg)
    {
        try
        {
            await connection.InvokeAsync("SeverReceiveMessage", userName, msg);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void SetPosition(float x, float y)
    {
        transform.position = new Vector2(x, y);
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