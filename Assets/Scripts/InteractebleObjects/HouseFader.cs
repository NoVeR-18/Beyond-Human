using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PolygonCollider2D))]
public class HouseFader : MonoBehaviour
{
    public float transparentAlpha = 0.3f;
    public float fadeDuration = 0.3f;

    public Tilemap roofTilemap; // �����
    public Tilemap floorTilemap; // ���
    [SerializeField]
    private List<SpriteRenderer> spriteRenderers = new();
    [SerializeField]
    private List<TilemapRenderer> tilemapRenderers = new();
    private Coroutine fadeRoutine;
    private PolygonCollider2D polygon;
    public void Start()
    {
        // �������� ��� SpriteRenderer
        spriteRenderers.AddRange(GetComponentsInChildren<SpriteRenderer>(includeInactive: true));

        // �������� ��� TilemapRenderer
        tilemapRenderers.AddRange(GetComponentsInChildren<TilemapRenderer>(includeInactive: true));
        polygon = GetComponent<PolygonCollider2D>();
        // ���������� ������� �� ������� ����� ����� ���
        GenerateOffsetTilesCollider();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeToAlpha(transparentAlpha));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeToAlpha(1f));
        }
    }

    IEnumerator FadeToAlpha(float targetAlpha)
    {
        float elapsed = 0f;

        Dictionary<SpriteRenderer, float> spriteStartAlpha = new();
        Dictionary<TilemapRenderer, float> tilemapStartAlpha = new();

        foreach (var sr in spriteRenderers)
            if (sr != null)
                spriteStartAlpha[sr] = sr.color.a;

        foreach (var tr in tilemapRenderers)
            if (tr != null)
                tilemapStartAlpha[tr] = tr.material.color.a;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            foreach (var sr in spriteRenderers)
            {
                if (sr == null) continue;
                Color c = sr.color;
                c.a = Mathf.Lerp(spriteStartAlpha[sr], targetAlpha, t);
                sr.color = c;
            }

            foreach (var tr in tilemapRenderers)
            {
                if (tr == null) continue;
                Color c = tr.material.color;
                c.a = Mathf.Lerp(tilemapStartAlpha[tr], targetAlpha, t);
                tr.material.color = c;
            }

            yield return null;
        }

        // ��������� ���������
        foreach (var sr in spriteRenderers)
        {
            if (sr == null) continue;
            Color c = sr.color;
            c.a = targetAlpha;
            sr.color = c;
        }

        foreach (var tr in tilemapRenderers)
        {
            if (tr == null) continue;
            Color c = tr.material.color;
            c.a = targetAlpha;
            tr.material.color = c;
        }
    }

    void GenerateOffsetTilesCollider()
    {
        if (floorTilemap == null || roofTilemap == null)
        {
            Debug.LogError("���� �� Tilemap �� ��������");
            return;
        }

        List<Vector2[]> tileRects = new();
        var roofBounds = roofTilemap.cellBounds;

        Vector2 tileSize = roofTilemap.cellSize;

        // �������� �� ���� ������� � �������� roofTilemap
        for (int x = roofBounds.xMin; x < roofBounds.xMax; x++)
        {
            for (int y = roofBounds.yMin; y < roofBounds.yMax; y++)
            {
                Vector3Int roofPos = new Vector3Int(x, y, 0);
                Vector3Int floorPos = new Vector3Int(x, y, 0);

                // ���������, ���� �� ������ � roofTilemap, �� ��� ������ � floorTilemap
                if (roofTilemap.HasTile(roofPos) && !floorTilemap.HasTile(floorPos))
                {
                    // �������� ������� ������� ������ � roofTilemap
                    Vector3 roofWorldPos = roofTilemap.CellToWorld(roofPos);
                    Vector2 center = (Vector2)roofWorldPos + tileSize / 2;

                    float w = tileSize.x / 2;
                    float h = tileSize.y / 2;

                    // ������� ������� ��� ����������
                    tileRects.Add(new Vector2[]
                    {
                    new Vector2(center.x - w, center.y - h),
                    new Vector2(center.x - w, center.y + h),
                    new Vector2(center.x + w, center.y + h),
                    new Vector2(center.x + w, center.y - h),
                    });
                }
            }
        }

        // ������������� ��������� ���� � PolygonCollider2D
        polygon.pathCount = tileRects.Count;
        for (int i = 0; i < tileRects.Count; i++)
        {
            Vector2[] localPath = new Vector2[tileRects[i].Length];
            for (int j = 0; j < tileRects[i].Length; j++)
            {
                localPath[j] = transform.InverseTransformPoint(tileRects[i][j]);
            }
            polygon.SetPath(i, localPath);
        }

        Debug.Log($"������� {polygon.pathCount} ���������� ����������� ��� ������ � roofTilemap, ������� ��� � floorTilemap");
    }
}
[CustomEditor(typeof(HouseFader))]
class HouseAreaHiderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HouseFader roofAreaHider = (HouseFader)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Area"))
        {
            roofAreaHider.Start();
        }
    }
}