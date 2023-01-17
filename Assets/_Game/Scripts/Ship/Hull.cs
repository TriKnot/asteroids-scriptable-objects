using ScriptableEvents;
using UnityEngine;
using Variables;

namespace Ship
{
    public class Hull : MonoBehaviour
    {
        //[SerializeField] private IntVariable _health;
        // [SerializeField] private IntReference _healthRef;
        // [SerializeField] private IntObservable _healthObservable;
        
        [SerializeField] private ShipSettings _shipSettings;
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (string.Equals(other.gameObject.tag, "Asteroid"))
            {
                Debug.Log("Hull collided with Asteroid");
                // TODO can we bake this into one call?
                _shipSettings.Health.ApplyChange(-1);
            }
        }
    }
}
