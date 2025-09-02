using NPCEnums;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.NPC.Dialogue
{
    [CreateAssetMenu(menuName = "NPC/Dialogue Set")]
    public class NPCDialogueSet : ScriptableObject
    {
        public List<DialogueSequence> sequences;

        private Dictionary<DialogueContext, List<DialogueSequence>> _dialogueDict;

        public void Init()
        {
            _dialogueDict = new Dictionary<DialogueContext, List<DialogueSequence>>();

            foreach (var seq in sequences)
            {
                if (!_dialogueDict.ContainsKey(seq.context))
                    _dialogueDict[seq.context] = new List<DialogueSequence>();

                _dialogueDict[seq.context].Add(seq);
            }
        }
        public List<DialogContent> GetRandomDialogue(DialogueContext context)
        {
            if (_dialogueDict == null || _dialogueDict.Count == 0)
                Init();

            if (_dialogueDict.TryGetValue(context, out var list) && list.Count > 0)
            {
                var randomSeq = list[Random.Range(0, list.Count)];
                return randomSeq.lines;
            }

            return null;
        }
    }
    [System.Serializable]
    public class DialogueSequence
    {
        public DialogueContext context;
        public List<DialogContent> lines;
    }

    public class DialogContent
    {

        [TextArea(2, 5)]
        public string Text;          // Реплика
        public float delayAfter = 1f;
    }

}