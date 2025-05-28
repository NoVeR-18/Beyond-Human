using NPCEnums;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCSaveData
{
    public string npcId; // Уникальное имя или ID
    public Vector2 position;
    public NPCActivityType currentActivity;
    public HouseData CurrentHouse;
    public int CurrentFloor;
}

[System.Serializable]
public class SaveContainer
{
    public List<NPCSaveData> allNpcData = new List<NPCSaveData>();
}