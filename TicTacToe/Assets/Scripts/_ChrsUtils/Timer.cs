using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*--------------------------------------------------------------------------------------*/
/*																						*/
/*	Timer: Updates the Timer and displays it on the Canvas                              */
/*																						*/
/*		Functions:																		*/
/*			private:																	*/
/*			    void Start ()                           								*/
/*			    void OnStartTimerEvent(GameEvent e)										*/
/*				void OnDestroy()														*/
/*																						*/
/*			public:																		*/
/*			    void AddDurationInSeconds(float t)										*/
/*				IEnumerator DecrementTimer()											*/
/*				string FloatToTime (float toConvert, string format)						*/
/*																						*/
/*--------------------------------------------------------------------------------------*/
public class Timer : MonoBehaviour 
{
    public bool tenSecondsLeft;
    [SerializeField]
    public float duration { get; private set; }                                 //  Current Time
    private bool stopTimer;
	public Text currentTime { get; private set; }                               //  UI reference to current timer


	/*--------------------------------------------------------------------------------------*/
	/*																						*/
	/*	Awake: Runs once at the begining of the gamen before Start. Initalizes variables.	*/
	/*																						*/
	/*--------------------------------------------------------------------------------------*/
	void Awake () 
	{
        stopTimer = false;
        tenSecondsLeft = false;
		currentTime = GetComponent<Text>();
        Services.EventManager.Register<StartTimerEvent>(OnStartTimerEvent);
        Services.EventManager.Register<StopTimerEvent>(OnStopTimerEvent);
	}

	/*--------------------------------------------------------------------------------------*/
	/*																						*/
	/*	OnStartTimerEvent: Handler for OnStartTimerEvent Event								*/
	/*			param:																		*/
	/*				GameEvent ige - access to readonly variables in event					*/
	/*																						*/
	/*--------------------------------------------------------------------------------------*/
	void OnStartTimerEvent(StartTimerEvent startEvent)
	{
        stopTimer = false;
        if (startEvent.decrementTime) StartCoroutine(DecrementTimer());
        else StartCoroutine(IncrementTimer());
	}

    void OnStopTimerEvent(StopTimerEvent stopEvent)
    {
        stopTimer = true;
    }

	/*--------------------------------------------------------------------------------------*/
	/*																						*/
	/*	OnDestroy: This function runs when this object is destroyed		               		*/
	/*																						*/
	/*--------------------------------------------------------------------------------------*/
    void OnDestroy()
    {
        Services.EventManager.Unregister<StartTimerEvent>(OnStartTimerEvent);
        StopAllCoroutines();
    }

	/*--------------------------------------------------------------------------------------*/
	/*																						*/
	/*	AddDurationInSeconds: Adds time to the timer		            					*/
	/*			param:																		*/
	/*				float t - The amount of time added to the timer in seconds				*/
	/*																						*/
	/*--------------------------------------------------------------------------------------*/
	public void AddDurationInSeconds(float t)
	{
		duration += t;
	}

    public void DisplayTime()
    {
        currentTime.text = FloatToTime(duration, "#00:00");
    }

	/*--------------------------------------------------------------------------------------*/
	/*																						*/
	/*	DecrementTimer: Decrements timer			                                   		*/
	/*																						*/
	/*--------------------------------------------------------------------------------------*/
	public IEnumerator DecrementTimer()
	{
		while (duration > 0)
		{
			yield return new WaitForSeconds(1);
			duration--;
            if(duration < 10 && !tenSecondsLeft)
            {
                tenSecondsLeft = true;
                Services.EventManager.Fire(new TenSecondsLeftEvent(tenSecondsLeft));
            }
            if(duration > 10)
            {
                tenSecondsLeft = false;
                Services.EventManager.Fire(new TenSecondsLeftEvent(tenSecondsLeft));
            }
			currentTime.text = FloatToTime(duration, "#00:00");
		}

        Services.EventManager.Fire(new TimeIsOverEvent(this));
        stopTimer = true;
		yield return new WaitForEndOfFrame();
	}

    public IEnumerator IncrementTimer()
    {
        while (!stopTimer)
        {
            yield return new WaitForSeconds(1);
            duration++;
            currentTime.text = FloatToTime(duration, "#00:00");
        }

        Services.EventManager.Fire(new TimeIsOverEvent(this));
        stopTimer = true;
        yield return new WaitForEndOfFrame();
    }

    /*--------------------------------------------------------------------------------------*/
    /*																						*/
    /*	FloatToTime: Converts a float to string formatted as a timer    					*/
    /*			param:																		*/
    /*				float toConvert - float to convert										*/
    /*				string format - the selected format to output							*/
    /*																						*/
    /*			return:																		*/
    /*				string (in the format of your choice)									*/
    /*																						*/
    /*--------------------------------------------------------------------------------------*/
    public string FloatToTime (float toConvert, string format)
	 {
         switch (format)
		 {
             case "00.0":
                 return string.Format("{0:00}:{1:0}", 
                     Mathf.Floor(toConvert) % 60,//seconds
                     Mathf.Floor((toConvert*10) % 10));//miliseconds
             break;
             case "#0.0":
                 return string.Format("{0:#0}:{1:0}", 
                     Mathf.Floor(toConvert) % 60,//seconds
                     Mathf.Floor((toConvert*10) % 10));//miliseconds
             break;
             case "00.00":
                 return string.Format("{0:00}:{1:00}", 
                     Mathf.Floor(toConvert) % 60,//seconds
                     Mathf.Floor((toConvert*100) % 100));//miliseconds
             break;
             case "00.000":
                 return string.Format("{0:00}:{1:000}", 
                     Mathf.Floor(toConvert) % 60,//seconds
                     Mathf.Floor((toConvert*1000) % 1000));//miliseconds
             break;
             case "#00.000":
                 return string.Format("{0:#00}:{1:000}", 
                     Mathf.Floor(toConvert) % 60,//seconds
                     Mathf.Floor((toConvert*1000) % 1000));//miliseconds
             break;
             case "#0:00":
                 return string.Format("{0:#0}:{1:00}",
                     Mathf.Floor(toConvert / 60),//minutes
                     Mathf.Floor(toConvert) % 60);//seconds
             break;
             case "#00:00":
                 return string.Format("{0:#00}:{1:00}", 
                     Mathf.Floor(toConvert / 60),//minutes
                     Mathf.Floor(toConvert) % 60);//seconds
             break;
             case "0:00.0":
                 return string.Format("{0:0}:{1:00}.{2:0}",
                     Mathf.Floor(toConvert / 60),//minutes
                     Mathf.Floor(toConvert) % 60,//seconds
                     Mathf.Floor((toConvert*10) % 10));//miliseconds
             break;
             case "#0:00.0":
                 return string.Format("{0:#0}:{1:00}.{2:0}",
                     Mathf.Floor(toConvert / 60),//minutes
                     Mathf.Floor(toConvert) % 60,//seconds
                     Mathf.Floor((toConvert*10) % 10));//miliseconds
             break;
             case "0:00.00":
                 return string.Format("{0:0}:{1:00}.{2:00}",
                     Mathf.Floor(toConvert / 60),//minutes
                     Mathf.Floor(toConvert) % 60,//seconds
                     Mathf.Floor((toConvert*100) % 100));//miliseconds
             break;
             case "#0:00.00":
                 return string.Format("{0:#0}:{1:00}.{2:00}",
                     Mathf.Floor(toConvert / 60),//minutes
                     Mathf.Floor(toConvert) % 60,//seconds
                     Mathf.Floor((toConvert*100) % 100));//miliseconds
             break;
             case "0:00.000":
                 return string.Format("{0:0}:{1:00}.{2:000}",
                     Mathf.Floor(toConvert / 60),//minutes
                     Mathf.Floor(toConvert) % 60,//seconds
                     Mathf.Floor((toConvert*1000) % 1000));//miliseconds
             break;
             case "#0:00.000":
                 return string.Format("{0:#0}:{1:00}.{2:000}",
                     Mathf.Floor(toConvert / 60),//minutes
                     Mathf.Floor(toConvert) % 60,//seconds
                     Mathf.Floor((toConvert*1000) % 1000));//miliseconds
             break;
         }
         return "error";
     }
}
