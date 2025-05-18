using System.Collections.Generic;
using UnityEngine;

public class DecorObject : MonoBehaviour, IDynamicObject
{
    [SerializeField] private Renderer renderComponent;
    [SerializeField] private Animator animatorComponent;
    [SerializeField] private List<MonoBehaviour> logicScripts;

    private void Start()
    {
        if (renderComponent == null)
            renderComponent = GetComponent<Renderer>();
        if (animatorComponent == null)
            animatorComponent = GetComponent<Animator>();
        foreach (var script in transform.GetComponentsInChildren<MonoBehaviour>())
        {
            if (script != this)
                logicScripts.Add(script);
        }

    }

    public void SetRenderActive(bool active)
    {
        if (renderComponent != null)
            renderComponent.enabled = active;
    }

    public void SetAnimationActive(bool active)
    {
        if (animatorComponent != null)
            animatorComponent.enabled = active;
    }

    public void SetLogicActive(bool active)
    {
        foreach (var script in logicScripts)
        {
            if (script != null)
                script.enabled = active;
        }
    }
}
