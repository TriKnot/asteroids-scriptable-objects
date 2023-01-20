using UnityEngine;

namespace SettingsScripts
{
    [CreateAssetMenu(fileName = "new AsteroidSpawnerSettings", 
        menuName = "ScriptableObjects/AsteroidSpawner Settings")]
    public class AsteroidSpawnerSettings : SettingsBase
    {
        public Vector2 SpawnRate;
        public float MinSpawnTime => SpawnRate.x;
        public float MaxSpawnTime => SpawnRate.y;
        
        public Vector2Int SpawnAmount;
        
        public int MinAmount => SpawnAmount.x;
        public int MaxAmount => SpawnAmount.y;

        public bool CanSpawnTop;
        public bool CanSpawnBot;
        public bool CanSpawnLeft;
        public bool CanSpawnRight;
        
    }
}