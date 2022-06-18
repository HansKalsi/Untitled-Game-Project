using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BoardTileClick : MonoBehaviour
{
    GameObject popUp;
    GameObject formDataREF;
    Tile tileREF;

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Board_Piece")
                {
                    popUp.SetActive(true);
                    formDataREF.GetComponent<TMPro.TextMeshProUGUI>().text = logTile();
                }

            }

        }
    }

    string logTile()
    {
        string msg = "Owner: " + tileREF.owner + "\n";
        foreach (Pop p in tileREF.population)
        {
            msg += p.logDetails();
        }
        foreach (Building b in tileREF.buildings)
        {
            msg += b.logDetails();
        }
        foreach (Resource r in tileREF.resources)
        {
            msg += r.logDetails();
        }
        return msg;
    }

    public void BoardTileClickSetUp(GameObject p, GameObject fD, ref List<Tile> t, int tCount)
    {
        popUp = p;
        formDataREF = fD;
        tileREF = t[tCount];
    }
}