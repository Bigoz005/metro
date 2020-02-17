using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeClass : MonoBehaviour
{

    public Canvas canvas1, canvas2;
    private Touch touch;
    private Vector2 start, end;
    private int currentCanvas = 1;
    private int detector = 1;

    public void Start()
    {
        canvas1.enabled = true;
        canvas2.enabled = false;
    }

    public void Update()
    {
        DetectSwap();
    }

    private void DetectSwap()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    start = touch.position;
                    break;

                case TouchPhase.Ended:
                    end = touch.position;

                    Swipe();
                    break;
            }
        }
        else if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                start = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                end = Input.mousePosition;
                Swipe();
            }
        }
    }

    private void Swipe()
    {
        if ((start.x - end.x) >= 100 || (start.x - end.x) <= -100)
        {
            detector++;

            if (start.x < end.x)
            {
                currentCanvas--;
                if (currentCanvas < 1) currentCanvas = 2;
            }
            else
            {
                currentCanvas++;
                if (currentCanvas > 2) currentCanvas = 1;
            }

            if (currentCanvas == 1)
            {
                canvas1.enabled = false;
                canvas2.enabled = true;
            }

            if (currentCanvas == 2)
            {
                canvas1.enabled = true;
                canvas2.enabled = false;
            }
        }
    }
}
