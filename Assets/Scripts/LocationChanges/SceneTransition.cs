using GameWorld;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class SceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public LocationId targetLocation;     // � ����� ������� ���
    public string targetEntrance;     // ����� ����� � ����� �����

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        // ��������� ������� �����
        LocationManager.Instance.SetCurrentLocation(targetLocation);

        // ��������� "���� ������" ����� ��������
        PlayerPrefs.SetString("NextEntrance", targetEntrance);
        PlayerPrefs.Save();

        // ������ �����
        var scene = LocationManager.Instance.worldDatabase.GetScene(targetLocation);
        if (scene != null)
            SceneManager.LoadScene(scene);
    }
}
