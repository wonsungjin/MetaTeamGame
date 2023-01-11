using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] CardInfo cardInfo;
    private Image image;

    /*자신의 오브젝트 이름과 같은 스크립터블 데이터를 읽어와서 설정한다
    스프라이트 랜더러도 같은 원리로 설정*/
    public void SetMyInfo(string myname)
    {
        name = myname;
        cardInfo = Resources.Load<CardInfo>($"ScriptableDBs/{name}");
        image = GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>($"Sprites/{cardInfo.objName}");
    }

    Color color = new Color(0.3f, 0.3f, 0.3f, 1);
    Color resetColor = new Color(1f, 1f, 1f, 1);
    /*커스텀덱 설정할 때 클릭하면 실행되는 함수*/
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameMGR.Instance.customDeckShop.isJoinShop)
        {
            // Debug.Log(GameMGR.Instance.customDeckShop.AddTierList(cardInfo.tier, cardInfo.objName));
            if (image.color.g == 1f)
            {
                image.color = color;
                
            }
            else if (image.color.g == 0.3f)
            {
                image.color = resetColor;
            } 
            if (GameMGR.Instance.customDeckShop.AddTierList(cardInfo.tier, cardInfo.objName) > 8)
            {
                Debug.Log("8chrhk");
                image.color = resetColor;
            }
        }
    }
    public void ClearClick()
    {
        image.color = resetColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameMGR.Instance.uIMGR.OnPointerEnter_CardInfo(cardInfo);
    }
}
