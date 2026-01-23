using UnityEngine;
using UnityEngine.SceneManagement;

public class JPTitleManager : MonoBehaviour
{
    private static readonly int Go = Animator.StringToHash("Go");
    private bool canStart;

    public void EnablePressButton()
    {
        canStart = true;
    }

    public void PressedStart()
    {
        if (!canStart) return;
        
        GetComponent<Animator>().SetTrigger(Go);
    }

    public void GoToIntro()
    {
        SceneManager.LoadScene("IntroCutscene");
    }
}
