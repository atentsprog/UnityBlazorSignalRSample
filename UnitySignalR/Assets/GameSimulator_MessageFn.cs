using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameSimulator : MonoBehaviour
{
    #region �α���
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
    #endregion �α���

    #region �г��� ��ü
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

        print($"{result.newNickname}���� �̸� ������ �Ϸ�Ǿ����ϴ�.");
    }
    #endregion �г��� ��ü
}
