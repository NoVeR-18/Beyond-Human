using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class AbilityBarUI : MonoBehaviour
    {
        [SerializeField] private Transform abilityContainer;
        [SerializeField] private GameObject abilitySlotPrefab;

        public void Setup(List<AbilityData> abilities)
        {
            foreach (Transform child in abilityContainer) Destroy(child.gameObject);

            foreach (var ability in abilities)
            {
                var slot = Instantiate(abilitySlotPrefab, abilityContainer);
                slot.GetComponent<AbilitySlotUI>().Init(ability);
            }
        }
    }


}