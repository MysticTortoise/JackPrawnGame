
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class JPEndingSceneControls : MonoBehaviour
{
    private static readonly int KillHim = Animator.StringToHash("KillHim");

    private float timer;
    private float timer2;

    public void EndItAll(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (timer < 5 || timer2 > 0) return;
        GetComponent<Animator>().SetTrigger(KillHim);
        transform.parent.parent.GetComponent<Animator>().SetTrigger(KillHim);
        timer2 = .01f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer2 > 0)
        {
            timer2 += Time.deltaTime;
            if (timer2 > 5)
            {
                RTFadeoutTransition.SceneTransition("Credits");
            }
        }
    }
}
