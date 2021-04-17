using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UIEventListener Expand Version 1
/// </summary>
public class UIEventListener : EventTrigger
{
    public delegate void VoidDelegate(GameObject go);

    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VoidDelegate onDrag;
    public VoidDelegate onDrop;
    public VoidDelegate onDeselect;
    public VoidDelegate onScroll;
    public VoidDelegate onMove;
    public VoidDelegate onInitializePotentialDrag;
    public VoidDelegate onBeginDrag;
    public VoidDelegate onEndDrag;
    public VoidDelegate onSubmit;
    public VoidDelegate onCancel;

    /// <summary>
    /// 公用方法
    /// </summary>
    public static UIEventListener Get(Transform trans)
    {
        UIEventListener listener = trans.GetComponent<UIEventListener>();
        if (listener == null)
        {
            listener = trans.gameObject.AddComponent<UIEventListener>();
        }
        
        return listener;
    }
    
    public override void OnPointerClick(PointerEventData eventData)
    {
         onClick?.Invoke(gameObject); //调用委托
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        onDown?.Invoke(gameObject);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke(gameObject);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke(gameObject);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        onUp?.Invoke(gameObject);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        onSelect?.Invoke(gameObject);
    }

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        onUpdateSelect?.Invoke(gameObject);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(gameObject);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        onDrop?.Invoke(gameObject);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        onDeselect?.Invoke(gameObject);
    }

    public override void OnScroll(PointerEventData eventData)
    {
        onScroll?.Invoke(gameObject);
    }

    public override void OnMove(AxisEventData eventData)
    {
        onMove?.Invoke(gameObject);
    }

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        onInitializePotentialDrag?.Invoke(gameObject);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke(gameObject);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(gameObject);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        onSubmit?.Invoke(gameObject);
    }

    public override void OnCancel(BaseEventData eventData)
    {
        onCancel?.Invoke(gameObject);
    }
}
