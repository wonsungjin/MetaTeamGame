using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignUp : MonoBehaviour
{
    GameObject signUp = null;
    Button signUpButton = null;

    void Awake()
    {
        signUp = GameObject.Find("SingUpButton");
        signUpButton = signUp.GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ButtonAddListener()
    {
        // signUpButton.onClick.AddListener();
    }
    
    
}
