using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SwimPromptUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Header("References")]
    [SerializeField] private Tilemap waterTilemap; // Вода (Tilemap)
    private PlayerController player;

    private Vector3Int targetCell;

    void Start()
    {
        panel.SetActive(false);
        player = FindAnyObjectByType<PlayerController>();

        if (waterTilemap == null)
            waterTilemap = FindAnyObjectByType<GridHierarchy>().background;

        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
    }

    /// <summary>
    /// Показывает окно подтверждения при клике по воде.
    /// </summary>
    public void ShowPrompt(Vector3Int cell)
    {
        targetCell = cell;
        panel.SetActive(true);
    }

    private void OnYesClicked()
    {
        if (player == null || waterTilemap == null)
        {
            Debug.LogWarning("SwimPromptUI: Не назначен PlayerController или Tilemap воды.");
            panel.SetActive(false);
            return;
        }

        // Переводим клетку воды в мировые координаты центра тайла
        Vector3 worldPos = waterTilemap.GetCellCenterWorld(targetCell);

        if (player != null && waterTilemap != null)
            player.EnterBoatMode(worldPos);

        panel.SetActive(false);
    }

    private void OnNoClicked()
    {
        panel.SetActive(false);
    }
}
