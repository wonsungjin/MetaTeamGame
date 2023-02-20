using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TimerTick : MonoBehaviour
{
    public Sprite[] sprites; // 이미지 배열
    public Image image; // 이미지 컴포넌트
    WaitForSeconds waittimer = new WaitForSeconds(0.3f);

    private int currentSprite = 0; // 현재 이미지 인덱스

    void Start()
    {
        StartCoroutine(AnimateSprite()); // 코루틴 시작
    }

    IEnumerator AnimateSprite()
    {
        while (true)
        {
            image.sprite = sprites[currentSprite]; // 이미지 변경
            currentSprite++; // 인덱스 증가

            if (currentSprite >= sprites.Length)
            {
                currentSprite = 0; // 배열 끝에 도달하면 처음으로 돌아감
            }

            if (GameMGR.Instance.uiManager.isTimerFast == false)
            {
                Debug.Log("타이머 슬로우");
                yield return waittimer; // 0.3초 대기
            }

            else if (GameMGR.Instance.uiManager.isTimerFast == true)
            {
                Debug.Log(GameMGR.Instance.uiManager.isTimerFast);
                Debug.Log("타이머 패스트");
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
