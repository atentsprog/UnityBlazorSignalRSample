using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace blazor.DB { }

[Serializable]
public partial class Userinfo
{
    public int id ;
    public int gold ;
    public int highscore;
    public string nickname;
    public string lastChatGroup;
}

[Serializable]
public partial class Account
{
    public int id ;
    public string deviceid ;
    public DateTime lastlogin ;
    public DateTime createTime ;
    public DateTime modificationTime ;
}