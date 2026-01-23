
using UnityEngine;

public class JPGoSprite : MonoBehaviour
{
    private static readonly int Go1 = Animator.StringToHash("Go");

    public void Go()
    {
        GetComponent<Animator>().SetTrigger(Go1);
    }
}
