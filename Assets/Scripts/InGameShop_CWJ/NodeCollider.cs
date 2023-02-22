using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCollider : MonoBehaviour
{
   [SerializeField] GameObject[] node = null;
    Collider2D[] nodeColl = null;

    private void Start()
    {
        nodeColl = new Collider2D[node.Length];

        for (int i = 0; i < node.Length; i++)
        {
            nodeColl[i] = node[i].GetComponent<Collider2D>();
        }

        for (int i = 0; i < nodeColl.Length; i++)
        {
            nodeColl[i].enabled = false;
        }
    }

    public void NodeCollOn()
    {
        if (GameMGR.Instance.uiManager.shopLevel == 3)
            nodeColl[0].enabled = true;
        else if (GameMGR.Instance.uiManager.shopLevel == 4)
            nodeColl[1].enabled = true;
        else if (GameMGR.Instance.uiManager.shopLevel == 5)
            nodeColl[2].enabled = true;
    }
}
