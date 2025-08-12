using UnityEngine;

/* An Item that can be equipped to increase armor/damage. */

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Equipments")]
public class Equipment : Item
{

    public EquipmentSlot equipSlot;     // What slot to equip it in
    public int healthPoints;
    public int airResistance;
    public int waterResistance;
    public int fireResistance;
    public int earthResistance;
    public int electricResistance;
    public int iceResistance;
    public int poisonResistance;
    public int bluntResistance;
    public int piercingResistance;
    public int curseResistance;
    public int holyResistance;
    public int magicResistance;

    public override void Use()
    {
    }

}

public enum EquipmentSlot { Helmet, Chest, Pants, Shoes, Gloves, Amulet }
