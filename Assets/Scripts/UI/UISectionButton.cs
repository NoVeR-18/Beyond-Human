using UnityEngine;
using UnityEngine.UI;

public class UISectionButton : MonoBehaviour
{
    public UISection section;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => UIManager.Instance.OpenSection(section));
    }
}
