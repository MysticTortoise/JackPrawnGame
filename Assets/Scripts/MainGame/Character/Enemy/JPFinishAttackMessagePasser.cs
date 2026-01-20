using UnityEngine;

public class JPFinishAttackMessagePasser : MonoBehaviour
{
    private JPFishEnemy fishe;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        fishe = transform.parent.GetComponent<JPFishEnemy>();
    }

    public void FinishAttack()
    {
        fishe.FinishAttack();
    }

    public void DoAttackHitbox()
    {
        fishe.DoAttackHitbox();
    }

    public void DoneBeingDead()
    {
        fishe.DoneDying();
    }
}