
using System;
using UnityEngine;

public class JPParticleFX : MonoBehaviour
{
    public float RemoveTime;

    private void Start()
    {
        if(RemoveTime != 0)
            Invoke(nameof(Finish), RemoveTime);
    }

    public void Finish()
    {
        Destroy(gameObject);
    }
}
