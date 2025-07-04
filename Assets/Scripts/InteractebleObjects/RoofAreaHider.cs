using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PolygonCollider2D))]
public class RoofAreaHider : MonoBehaviour
{
    public Tilemap floorTilemap;          // Tilemap с полом (по нему строится область)
    public Tilemap furnitureTilemap;
    public GameObject roofObject;         // Объект с крышей, который нужно скрывать

    public float offset = -0.2f; // Отступ внутрь (отрицательный)

    private PolygonCollider2D polygon;

    public void Start()
    {
        roofObject.SetActive(true);
        polygon = GetComponent<PolygonCollider2D>();
        GenerateOutlineColliderWithOffset();
        furnitureTilemap.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && roofObject != null)
        {
            furnitureTilemap.gameObject.SetActive(true);
            roofObject.SetActive(false);

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && roofObject != null)
        {
            roofObject.SetActive(true);
            furnitureTilemap.gameObject.SetActive(true);
        }
    }

    void GenerateOutlineColliderWithOffset()
    {
        if (floorTilemap == null)
        {
            Debug.LogError("Tilemap не назначен");
            return;
        }

        List<Vector2> outline = new();
        var bounds = floorTilemap.cellBounds;
        Vector2 tileSize = floorTilemap.cellSize;

        HashSet<(Vector2, Vector2)> edges = new();

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (!floorTilemap.HasTile(pos)) continue;

                Vector3 tileWorldPos = floorTilemap.CellToWorld(pos);
                Vector2 center = (Vector2)tileWorldPos + tileSize / 2;
                float w = tileSize.x / 2;
                float h = tileSize.y / 2;

                Vector2 topLeft = new Vector2(center.x - w, center.y + h);
                Vector2 topRight = new Vector2(center.x + w, center.y + h);
                Vector2 bottomLeft = new Vector2(center.x - w, center.y - h);
                Vector2 bottomRight = new Vector2(center.x + w, center.y - h);
                AddEdge(edges, topLeft, topRight);     // верх
                AddEdge(edges, topRight, bottomRight); // право
                AddEdge(edges, bottomRight, bottomLeft); // низ
                AddEdge(edges, bottomLeft, topLeft);   // лево
            }
        }

        // Преобразуем края в цепочку точек (outline)
        outline = TraceOutline(edges);

        // Смещаем точки наружу
        List<Vector2> offsetOutline = OffsetOutline(outline, offset);

        polygon.pathCount = 1;
        polygon.SetPath(0, offsetOutline.Select(p => (Vector2)transform.InverseTransformPoint(p)).ToArray());

        Debug.Log("Контур с отступом сгенерирован");
    }
    void AddEdge(HashSet<(Vector2, Vector2)> edges, Vector2 a, Vector2 b)
    {
        var edge = (a, b);
        var reverseEdge = (b, a);

        if (edges.Contains(reverseEdge))
            edges.Remove(reverseEdge);
        else
            edges.Add(edge);
    }
    List<Vector2> TraceOutline(HashSet<(Vector2, Vector2)> edges)
    {
        if (edges.Count == 0) return new List<Vector2>();

        var outline = new List<Vector2>();
        var edgeList = new List<(Vector2, Vector2)>(edges);

        // Начнем с первой точки
        var currentEdge = edgeList[0];
        outline.Add(currentEdge.Item1);
        outline.Add(currentEdge.Item2);

        edgeList.RemoveAt(0);

        Vector2 currentPoint = currentEdge.Item2;

        while (edgeList.Count > 0)
        {
            var nextEdgeIndex = edgeList.FindIndex(e => e.Item1 == currentPoint);
            if (nextEdgeIndex == -1)
            {
                // Может быть, ребро задано в обратном порядке
                nextEdgeIndex = edgeList.FindIndex(e => e.Item2 == currentPoint);
                if (nextEdgeIndex == -1) break; // не замкнутый контур
                var reversed = (edgeList[nextEdgeIndex].Item2, edgeList[nextEdgeIndex].Item1);
                outline.Add(reversed.Item2);
                currentPoint = reversed.Item2;
            }
            else
            {
                var nextEdge = edgeList[nextEdgeIndex];
                outline.Add(nextEdge.Item2);
                currentPoint = nextEdge.Item2;
            }

            edgeList.RemoveAt(nextEdgeIndex);
        }

        return outline;
    }
    List<Vector2> OffsetOutline(List<Vector2> original, float offsetAmount)
    {
        List<Vector2> offsetPoints = new();
        int count = original.Count;

        for (int i = 0; i < count; i++)
        {
            Vector2 prev = original[(i - 1 + count) % count];
            Vector2 curr = original[i];
            Vector2 next = original[(i + 1) % count];

            Vector2 dir1 = (curr - prev).normalized;
            Vector2 dir2 = (next - curr).normalized;

            Vector2 normal1 = new Vector2(-dir1.y, dir1.x);
            Vector2 normal2 = new Vector2(-dir2.y, dir2.x);

            Vector2 offsetDir = (normal1 + normal2).normalized;
            offsetPoints.Add(curr + offsetDir * offsetAmount);
        }

        return offsetPoints;
    }
}
[CustomEditor(typeof(RoofAreaHider))]
class RoofAreaHiderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoofAreaHider roofAreaHider = (RoofAreaHider)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Area"))
        {
            roofAreaHider.Start();
        }
    }
}