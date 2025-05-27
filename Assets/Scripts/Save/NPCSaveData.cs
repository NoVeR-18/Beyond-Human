using NPCEnums;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCSaveData
{
    public string npcId; // Уникальное имя или ID
    public Vector2 position;
    public NPCActivityType currentActivity;
}

[System.Serializable]
public class NPCSaveContainer
{
    public List<NPCSaveData> allNpcData = new List<NPCSaveData>();
}