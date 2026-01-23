using UnityEngine;

public class JPSeagullAnimMessagePasser : MonoBehaviour
{
    private JPSeagullEnemy seagull;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        seagull = transform.parent.GetComponent<JPSeagullEnemy>();
    }

    public void DoneBeingDead()
    {
        seagull.DoneDying();
    }
}