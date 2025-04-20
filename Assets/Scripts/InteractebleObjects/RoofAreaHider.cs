using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofAreaHider : MonoBehaviour
{
    public Tilemap WallsTilemap;                     // Общий Tilemap с крышами
    public Tilemap floorTilemap;                     // Общий Tilemap с крышами
    public BoundsInt roofArea;                      // Область крыши, которую скрывать
    public TileBase[] savedTiles;                  // Сохраняем, чтобы вернуть при выходе
    public BoxCollider2D roofAreaCollider;          // Коллайдер для определения области крыши
    public void Start()
    {
        //roofAreaCollider = GetComponent<BoxCollider2D>();

        //// Центр коллайдера в мировых координатах
        //Vector2 worldCenter = (Vector2)transform.position;

        //// Границы коллайдера (левая нижняя и правая верхняя точки)
        //Vector2 worldMin = worldCenter - (Vector2)FloorTilemap.size * 0.5f;
        //Vector2 worldMax = worldCenter + (Vector2)FloorTilemap.size * 0.5f;

        //// Переводим в клеточные координаты тайлмапа
        //Vector3Int cellMin = WallsTilemap.WorldToCell(worldMin);
        //Vector3Int cellMax = WallsTilemap.WorldToCell(worldMax);

        //// Считаем размер области (не забываем +1, чтобы включить край)
        //Vector3Int areaSize = cellMax - cellMin + Vector3Int.one;

        // Назначаем область
        //roofArea = new BoundsInt(cellMin, areaSize);
        //roofAreaCollider.size = new Vector2(FloorTilemap.size.x, FloorTilemap.size.y);
        roofAreaCollider = GetComponent<BoxCollider2D>();
        roofAreaCollider.isTrigger = true;

        // Получим реальные занятые клетки
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
            Debug.LogWarning("Не найдено ни одного тайла в tilemap");
            return;
        }

        // Добавим +1 к maxPos, так как max не включается в размеры
        maxPos += Vector3Int.one;

        Vector3 worldMin = floorTilemap.CellToWorld(minPos);
        Vector3 worldMax = floorTilemap.CellToWorld(maxPos);
        Vector3 center = (worldMin + worldMax) / 2f;
        Vector3 size = worldMax - worldMin;

        // Уменьшим размер для безопасной зоны
        size -= new Vector3(0.4f, 0.4f);

        roofAreaCollider.offset = floorTilemap.transform.InverseTransformPoint(center) - transform.localPosition;
        roofAreaCollider.size = size;

        savedTiles = WallsTilemap.GetTilesBlock(WallsTilemap.cellBounds);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ClearRoofArea(); // Скрываем плитки крыши
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RestoreRoofArea(); // Возвращаем плитки
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