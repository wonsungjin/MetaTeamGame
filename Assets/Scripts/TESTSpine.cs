using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTSpine : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation = null;

    private void Start()
    {
        skeletonAnimation = gameObject.GetComponent<SkeletonAnimation>();
    }

    void TestAnim()
    {
        
    }    
}
