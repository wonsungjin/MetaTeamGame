using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEFF : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 normalScale;    // The normal scale of the button
    private Vector3 pressedScale;   // The scale of the button when pressed
    private float scaleRatio = 0.9f;    // The ratio of the pressed scale to the normal scale
    private Button button;  // The button component
    void Start()
    {
        // Get the normal scale of the button
        normalScale = transform.localScale;

        // Calculate the pressed scale of the button
        pressedScale = normalScale * scaleRatio;

        // Get the button component
        button = GetComponent<Button>();
    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        // Set the scale of the button to the pressed scale when it is pressed
        transform.localScale = pressedScale;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        // Set the scale of the button back to the normal scale when it is released
        transform.localScale = normalScale;
    }
}
