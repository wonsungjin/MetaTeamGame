using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse2D : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        MouseRay();
    }
    void MouseRay()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f);

        if(hit.collider != null)
        {
            if(hit.collider.CompareTag("Monster"))
            {
                Debug.Log("마우스로 찍음");
            }
        }
    }

}
