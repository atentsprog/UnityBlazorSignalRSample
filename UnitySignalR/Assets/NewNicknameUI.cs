using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewNicknameUI : MonoBehaviour
{
    public Button button;
    public InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        GetComponentInParent<GameSimulator>().RequestChangeNickname(inputField.text);
        gameObject.SetActive(false);
    }
}
