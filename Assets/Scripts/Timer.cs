using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float time = 5f;
    public bool isRuning = false;
    [SerializeField] private TextMeshProUGUI timeText;
    // Start is called before the first frame update
    void Start()
    {
        isRuning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRuning)
        {
            if (time > 0)
            {
                time = time - Time.deltaTime;
            }
            else
            {
                isRuning = false;
                time = 0;
            }
        }

        timeText.text = Mathf.FloorToInt((time / 60)).ToString("00") + ":" + Mathf.FloorToInt((time % 60)).ToString("00");
    }

    public void RestartTimer(float t)
    {
        time = t;
        isRuning = true;
    }
}
