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

        connection.On<Command, string>("ClientReceiveMessage", OnReceiveMessage);

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
        lock(mainThreadFn)
        { 
            mainThreadFn.Add(() =>
            {
                onReceiveCommand(command, jsonStr);
                //OnReceiveCommand(command, jsonStr);
            });
        }
    }
    public Action<Command, string> onReceiveCommand;

    public string message = "Hello!";

    public void SendToServer(RequestMsg request)
    {
        request.userID = UserData.Instance.userinfo.id;
        string json = JsonConvert.SerializeObject(request);

        if(connection.State == HubConnectionState.Disconnected)
        {
            _ = ConnectAndSend(request);
            return;
        }

        connection.InvokeAsync("SeverReceiveMessage", request.command, json);
    }

    private async Task ConnectAndSend(RequestMsg request)
    {
        await connection.StartAsync();
        if (connection.State == HubConnectionState.Connected)
        {
            GetComponent<GameSimulator>().RequestLogin();
            //보내지 못했던 명령 전송.
            _ = connection.InvokeAsync("SeverReceiveMessage", request.command, request.ToJson());
        }
    }

    List<Action> mainThreadFn = new List<Action>();
    public Text connectState;
    void SetCoonectStateLog(string log)
    {
        connectState.text = log;
        print(log);
    }
    HubConnectionState lastConnectState = HubConnectionState.Disconnected;
    private void Update()
    {
        if(lastConnectState != connection.State)
        {
            lastConnectState = connection.State;
            SetCoonectStateLog(lastConnectState.ToString());
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
