using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackSound : MonoBehaviour
{
    Camera mainCam = null;
    float distance = 10;

    private void Start()
    {
        mainCam = Camera.main;
    }
    private void OnMouseUp()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = mainCam.ScreenToWorldPoint(mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(objPosition, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("BattleZone")
                || hit.collider.gameObject.CompareTag("Sell") || hit.collider.gameObject.CompareTag("SelectRing")
                || hit.collider.gameObject.CompareTag("ShopLevelUp") || hit.collider.gameObject.CompareTag("FullZone")
                || hit.collider.gameObject.CompareTag("SpecialZone") || hit.collider.gameObject.CompareTag("RefreshButton")
                || hit.collider.gameObject.CompareTag("Rect") || hit.collider.gameObject.CompareTag("BattleZone"))
            {
                return;
            }

            if (hit.collider.gameObject.CompareTag("Monster") || hit.collider.gameObject.CompareTag("BattleMonster") || hit.collider.gameObject.CompareTag("BattleMonster2")
                || hit.collider.gameObject.CompareTag("BattleMonster3") || hit.collider.gameObject.CompareTag("FreezeCard"))
            {
                GameMGR.Instance.audioMGR.SoundMonsterClick();
            }

            if (hit.collider.gameObject.CompareTag("Option") || hit.collider.gameObject.CompareTag("AllButton"))
            {
                GameMGR.Instance.audioMGR.SoundButton();
            }

            // 빈곳 누를시 나오는 소리
            if (hit.collider.gameObject.CompareTag("BackImage"))
            {
                GameMGR.Instance.audioMGR.SoundMouseClick();
            }
        }
    }
}
