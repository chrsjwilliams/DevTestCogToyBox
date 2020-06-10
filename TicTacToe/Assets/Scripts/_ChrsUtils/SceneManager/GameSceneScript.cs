using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class GameSceneScript : Scene<TransitionData>
{
    public enum PlayerNum{PLAYER1 = 0, PLAYER2}
    public bool endGame;

    public static bool hasWon { get; private set; }

    public const int LEFT_CLICK = 0;
    public const int RIGHT_CLICK = 1;

    TaskManager _tm = new TaskManager();

    public GameBoard board;

    public Player[] players;

    public int currentPlayerIndex;
    public Player currentPlayer { get; private set; }

    [SerializeField] private TextMeshProUGUI _turnIndicator;
    public TextMeshProUGUI TurnIndicator
    {
        get { return _turnIndicator; }
    }

    [SerializeField] private Image _turnIndicatorIcon;
    [SerializeField] private Image _homeButtonIcon;
    [SerializeField] private Image _replayButtonIcon;
    [SerializeField] private Image audioStatusIcon;

    [SerializeField] private SpriteRenderer _gradient;

    private Color _transparent = new Color(1, 1, 1, 0);
    private Color _iconGray = new Color(108 / 255f, 108 / 255f, 108 / 255f);

    internal override void OnEnter(TransitionData data)
    {
        Services.AudioManager.StopClip();
        if(!Services.AudioManager.muted)
            Services.AudioManager.SetVolume(0.5f);
        Services.AudioManager.PlayClip(Clips.MATCH_SONG);
        Services.GameManager.UpdateMutIcon(audioStatusIcon);

        Services.GameScene = this;
        players = new Player[Services.GameManager.TotalPlayers];
        for (int i = 0; i < Services.GameManager.TotalPlayers; i++)
        {
            players[i] = Instantiate(Services.Prefabs.Player, Vector3.zero, Quaternion.identity, transform);
            int playerNum = i + 1;
            players[i].name = "Player " + playerNum;
            Color[] colors;
            switch(i)
            {
                case 0: colors = Services.GameManager.Player1Color; break;
                case 1: colors = Services.GameManager.Player2Color; break;
                default: colors = Services.GameManager.Player1Color; break;
            }
            players[i].Init(playerNum, Services.GameManager.AvailableIcons[i], colors);
        }

        board = GetComponent<GameBoard>();
        BoardInfo info;
        info.row = 3;
        info.col = 3;


        board.Init(info);

        currentPlayerIndex = UnityEngine.Random.Range(0, 2);
        currentPlayer = players[currentPlayerIndex];

        if(currentPlayerIndex == 0)
        {
            currentPlayerIndex = (int)PlayerNum.PLAYER1;
            _turnIndicatorIcon.sprite = players[(int)PlayerNum.PLAYER1].PlayerIcon;
        }
        else
        {
            currentPlayerIndex = (int)PlayerNum.PLAYER2;
            _turnIndicatorIcon.sprite = players[(int)PlayerNum.PLAYER2].PlayerIcon;
        }

        // These are independent tasks I want to complete when entering the game scene
        Task fadeIndicatorIconTask = new LERPColor(_turnIndicatorIcon, _transparent, currentPlayer.playerColor[0], 3f);
        Task fadeIndicatorTextTask = new LERPColor(_turnIndicator, _transparent, currentPlayer.playerColor[0], 3f);
        Task fadeHomeButtonTask = new LERPColor(_homeButtonIcon, _transparent, _iconGray, 1f);
        Task fadeReplayButtonTask = new LERPColor(_replayButtonIcon, _transparent, _iconGray, 1f);

        // I want to compelte these tasks in parallel so I use a TaskTree.
        // How it works:
        //              A Task tree will complete the Task at its root first, then move on to complete
        //              the tasks in in child nodes in parallel.
        TaskTree uiEntryTask = new TaskTree(new EmptyTask(), 
                                                new TaskTree(fadeIndicatorIconTask),
                                                new TaskTree(fadeIndicatorTextTask),
                                                new TaskTree(fadeHomeButtonTask),
                                                new TaskTree(fadeReplayButtonTask));

        _tm.Do(uiEntryTask);

        Services.EventManager.Register<PlayMadeEvent>(OnPlayMade);
        Services.EventManager.Register<GameEndEvent>(OnGameEnd);
    }

    internal override void OnExit()
    {
        Services.EventManager.Unregister<PlayMadeEvent>(OnPlayMade);
        Services.EventManager.Unregister<GameEndEvent>(OnGameEnd);
    }
    public void OnPlayMade(PlayMadeEvent e)
    {
        if(endGame) return;
        if (currentPlayerIndex == (int)PlayerNum.PLAYER1)
        {
            currentPlayerIndex = (int)PlayerNum.PLAYER2;
            _turnIndicator.color = Services.GameManager.Player2Color[0];
            _turnIndicatorIcon.sprite = players[(int)PlayerNum.PLAYER2].PlayerIcon;
            _turnIndicatorIcon.color = Services.GameManager.Player2Color[0];
        }
        else
        {
            currentPlayerIndex = (int)PlayerNum.PLAYER1;
            _turnIndicator.color = Services.GameManager.Player1Color[0];
            _turnIndicatorIcon.sprite = players[(int)PlayerNum.PLAYER1].PlayerIcon;
            _turnIndicatorIcon.color = Services.GameManager.Player1Color[0];
        }

        currentPlayer = players[currentPlayerIndex];
        
    }

    public void OnGameEnd(GameEndEvent e)
    {
        endGame = true;
        if(e.winner != null)
        {
            float pitch = e.winner.playerNum == 0 ? 0.8f : 2f;
            Services.AudioManager.CreateTrackAndPlay(Clips.WIN, pitch);
            _turnIndicatorIcon.sprite = e.winner.PlayerIcon;
            _turnIndicatorIcon.color = e.winner.playerColor[0];
            _turnIndicator.color = e.winner.playerColor[0];
            _turnIndicator.text = "    WINS";
            WinnerConfetti winnerConfetti = Instantiate(Services.Prefabs.WinnerConfetti);
            winnerConfetti.Init(e.winner);
            Task fadeGradient = new LERPColor(_gradient, _transparent, e.winner.playerColor[0], 0.75f);
            _tm.Do(fadeGradient);
        }
        else
        {
            Services.AudioManager.CreateTrackAndPlay(Clips.TIE);
            _turnIndicatorIcon.color = new Color(0, 0, 0, 0);
            _turnIndicator.color = new Color(127 / 256f, 127 / 256f, 127 / 256f);
            _turnIndicator.text = "TIE GAME";
            Task fadeGradient = new LERPColor(_gradient, _transparent, _iconGray, 0.75f);
            _tm.Do(fadeGradient);
        }
    }

    public void OnRestartPressed()
    {
        Services.AudioManager.CreateTrackAndPlay(Clips.TAP);
        Services.AudioManager.FadeAudio();
        Services.EventManager.Fire(new RefreshGameBaord());

        Task fadeIndicatorIconTask = new LERPColor(_turnIndicatorIcon, _turnIndicatorIcon.color, _transparent, 0.5f);
        Task fadeIndicatorTextTask = new LERPColor(_turnIndicator, _turnIndicator.color,_transparent, 0.5f);
        Task fadeHomeButtonTask = new LERPColor(_homeButtonIcon, _iconGray, _transparent,0.5f);
        Task fadeReplayButtonTask = new LERPColor(_replayButtonIcon, _iconGray, _transparent,0.5f);
        Task fadeGradient = new LERPColor(_gradient, _gradient.color, _transparent, 0.75f);

        //  Here we have a modification of the Task tree where we want to compelte the fading
        //  board animation and waiting tasks in parallel, than compete the ResetGameScene Task.
        //  To accomplish this, I made the ResetGameScene Task a child of the Wait task.
        TaskTree restartGameTasks = new TaskTree(new EmptyTask(), 
                                                new TaskTree(fadeIndicatorIconTask),
                                                new TaskTree(fadeIndicatorTextTask),
                                                new TaskTree(fadeHomeButtonTask),
                                                new TaskTree(fadeReplayButtonTask),
                                                new TaskTree(fadeGradient),
                                                new TaskTree(new BoardEntryAnimation()),
                                                new TaskTree(new Wait(3),
                                                    new TaskTree(new ActionTask(ResetGameScene))));

        _tm.Do(restartGameTasks);

    }

    public void ResetGameScene()
    {
        Services.Scenes.Swap<GameSceneScript>();
    }

    public void OnHomePressed()
    {
        Services.AudioManager.CreateTrackAndPlay(Clips.TAP);
        Services.AudioManager.FadeAudio();
        Services.EventManager.Fire(new RefreshGameBaord());

        Task fadeIndicatorIconTask = new LERPColor(_turnIndicatorIcon, _turnIndicatorIcon.color, _transparent, 0.5f);
        Task fadeIndicatorTextTask = new LERPColor(_turnIndicator, _turnIndicator.color,_transparent, 0.5f);
        Task fadeHomeButtonTask = new LERPColor(_homeButtonIcon, _iconGray, _transparent,0.5f);
        Task fadeReplayButtonTask = new LERPColor(_replayButtonIcon, _iconGray, _transparent,0.5f);
        Task fadeGradient = new LERPColor(_gradient, _gradient.color, _transparent, 0.75f);

        // This is the smae idea found in the OnRestartPressed function above
        TaskTree returnHomeTasks = new TaskTree(new EmptyTask(), 
                                                new TaskTree(fadeIndicatorIconTask),
                                                new TaskTree(fadeIndicatorTextTask),
                                                new TaskTree(fadeHomeButtonTask),
                                                new TaskTree(fadeReplayButtonTask),
                                                new TaskTree(fadeGradient),
                                                new TaskTree(new BoardEntryAnimation()),
                                                new TaskTree(new Wait(3),
                                                    new TaskTree(new ActionTask(ReturnHome))));
        _tm.Do(returnHomeTasks);
    }

    public void ReturnHome()
    {
        Services.Scenes.Swap<TitleSceneScript>();
    }

    public void ToggleMute()
    {
        Services.GameManager.ToggleMute(audioStatusIcon);
    }
    private void EndGame()
    {
        Services.AudioManager.FadeAudio();

    }
    
	// Update is called once per frame
	void Update ()
    {
        _tm.Update();
	}
}
