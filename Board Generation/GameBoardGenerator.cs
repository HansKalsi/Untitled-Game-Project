using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardGenerator : MonoBehaviour
{
    public GameObject prefab;
    public GameObject popUpObject;
    public GameObject formDataREFObject;

    public GameBoardTile GenerateTile(int x, int y, Material m, ref List<Tile> t, int tCount)
    {
        GameObject spawnedTile = Instantiate(prefab);
        var boardTile = spawnedTile.GetComponent<GameBoardTile>();
        if (boardTile == null)
        {
            boardTile = spawnedTile.AddComponent<GameBoardTile>();
        }
        var boardTileClickEvent = spawnedTile.GetComponent<BoardTileClick>();
        if (boardTileClickEvent == null)
        {
            boardTileClickEvent = spawnedTile.AddComponent<BoardTileClick>();
        }
        boardTileClickEvent.BoardTileClickSetUp(popUpObject, formDataREFObject, ref t, tCount);

        boardTile.x = x;
        boardTile.y = y;
        boardTile.updateMaterial(m);

        return boardTile;
    }
}
