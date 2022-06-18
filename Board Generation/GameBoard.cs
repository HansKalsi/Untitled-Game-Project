using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public GameBoardGenerator generator;
    public Material uncontrolledMaterial;
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;

    public int width;
    public int height;

    private GameBoardTile[,] boardPieces;
    public GameBoardTile[,] BoardPieces { get { return boardPieces; } }

    public void GenerateGameBoard(ref List<Tile> tiles)
    {
        int tracker = 0;
        boardPieces = new GameBoardTile[width, height];
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                boardPieces[x, y] = generator.GenerateTile(x, y, uncontrolledMaterial, ref tiles, tracker);
                tracker += 1;
            }
        }
    }

    public void UpdateBoard(ref List<Tile> tiles) {
        int tracker = 0;
        foreach (GameBoardTile bP in boardPieces)
        {
            switch (tiles[tracker].owner)
            {
                case "Red":
                    bP.GetComponent<Renderer>().material = redMaterial;
                    break;
                case "Green":
                    bP.GetComponent<Renderer>().material = greenMaterial;
                    break;
                case "Blue":
                    bP.GetComponent<Renderer>().material = blueMaterial;
                    break;
                default:
                    bP.GetComponent<Renderer>().material = uncontrolledMaterial;
                    break;
            }
            tracker += 1;
        }
    }
}
