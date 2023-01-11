using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse2D : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public RaycastHit2D hit;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        MouseRay();
    }
    void MouseRay()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f);


        if(hit.collider != null)
        {
            if(hit.collider.CompareTag("Monster"))
            {
                hit.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }

}
