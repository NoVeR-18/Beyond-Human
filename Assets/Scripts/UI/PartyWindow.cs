using System;
using System.Collections.Generic;
using UnityEngine;

public class PartyWindow : UIWindow
{
    [SerializeField] private GameObject content;

    [Header("Mode Switch")]
    [SerializeField] private ItemsInventoryPanel gearPanelRoot;
    [SerializeField] private SkillInventoryPanel skillsPanelRoot;

    [Header("Party Members")]
    [SerializeField] private Transform characterPanelContainer;
    [SerializeField] private PartyCharacterPanel characterPanelPrefab;

    private List<PartyCharacterPanel> activePanels = new();

    public static Action UpdateItems;

    public override void Show()
    {
        content.SetActive(true);
        ShowGearMode();
        UpdateUI();
        var v = PartyManager.Instance.CharacterToBattleParticiant();
    }

    public override void Hide()
    {
        content.SetActive(false);
    }

    public void ShowGearMode()
    {
        gearPanelRoot.gameObject.SetActive(true);
        skillsPanelRoot.gameObject.SetActive(false);
        UpdateItems = null;
        UpdateItems += gearPanelRoot.UpdateUI;  // ��������� ��������� ��� ������������ �� ����� ����������
    }

    public void ShowSkillsMode()
    {
        gearPanelRoot.gameObject.SetActive(false);
        skillsPanelRoot.gameObject.SetActive(true);
        UpdateItems = null;
        UpdateItems += skillsPanelRoot.UpdateUI;  // ��������� ��������� ��� ������������ �� ����� ����������
    }

    private void UpdateUI()
    {

        foreach (var panel in activePanels)
            Destroy(panel.gameObject);
        activePanels.Clear();

        var members = PartyManager.Instance.GetActiveMembers(); // ���������� �� 4-� ����������

        foreach (var character in members)
        {
            var panel = Instantiate(characterPanelPrefab, characterPanelContainer);
            panel.SetData(character);
            activePanels.Add(panel);
        }
    }

}
