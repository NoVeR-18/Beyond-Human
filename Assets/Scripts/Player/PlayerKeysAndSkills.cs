using System.Collections.Generic;
using UnityEngine;

public class PlayerKeysAndSkills : MonoBehaviour
{
    public int lockpickingLevel = 1;
    private HashSet<string> keys = new HashSet<string>();

    public bool HasKey(string keyID)
    {
        return keys.Contains(keyID);
    }

    public void AddKey(string keyID)
    {
        keys.Add(keyID);
    }
}
