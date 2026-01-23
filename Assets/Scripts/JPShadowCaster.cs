using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class JPShadowCaster : MonoBehaviour
{
    [SerializeField] private JPProjectedCollider BaseCollider;
    [SerializeField] private Vector2 Offset;
    
    private JPParallaxFloor mainFloor;

    private void Start()
    {
        mainFloor = FindObjectsByType<JPParallaxFloor>(FindObjectsSortMode.None).First(p => p.primary);
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!BaseCollider)
            return;
        
        
        
        transform.position = JPProjection.projectPoint(new Vector3(
            BaseCollider.GetCenter().x,
            0,
            BaseCollider.GetCenter().z
            ), mainFloor) + Offset;
    }
}
