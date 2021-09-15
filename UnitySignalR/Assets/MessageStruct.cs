using blazor.DB;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_5_3_OR_NEWER
using Newtonsoft.Json;
using UnityEngine;
internal class JsonIncludeAttribute : Attribute
{
}
#else
using System.Text.Json.Serialization;
#endif

public enum Command
{
    None,
    // �α���.
    RequestLogin        = 1,
    ResultLogin         = 2,

    // �������� ����/Ŭ����
    RequestStageEnter   = 10,
    ResultStageEnter    = 11,
    RequestStageClear   = 12,
    ResultStageClear    = 13,

    // ������ ����
    RequestBuyItem      = 20,
    ResultBuyItem       = 21,

    // ���� ��û
    RequestReward       = 30,
    ResultReward        = 31,
}

public enum ErrorCode
{
    Succeed = 0, // ����(���� ����

    Invaild_Reward_Type,    // ��ȿ���� ���� ���� Ÿ��.
}

[Serializable]
public class MsgHeader
{
    public MsgHeader(Command _command)
    {
        command = _command;
    }
    [JsonInclude]
    public Command command;
}
[Serializable]
public class RequestMsg : MsgHeader
{
    [JsonInclude]
    public int userID;
    public RequestMsg(Command _command) : base(_command) { }
}

[Serializable]
public class ResultMsg : MsgHeader
{
    public ResultMsg(Command command) : base(command) { }
    [JsonInclude]
    public ErrorCode result;
}

public class RequestLogin : RequestMsg
{
    [JsonInclude]
    public string deviceID;

    public RequestLogin() : base(Command.RequestLogin){}
}


public class ResultLogin : ResultMsg
{
    public ResultLogin() : base(Command.ResultLogin) { }
    [JsonInclude]
    public Account account;
    [JsonInclude]
    public Userinfo userinfo;
}

class RequestReward : RequestMsg
{
    [JsonInclude]
    public string rewardType;
    public RequestReward() : base(Command.RequestReward) { }
}

public class ResultReward : ResultMsg
{
    public ResultReward() : base(Command.ResultReward) { }
    [JsonInclude]
    public int rewardGold; // �̹��� �߰��Ȱ��.
    [JsonInclude]
    public int currentGold; // rewardGold�� ��ģ ���
}