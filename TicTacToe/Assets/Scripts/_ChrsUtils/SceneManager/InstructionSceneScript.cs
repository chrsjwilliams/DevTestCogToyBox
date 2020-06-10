using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionSceneScript : Scene<TransitionData>
{
    [SerializeField] private float SECONDS_TO_WAIT = 0.5f;
    TaskManager _tm = new TaskManager();

    private Text instructions;
    private Text click;

    internal override void OnEnter(TransitionData data)
    {
        //instructions = GameObject.Find("Instructions").GetComponent<Text>();
        click = GameObject.Find("Click").GetComponent<Text>();

        //Color instructColor = instructions.color;
        //Color clickColor = new Color(106f/256f, 171f/256f, 173f/256f);


        
        Services.GameManager.SetDuration(0);
    }

    internal override void OnExit()
    {

    }

    private void StartGame()
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
    void Update ()
    {
        _tm.Update();
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            StartGame();
        }
    }
}
