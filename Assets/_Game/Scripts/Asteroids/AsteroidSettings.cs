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
        
        public float _minForce => Force.x;
        public float _maxForce => Force.y;
        public float _minSize => Size.x;
        public float _maxSize => Size.y;
        public float _minTorque => Torque.x;
        public float _maxTorque => Torque.y;

    }
}