using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofAreaHider : MonoBehaviour
{
    public Tilemap WallsTilemap;                     // ����� Tilemap � �������
    public Tilemap floorTilemap;                     // ����� Tilemap � �������
    public BoundsInt roofArea;                      // ������� �����, ������� ��������
    public TileBase[] savedTiles;                  // ���������, ����� ������� ��� ������
    public BoxCollider2D roofAreaCollider;          // ��������� ��� ����������� ������� �����
    public void Start()
    {
        //roofAreaCollider = GetComponent<BoxCollider2D>();

        //// ����� ���������� � ������� �����������
        //Vector2 worldCenter = (Vector2)transform.position;

        //// ������� ���������� (����� ������ � ������ ������� �����)
        //Vector2 worldMin = worldCenter - (Vector2)FloorTilemap.size * 0.5f;
        //Vector2 worldMax = worldCenter + (Vector2)FloorTilemap.size * 0.5f;

        //// ��������� � ��������� ���������� ��������
        //Vector3Int cellMin = WallsTilemap.WorldToCell(worldMin);
        //Vector3Int cellMax = WallsTilemap.WorldToCell(worldMax);

        //// ������� ������ ������� (�� �������� +1, ����� �������� ����)
        //Vector3Int areaSize = cellMax - cellMin + Vector3Int.one;

        // ��������� �������
        //roofArea = new BoundsInt(cellMin, areaSize);
        //roofAreaCollider.size = new Vector2(FloorTilemap.size.x, FloorTilemap.size.y);
        roofAreaCollider = GetComponent<BoxCollider2D>();
        roofAreaCollider.isTrigger = true;

        // ������� �������� ������� ������
        BoundsInt bounds = floorTilemap.cellBounds;

        Vector3Int minPos = new Vector3Int(int.MaxValue, int.MaxValue, 0);
        Vector3Int maxPos = new Vector3Int(int.MinValue, int.MinValue, 0);

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (floorTilemap.HasTile(pos))
            {
                minPos = Vector3Int.Min(minPos, pos);
                maxPos = Vector3Int.Max(maxPos, pos);
            }
        }

        if (minPos.x > maxPos.x || minPos.y > maxPos.y)
        {
            Debug.LogWarning("�� ������� �� ������ ����� � tilemap");
            return;
        }

        // ������� +1 � maxPos, ��� ��� max �� ���������� � �������
        maxPos += Vector3Int.one;

        Vector3 worldMin = floorTilemap.CellToWorld(minPos);
        Vector3 worldMax = floorTilemap.CellToWorld(maxPos);
        Vector3 center = (worldMin + worldMax) / 2f;
        Vector3 size = worldMax - worldMin;

        // �������� ������ ��� ���������� ����
        size -= new Vector3(0.4f, 0.4f);

        roofAreaCollider.offset = floorTilemap.transform.InverseTransformPoint(center) - transform.localPosition;
        roofAreaCollider.size = size;

        savedTiles = WallsTilemap.GetTilesBlock(WallsTilemap.cellBounds);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ClearRoofArea(); // �������� ������ �����
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RestoreRoofArea(); // ���������� ������
        }
    }

    private void ClearRoofArea()
    {
        TileBase[] emptyTiles = new TileBase[savedTiles.Length];
        WallsTilemap.SetTilesBlock(WallsTilemap.cellBounds, emptyTiles);
    }

    private void RestoreRoofArea()
    {
        WallsTilemap.SetTilesBlock(WallsTilemap.cellBounds, savedTiles);
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