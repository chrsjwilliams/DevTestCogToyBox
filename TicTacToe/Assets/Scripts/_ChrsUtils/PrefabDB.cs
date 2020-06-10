using UnityEngine;


[CreateAssetMenu (menuName = "Prefab DB")]
public class PrefabDB : ScriptableObject
{

    [SerializeField] private Player _player;
    public Player Player
    {
        get { return _player; }
    }

    [SerializeField] private GameObject[] _scenes;
    public GameObject[] Scenes
    {
        get { return _scenes; }
    }

    [SerializeField]private TileSpace _tile;
    public TileSpace Tile
    {
        get { return _tile; }
    }

    [SerializeField] private Shockwave _shockwave;
    public Shockwave Shockwave
    {
        get { return _shockwave; }
    }

    [SerializeField] private WinnerConfetti _winnerConfetti;
    public WinnerConfetti WinnerConfetti
    {
        get { return _winnerConfetti; }
    }

}
