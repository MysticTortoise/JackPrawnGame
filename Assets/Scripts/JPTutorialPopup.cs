
using System;
using UnityEngine;

public class JPTutorialPopup : MonoBehaviour
{
    [SerializeField] private float AppearDist;
    public Sprite Sprite;

    public static JPTutorialPopup activePopup;

    private JPCharacter player;

    private void Start()
    {
        player = FindAnyObjectByType<JPPlayerController>().GetPlayer();
    }

    private void Update()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) < AppearDist)
        {
            activePopup = this;
        } else if (activePopup == this)
        {
            activePopup = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(AppearDist * 2, 15, 0));
    }
}
