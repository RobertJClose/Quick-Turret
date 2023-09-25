using QuickTurret.Selection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurretClickReceiver : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        SelectionManager.Instance.SetSelectedGameObject(gameObject);
    }
}
