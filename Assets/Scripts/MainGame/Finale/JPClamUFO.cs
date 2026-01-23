
using System;
using UnityEngine;
using UnityEngine.Serialization;

public enum JPClamUFOState
{
    Moving,
    Attacking,
    Down,
    Rising
}

public class JPClamUFO : MonoBehaviour
{
    [FormerlySerializedAs("leftDist")] [SerializeField] private float LeftDist;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float UfoTurnAmount;

    [NonSerialized] public JPClamUFOState state = JPClamUFOState.Moving;

    private float leftPos;
    private float rightPos;
    private float posAlpha;
    private float posTimer;

    private float ufoRotGoal;
    private float ufoRot;

    private Transform visual;

    private void Start()
    {
        rightPos = transform.position.x;
        leftPos = rightPos - LeftDist;

        visual = transform.Find("Visual");
    }

    private void Update()
    {
        switch (state)
        {
            case JPClamUFOState.Moving:
                MoveTick();
                break;
        }

        ufoRot = JPMath.Damp(ufoRot, ufoRotGoal, 3, Time.deltaTime);
        visual.rotation = Quaternion.Euler(0, 0, ufoRot);
    }


    private void MoveTick()
    {
        posTimer += Time.deltaTime * MoveSpeed;

        posAlpha = (Mathf.Sin(posTimer) + 1) / 2;
        
        transform.position = new Vector3(
            Mathf.Lerp(rightPos, leftPos, posAlpha),
            transform.position.y, transform.position.z);
        ufoRotGoal = Mathf.Cos(posTimer + 0.5f) * UfoTurnAmount;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.left * LeftDist);
    }
}
