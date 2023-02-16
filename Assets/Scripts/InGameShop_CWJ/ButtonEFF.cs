using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonEFF : MonoBehaviour
{
   

    private void OnMouseDown()
    {
        gameObject.transform.localScale = new Vector3(5.4f,5.4f,5.4f);
    }
    private void OnMouseUp()
    {
        gameObject.transform.localScale = new Vector3(6.02f, 6.02f, 6.02f);
    }
}
