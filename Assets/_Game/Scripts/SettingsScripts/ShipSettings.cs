using UnityEngine;
using Variables;

namespace SettingsScripts
{
    [CreateAssetMenu(fileName = "new ShipSettings", 
        menuName = "ScriptableObjects/Ship Settings")]
    public class ShipSettings : SettingsBase
    {
        [Range(0f, 30f)] 
        public float Throttle;
        [Range(0f, 30f)] 
        public float Rotation;
        
        public IntVariable Health;
    }
}
