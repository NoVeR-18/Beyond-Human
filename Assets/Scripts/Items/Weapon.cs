using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Weapon")]
public class Weapon : Item
{
    public int damageModifier;

    // Called when pressed in the inventory
    public override void Use()
    {
        //EquipmentManager.instance.Equip(this);	// Equip
        //RemoveFromInventory();	// Remove from inventory
    }
}
