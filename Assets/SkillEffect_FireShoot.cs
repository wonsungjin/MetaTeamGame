using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect_FireShoot : MonoBehaviour
{
    public float aliveTime = 0;


    private void OnDisable()
    {
        aliveTime = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(aliveTime > 5) GameMGR.Instance.objectPool.DestroyPrefab(gameObject);
        transform.Translate(0, -0.2f, 0);
        aliveTime += 0.2f;
    }
}
