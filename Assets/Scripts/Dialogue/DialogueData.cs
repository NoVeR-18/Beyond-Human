using System.Collections.Generic;
using UnityEngine;
public enum SpeakerType
{
    Player,
    NPC
}

[System.Serializable]
public class DialogueLine
{
    public SpeakerType speaker;
    [TextArea(2, 5)]
    public string Text;

}
[CreateAssetMenu(menuName = "Dialogue/Dialogue")]
public class DialogueData : ScriptableObject
{

    public List<DialogueLine> lines = new List<DialogueLine>();

    [Header("What hapening after dialogue")]
    public DialogueAction result;
}
