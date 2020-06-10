using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameSceneScript : Scene<TransitionData>
{

    [SerializeField] private float SECONDS_TO_WAIT = 0.5f;
    TaskManager _tm = new TaskManager();

    internal override void OnEnter(TransitionData data)
    {
        
    }

    internal override void OnExit()
    {

    }

    private void RefreshGame()
    {
    }

    private void TitleTransition()
    {

    }

    private void ChangeScene()
    {
        Services.Scenes.Swap<GameSceneScript>();
    }

    // Update is called once per frame
    void Update()
    {
        _tm.Update();
       
    }
}
