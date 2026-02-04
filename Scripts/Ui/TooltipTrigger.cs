

using System;
using BJect;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [NonSerialized, Inject] public StaticTooltip tooltip;

    public string title;
    public string desc;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Show(title, desc);
        pointerStay = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
        pointerStay = false;
    }

    [NonSerialized] public bool pointerStay;

    public void Update()
    {
        if (pointerStay)
        {
            tooltip.Set(title, desc);
        }
    }
}