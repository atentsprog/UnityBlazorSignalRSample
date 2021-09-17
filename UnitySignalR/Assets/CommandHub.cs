using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Threading;

public class CommandHub : MonoBehaviour
{
    private static HubConnection connection;
    public string baseURL = "http://localhost:5001/command";
    GameSimulator gameSimulator;
    void Start()
    {
        Debug.Log("Hello World!");

        connection = new HubConnectionBuilder()
            .WithUrl(baseURL)
            .Build();
        connection.Closed += async (error) =>
        {
            print(error);
            mainThreadFn.Add(AutoReconnect);
            await Task.Delay(1);
        };
        gameSimulator = GetComponent<GameSimulator>();

        AutoReconnect();
    }

    Coroutine autoReconnectCoHandle;
    CancellationTokenSource tokenSource;
    private void AutoReconnect()
    {
        StopReconnectCo();
        autoReconnectCoHandle = StartCoroutine(AutoReconnectCo());
    }

    private void StopReconnectCo()
    {
        if (tokenSource != null)
            tokenSource.Cancel();

        if (autoReconnectCoHandle != null)
            StopCoroutine(autoReconnectCoHandle);
    }

    public float delayConnect = 3;
    private IEnumerator AutoReconnectCo()
    {
        while (true)
        {
            print("연결 시도 시작");
            tokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;
            StartCoroutine(CancelTokenCo());

            Task task = connection.StartAsync(cancellationToken);
            yield return new WaitUntil(() => task.IsCompleted);

            if (connection.State == HubConnectionState.Connected)
            {
                GetComponent<GameSimulator>().RequestLogin();
                tokenSource = null;
                break;
            }
        }
    }

    private IEnumerator CancelTokenCo()
    {
        yield return new WaitForSeconds(delayConnect);

        if (tokenSource != null)
            tokenSource.Cancel();
    }

    void OnReceiveMessage(Command command, string jsonStr)
    {
        print($"받음 {command}:{jsonStr}");
        lock (mainThreadFn)
        { 
            mainThreadFn.Add(() =>
            {
                gameSimulator.OnReceiveCommand(command, jsonStr);
            });
        }
    }

    public string message = "Hello!";

    public void SendToServer(RequestMsg request)
    {
        request.userID = UserData.Instance.userinfo.id;
        string json = JsonConvert.SerializeObject(request);
        print($"보내기 {request.command}:{json}");
        connection.InvokeAsync("SeverReceiveMessage", request.command, json);
    }

    List<Action> mainThreadFn = new List<Action>();

    HubConnectionState previousConnectState = HubConnectionState.Disconnected;
    private void Update()
    {
        if (previousConnectState != connection.State)
        {
            previousConnectState = connection.State;
            Camera.main.backgroundColor = previousConnectState == HubConnectionState.Connected ? Color.green : Color.red;
        }

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
