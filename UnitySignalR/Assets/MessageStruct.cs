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
    // 로그인.
    RequestLogin        = 1,
    ResultLogin         = 2,

    // 스테이지 엔터/클리어
    RequestStageEnter   = 10,
    ResultStageEnter    = 11,
    RequestStageClear   = 12,
    ResultStageClear    = 13,

    // 아이템 구입
    RequestBuyItem      = 20,
    ResultBuyItem       = 21,

    // 닉네임 교체
    RequestChangeNickname = 30,
    ResultChangeNickname = 31,
}
public enum ErrorCode
{
    Succeed = 0,

    AlreadyExist,   // 이미 존재합니다
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
public class RequestChangeNickname : RequestMsg
{
    [JsonInclude]
    public string newNickname;

    public RequestChangeNickname() : base(Command.RequestChangeNickname) { }
}


public class ResultChangeNickname : ResultMsg
{
    public ResultChangeNickname() : base(Command.ResultChangeNickname) { }
    [JsonInclude]
    public string newNickname;
}