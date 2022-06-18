using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void toggleActive()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
