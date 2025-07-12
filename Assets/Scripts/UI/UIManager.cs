using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [System.Serializable]
    public struct WindowBinding
    {
        public UISection section;
        public UIWindow window;
    }

    public List<WindowBinding> windows;
    public DialogueWindow dialogueWindow;
    public TradeWindow tradeWindow;
    public ChestWindow chestWindow;


    private Dictionary<UISection, UIWindow> _windowMap;
    private UISection? _currentSection;

    private Dictionary<KeyCode, UISection> _hotkeys = new Dictionary<KeyCode, UISection>()
    {
        { KeyCode.P, UISection.Party },
        { KeyCode.I, UISection.Inventory },
        { KeyCode.M, UISection.Map },
        { KeyCode.Q, UISection.Quest },
        { KeyCode.F, UISection.Fractions },
        { KeyCode.B, UISection.Abilities }
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _windowMap = new Dictionary<UISection, UIWindow>();
        CloseTabs();
    }

    private void CloseTabs()
    {
        foreach (var w in windows)
        {
            _windowMap[w.section] = w.window;
            w.window.Hide();
        }
        dialogueWindow.HideDialogue();
        tradeWindow.Hide();
        chestWindow.Hide();
    }

    void Update()
    {
        foreach (var pair in _hotkeys)
        {
            if (Input.GetKeyDown(pair.Key))
            {
                // Если уже открыта — закрыть
                if (_currentSection != null && _currentSection == pair.Value)
                {
                    if (_windowMap.TryGetValue(_currentSection.Value, out var current))
                    {
                        current.Hide();
                        _currentSection = null;
                    }
                }
                else
                {
                    foreach (var w in windows)
                    {
                        _windowMap[w.section] = w.window;
                        w.window.Hide();
                    }
                    dialogueWindow.HideDialogue();
                    tradeWindow.Hide();
                    chestWindow.Hide();
                    OpenSection(pair.Value);
                }
                break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTabs();
        }

    }
    public void OpenSection(UISection section)
    {
        if (_currentSection != null && _windowMap.TryGetValue(_currentSection.Value, out var current))
        {
            current.Hide();
        }

        if (_windowMap.TryGetValue(section, out var newWindow))
        {
            newWindow.Show();
            _currentSection = section;
        }
    }
}
