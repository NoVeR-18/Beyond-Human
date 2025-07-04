using UnityEngine;

/* An Item that can be consumed. So far that just means regaining health */

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Consumable")]
public class Consumable : Item
{

    public int healthGain;		// How much Health?
    public int hungerGain;		// How much Hunger?
    public int thirstyGain;      // How much Thirsty?

    // This is called when pressed in the inventory
    public override void Use()
    {
        //RemoveFromInventory();  // Remove the item after use
    }

}
