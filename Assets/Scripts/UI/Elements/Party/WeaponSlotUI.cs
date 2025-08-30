using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private bool isMainHand;

    private Weapon equippedWeapon;
    private Character ownerCharacter;

    public void SetWeapon(Weapon weapon, Character character, bool mainWeapon = true)
    {
        equippedWeapon = weapon;
        ownerCharacter = character;
        isMainHand = mainWeapon;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (equippedWeapon != null)
        {
            weaponIcon.sprite = equippedWeapon.icon;
            weaponNameText.text = equippedWeapon.itemName;
        }
        else
        {
            weaponIcon.sprite = null;
            weaponNameText.text = "Empty";
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var drag = DragManager.instance.GetDraggedItem();
        if (drag != null && drag is Weapon weapon)
        {
            // Если уже что-то экипировано — вернуть в инвентарь
            if (equippedWeapon != null)
                Inventory.Instance.Add(equippedWeapon);

            // Убираем старое оружие из персонажа
            if (isMainHand)
                ownerCharacter.weaponMainHand = null;
            else
                ownerCharacter.weaponOffHand = null;

            // Убираем из инвентаря и назначаем
            Inventory.Instance.Remove(weapon);
            ownerCharacter.EquipWeapon(weapon, isMainHand);
            equippedWeapon = weapon;
            UpdateUI();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (equippedWeapon != null)
        {
            var character = GetComponentInParent<PartyCharacterPanel>().GetCharacter();
            character.UnequipItem(equippedWeapon);
            Inventory.Instance.Add(equippedWeapon);
            equippedWeapon = null;
            UpdateUI();
            PartyWindow.UpdateItems?.Invoke();
        }
    }
}
