using UnityEngine;

public class FractionsWindow : UIWindow
{
    [SerializeField] private GameObject content;

    public override void Show()
    {
        content.SetActive(true);
        RefreshUI();
    }
    public override void Hide() => content.SetActive(false);

    [SerializeField] private Transform container;
    [SerializeField] private FactionUIElement factionPrefab;
    [SerializeField] private FactionData playerFaction;

    public void RefreshUI()
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        var allFactions = FactionManager.Instance.factions;

        foreach (var faction in allFactions)
        {
            if (faction.factionType == NPCEnums.FactionType.Player) continue;

            var element = Instantiate(factionPrefab, container);
            element.Setup(faction, playerFaction.GetAttitudeTowards(faction));
        }
    }
}
