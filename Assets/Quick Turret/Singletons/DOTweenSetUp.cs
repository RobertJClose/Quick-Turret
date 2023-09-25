using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.Singletons
{
    public class DOTweenSetUp : MonoBehaviour
    {
        public static DOTweenSetUp Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            else
                Instance = this;

            DOTween.SetTweensCapacity(2000, 2000);
        }
    }
}