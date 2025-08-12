using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Weapon")]
public class Weapon : Item
{
    public int airDamage;
    public int waterDamage;
    public int fireDamage;
    public int earthDamage;
    public int electricDamage;
    public int iceDamage;
    public int poisonDamage;
    public int bluntDamage;
    public int piercingDamage;
    public int curseDamage;
    public int holyDamage;
    public int magicDamage;

    public override void Use()
    {

    }
}
