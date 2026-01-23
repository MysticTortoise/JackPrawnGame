using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPGameOver : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        StartCoroutine(DoStuff());
    }

    private static IEnumerator DoStuff()
    {
        yield return new WaitForSeconds(6);
        RTFadeoutTransition.SceneTransition("Title");
    }
}
