using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class BlockScroll : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    private bool isClick;
    private Image image;

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("나감");
        //image.raycastTarget = true;
    }
    WaitForSeconds delay = new WaitForSeconds(0.5f);
    IEnumerator COR_DoubleClick()
    {
        if(isClick==false) isClick = true;
        else
        {
            Debug.Log("더블클릭");
            image.raycastTarget = false;
            
            yield break;
        }
        yield return delay;
        isClick = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("클릭");
        StartCoroutine(COR_DoubleClick());

    }
}
