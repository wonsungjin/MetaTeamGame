using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] List<GameObject> player = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(player.Count);

        player.Remove(player[0]);

        Debug.Log(player.Count);
        Debug.Log(player[0] + "player[0]");

        if (player[1] == null)
        {
            Debug.Log("Null!!");
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    



}
