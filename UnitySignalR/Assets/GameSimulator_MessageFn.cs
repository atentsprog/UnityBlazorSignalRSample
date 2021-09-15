using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameSimulator : MonoBehaviour
{
    #region 로그인
    private void RequestLogin()
    {
        commandHub.Login();
    }
    private void ResultLogin(string jsonStr)
    {
        ResultLogin resultLogin = JsonConvert.DeserializeObject<ResultLogin>(jsonStr);
        print(resultLogin.userinfo.Gold);
        UserData.Instance.userinfo = resultLogin.userinfo;
        UserData.Instance.account = resultLogin.account;
    }
    #endregion 로그인

    #region 닉네임 교체
    void RequestChangeNickname()
    {
        RequestChangeNickname request = new RequestChangeNickname();
        request.newNickname = input1.text;
        SendToServer(request);
    }

    void ResultChangeNickname(string jsonStr)
    {
        ResultChangeNickname result = JsonConvert.DeserializeObject<ResultChangeNickname>(jsonStr);

        if (ShowErrorCodeIfNotSucceed(result.result))
            return;

        print($"{result.newNickname}으로 이름 변경이 완료되었습니다.");
    }
    #endregion 닉네임 교체
}
