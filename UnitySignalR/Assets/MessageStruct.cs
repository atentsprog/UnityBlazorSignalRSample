using System.Collections;
using System.Collections.Generic;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
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
}

public class RequestLogin
{
    public string deviceID;
}
public class ResultLogin
{
    public int id;
    public int gold;
}
