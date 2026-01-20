using UnityEngine;

public class JPPlayerAnimatorMessenger : MonoBehaviour
{
    private JPPlayer player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        player = transform.parent.GetComponent<JPPlayer>();
    }

    public void CheckQueuedJab()
    {
        player.CheckQueuedJab(true);
    }

    public void CheckQueuedJabNoFree()
    {
        player.CheckQueuedJab(false);
    }

    public void DoJabHitbox()
    {
        player.DoJabHitbox();
    }
    
    public void UppercutLiftoff()
    {
        player.UppercutLiftoff();
    }
}
