using UnityEngine;

public class DynamicObjectManager : MonoBehaviour
{
    [Header("Радиусы активации")]
    public float renderRadius = 30f;
    public float animationRadius = 20f;
    public float logicRadius = 100f;

    private CircleCollider2D renderCollider;
    private CircleCollider2D animationCollider;
    private CircleCollider2D logicCollider;

    private void Awake()
    {
        // Создание коллайдеров на лету
        renderCollider = CreateCollider("RenderCollider", renderRadius, typeof(RenderActivation));
        animationCollider = CreateCollider("AnimationCollider", animationRadius, typeof(AnimationActivation));
        logicCollider = CreateCollider("LogicCollider", logicRadius, typeof(LogicActivation));
    }

    private CircleCollider2D CreateCollider(string name, float radius, System.Type activationType)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;

        var collider = go.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = radius;

        go.AddComponent(activationType); // Добавляем нужный обработчик

        return collider;
    }
}
