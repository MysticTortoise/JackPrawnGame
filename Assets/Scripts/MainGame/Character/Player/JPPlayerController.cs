
using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class JPPlayerController : MonoBehaviour
{
    [CanBeNull] private JPCharacter player;

    private bool canInput = true;

    private float deathTime;

    private void GetPlayerObj()
    {
        if (player != null) return;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<JPCharacter>() == null) continue;
            player = child.GetComponent<JPCharacter>();
            break;
        }
    }

    private void Start()
    {
        GetPlayerObj();
    }

    public void EnableInput()
    {
        canInput = true;
    }

    public void DisableInput()
    {
        canInput = false;
        if (player == null) return;
        
        player.moveInput = Vector2.zero;
        player.ReleaseAttack();
    }

    private bool CanInput()
    {
        return player != null && canInput;
    }

    private void Update()
    {
        if (player.dead)
        {
            deathTime += Time.deltaTime;
            if (deathTime > 1.5f)
            {
                deathTime = -99;
                RTFadeoutTransition.SceneTransition("GameOver");
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!CanInput()) return;
        
        player.moveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!CanInput()) return;
        
        if (context.started)
        {
            player.BeginAttack();
        } else if (context.canceled)
        {
            player.ReleaseAttack();
        }
    }

    public JPCharacter GetPlayer()
    {
        GetPlayerObj();
        return player;
    }

    public void BeDone()
    {
        FindAnyObjectByType<JPLevelEndCutscene>().ForceCutscene();
    }   
}
