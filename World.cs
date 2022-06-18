using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class World : MonoBehaviour
{
    public GameObject WeekGO;
    public GameObject YearGO;
    public int week;
    public int year;
    public bool paused;

    public void Construct()
    {
        paused = true;
        week = 1;
        year = 0;
        updateUI();
    }

    public bool AdvanceWeek()
    {
        if (!paused)
        {
            if (week == 52)
            {
                advanceYear();
                week = 0;
            }
            week += 1;
            updateUI();
            return true;
        }
        return false;
    }

    void advanceYear()
    {
        year += 1;
    }

    void updateUI()
    {
        WeekGO.GetComponent<TMPro.TextMeshProUGUI>().text = week.ToString();
        YearGO.GetComponent<TMPro.TextMeshProUGUI>().text = year.ToString();
    }

    public void playButton()
    {
        paused = !paused;
    }
}
