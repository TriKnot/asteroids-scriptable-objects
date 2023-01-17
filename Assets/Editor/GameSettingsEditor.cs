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
        private ShipSettings _shipSettings;
        
        //Astroid references
        private MinMaxSlider _astroidForceField;
        private MinMaxSlider _astroidSizeField;
        private MinMaxSlider _astroidTorqueField;
        private AsteroidSettings _asteroidSettings;

        
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
            
            _shipSettings = Resources.Load<ShipSettings>("ScriptableObjects/ShipSettings");
            _asteroidSettings = Resources.Load<AsteroidSettings>("ScriptableObjects/AsteroidSettings");
            InitFields();
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
            _astroidForceField = rootVisualElement.Q<MinMaxSlider>("Force");
            _astroidForceField.RegisterValueChangedCallback(OnAsteroidForceFieldChanged);
            _astroidForceField.SetValueWithoutNotify(_asteroidSettings.Force);
            
            _astroidSizeField = rootVisualElement.Q<MinMaxSlider>("Size");
            _astroidSizeField.RegisterValueChangedCallback(OnAsteroidSizeFieldChanged);
            _astroidSizeField.SetValueWithoutNotify(_asteroidSettings.Size);
            
            _astroidTorqueField = rootVisualElement.Q<MinMaxSlider>("Torque");
            _astroidTorqueField.RegisterValueChangedCallback(OnAsteroidTorqueFieldChanged);
            _astroidTorqueField.SetValueWithoutNotify(_asteroidSettings.Torque);
            
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
