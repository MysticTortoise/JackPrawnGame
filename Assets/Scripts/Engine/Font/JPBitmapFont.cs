
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "JackPrawn/JPBitmapFont", order = 1)]
public class JPBitmapFont : ScriptableObject
{
    public Sprite[] charactersStartingAt32;
    public float Spacing;
}
