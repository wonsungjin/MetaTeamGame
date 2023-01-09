using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] CardInfo cardInfo;
    private Image image;
    private bool isTouch;

    /*자신의 오브젝트 이름과 같은 스크립터블 데이터를 읽어와서 설정한다
    스프라이트 랜더러도 같은 원리로 설정*/
    private void Start()
    {

    }
    public void SetMyInfo(string myname)
    {
        name = myname;
        cardInfo = Resources.Load<CardInfo>($"ScriptableDBs/{name}");
        image = GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>($"Sprites/{cardInfo.objName}");
    }

    /*커스텀덱 설정할 때 클릭하면 실행되는 함수*/ 
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameMGR.Instance.customDeckShop.isJoinShop)
        {
            if (GameMGR.Instance.customDeckShop.AddTierList(cardInfo.tier, cardInfo.objName))
            {
                if (isTouch)
                {
                    isTouch = false;
                    image.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    isTouch = true;
                    image.color = new Color(0.3f, 0.3f, 0.3f, 1);
                }
            }
            else Debug.Log("8마리 넘음");
        }
    }                      

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }
}
