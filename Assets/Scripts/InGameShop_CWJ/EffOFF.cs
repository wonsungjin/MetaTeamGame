using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffOFF : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(COR_OFF());
    }



    IEnumerator COR_OFF()
    {
        yield return new WaitForSeconds(0.3f);
        GameMGR.Instance.objectPool.DestroyPrefab(transform.gameObject);
    }
}
