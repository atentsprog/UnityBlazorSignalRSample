using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

#if UNITY_5_6_OR_NEWER
namespace ShareClientAndServer
{
    internal class JsonIncludeAttribute : Attribute
    {
    }
}
#else
using System.Text.Json.Serialization;
#endif

namespace ShareClientAndServer
{
    public enum Command
    {
        None,
        LoginRequest,
        LoginResult,
    }
    public enum ResultCode
    {
        Succeed,
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
        public RequestMsg(Command _command) : base(_command) {}
    }

    [Serializable]
    public class ResultMsg : MsgHeader
    {
        public ResultMsg(Command command) : base(command) { }
        [JsonInclude]
        public ResultCode result;
    }
    [Serializable]
    public class LoginRequestMsg : RequestMsg
    {
        public LoginRequestMsg() : base(Command.LoginRequest) { }
        [JsonInclude]
        public string deviceID;
    }


    [Serializable]
    public class LoginResultMsg : ResultMsg
    {
        [JsonInclude]
        public int gold;
        public LoginResultMsg() : base(Command.LoginResult) { }
    }
}
