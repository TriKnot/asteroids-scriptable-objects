using UnityEngine;
using Variables;

namespace Ship
{
    [CreateAssetMenu(fileName = "new ShipSettings", 
        menuName = "ScriptableObjects/Ship Settings")]
    public class ShipSettings : ScriptableObject
    {
        [Range(0f, 30f)] 
        public float Throttle;
        [Range(0f, 30f)] 
        public float Rotation;
        
        public IntVariable Health;
    }
}
