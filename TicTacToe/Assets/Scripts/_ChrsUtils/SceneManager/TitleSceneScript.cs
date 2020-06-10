using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleSceneScript : Scene<TransitionData>
{
    public KeyCode startGame = KeyCode.Space;

    public bool hasLoadGame = false;
    [SerializeField]private float SECONDS_TO_WAIT = 0.1f;

    private TaskManager _tm = new TaskManager();

    [SerializeField] private TextMeshProUGUI[] titleText;

    [SerializeField] private TextMeshProUGUI[] buttonText;

    [SerializeField] private Flock[] _flocks;
    [SerializeField] private Image audioStatusIcon;

    private Color transparent = new Color(0, 0, 0, 0);
    private float t = 0;
    internal override void OnEnter(TransitionData data)
    {
        Services.EventManager.Register<GameLoadEvent>(OnGameLoad);
        Services.AudioManager.StopClip();
        if(!Services.AudioManager.muted)
            Services.AudioManager.SetVolume(0.5f);
        Services.AudioManager.PlayClip(Clips.TITLE_SONG);
        Services.GameManager.UpdateMutIcon(audioStatusIcon);
        
        for (int i = 0; i < titleText.Length; i++)
        {
            titleText[i].gameObject.SetActive(false);
        }

        buttonText[0].color = new Color(0, 0, 0, 0);

        TaskQueue titleEntryTasks = new TaskQueue();
        Task slideTitleIn = new TitleEntryAnimation(titleText);
        Task fadeFlockX = new ActionTask(_flocks[0].FadeInFlockAgents);
        Task fadeFlockO = new ActionTask(_flocks[1].FadeInFlockAgents);
        titleEntryTasks.Add(fadeFlockX);
        titleEntryTasks.Add(fadeFlockO);
        titleEntryTasks.Add(slideTitleIn);


        _tm.Do(titleEntryTasks);
    
    }

    internal override void OnExit()
    {

    }

    public void OnGameLoad(GameLoadEvent e)
    {
        hasLoadGame = true;
    }

    public void StartGame()
    {
        Services.AudioManager.CreateTrackAndPlay(Clips.TAP);
        Services.AudioManager.FadeAudio();

        hasLoadGame = false;
        TaskQueue startGameTasks = new TaskQueue();
        Task slideTitleOut = new TitleEntryAnimation(titleText, true);
        Task fadeStartText = new LERPColor(buttonText, buttonText[0].color, transparent, 0.3f);
        Task fadeFlockX = new ActionTask(_flocks[0].FadeOutFlockAgents);
        Task fadeFlockO = new ActionTask(_flocks[1].FadeOutFlockAgents);
        Task beginGame = new ActionTask(TransitionToGame);

        startGameTasks.Add(fadeStartText);
        startGameTasks.Add(fadeFlockO);
        startGameTasks.Add(fadeFlockX);
        startGameTasks.Add(slideTitleOut);
        startGameTasks.Add(beginGame);

        _tm.Do(startGameTasks);
    }

    public void ToggleMute()
    {
        Services.GameManager.ToggleMute(audioStatusIcon);
    }

    private void TransitionToGame()
    {
        Services.Scenes.Swap<GameSceneScript>();
    }

    private void Update()
    {
        _tm.Update();
        if(hasLoadGame)
        {
            t += Time.deltaTime;
            buttonText[0].color = Color.Lerp(transparent, Color.black, Mathf.PingPong(t,1.5f));
        }
    }
}
