using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;   // Имя (например "Волшебник" или "ГГ")
        [TextArea(2, 5)]
        public string text;          // Реплика
    }

    public List<DialogueLine> lines = new List<DialogueLine>();

    [Header("Что произойдет после диалога")]
    public DialogueAction result;
}
