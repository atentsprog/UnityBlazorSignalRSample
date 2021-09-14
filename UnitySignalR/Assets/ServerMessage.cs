using ShareClientAndServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ServerMessage : MonoBehaviour
{
    List<Action> mainThreadFn = new List<Action>();
    ClientCommandHub connection;
    private void Awake()
    {
        connection = GetComponent<ClientCommandHub>();
    }
    public void OnReceiveCommand(Command command, string resultJson)
    {
        Debug.Log($"{command}: {resultJson}");


        mainThreadFn.Add(
                () => ExecuteCommand(command, resultJson)
            );
    }
    private void Update()
    {
        lock (mainThreadFn)
        {
            foreach (var item in mainThreadFn)
                item();
            mainThreadFn.Clear();
        }
    }

    private void ExecuteCommand(Command command, string resultJson)
    {
        switch (command)
        {
            case Command.LoginResult: OnLoginResult(resultJson); break;
        }
    }

    public async Task SendLoginRequest()
    {
        LoginRequestMsg request = new LoginRequestMsg();
        request.deviceID = UserInfo.DeviceID;
        await connection.SendToServer(request);
    }

    private void OnLoginResult(string resultJson)
    {
        LoginResultMsg result = JsonUtility.FromJson<LoginResultMsg>(resultJson);
        print(result.gold);
    }
}