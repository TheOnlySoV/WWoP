using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeUtil : MonoBehaviour
{
    public TextMeshProUGUI timeUI;

    public bool militaryTime = false;

    public void Awake()
    {
        timeUI = GetComponent<TextMeshProUGUI>();

        InvokeRepeating("UpdateTimeUI", 0f, .1f);
    }

    void UpdateTimeUI()
    {
        int hour = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;

        if (!militaryTime)
        {
            string tod = hour > 12 ? "PM" : "AM";
            if (hour > 13)
                hour -= 12;
            timeUI.text = $"{hour.ToString().PadLeft(2, '0')}:{minute.ToString().PadLeft(2, '0')} {tod}";
        } else {
            timeUI.text = $"{hour.ToString().PadLeft(2, '0')}{minute.ToString().PadLeft(2, '0')}";
        }
    }
}
