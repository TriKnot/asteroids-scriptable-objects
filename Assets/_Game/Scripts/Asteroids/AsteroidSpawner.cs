using System;
using SettingsScripts;
using UnityEngine;
using Variables;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidSpawner : MonoBehaviour
    {

        [SerializeField] private Asteroid _asteroidPrefab;
        [SerializeField] private AsteroidSpawnerSettings _asteroidSpawnerSettings;
        
        private float _timer;
        private float _nextSpawnTime;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            Spawn();
            UpdateNextSpawnTime();
        }

        private void Update()
        {
            UpdateTimer();

            if (!ShouldSpawn())
                return;

            Spawn();
            UpdateNextSpawnTime();
            _timer = 0f;
        }

        private void UpdateNextSpawnTime()
        {
            _nextSpawnTime = Random.Range(_asteroidSpawnerSettings.MinSpawnTime, _asteroidSpawnerSettings.MaxSpawnTime);
        }

        private void UpdateTimer()
        {
            _timer += Time.deltaTime;
        }

        private bool ShouldSpawn()
        {
            return _timer >= _nextSpawnTime;
        }

        private void Spawn()
        {
            var amount = Random.Range(_asteroidSpawnerSettings.MinAmount, _asteroidSpawnerSettings.MaxAmount + 1);
            
            for (var i = 0; i < amount; i++)
            {
                var location = GetSpawnLocation();
                var position = GetStartPosition(location);
                Instantiate(_asteroidPrefab, position, Quaternion.identity);
            }
        }

        private SpawnLocation GetSpawnLocation()
        {
            SpawnLocation location;

            do{
                location = (SpawnLocation)Random.Range(0, 4);
            } while (location == SpawnLocation.Top && _asteroidSpawnerSettings.CanSpawnTop == false ||
                     location == SpawnLocation.Bottom && _asteroidSpawnerSettings.CanSpawnBot == false ||
                     location == SpawnLocation.Left && _asteroidSpawnerSettings.CanSpawnLeft == false||
                     location == SpawnLocation.Right && _asteroidSpawnerSettings.CanSpawnRight == false );
            return location;
        }

        private Vector3 GetStartPosition(SpawnLocation spawnLocation)
        {
            var pos = new Vector3 { z = Mathf.Abs(_camera.transform.position.z) };
            
            const float padding = 5f;
            switch (spawnLocation)
            {
                case SpawnLocation.Top:
                    pos.x = Random.Range(0f, Screen.width);
                    pos.y = Screen.height + padding;
                    break;
                case SpawnLocation.Bottom:
                    pos.x = Random.Range(0f, Screen.width);
                    pos.y = 0f - padding;
                    break;
                case SpawnLocation.Left:
                    pos.x = 0f - padding;
                    pos.y = Random.Range(0f, Screen.height);
                    break;
                case SpawnLocation.Right:
                    pos.x = Screen.width - padding;
                    pos.y = Random.Range(0f, Screen.height);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(spawnLocation), spawnLocation, null);
            }
            
            return _camera.ScreenToWorldPoint(pos);
        }
    }

    public enum SpawnLocation
    {
        Top,
        Bottom,
        Left,
        Right
    }
}
