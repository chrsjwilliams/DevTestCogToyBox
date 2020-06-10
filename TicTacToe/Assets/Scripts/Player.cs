using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerNum { get; private set; }
    [SerializeField] private Sprite _playerIcon;
    public Sprite PlayerIcon{ get { return _playerIcon; } }
    public bool isTurn;

    public Color[] playerColor;

    public void Init(int num,Sprite icon, Color[] colors)
    {
        playerNum = num;
        _playerIcon = icon;
        playerColor = colors;
    }

}
