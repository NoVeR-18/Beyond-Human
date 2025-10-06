using Assets.Scripts.Player;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private List<EquipmentSprite> slots;
    [SerializeField] private List<WeaponSprite> weaponSlots;

    private Direction currentDirection = Direction.Front;

    private void Start()
    {
        if (slots != null && slots.Count > 0)
        {
            slots.RemoveAll(slot => slot == null);
        }

        if (weaponSlots != null && weaponSlots.Count > 0)
        {
            weaponSlots.RemoveAll(slot => slot == null);
        }
    }


    public void SetDirection(Direction dir)
    {
        currentDirection = dir;

        foreach (var slot in slots)
        {
            if (slot != null)
                slot.UpdateDirection(currentDirection);
        }
    }

    public void EquipItem(EquipmentSlot slotType, EquipmentSprites sprites)
    {
        var slot = GetSlot(slotType);
        if (slot != null)
        {
            slot.SetEquipment(sprites);
            slot.UpdateDirection(currentDirection);
        }
        else
        {
            Debug.LogWarning($"Slot {slotType} not found in EquipmentManager!");
        }
    }
    public void EquipWeapon(int index, EquipmentSprites sprites)
    {
        if (index < 0 || index >= weaponSlots.Count)
        {
            Debug.LogWarning($"Index {index} out diapasone of weaponSlots!");
            return;
        }

        var slot = weaponSlots[index];
        slot.SetEquipment(sprites);
        slot.UpdateDirection(currentDirection);

    }


    public void UnequipItem(EquipmentSlot slotType)
    {
        var slot = GetSlot(slotType);
        if (slot != null)
        {
            slot.SetEquipment(null);
            slot.UpdateDirection(currentDirection);
        }
    }

    private EquipmentSprite GetSlot(EquipmentSlot slotType)
    {
        foreach (var slot in slots)
        {
            if (slot.SlotType == slotType)
                return slot;
        }
        return null;
    }
}