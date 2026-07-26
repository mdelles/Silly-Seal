using UnityEngine;
using SillySeal.Player;

namespace SillySeal.Environment
{
    [RequireComponent(typeof(Collider))]
    public class WaterVolume : MonoBehaviour
    {
        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out SealController seal))
                seal.EnterWater();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out SealController seal))
                seal.ExitWater();
        }
    }
}
