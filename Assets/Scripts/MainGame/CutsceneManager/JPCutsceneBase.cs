using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JPCutsceneBase : MonoBehaviour
{
    [SerializeField] private float TriggerDistance;
    
    
    [NonSerialized] protected bool cutsceneHasBeenTriggered = false;
    [NonSerialized] protected bool cutsceneInProgress = false;

    //[SerializeField] private JPDialogueMessage[] messages;


    protected JPPlayerController[] playerControllers;
    protected JPFollowCamera playerCamera;

    protected virtual void BeginCutscene()
    {
        foreach (JPPlayerController controller in playerControllers)
        {
            controller.DisableInput();
        }

        cutsceneHasBeenTriggered = true;
        cutsceneInProgress = true;
    }

    protected virtual void EndCutscene()
    {
        cutsceneInProgress = false;
        foreach (JPPlayerController controller in playerControllers)
        {
            controller.EnableInput();
        }
    }

    protected virtual IEnumerator CutsceneScript()
    {
        yield return new WaitForSeconds(1);
        Transform oldTarget = playerCamera.Target;
        playerCamera.Target = transform;

        yield return new WaitForSeconds(2);


        //yield return FindAnyObjectByType<JPDialogueHandler>().DoDialogue(messages);
        playerCamera.Target = oldTarget;
        
        EndCutscene();
        yield return null;
    }

    protected virtual bool CanTriggerCutscene()
    {
        return !(cutsceneHasBeenTriggered || cutsceneInProgress);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        playerControllers = FindObjectsByType<JPPlayerController>(FindObjectsSortMode.None);
        playerCamera = FindAnyObjectByType<JPFollowCamera>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!CanTriggerCutscene()) return;

        if (playerControllers
            .Any(controller => 
                Mathf.Abs(controller.GetPlayer().transform.position.x - transform.position.x) < TriggerDistance))
        {
            ForceCutscene();
        }
    }

    public void ForceCutscene()
    {
        if (!CanTriggerCutscene()) return;
        
        BeginCutscene();
        StartCoroutine(CutsceneScript());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawWireCube(transform.position,
            new Vector3(TriggerDistance * 2, 20, 0));
    }
}
