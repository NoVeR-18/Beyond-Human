using NPCEnums;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.NPC.Dialogue
{
    [CreateAssetMenu(menuName = "NPC/Dialogue Set")]
    public class NPCDialogueSet : ScriptableObject
    {
        public List<DialogueSequence> sequences;

        private Dictionary<DialogueContext, List<DialogueLine>> _dialogueDict;

        public void Init()
        {
            _dialogueDict = new Dictionary<DialogueContext, List<DialogueLine>>();
            foreach (var seq in sequences)
            {
                if (!_dialogueDict.ContainsKey(seq.context))
                {
                    _dialogueDict[seq.context] = seq.lines;
                }
                else
                {
                    Debug.LogWarning($"Duplicate context {seq.context} in dialogue set");
                }
            }
        }

        public List<DialogueLine> GetDialogue(DialogueContext context)
        {
            if (_dialogueDict == null) Init();
            return _dialogueDict.TryGetValue(context, out var lines) ? lines : null;
        }
    }
    [System.Serializable]
    public class DialogueSequence
    {
        public DialogueContext context;
        public List<DialogueLine> lines;
    }
    [System.Serializable]
    public class DialogueLine
    {
        [TextArea]
        public string text;
        public float delayAfter = 1f;
    }

}