using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardTile : MonoBehaviour
{
    public int x;
    public int y;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = Vector3.right * x + Vector3.up * y;
    }

    public void updateMaterial(Material m) {
        GetComponent<Renderer>().material = m;
    }
}
