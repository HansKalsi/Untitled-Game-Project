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

    public void Construct()
    {
        week = 1;
        year = 0;
        WeekGO.GetComponent<TMPro.TextMeshProUGUI>().text = week.ToString();
        YearGO.GetComponent<TMPro.TextMeshProUGUI>().text = year.ToString();
    }

    public void advanceWeek()
    {
        if (week == 52)
        {
            advanceYear();
            week = 0;
        }
        week += 1;
        WeekGO.GetComponent<TMPro.TextMeshProUGUI>().text = week.ToString();
        YearGO.GetComponent<TMPro.TextMeshProUGUI>().text = year.ToString();
    }

    void advanceYear()
    {
        year += 1;
    }
}
