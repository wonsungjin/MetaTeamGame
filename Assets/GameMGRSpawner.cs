using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMGRSpawner : MonoBehaviour
{
   [SerializeField]  GameObject GMR = null;
    private void Awake()
    {
        GameObject obj =  Instantiate(GMR);
        Singleton<GameMGR>.Instance = obj.GetComponent<GameMGR>();

    }
}
