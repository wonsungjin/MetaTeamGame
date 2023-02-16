using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainBroken : MonoBehaviour
{
    WaitForSeconds wait = new WaitForSeconds(0.123f);

    [SerializeField] GameObject[] chain;
    [SerializeField] GameObject[] chainBrok;
    int num;

   public  void ChainsBroken()
    {
        if (GameMGR.Instance.uiManager.shopLevel == 3)
        {
            num = 0;

            StartCoroutine(ChainBro(num));
        }
        if (GameMGR.Instance.uiManager.shopLevel == 4)
        {
            num = 1;

            StartCoroutine(ChainBro(num));
        }
        if (GameMGR.Instance.uiManager.shopLevel == 5)
        {
            num = 2;

            StartCoroutine(ChainBro(num));
        }
    }

    IEnumerator ChainBro(int a)
    {
        chain[a].SetActive(false);
        yield return wait;
        chainBrok[a].SetActive(true);
        yield return new WaitForSeconds(0.3f);
        chainBrok[a].SetActive(false);
    }
}
