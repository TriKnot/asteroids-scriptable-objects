using UnityEngine;
using Variables;

namespace Asteroids
{
    [CreateAssetMenu(fileName = "new AsteroidSettings", 
        menuName = "ScriptableObjects/Asteroid Settings")]
    public class AsteroidSettings : ScriptableObject
    {
        public Vector2 Force;
        public Vector2 Size;
        public Vector2 Torque;
        
        public float MinForce => Force.x;
        public float MaxForce => Force.y;
        public float MinSize => Size.x;
        public float MaxSize => Size.y;
        public float MinTorque => Torque.x;
        public float MaxTorque => Torque.y;

    }
}