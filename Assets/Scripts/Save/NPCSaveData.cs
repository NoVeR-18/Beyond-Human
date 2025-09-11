using NPCEnums;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCSaveData
{
    public string npcId; // Уникальное имя или ID
    public Vector2 position;
    public NPCActivityType currentActivity;
    public string destinationId; // вместо NavTargetPoint
    public HouseData CurrentHouse;
    public int CurrentFloor;
    public bool isDead = false;
}

[System.Serializable]
public class SaveContainer
{
    public List<NPCSaveData> allNpcData = new List<NPCSaveData>();
}
[Serializable]
public class PlayerSaveData
{
    public Vector3 position;
    public Quaternion rotation;

    public int health;

}