using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterTilemapInteractor : MonoBehaviour
{
    [SerializeField] private Tilemap waterTilemap;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SwimPromptUI swimPrompt;
    private void Start()
    {
        if (waterTilemap == null)
            waterTilemap = FindAnyObjectByType<GridHierarchy>().background;

        if (mainCamera == null)
            mainCamera = Camera.main;
        if (swimPrompt == null)
            swimPrompt = FindAnyObjectByType<SwimPromptUI>();

    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // À Ã
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = waterTilemap.WorldToCell(worldPos);
            if (swimPrompt != null && waterTilemap != null)
                if (waterTilemap.HasTile(cellPos))
                {
                    swimPrompt.ShowPrompt(cellPos);
                }
        }
    }
}
