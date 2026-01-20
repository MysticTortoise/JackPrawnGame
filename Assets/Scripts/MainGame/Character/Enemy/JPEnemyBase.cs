
public class JPEnemyBase : JPCharacter
{
    protected new void Start()
    {
        base.Start();
        Faction = JPCharacterFaction.Enemy;
    }
}