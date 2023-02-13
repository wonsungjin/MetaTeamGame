using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackSound : MonoBehaviour
{
    private void OnMouseUp()
    {
        // 빈곳 누를시 나오는 소리
        if (gameObject.CompareTag("BackImage"))
        {
            GameMGR.Instance.audioMGR.SoundMouseClick();
            Debug.Log("클릭으로 들어옿ㅁ");
        }
    }
}
