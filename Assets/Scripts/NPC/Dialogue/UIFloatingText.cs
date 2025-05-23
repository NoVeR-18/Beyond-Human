using TMPro;
using UnityEngine;

namespace Assets.Scripts.NPC.Dialogue
{
    public class UIFloatingText : MonoBehaviour
    {
        public TextMeshPro textMesh;
        public float lifetime = 2f;

        public static void Create(Vector3 position, string text)
        {
            var prefab = Resources.Load<UIFloatingText>("UI/FloatingText");
            var instance = Instantiate(prefab, position, Quaternion.identity);
            instance.textMesh.text = text;
            Destroy(instance.gameObject, instance.lifetime);
        }
    }

}
