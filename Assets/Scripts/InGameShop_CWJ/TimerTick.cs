using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TimerTick : MonoBehaviour
{
    public Sprite[] images; // array of sprites representing the images in the sequence
    private int currentImageIndex = 0; // index of the current image in the sequence


    private void Start()
    {
        StartCoroutine(ShowImages());
    }

    private Sprite nextImage()
    {
        // Get the next image in the sequence
        Sprite nextSprite = images[currentImageIndex];

        // Increment the index, wrapping around to the beginning of the array if necessary
        currentImageIndex = (currentImageIndex + 1) % images.Length;

        return nextSprite;
    }


    IEnumerator ShowImages()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        while (true)
        {
            // Switch to the next image in the sequence
            renderer.sprite = nextImage();

            // Wait for 0.1 seconds
            yield return new WaitForSeconds(0.1f);
        }
    }
}
