using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace blazor.DB { }

public partial class Userinfo
{
    public int Id { get; set; }
    public int Gold { get; set; }
    public int Highscore { get; set; }
}

public partial class Account
{
    public int Id { get; set; }
    public string Deviceid { get; set; }
    public DateTime Lastlogin { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime ModificationTime { get; set; }
}