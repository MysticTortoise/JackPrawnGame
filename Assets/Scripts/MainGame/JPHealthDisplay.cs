
using System;
using JetBrains.Annotations;
using UnityEngine;

public class JPHealthDisplay : MonoBehaviour
{

    [CanBeNull] public JPCharacter Target;
    
    private void Update()
    {
        float curX = transform.localScale.x;
        float targX = Target ? (float)Target.currentHealth / Target.MaxHealth * 100f : 0;
        
        transform.localScale = 
            new Vector3(JPMath.Damp(curX, targX, 2, Time.deltaTime), 1, 1);
    }
}
