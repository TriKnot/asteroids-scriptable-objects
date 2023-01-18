using System;
using Asteroids;
using Ship;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//[CustomEditor(typeof(GameSettings))]
namespace Editor
{
    public class GameSettingsEditor : EditorWindow
    {
        
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        
        //Ship references
        private FloatField _shipThrottlePowerField;
        private FloatField _shipRotationPowerField;
        private IntegerField _shipStartingHealthField;
        
        //Astroid references
        private MinMaxSlider _asteroidForceField;
        private MinMaxSlider _asteroidSizeField;
        private MinMaxSlider _asteroidTorqueField;
        
        //Asteroid spawner references
        private MinMaxSlider _spawnRateField;
        private SliderInt _spawnAmountMinField;
        private SliderInt _spawnAmountMaxField;
        private Toggle _asteroidsSpawnTop;
        private Toggle _asteroidsSpawnBot;
        private Toggle _asteroidsSpawnLeft;
        private Toggle _asteroidsSpawnRight;
        
        [Header("Settings Scriptable Objects")]
        [SerializeField] private ShipSettings _shipSettings;
        [SerializeField] private AsteroidSettings _asteroidSettings;
        [SerializeField] private AsteroidSpawnerSettings _asteroidSpawnerSettings;
        
        [MenuItem("Tools/Game Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<GameSettingsEditor>();
            window.titleContent = new GUIContent("Game Settings");
            window.Show();
        }

        private void CreateGUI()
        {
            if (visualTreeAsset == null)
            {
                visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/GameSettingsEditor.uxml");
                if(visualTreeAsset == null) return; // If still null, get out of here
            }
            var root = rootVisualElement;
            visualTreeAsset.CloneTree(root);
            
            LoadSettings();
            
            InitFields();
        }
        
        
        private void LoadSettings()
        {
            if(_shipSettings == null)
                _shipSettings = Resources.Load<ShipSettings>("ScriptableObjects/ShipSettings");
            if(_asteroidSettings == null)
                _asteroidSettings = Resources.Load<AsteroidSettings>("ScriptableObjects/AsteroidSettings");
            if(_asteroidSpawnerSettings == null)
                _asteroidSpawnerSettings = Resources.Load<AsteroidSpawnerSettings>("ScriptableObjects/AsteroidSpawnerSettings");
        }
        
        private void InitFields()
        {
            //Get references to and initialize fields
            //Ship
            _shipThrottlePowerField = rootVisualElement.Q<FloatField>("ThrottlePower");
            _shipThrottlePowerField.RegisterValueChangedCallback(OnShipThrottleChanged);
            _shipThrottlePowerField.SetValueWithoutNotify(_shipSettings.Throttle);
            
            _shipRotationPowerField = rootVisualElement.Q<FloatField>("RotationPower");
            _shipRotationPowerField.RegisterValueChangedCallback(OnShipRotationChanged);
            _shipRotationPowerField.SetValueWithoutNotify(_shipSettings.Rotation);
            
            _shipStartingHealthField = rootVisualElement.Q<IntegerField>("StartingHealth");
            _shipStartingHealthField.RegisterValueChangedCallback(OnShipHealthChanged);
            _shipStartingHealthField.SetValueWithoutNotify(_shipSettings.Health.value);
            
            
            //Asteroid
            _asteroidForceField = rootVisualElement.Q<MinMaxSlider>("Force");
            _asteroidForceField.RegisterValueChangedCallback(OnAsteroidForceFieldChanged);
            _asteroidForceField.SetValueWithoutNotify(_asteroidSettings.Force);
            
            _asteroidSizeField = rootVisualElement.Q<MinMaxSlider>("Size");
            _asteroidSizeField.RegisterValueChangedCallback(OnAsteroidSizeFieldChanged);
            _asteroidSizeField.SetValueWithoutNotify(_asteroidSettings.Size);
            
            _asteroidTorqueField = rootVisualElement.Q<MinMaxSlider>("Torque");
            _asteroidTorqueField.RegisterValueChangedCallback(OnAsteroidTorqueFieldChanged);
            _asteroidTorqueField.SetValueWithoutNotify(_asteroidSettings.Torque);
            
            //AsteroidSpawner
            _spawnRateField = rootVisualElement.Q<MinMaxSlider>("SpawnRate");
            _spawnRateField.RegisterValueChangedCallback(OnSpawnRateFieldChanged);
            _spawnRateField.SetValueWithoutNotify(_asteroidSpawnerSettings.SpawnRate);
            
            _spawnAmountMinField = rootVisualElement.Q<SliderInt>("SpawnAmountMin");
            _spawnAmountMinField.RegisterValueChangedCallback(OnMinSpawnAmountFieldChanged);
            _spawnAmountMinField.SetValueWithoutNotify(_asteroidSpawnerSettings.SpawnAmount.x);
            
            _spawnAmountMaxField = rootVisualElement.Q<SliderInt>("SpawnAmountMax");
            _spawnAmountMaxField.RegisterValueChangedCallback(OnMaxSpawnAmountFieldChanged);
            _spawnAmountMaxField.SetValueWithoutNotify(_asteroidSpawnerSettings.SpawnAmount.y);
            
            _asteroidsSpawnTop = rootVisualElement.Q<Toggle>("AsteroidsSpawnTop");
            _asteroidsSpawnTop.RegisterValueChangedCallback(evt => OnAsteroidSpawnPositionChanged(evt, SpawnLocation.Top));
            _asteroidsSpawnTop.SetValueWithoutNotify(_asteroidSpawnerSettings.CanSpawnTop);
            
            _asteroidsSpawnBot = rootVisualElement.Q<Toggle>("AsteroidsSpawnBot");
            _asteroidsSpawnBot.RegisterValueChangedCallback(evt => OnAsteroidSpawnPositionChanged(evt, SpawnLocation.Bottom));
            _asteroidsSpawnBot.SetValueWithoutNotify(_asteroidSpawnerSettings.CanSpawnBot);
            
            _asteroidsSpawnLeft = rootVisualElement.Q<Toggle>("AsteroidsSpawnLeft");
            _asteroidsSpawnLeft.RegisterValueChangedCallback(evt => OnAsteroidSpawnPositionChanged(evt, SpawnLocation.Left));
            _asteroidsSpawnLeft.SetValueWithoutNotify(_asteroidSpawnerSettings.CanSpawnLeft);
            
            _asteroidsSpawnRight = rootVisualElement.Q<Toggle>("AsteroidsSpawnRight");
            _asteroidsSpawnRight.RegisterValueChangedCallback(evt => OnAsteroidSpawnPositionChanged(evt, SpawnLocation.Right));
            _asteroidsSpawnRight.SetValueWithoutNotify(_asteroidSpawnerSettings.CanSpawnRight);
            
        }
        private void OnAsteroidSpawnPositionChanged(ChangeEvent<bool> evt, SpawnLocation spawnLocation)
        {
            switch (spawnLocation)
            {
                case SpawnLocation.Bottom:
                    _asteroidSpawnerSettings.CanSpawnBot = evt.newValue;
                    break;
                case SpawnLocation.Top:
                    _asteroidSpawnerSettings.CanSpawnTop = evt.newValue;
                    break;
                case SpawnLocation.Left:
                    _asteroidSpawnerSettings.CanSpawnLeft = evt.newValue;
                    break;
                case SpawnLocation.Right:
                    _asteroidSpawnerSettings.CanSpawnRight = evt.newValue;
                    break; 
                default:
                    throw new ArgumentOutOfRangeException(nameof(spawnLocation), spawnLocation, null);
            }
            EditorUtility.SetDirty(_asteroidSpawnerSettings);
        }

        private void OnMaxSpawnAmountFieldChanged(ChangeEvent<int> evt)
        {
            EditorUtility.SetDirty(_asteroidSpawnerSettings);
            var minVal = _asteroidSpawnerSettings.SpawnAmount.x;
            if(_asteroidSpawnerSettings.SpawnAmount.x > _asteroidSpawnerSettings.SpawnAmount.y)
            {
                minVal = evt.newValue;
                _spawnAmountMinField.SetValueWithoutNotify(minVal);
            }
            _asteroidSpawnerSettings.SpawnAmount = new Vector2Int(minVal, evt.newValue);
        }

        private void OnMinSpawnAmountFieldChanged(ChangeEvent<int> evt)
        {
            EditorUtility.SetDirty(_asteroidSpawnerSettings);
            var maxVal = _asteroidSpawnerSettings.SpawnAmount.y;
            if(_asteroidSpawnerSettings.SpawnAmount.x > _asteroidSpawnerSettings.SpawnAmount.y)
            {
                maxVal = evt.newValue;
                _spawnAmountMaxField.SetValueWithoutNotify(maxVal);
            }
            _asteroidSpawnerSettings.SpawnAmount = new Vector2Int(evt.newValue, maxVal);
        }

        private void OnSpawnRateFieldChanged(ChangeEvent<Vector2> evt)
        {
            EditorUtility.SetDirty(_asteroidSpawnerSettings);
            _asteroidSpawnerSettings.SpawnRate = evt.newValue;
        }


        private void OnAsteroidTorqueFieldChanged(ChangeEvent<Vector2> evt)
        {
            EditorUtility.SetDirty(_asteroidSettings);
            _asteroidSettings.Torque = evt.newValue;
        }

        private void OnAsteroidSizeFieldChanged(ChangeEvent<Vector2> evt)
        {
            EditorUtility.SetDirty(_asteroidSettings);
            _asteroidSettings.Size = evt.newValue;
        }

        private void OnAsteroidForceFieldChanged(ChangeEvent<Vector2> evt)
        {
            EditorUtility.SetDirty(_asteroidSettings);
            _asteroidSettings.Force = evt.newValue;
        }
        
        

        private void OnShipThrottleChanged(ChangeEvent<float> evt)
        {
            EditorUtility.SetDirty(_shipSettings);
            _shipSettings.Throttle = evt.newValue;
        }

        private void OnShipRotationChanged(ChangeEvent<float> evt)
        {
            EditorUtility.SetDirty(_shipSettings);
            _shipSettings.Throttle = evt.newValue;
        }
        
        private void OnShipHealthChanged(ChangeEvent<int> evt)
        {
            EditorUtility.SetDirty(_shipSettings);
            _shipSettings.Health.value = evt.newValue;
        }
        
    }
}
