
using System;
using JetBrains.Annotations;
using UnityEngine;

public class JPHealthDisplay : MonoBehaviour
{

    [CanBeNull] public JPCharacter Target;
    
    private void Update()
    {
        transform.localScale = Target ? 
            new Vector3((float)Target.CurrentHealth / Target.MaxHealth * 100f, 1, 1) : 
            Vector3.zero;
    }
}
