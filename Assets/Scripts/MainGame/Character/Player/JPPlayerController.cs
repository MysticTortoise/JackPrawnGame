
using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class JPPlayerController : MonoBehaviour
{
    [CanBeNull] private JPCharacter player;

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
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (player == null) return;
        
        player.moveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (player == null) return;
        if (context.started)
        {
            player.BeginAttack();
        } else if (context.performed)
        {
            player.ReleaseAttack();
        }
    }

    public JPCharacter GetPlayer()
    {
        GetPlayerObj();
        return player;
    }
}
