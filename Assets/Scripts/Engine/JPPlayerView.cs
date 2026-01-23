
using System;
using UnityEngine;

public class JPPlayerView : MonoBehaviour
{
    private static JPPlayerView _singleton = null;

    private void Start()
    {
        if (_singleton != null)
        {
            Destroy(gameObject);
            return;
        }

        _singleton = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
}
