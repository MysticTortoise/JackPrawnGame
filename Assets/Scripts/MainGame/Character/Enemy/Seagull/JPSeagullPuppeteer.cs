
using UnityEngine;

public class JPSeagullPuppeteer : JPEnemyPuppeteer
{
    private bool rightSide;

    void UpdateSide()
    {
        rightSide = enemy.transform.position.x > target.transform.position.x;
    }


    public JPSeagullPuppeteer(JPCharacter enemy, JPCharacter target) : base(enemy, target)
    {
        UpdateSide();
    }
    
    public override void Direct()
    {
        if (((JPSeagullEnemy)enemy).attackPhase != 0)
        {
            enemy.SetFacingDir(rightSide);
            UpdateSide();
            return;
        }
        
        
        float dist = GoToGoal(new Vector2(
            target.transform.position.x + (approachDist * (rightSide ? -1 : 1)),
            target.transform.position.z));

        if (!(dist < inRangeDist)) return;
        
        // Attack
        enemy.moveInput = new Vector2(rightSide ? 1 : -1, 0);
        enemy.BeginAttack();
    }
}
