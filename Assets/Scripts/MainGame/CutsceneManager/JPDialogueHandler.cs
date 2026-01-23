
using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[Serializable]
public class JPDialogueMessage
{
    [FormerlySerializedAs("message")] public string Message;
    [CanBeNull] public AudioClip SoundClip;
}

public class JPDialogueHandler : MonoBehaviour
{
    private static readonly int OpenAnimID = Animator.StringToHash("IsOpen");

    //private PlayerInput playerInput;
    private JPScrollText scrollText;
    private Animator animator;

    private bool dialogueUp;
    private int dialogueID;
    private bool canAdvance;
        
    private void Start()
    {
        //playerInput = GetComponent<PlayerInput>();
        scrollText = transform.GetChild(0).GetComponent<JPScrollText>();
        animator = GetComponent<Animator>();
        scrollText.OnFinishedWriting += DoneWritingText;
    }

    private void DoneWritingText()
    {
        canAdvance = true;
    }

    public IEnumerator DoDialogue(JPDialogueMessage[] dialogueMessages)
    {
        dialogueUp = true;
        dialogueID = 0;

        yield return new WaitForSeconds(1);

        while (dialogueID < dialogueMessages.Length)
        {
            JPDialogueMessage thisMessage = dialogueMessages[dialogueID];
            int thisMessageID = dialogueID;
            scrollText.SetMessage(thisMessage.Message);
            canAdvance = false;
            
            if(thisMessage.SoundClip)
                JPQuickSound.PlayQuickSound(thisMessage.SoundClip);

            yield return new WaitUntil(() => dialogueID > thisMessageID);
        }
        scrollText.SetMessage("");

        dialogueUp = false;
        canAdvance = false;
        yield return null;
    }

    public void Advance(InputAction.CallbackContext context)
    {
        if (!dialogueUp /*|| !canAdvance*/ || !context.started) return;

        dialogueID++;
    }

    private void Update()
    {
        animator.SetBool(OpenAnimID, dialogueUp);
    }
}
