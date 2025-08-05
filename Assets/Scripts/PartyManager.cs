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
                //if (character.portrait != null) 
                //portraitName = character.portrait.name,
                skillDatas = new(),
                remainingSkillPoints = character.remainingSkillPoints,
                equipped = new(),
                mainHandItem = character.weaponMainHand?.itemName,
                offHandItem = character.weaponOffHand?.itemName
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
                portrait = LoadPortraitSprite(data.portraitName),
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

            activeMembers.Add(character);
        }
    }


    private Sprite LoadPortraitSprite(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
            return null;

        // Portrait должен быть загружен из Resources
        return Resources.Load<Sprite>($"Portraits/{spriteName}");
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
}

[System.Serializable]
public class EquippedItemData
{
    public EquipmentSlot slot;
    public string itemName;
}
