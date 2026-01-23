
using System.Collections;
using UnityEngine;

public class JPClamCutscene : JPCutsceneBase
{
    private static readonly int AnimProgress = Animator.StringToHash("Progress");
    private static readonly int TurnOnUFOAnimID = Animator.StringToHash("TurnOn");
    private static readonly int StartActualUFOAnimID = Animator.StringToHash("StartActual");
    [SerializeField] private JPDialogueMessage[] IntroMessages;
    [SerializeField] private JPDialogueMessage[] BossBeginMessage;
    protected override IEnumerator CutsceneScript()
    {
        var animator = transform.parent.GetComponent<Animator>();

        bool normal = true;
        
        // Wait
        
        yield return new WaitForSeconds(1);
        var oldTarg = playerCamera.Target;
        var player = playerCamera.Target.parent.GetComponent<JPCharacter>();
        var playerKeepIn = playerCamera.Target.parent.GetComponent<JPForceInsideCamera>();

        // Camera pan
        playerCamera.Target = transform.parent.Find("ViewClamP");
        playerKeepIn.Detach();


        yield return new WaitForSeconds(normal?5:0);

        // Player walks in
        player.moveInput = Vector2.right;
        yield return new WaitForSeconds(normal?2:0);
        player.moveInput = Vector2.zero;
        
        // Start dialogue
        animator.SetInteger(AnimProgress, 1);
        yield return FindAnyObjectByType<JPDialogueHandler>().DoDialogue(IntroMessages);
        
        // Summon UFO and get in
        animator.SetInteger(AnimProgress, 2);
        yield return new WaitForSeconds(6);

        // Activate UFO
        /**/
        var boss = transform.parent.Find("Boss");
        var ufoAnimator = boss.Find("Visual").GetComponent<Animator>();
        ufoAnimator.SetTrigger(TurnOnUFOAnimID);
        yield return new WaitForSeconds(normal?2:0);
        
        // LET THE GAMES BEGIN
        yield return FindAnyObjectByType<JPDialogueHandler>().DoDialogue(BossBeginMessage);
        
        var ufo = boss.GetComponent<JPClamUFO>();
        ufo.enabled = true;
        ufoAnimator.SetTrigger(StartActualUFOAnimID);
        animator.enabled = false;
        
        transform.parent.Find("PreMusic").gameObject.SetActive(false);
        transform.parent.Find("BossMusic").gameObject.SetActive(true);

        playerCamera.Target = oldTarg;
        
        
        EndCutscene();
        yield return null;
    }
}
