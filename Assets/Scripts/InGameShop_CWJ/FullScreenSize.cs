using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class FullScreenSize : MonoBehaviour
{


    public AspectRatioFitter fit;

    // Update is called once per frame
    void Update()
    {
        float ratio = (float)Screen.width / (float)Screen.height;
        fit.aspectRatio = ratio;
    }
}
