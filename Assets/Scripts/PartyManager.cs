using BattleSystem;
using GameUtils.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance { get; private set; }

    [SerializeField]
    private List<Character> activeMembers = new();
    public List<Character> GetActiveMembers() => activeMembers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        StartCoroutine(LoadParty());
    }

    public void SaveParty()
    {
        SaveUtils.EnsureDirectory();

        PartySaveData saveData = new();

        foreach (var character in activeMembers)
        {
            CharacterSaveData data = new()
            {
                characterName = character.characterName,
                skillDatas = new(),
                remainingSkillPoints = character.remainingSkillPoints,
                equipped = new(),
                mainHandItem = character.weaponMainHand?.itemName,
                offHandItem = character.weaponOffHand?.itemName,

                // Если есть battleCharacter — сохраняем его имя
                battleCharacterName = character.battleCharacter != null
        ? character.battleCharacter.name + ".prefab"
        : null
            };

            // Словарь -> список для сериализации
            foreach (var kvp in character.equippedItems)
            {
                if (kvp.Value != null)
                {
                    data.equipped.Add(new EquippedItemData
                    {
                        slot = kvp.Key,
                        itemName = kvp.Value.itemName
                    });
                }
            }
            foreach (var skill in character.equippedSkills)
            {
                data.skillDatas.Add(skill.itemName);
            }

            saveData.characters.Add(data);
        }

        File.WriteAllText(SaveUtils.PartyFile, JsonUtility.ToJson(saveData, true));
    }
    private IEnumerator LoadParty()
    {
        if (!File.Exists(SaveUtils.PartyFile))
            yield break;

        string json = File.ReadAllText(SaveUtils.PartyFile);
        PartySaveData saveData = JsonUtility.FromJson<PartySaveData>(json);
        activeMembers.Clear();

        foreach (var data in saveData.characters)
        {
            Character character = new()
            {
                characterName = data.characterName,
                remainingSkillPoints = data.remainingSkillPoints,
                equippedSkills = new(),
                equippedItems = new()
            };

            // Загружаем экипировку из списка обратно в словарь
            foreach (var eq in data.equipped)
            {
                var handle = Addressables.LoadAssetAsync<Equipment>(eq.itemName);
                yield return handle;

                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    character.equippedItems[eq.slot] = handle.Result;
                }
                else
                {
                    Debug.LogWarning($"Не удалось загрузить экипировку: {eq.itemName}");
                }
            }
            foreach (var skill in data.skillDatas)
            {
                var handle = Addressables.LoadAssetAsync<SkillData>(skill);
                yield return handle;
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    character.equippedSkills.Add(handle.Result);
                }
                else
                {
                    Debug.LogWarning($"Не удалось загрузить экипировку: {skill}");
                }
            }

            // Основное оружие
            if (!string.IsNullOrEmpty(data.mainHandItem))
            {
                var handle = Addressables.LoadAssetAsync<Weapon>(data.mainHandItem);
                yield return handle;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                    character.weaponMainHand = handle.Result;
                else
                    Debug.LogWarning($"Не удалось загрузить оружие (main hand): {data.mainHandItem}");
            }

            // Вторичное оружие
            if (!string.IsNullOrEmpty(data.offHandItem))
            {
                var handle = Addressables.LoadAssetAsync<Weapon>(data.offHandItem);
                yield return handle;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                    character.weaponOffHand = handle.Result;
                else
                    Debug.LogWarning($"Не удалось загрузить оружие (off hand): {data.offHandItem}");
            }
            if (!string.IsNullOrEmpty(data.battleCharacterName))
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(data.battleCharacterName);
                yield return handle;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var prefab = handle.Result;
                    var bc = prefab.GetComponent<BattleCharacter>();
                    if (bc != null)
                    {
                        character.battleCharacter = bc;
                        character.portrait = prefab.GetComponent<SpriteRenderer>()?.sprite;
                    }
                    else
                    {
                        Debug.LogWarning($"Префаб {data.battleCharacterName} не содержит компонент BattleCharacter");
                    }
                }
                else
                {
                    Debug.LogWarning($"Не удалось загрузить боевой префаб: {data.battleCharacterName}");
                }
            }
            activeMembers.Add(character);
        }
    }

    public void AddMember(Character character)
    {
        if (!activeMembers.Contains(character) && activeMembers.Count < 4)
            activeMembers.Add(character);
    }

    public void RemoveMember(Character character)
    {
        if (activeMembers.Contains(character))
            activeMembers.Remove(character);
    }

    public List<BattleParticipantData> CharacterToBattleParticiant()
    {
        List<BattleParticipantData> participants = new();

        foreach (var character in activeMembers)
        {
            BattleParticipantData participant = new()
            {
                prefab = character.battleCharacter.gameObject,
                stats = new CharacterStats(),
            };
            foreach (var kvp in character.equippedItems)
            {
                if (kvp.Value != null)
                {
                    // Добавляем сопротивления
                    participant.stats.airResistance += kvp.Value.airResistance;
                    participant.stats.waterResistance += kvp.Value.waterResistance;
                    participant.stats.fireResistance += kvp.Value.fireResistance;
                    participant.stats.earthResistance += kvp.Value.earthResistance;
                    participant.stats.electricResistance += kvp.Value.electricResistance;
                    participant.stats.iceResistance += kvp.Value.iceResistance;
                    participant.stats.poisonResistance += kvp.Value.poisonResistance;
                    participant.stats.bluntResistance += kvp.Value.bluntResistance;
                    participant.stats.piercingResistance += kvp.Value.piercingResistance;
                    participant.stats.curseResistance += kvp.Value.curseResistance;
                    participant.stats.holyResistance += kvp.Value.holyResistance;

                }
            }
            foreach (var weapon in new[] { character.weaponMainHand, character.weaponOffHand })
            {
                if (weapon != null)
                {
                    participant.stats.airDamage += weapon.airDamage;
                    participant.stats.waterDamage += weapon.waterDamage;
                    participant.stats.fireDamage += weapon.fireDamage;
                    participant.stats.earthDamage += weapon.earthDamage;
                    participant.stats.electricDamage += weapon.electricDamage;
                    participant.stats.iceDamage += weapon.iceDamage;
                    participant.stats.poisonDamage += weapon.poisonDamage;
                    participant.stats.bluntDamage += weapon.bluntDamage;
                    participant.stats.piercingDamage += weapon.piercingDamage;
                    participant.stats.curseDamage += weapon.curseDamage;
                    participant.stats.holyDamage += weapon.holyDamage;
                }
            }


            foreach (var skill in character.equippedSkills)
            {
                participant.abilities.Add(skill.ability);
            }
            participants.Add(participant);
        }

        return participants;
    }

    private void OnApplicationQuit()
    {
        SaveParty();
    }
}

[System.Serializable]
public class PartySaveData
{
    public List<CharacterSaveData> characters = new();
}

[System.Serializable]
public class CharacterSaveData
{
    public string characterName;
    public string portraitName; // имя ассета спрайта
    public List<string> skillDatas = new();
    public List<EquippedItemData> equipped = new();
    public string mainHandItem;
    public string offHandItem;

    public int remainingSkillPoints;


    public string battleCharacterName;
}

[System.Serializable]
public class EquippedItemData
{
    public EquipmentSlot slot;
    public string itemName;
}
