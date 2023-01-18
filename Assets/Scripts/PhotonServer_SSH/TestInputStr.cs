using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestInputStr : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI idText = null;

    [SerializeField]
    TextMeshProUGUI pwText = null;


    public void InputText(string inStrID, string inStrPW)
    {
        idText.text = inStrID;
        pwText.text = inStrPW;
    }
}
