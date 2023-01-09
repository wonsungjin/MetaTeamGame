using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    [SerializeField] CardInfo cardInfo;
    private SpriteRenderer spriteRenderer;
    private bool isTouch;


    /*자신의 오브젝트 이름과 같은 스크립터블 데이터를 읽어와서 설정한다
    스프라이트 랜더러도 같은 원리로 설정*/
    public void SetMyInfo(string myname)
    {
        name = myname;
        cardInfo = Resources.Load<CardInfo>($"ScriptableDBs/{name}");
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log(Resources.Load<Sprite>($"Sprites/{cardInfo.objName}"));
        Debug.Log(cardInfo.objName);
        spriteRenderer.sprite = Resources.Load<Sprite>($"Sprites/{cardInfo.objName}");
    }

    /*커스텀덱 설정할 때 클릭하면 실행되는 함수*/ 
    public void OnMouseDown()
    {
        if (GameMGR.Instance.customDeckShop.isJoinShop)
        {
            if (GameMGR.Instance.customDeckShop.AddTierList(cardInfo.tier, cardInfo.objName))
            {
                if (isTouch)
                {
                    isTouch = false;
                    spriteRenderer.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    isTouch = true;
                    spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 1);
                }
            }
            else Debug.Log("8마리 넘음");
        }
    }

}
