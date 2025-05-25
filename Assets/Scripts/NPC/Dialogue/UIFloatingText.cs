using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.NPC.Dialogue
{
    public class UIFloatingText : MonoBehaviour
    {
        public TextMeshPro textMesh;
        public float lifetime = 2f;
        public float typingSpeed = 0.04f;

        public static void Create(Vector3 position, string text, float typingSpeed = 0.04f)
        {
            var prefab = Resources.Load<UIFloatingText>("UI/FloatingText");
            var instance = Instantiate(prefab, position, Quaternion.identity);
            instance.typingSpeed = typingSpeed;
            instance.StartCoroutine(instance.TypeTextWithDelayedDestroy(text));
        }

        private IEnumerator TypeTextWithDelayedDestroy(string text)
        {
            yield return StartCoroutine(TypeText(text));
            yield return new WaitForSeconds(lifetime);
            Destroy(gameObject);
        }

        private IEnumerator TypeText(string text)
        {
            textMesh.text = "";
            foreach (char c in text)
            {
                textMesh.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }

}
