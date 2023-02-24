using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private GameObject selectedObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                selectedObject = hit.collider.transform.parent.gameObject;
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (selectedObject != null && (selectedObject == gameObject || col.gameObject == gameObject))
        {
            Destroy(col.gameObject);
            // 다른 게임오브젝트에 삭제된 게임오브젝트의 정보를 넘겨주는 코드
        }
    }
}