
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class JPFontDrawer : MonoBehaviour
{
    public JPBitmapFont Font;

    [SerializeField] private string StartingMessage;
    [SerializeField] private int StartingCharactersPerLine;
    [FormerlySerializedAs("zOrder")] public int ZOrder;
    
    private string message;
    private int showCharCount = int.MaxValue;
    private int charactersPerLine;
    private List<SpriteRenderer> characters = new();

    private void UpdateShownChars()
    {
        for (int i = 0; i < message.Length; i++)
        {
            characters[i].enabled = i < showCharCount;
        }
    }
    
    private void RedrawMessage()
    {
        while (characters.Count < message.Length)
        {
            var charObj = new GameObject();
            charObj.transform.SetParent(transform);

            charObj.transform.localPosition = new Vector3(
                (characters.Count % charactersPerLine) * Font.Spacing,
                // ReSharper disable once PossibleLossOfFraction
                (characters.Count / charactersPerLine) * -Font.Spacing,
                0
            );
            
            var spriteRenderer = charObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = ZOrder;
            characters.Add(spriteRenderer);
        }

        while (characters.Count > message.Length)
        {
            Destroy(characters[message.Length]);
            characters.RemoveAt(message.Length);
        }

        for (int i = 0; i < message.Length; i++)
        {
            characters[i].sprite = Font.charactersStartingAt32[message[i] - 32];
        }
        UpdateShownChars();
    }

    public void SetMessage(string newMessage)
    {
        message = newMessage.ToUpper();
        RedrawMessage();
    }

    public void SetCharCount(int newCharCount)
    {
        if (newCharCount == -1)
            newCharCount = int.MaxValue;

        showCharCount = newCharCount;
        RedrawMessage();
    }

    private void Start()
    {
        charactersPerLine = StartingCharactersPerLine;
        SetMessage(StartingMessage);
    }
}
