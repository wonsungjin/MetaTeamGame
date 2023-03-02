using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DestroySelf : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(COR_Remove());
        
    }

    IEnumerator COR_Remove()
    {
        yield return new WaitForSeconds(1f);
        GameMGR.Instance.objectPool.DestroyPrefab(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
