
using System.Collections.Generic;
using UnityEngine;

public class JPCameraBlocker : MonoBehaviour
{
    public static HashSet<JPCameraBlocker> ActiveBlockers = new();
    
    public bool BlockRightMovement;

    private void OnEnable()
    {
        ActiveBlockers.Add(this);
    }

    private void OnDisable()
    {
        ActiveBlockers.Remove(this);
    }
}
