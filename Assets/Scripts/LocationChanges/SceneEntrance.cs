using UnityEngine;

public class SceneEntrance : MonoBehaviour
{
    public string entranceId;

    private void Start()
    {
        string nextEntrance = PlayerPrefs.GetString("NextEntrance", entranceId);
        if (entranceId == nextEntrance)
        {
            SaveSystem.Instance.PlayerEntrance(transform);
            PlayerPrefs.SetString("NextEntrance", "");
        }
    }
}
