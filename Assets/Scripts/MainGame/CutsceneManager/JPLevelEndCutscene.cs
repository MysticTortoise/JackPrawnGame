
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class JPLevelEndCutscene : JPCutsceneBase
{
    public static string UpcomingLevelName;
    public static string UpcomingSceneName;
    
    [SerializeField] private string NextLevelName;
    [SerializeField] private string NextSceneName;
    
    protected override IEnumerator CutsceneScript()
    {
        var player =  playerCamera.Target.parent.GetComponent<JPCharacter>();
        var playerKeepIn =  playerCamera.Target.parent.GetComponent<JPForceInsideCamera>();
        
        playerKeepIn.Detach();
        player.moveInput = Vector2.right;
        playerCamera.Target = transform;

        UpcomingLevelName = NextLevelName;
        UpcomingSceneName = NextSceneName;
        
        RTFadeoutTransition.SceneTransition("Intermission");

        while (AudioListener.volume > 0)
        {
            AudioListener.volume -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        while (true)
            yield return new WaitForSeconds(999);
        
        // ReSharper disable once IteratorNeverReturns
    }
}
