using Assets.Scripts.NPC;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class NPCRender : MonoBehaviour, IDynamicObject
{
    [SerializeField] private Renderer renderComponent;
    [SerializeField] private Animator animatorComponent;
    [SerializeField] private List<MonoBehaviour> logicScripts;

    private void Awake()
    {
        if (renderComponent == null)
            renderComponent = GetComponent<Renderer>();
        if (animatorComponent == null)
            animatorComponent = GetComponent<Animator>();
        foreach (var script in transform.GetComponentsInChildren<MonoBehaviour>())
        {
            if (script != this && !(script as NPCController))
                logicScripts.Add(script);
        }
        SetRenderActive(false);
        SetAnimationActive(false);
        SetLogicActive(false);
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
