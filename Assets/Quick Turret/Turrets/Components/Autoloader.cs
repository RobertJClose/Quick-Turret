using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.TurretComponents
{
    public class Autoloader : MonoBehaviour
    {
        [SerializeField]
        private List<AmmoType> ammoBelt = new List<AmmoType>();
        [SerializeField]
        private int beltSize;

        [Min(0f)]
        public float ReloadTime = 0.5f;

        public int ChamberedRoundIndex { get; private set; }
        public int BeltSize => beltSize;
        public AmmoType ChamberedRound => IsLoaded ? ammoBelt[ChamberedRoundIndex] : null;
        public bool IsLoaded;

        public event System.EventHandler<OnLoadedEventArgs> OnLoaded;

        public class OnLoadedEventArgs : System.EventArgs
        {

        }

        private IEnumerator Start()
        {
            ChamberedRoundIndex = 0;
            IsLoaded = true;

            yield return new WaitForEndOfFrame();
            OnLoadedEventArgs e = new OnLoadedEventArgs();
            RaiseOnLoaded(e);
        }

        private void OnValidate()
        {
            ChamberedRoundIndex = Mathf.Clamp(ChamberedRoundIndex, 0, beltSize - 1);
            beltSize = Mathf.Max(beltSize, 0);
            ResizeAmmoList();
        }

        public void AdjustBeltSize(int newSize)
        {
            beltSize = Mathf.Max(0, newSize);
            ResizeAmmoList();
        }

        public void AdvanceBelt()
        {
            if (ChamberedRoundIndex >= ammoBelt.Count - 1)
                ChamberedRoundIndex = 0;
            else
                ChamberedRoundIndex += 1;
        }

        public void ChangeAmmo(int index, AmmoType ammoType)
        {
            if (index + 1 > beltSize)
                throw new System.ArgumentOutOfRangeException(nameof(index), $"The ammo belt is smaller than the specified index.\nBelt size: {beltSize}\n Index: {index}");

            ammoBelt[index] = ammoType;
        }

        public AmmoType DrawChamberedRound()
        {
            if (ammoBelt.Count == 0)
                return null;

            if (!IsLoaded)
                return null;

            int initialPosition = ChamberedRoundIndex;
            AmmoType round = ammoBelt[ChamberedRoundIndex];
            AdvanceBelt();

            // If this slot in the ammo belt is filled with null, we continue advancing
            // through the belt until we come across a non-null round, or we get back
            // to where we started.
            while (round == null)
            {
                if (ChamberedRoundIndex == initialPosition)
                    return null;

                round = ammoBelt[ChamberedRoundIndex];
                AdvanceBelt();
            }

            IsLoaded = false;
            StartCoroutine(Reload(ReloadTime));
            return round;
        }

        public AmmoType GetRoundAtIndex(int index) => ammoBelt[index];

        public void ResetBelt() => ChamberedRoundIndex = 0;

        private void RaiseOnLoaded(OnLoadedEventArgs e)
        {
            System.EventHandler<OnLoadedEventArgs> raiseEvent = OnLoaded;

            if (raiseEvent != null)
                raiseEvent(this, e);
        }

        private IEnumerator Reload(float reloadTime)
        {
            yield return new WaitForSeconds(reloadTime);

            IsLoaded = true;
            
            OnLoadedEventArgs onLoadedEventArgs = new OnLoadedEventArgs();
            RaiseOnLoaded(onLoadedEventArgs);
        }

        private void ResizeAmmoList()
        {
            while (ammoBelt.Count < beltSize)
                ammoBelt.Add(null);

            while (ammoBelt.Count > beltSize)
                ammoBelt.RemoveAt(ammoBelt.Count - 1);
        }
    }
}
