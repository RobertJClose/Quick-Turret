using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.Selection
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance { get; private set; }

        public object ContextObject { get; private set; }
        public GameObject Selection { get; private set; }

        public event EventHandler<OnSelectionChangedEventArgs> OnSelectionChanged;

        public class OnSelectionChangedEventArgs : EventArgs
        {

        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            else
                Instance = this;
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            SetSelectedGameObject(null);
        }

        protected virtual void RaiseSelectionChanged(OnSelectionChangedEventArgs e)
        {
            EventHandler<OnSelectionChangedEventArgs> raiseEvent = OnSelectionChanged;

            if (raiseEvent != null)
                raiseEvent(this, e);
        }

        public void SetSelectedGameObject(GameObject gameObject, object context = null)
        {
            Selection = gameObject;
            ContextObject = context;

            RaiseSelectionChanged(new OnSelectionChangedEventArgs());

            //DebugSelectionChanging();
        }

        private void DebugSelectionChanging()
        {
            Debug.Log($"Selection changed to {Selection}");
        }
    }
}
