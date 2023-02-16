using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTick : MonoBehaviour
{
    public float rotateSpeed = 30f;
    public float rotateAngle = 5f;
    private float startingAngle;
    private float targetAngle;
    private bool rotateRight = true;

    void Start()
    {
        startingAngle = transform.eulerAngles.z;
        targetAngle = startingAngle + rotateAngle;
    }

    void Update()
    {
        float currentAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotateSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, currentAngle);

        if (currentAngle == targetAngle)
        {
            if (rotateRight)
            {
                targetAngle = startingAngle - rotateAngle;
            }
            else
            {
                targetAngle = startingAngle + rotateAngle;
            }

            rotateRight = !rotateRight;
        }

        if (GameMGR.Instance.uiManager.isTimerFast == true)
        {
            TimerTickFaster();
        }
    }

    void TimerTickFaster()
    {
        rotateSpeed = 50f;
    }
}
