using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoardInfo
{
    public int col;
    public int row;
}



public class GameBoard : MonoBehaviour
{

    public List<HashSet<Vector2>> winningSets = new List<HashSet<Vector2>>();

    public int emptySpaces;
    private float _tileOffset = 1.6f;
    public TileSpace[,] gameBoard;
    public BoardInfo info;

    [SerializeField] public TileSpace tile;

    private TaskManager _tm = new TaskManager();

    public void EstablishWinningSets()
    {
        // VERTICAL
        HashSet<Vector2> vert1 = new HashSet<Vector2>();
        vert1.Add(new Vector2(0, 0));
        vert1.Add(new Vector2(0, 1));
        vert1.Add(new Vector2(0, 2));

        HashSet<Vector2> vert2 = new HashSet<Vector2>();
        vert2.Add(new Vector2(1, 0));
        vert2.Add(new Vector2(1, 1));
        vert2.Add(new Vector2(1, 2));

        HashSet<Vector2> vert3 = new HashSet<Vector2>();
        vert3.Add(new Vector2(2, 0));
        vert3.Add(new Vector2(2, 1));
        vert3.Add(new Vector2(2, 2));

        // HORIZONTAL
        HashSet<Vector2> hori1 = new HashSet<Vector2>();
        hori1.Add(new Vector2(0, 0));
        hori1.Add(new Vector2(1, 0));
        hori1.Add(new Vector2(2, 0));

        HashSet<Vector2> hori2 = new HashSet<Vector2>();
        hori2.Add(new Vector2(0, 1));
        hori2.Add(new Vector2(1, 1));
        hori2.Add(new Vector2(2, 1));

        HashSet<Vector2> hori3 = new HashSet<Vector2>();
        hori3.Add(new Vector2(0, 2));
        hori3.Add(new Vector2(1, 2));
        hori3.Add(new Vector2(2, 2));

        // DIAGONAL
        HashSet<Vector2> diag1 = new HashSet<Vector2>();
        diag1.Add(new Vector2(0, 0));
        diag1.Add(new Vector2(1, 1));
        diag1.Add(new Vector2(2, 2));

        HashSet<Vector2> diag2 = new HashSet<Vector2>();
        diag2.Add(new Vector2(2, 0));
        diag2.Add(new Vector2(1, 1));
        diag2.Add(new Vector2(0, 2));

        winningSets.Add(vert1);
        winningSets.Add(vert2);
        winningSets.Add(vert3);

        winningSets.Add(hori1);
        winningSets.Add(hori2);
        winningSets.Add(hori3);

        winningSets.Add(diag1);
        winningSets.Add(diag2);
    }

    public void Init(BoardInfo i)
    {
        info = i;
        gameBoard = new TileSpace[info.col, info.row];
        emptySpaces = info.col * info.row;
        int tileNum = 0;
        for (int x = 0; x < info.col; x++)
        {
            for (int y = 0; y < info.row; y++)
            {
                float xPos = x * _tileOffset;
                float yPos = y * _tileOffset;
                Vector3 tilePos = new Vector3(xPos, yPos, 0);
                TileSpace tile = Instantiate(Services.Prefabs.Tile, tilePos, Quaternion.identity, transform);
                tile.name = "TileSpace X: [" + x + ", " + y + "]";
                tile.Init(x, y, tileNum);
                gameBoard[x, y] = tile;
                tile.gameObject.SetActive(false);
                tileNum++;
            }
        }

        EstablishWinningSets();
        Services.EventManager.Register<PlayMadeEvent>(OnPlayMade);

        Task entryAnimation = new BoardEntryAnimation(true);
        _tm.Do(entryAnimation);
    }

    private void OnDestroy() 
    {
        Services.EventManager.Unregister<PlayMadeEvent>(OnPlayMade);

    }

    public void BoardExitAnimation()
    {

        Task entryAnimation = new BoardEntryAnimation();
        _tm.Do(entryAnimation);
    }

    public void OnPlayMade(PlayMadeEvent e)
    {
        emptySpaces--;
        if(CheckForWin(e.tileSpace.coord))
        {
            Services.EventManager.Fire(new GameEndEvent(e.player));
        }
        else if(AllSpacesOccupied())
        {
            Services.EventManager.Fire(new GameEndEvent(null));
        }
        else
        {

        }
 
    }

    public bool AllSpacesOccupied()
    {
        return emptySpaces == 0;
    }
    public bool CheckForWin(Vector2 coord)
    {
        List<HashSet<Vector2>> candidateSets = new List<HashSet<Vector2>>();

        foreach(HashSet<Vector2> set in winningSets)
        {
            if(set.Contains(coord))
            {
                candidateSets.Add(set);
            }
        }

        foreach(HashSet<Vector2> set in candidateSets)
        {
            if(CheckSet(set, coord))
                return true;
            else
                continue;
        }

        return false;
    }

    private bool CheckSet(HashSet<Vector2> set, Vector2 candidteCoord)
    {
        Player occupyingPlayer = gameBoard[(int)candidteCoord.x, (int)candidteCoord.y].occupyingPlayer;
        if(occupyingPlayer == null) return false;
        foreach(Vector2 coord in set)
        {
            if(occupyingPlayer != gameBoard[(int)coord.x, (int)coord.y].occupyingPlayer)
                return false;
        }
        return true;
    }

    

// Update is called once per frame
    void Update()
    {
        _tm.Update();
    }  
    
}
