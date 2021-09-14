using System;
using System.Collections;
using System.Collections.Generic;


#if UNITY_5_3_OR_NEWER
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
}

public class RequestLogin
{
    public string deviceID;
}
public class ResultLogin
{
    [JsonInclude]
    public int id;
    [JsonInclude] public int gold;
}
