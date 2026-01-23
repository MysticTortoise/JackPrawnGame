
using System.Collections;
using UnityEngine;

public class JPBoardwalkIntroCutscene : JPCutsceneBase
{
    [SerializeField] private JPDialogueMessage[] testMessages;
    protected override IEnumerator CutsceneScript()
    {
        yield return new WaitForSeconds(1);
        Transform oldTarget = playerCamera.Target;
        var player = oldTarget.parent.GetComponent<JPCharacter>();
        var playerKeepIn = oldTarget.parent.GetComponent<JPForceInsideCamera>();
        
        playerKeepIn.Detach();
        playerCamera.Target = transform.Find("CamTarg");
        yield return new WaitForSeconds(3);
        
        var encounter = transform.Find("Encounter").GetComponent<JPEnemyEncounter>();
        encounter.ForceTriggerEncounter();

        yield return new WaitForSeconds(3);
        playerKeepIn.ReAttachWhenInBounds();

        //yield return FindAnyObjectByType<JPDialogueHandler>().DoDialogue(messages);
        playerCamera.Target = encounter.transform;
        
        EndCutscene();
        yield return null;
    }
}
