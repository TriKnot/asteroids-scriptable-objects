using System;
using Asteroids;
using SettingsScripts;
using Ship;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//[CustomEditor(typeof(GameSettings))]
namespace Editor
{
    public class GameSettingsEditor : EditorWindow
    {
        
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private StyleSheet styleSheet;
        
        //Ship references
        private PropertyField _shipThrottlePowerSlider;
        private PropertyField _shipRotationPowerField;
        private PropertyField _shipStartingHealthField;
        
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
        [SerializeField] private ShipSettings shipSettings;
        [SerializeField] private AsteroidSettings asteroidSettings;
        [SerializeField] private AsteroidSpawnerSettings asteroidSpawnerSettings;
        
        [MenuItem("Tools/Game Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<GameSettingsEditor>();
            window.titleContent = new GUIContent("Game Settings");
            window.Show();
        }

        private void CreateGUI()
        {
            if (styleSheet is null)
            {
                styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Editor/StyleSheet/Game_Settings.uss");
                
            }
            if (visualTreeAsset == null)
            {
                visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/GameSettingsEditor.uxml");
                if(visualTreeAsset == null) return; // If still null, get out of here
            }
            var root = rootVisualElement;
            
            if(styleSheet is not null)
            {
                root.styleSheets.Add(styleSheet);
            }         
            
            /////////////////////////////
            // Note with pointer to new Editor
            Label label = new Label("OBS: \nDepricated, use the new editor v2 from Tools menu. " +
                                    "\nSaving this one to show dev process.");
            label.AddToClassList("-label-depricated");
            root.Add(label);
            /////////////////////////////
            
            visualTreeAsset.CloneTree(root);
            
            
            LoadSettings();
            InitFields();
        }
        
        
        private void LoadSettings()
        {
            if(shipSettings == null)
                shipSettings = Resources.Load<ShipSettings>("ScriptableObjects/ShipSettings");
            if(asteroidSettings == null)
                asteroidSettings = Resources.Load<AsteroidSettings>("ScriptableObjects/AsteroidSettings");
            if(asteroidSpawnerSettings == null)
                asteroidSpawnerSettings = Resources.Load<AsteroidSpawnerSettings>("ScriptableObjects/AsteroidSpawnerSettings");
        }
        
        private void InitFields()
        {
            //Get references to and initialize fields
            //Ship
            _shipThrottlePowerSlider = rootVisualElement.Q<PropertyField>("ThrottlePower");
            _shipThrottlePowerSlider.BindProperty(new SerializedObject(shipSettings).FindProperty("Throttle"));
            
            _shipRotationPowerField = rootVisualElement.Q<PropertyField>("RotationPower");
            _shipRotationPowerField.BindProperty(new SerializedObject(shipSettings).FindProperty("Rotation"));
            
            _shipStartingHealthField = rootVisualElement.Q<PropertyField>("StartingHealth");
            _shipStartingHealthField.BindProperty(new SerializedObject(shipSettings.Health).FindProperty("IntValue"));

            //Asteroid
            _asteroidForceField = rootVisualElement.Q<MinMaxSlider>("Force");
            _asteroidForceField.RegisterValueChangedCallback(OnAsteroidForceFieldChanged);
            _asteroidForceField.SetValueWithoutNotify(asteroidSettings.Force);
            
            _asteroidSizeField = rootVisualElement.Q<MinMaxSlider>("Size");
            _asteroidSizeField.RegisterValueChangedCallback(OnAsteroidSizeFieldChanged);
            _asteroidSizeField.SetValueWithoutNotify(asteroidSettings.Size);
            
            _asteroidTorqueField = rootVisualElement.Q<MinMaxSlider>("Torque");
            _asteroidTorqueField.RegisterValueChangedCallback(OnAsteroidTorqueFieldChanged);
            _asteroidTorqueField.SetValueWithoutNotify(asteroidSettings.Torque);
            
            //AsteroidSpawner
            _spawnRateField = rootVisualElement.Q<MinMaxSlider>("SpawnRate");
            _spawnRateField.RegisterValueChangedCallback(OnSpawnRateFieldChanged);
            _spawnRateField.SetValueWithoutNotify(asteroidSpawnerSettings.SpawnRate);
            
            _spawnAmountMinField = rootVisualElement.Q<SliderInt>("SpawnAmountMin");
            _spawnAmountMinField.RegisterValueChangedCallback(evt => OnSpawnAmountFieldChanged(evt, true));
            _spawnAmountMinField.SetValueWithoutNotify(asteroidSpawnerSettings.SpawnAmount.x);
            
            _spawnAmountMaxField = rootVisualElement.Q<SliderInt>("SpawnAmountMax");
            _spawnAmountMaxField.RegisterValueChangedCallback(evt => OnSpawnAmountFieldChanged(evt, false));
            _spawnAmountMaxField.SetValueWithoutNotify(asteroidSpawnerSettings.SpawnAmount.y);
            
            _asteroidsSpawnTop = rootVisualElement.Q<Toggle>("AsteroidsSpawnTop");
            _asteroidsSpawnTop.RegisterValueChangedCallback(evt => OnAsteroidSpawnPositionChanged(evt, SpawnLocation.Top));
            _asteroidsSpawnTop.SetValueWithoutNotify(asteroidSpawnerSettings.CanSpawnTop);
            
            _asteroidsSpawnBot = rootVisualElement.Q<Toggle>("AsteroidsSpawnBot");
            _asteroidsSpawnBot.RegisterValueChangedCallback(evt => OnAsteroidSpawnPositionChanged(evt, SpawnLocation.Bottom));
            _asteroidsSpawnBot.SetValueWithoutNotify(asteroidSpawnerSettings.CanSpawnBot);
            
            _asteroidsSpawnLeft = rootVisualElement.Q<Toggle>("AsteroidsSpawnLeft");
            _asteroidsSpawnLeft.RegisterValueChangedCallback(evt => OnAsteroidSpawnPositionChanged(evt, SpawnLocation.Left));
            _asteroidsSpawnLeft.SetValueWithoutNotify(asteroidSpawnerSettings.CanSpawnLeft);
            
            _asteroidsSpawnRight = rootVisualElement.Q<Toggle>("AsteroidsSpawnRight");
            _asteroidsSpawnRight.RegisterValueChangedCallback(evt => OnAsteroidSpawnPositionChanged(evt, SpawnLocation.Right));
            _asteroidsSpawnRight.SetValueWithoutNotify(asteroidSpawnerSettings.CanSpawnRight);
            
        }
        private void OnAsteroidSpawnPositionChanged(ChangeEvent<bool> evt, SpawnLocation spawnLocation)
        {
            switch (spawnLocation)
            {
                case SpawnLocation.Bottom:
                    asteroidSpawnerSettings.CanSpawnBot = evt.newValue;
                    break;
                case SpawnLocation.Top:
                    asteroidSpawnerSettings.CanSpawnTop = evt.newValue;
                    break;
                case SpawnLocation.Left:
                    asteroidSpawnerSettings.CanSpawnLeft = evt.newValue;
                    break;
                case SpawnLocation.Right:
                    asteroidSpawnerSettings.CanSpawnRight = evt.newValue;
                    break; 
                default:
                    throw new ArgumentOutOfRangeException(nameof(spawnLocation), spawnLocation, null);
            }
            EditorUtility.SetDirty(asteroidSpawnerSettings);
        }

        private void OnSpawnAmountFieldChanged(ChangeEvent<int> evt, bool isMin)
        {
            EditorUtility.SetDirty(asteroidSpawnerSettings);
            var minVal = asteroidSpawnerSettings.SpawnAmount.x;
            var maxVal = asteroidSpawnerSettings.SpawnAmount.y;
            
            if(isMin)
            {
                minVal = evt.newValue;
                if(minVal > maxVal)
                {
                    maxVal = minVal;
                    _spawnAmountMaxField.SetValueWithoutNotify(maxVal);
                }
            }
            else
            {
                maxVal = evt.newValue;
                if(minVal > maxVal)
                {
                    minVal = maxVal;
                    _spawnAmountMinField.SetValueWithoutNotify(minVal);
                }
            }

            asteroidSpawnerSettings.SpawnAmount = new Vector2Int(minVal, maxVal);
        }

        private void OnSpawnRateFieldChanged(ChangeEvent<Vector2> evt)
        {
            EditorUtility.SetDirty(asteroidSpawnerSettings);
            asteroidSpawnerSettings.SpawnRate = evt.newValue;
        }

        private void OnAsteroidTorqueFieldChanged(ChangeEvent<Vector2> evt)
        {
            EditorUtility.SetDirty(asteroidSettings);
            asteroidSettings.Torque = evt.newValue;
        }

        private void OnAsteroidSizeFieldChanged(ChangeEvent<Vector2> evt)
        {
            EditorUtility.SetDirty(asteroidSettings);
            asteroidSettings.Size = evt.newValue;
        }

        private void OnAsteroidForceFieldChanged(ChangeEvent<Vector2> evt)
        {
            EditorUtility.SetDirty(asteroidSettings);
            asteroidSettings.Force = evt.newValue;
        }
        
        private void OnShipThrottleChanged(ChangeEvent<float> evt)
        {
            EditorUtility.SetDirty(shipSettings);
            shipSettings.Throttle = evt.newValue;
        }

        private void OnShipRotationChanged(ChangeEvent<float> evt)
        {
            EditorUtility.SetDirty(shipSettings);
            shipSettings.Rotation = evt.newValue;
        }
        
        private void OnShipHealthChanged(ChangeEvent<int> evt)
        {
            EditorUtility.SetDirty(shipSettings);
            shipSettings.Health.SetValue(evt.newValue);
        }
        
    }
}
