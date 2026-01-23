
using System;
using UnityEngine;

public class IntroDialogueScroller : MonoBehaviour
{
    private JPScrollText textScroller;

    [SerializeField] private string[] messages;
    private int messageID = -1;

    [SerializeField] private float TimeBetweenBreaks;
    private float timer;

    public void Start()
    {
        textScroller = GetComponent<JPScrollText>();
        textScroller.OnFinishedWriting += DoneScrolling;
        NextMessage();
    }

    private void DoneScrolling()
    {
        timer = TimeBetweenBreaks;
    }

    private void Update()
    {
        if (!(timer > 0)) return;
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            NextMessage();
        }
    }

    public void EndCustcene()
    {
        JPLevelEndCutscene.UpcomingLevelName = "                   TUTORIAL";
        JPLevelEndCutscene.UpcomingSceneName = "Tutorial";
        RTFadeoutTransition.SceneTransition("Intermission");
    }


    private void NextMessage()
    {
        messageID++;
        if (messageID >= messages.Length)
        {
            // go to game
            EndCustcene();
            return;
        }
        textScroller.SetMessage(messages[messageID]);
    }
    
}
