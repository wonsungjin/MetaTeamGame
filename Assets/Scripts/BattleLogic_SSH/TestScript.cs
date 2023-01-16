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

        player[0] = null;

        Debug.Log(player.Count);
        Debug.Log(player[0] + "player[0]");

        if (player[0] == null)
        {
            Debug.Log("Null!!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
