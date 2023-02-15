using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTick : MonoBehaviour
{

    float tickTime = 0.6f;
    Quaternion leftTick = new Quaternion(0, 0, -15, 0);
    Quaternion rightTick = new Quaternion(0, 0, 15, 0);
    bool tickEnd = false;

    private void Start()
    {
        TickTime();
    }

    void TickTime()
    {
        if(tickEnd == false)
        {
            transform.rotation = Quaternion.Slerp(Quaternion.identity, rightTick, tickTime);
            tickEnd = true;

            TickTime();
        }

        if(tickEnd== true)
        {
            transform.rotation = Quaternion.Slerp(Quaternion.identity, leftTick, tickTime);
            tickEnd = true;

            TickTime();
        }
    }
}
