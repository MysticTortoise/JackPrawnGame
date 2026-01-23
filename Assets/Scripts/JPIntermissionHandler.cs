
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JPIntermissionHandler : MonoBehaviour
{
    private void Start()
    {
        var text = GetComponent<JPFontDrawer>();
        
        text.SetMessage("NOW ENTERING....            " + JPLevelEndCutscene.UpcomingLevelName);
        StartCoroutine(NextScene());
        AudioListener.volume = 1;
    }

    private IEnumerator NextScene()
    {
        yield return new WaitForSeconds(4);
        RTFadeoutTransition.SceneTransition(JPLevelEndCutscene.UpcomingSceneName);
    }
}
