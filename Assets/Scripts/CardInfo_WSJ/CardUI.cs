using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CardInfo cardInfo;
    private Image image;
    bool isWhiteLine;

    /*자신의 오브젝트 이름과 같은 스크립터블 데이터를 읽어와서 설정한다
    스프라이트 랜더러도 같은 원리로 설정*/
    public void SetMyInfo(string myname)
    {
        name = myname;
        Debug.Log(name) ;
        cardInfo = Resources.Load<CardInfo>($"ScriptableDBs/{name}");
        image = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        Debug.Log(cardInfo.objName);
        image.sprite = Resources.Load<Sprite>($"Sprites/Nomal/{name}");
    }

    Color FrameColor = new Color(1f, 1f, 1f, 1/255f);
    /*커스텀덱 설정할 때 클릭하면 실행되는 함수*/
    public void OffFrame()
    {
        transform.GetChild(0).GetComponent<Image>().color = FrameColor;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameMGR.Instance.customDeckShop.isJoinShop)
        {
            // Debug.Log(GameMGR.Instance.customDeckShop.AddTierList(cardInfo.tier, cardInfo.objName));
            if (!isWhiteLine)
            {
                isWhiteLine = true;
                image.sprite = Resources.Load<Sprite>($"Sprites/WhiteLine/{name}WhiteLine");

            }
            else if (isWhiteLine)
            {
                isWhiteLine = false;
                image.sprite = Resources.Load<Sprite>($"Sprites/Nomal/{name}");
            } 
            if (GameMGR.Instance.customDeckShop.AddTierList(cardInfo.tier, cardInfo.objName) > 8)
            {
                isWhiteLine = false;
                Debug.Log("8chrhk");
                image.sprite = Resources.Load<Sprite>($"Sprites/Nomal/{name}");
            }
        }
    }
    public void ClearClick()
    {
        isWhiteLine = false;
        image.sprite = Resources.Load<Sprite>($"Sprites/Nomal/{name}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameMGR.Instance.uiManager.OnPointerEnter_CardInfo(cardInfo);
    }
}
