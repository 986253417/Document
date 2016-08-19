using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// add this to new controller Canvas for monitor the major UI event
// don't forget to init the controllerId and Register the callback
[RequireComponent(typeof(Canvas))]
public class GameCanvasEventMonitor : MonoBehaviour
{
    public EControllerIndex controllerId;
    private List<ISubUIEventHandler> SubUIMonitor = new List<ISubUIEventHandler>();
    public Action<EControllerIndex, BaseEventData> OnUIWidgetClick;
    public Action<EControllerIndex, BaseEventData> OnUIWidgetValueChange;
    private bool initialized = false;


    void OnEnable()
    {
        if (!initialized)
        {
            RegisterSelectableEvents();
            initialized = true;
        }
        // set listener
        var uiEventManager = FindObjectOfType<GameUIEventManager>();
        if(uiEventManager != null)
            uiEventManager.SetCanvasEventMonitor(this);        
    }

    void OnDisable()
    {
        OnUIWidgetClick = null;
        OnUIWidgetValueChange = null;
        SubUIMonitor.Clear();
    }

    public void RegisterSubEventHander(ISubUIEventHandler eventHandle)
    {
        if (!SubUIMonitor.Contains(eventHandle))
        {
            SubUIMonitor.Add(eventHandle);
        }
    }

    public void UnRegisterSubEventHander(ISubUIEventHandler eventHandle)
    {
        if (SubUIMonitor.Contains(eventHandle))
        {
            SubUIMonitor.Remove(eventHandle);
        }
    }
    
    private void OnClick(BaseEventData data)
    {
        if (null != OnUIWidgetClick)
        {
            OnUIWidgetClick(controllerId, data);
        }

        for (int i = 0; i < SubUIMonitor.Count; i++)
        {
            SubUIMonitor[i].OnUIWidgetClick(data);
        }
    }

    private void OnValueChange(BaseEventData data)
    {
        if (null != OnUIWidgetValueChange)
        {
            OnUIWidgetValueChange(controllerId, data);
        }

        for (int i = 0; i < SubUIMonitor.Count; i++)
        {
            SubUIMonitor[i].OnUIWidgetValueChange(data);
        }
    }
    //Register Event
    private void RegisterSelectableEvents()
    {
        Selectable[] selectables = GetComponentsInChildren<Selectable>(true);
        foreach (var selectable in selectables)
        {
            EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
            if (null == trigger)
                trigger = selectable.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            if (selectable is Button)
            {
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener(OnClick);
            }
            else if (selectable is Slider)
            {
                entry.eventID = EventTriggerType.Drag;
                entry.callback.AddListener(OnValueChange);
            }
            else if (selectable is Toggle)
            {
                entry.eventID = EventTriggerType.UpdateSelected;
                entry.callback.AddListener(OnValueChange);
            }

            trigger.triggers.Add(entry);
        }
    }
}


