using Asteroids;
using SettingsScripts;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class ImprovedGameSettingsEditor : EditorWindow
    {

        [SerializeField] private StyleSheet styleSheet;
        
        private VisualElement _root;
        
        [MenuItem("Tools/Game Settings v2")]
        public static void ShowWindow()
        {
            var window = GetWindow<ImprovedGameSettingsEditor>();
            window.titleContent = new GUIContent("Game Settings v2");
            window.Show();
        }

        private void CreateGUI()
        {
            CreateUI();
        }
        
        private void CreateUI()
        {
            // Get style sheet if not already set in inspector.
            styleSheet = styleSheet == null ? Resources.Load<StyleSheet>("GameSettings") : styleSheet;

            _root = rootVisualElement;
            
            // Load style sheet if found.
            if(styleSheet is not null)
            {
                _root.styleSheets.Add(styleSheet);
            }            
            
            // Find all settings of each type and create UI for them with their own methods.
            ShipSettings[] shipSettingsArray = GetAssetsOfType<ShipSettings>();
            foreach (var ship in shipSettingsArray)
            {
                var shipFoldout = CreateShipFoldout(ship);
                _root.Add(shipFoldout);
            }
            
            AsteroidSettings[] asteroidSettingsArray = GetAssetsOfType<AsteroidSettings>();
            foreach (var asteroid in asteroidSettingsArray)
            {
                var asteroidFoldout = CreateAsteroidFoldout(asteroid);
                _root.Add(asteroidFoldout);
            }
            
            AsteroidSpawnerSettings[] asteroidSpawnerSettingsArray = GetAssetsOfType<AsteroidSpawnerSettings>();
            foreach (var asteroidSpawner in asteroidSpawnerSettingsArray)
            {
                var asteroidSpawnerFoldout = CreateAsteroidSpawnerFoldout(asteroidSpawner);
                _root.Add(asteroidSpawnerFoldout);
            }        
            
            // Create redraw UI button for testing and for whenever the user wants to update the UI -> For example if they created a new settings object.
            Button redrawButton = new Button(ReDraw);
            redrawButton.text = "Reload Window";
            _root.Add(redrawButton);
            
        }

        // I Create a foldout for each of the three types of settings objects and populate these with Visualobjects for each of the variables in the settings objects.
        // This way I have better control over the layout through the style sheet.
        private Foldout CreateShipFoldout(ShipSettings shipSettings)
        {
            // Foldout
            var shipFoldout = new Foldout() 
                {viewDataKey = shipSettings.name, text = shipSettings.name};
            shipFoldout.AddToClassList("asteroids-foldout");
            
            // Label
            Label shipNameLabel = new Label("Ship");
            shipNameLabel.AddToClassList("asteroids-header");
            shipFoldout.Add(shipNameLabel);

            SerializedObject shipSO = new SerializedObject(shipSettings);

            // Throttle
            var pfThrottle = CreateField(shipSO, "Throttle");
            shipFoldout.Add(pfThrottle);
            
            // Rotation
            var pfRotation = CreateField(shipSO, "Rotation");
            shipFoldout.Add(pfRotation);
           
            SerializedObject so = new SerializedObject(shipSettings.Health);
            
            // Health
            var pfHealth = CreateField(so, "IntValue", "Starting Health");
            shipFoldout.Add(pfHealth);
            
            return shipFoldout;
        }

        private Foldout CreateAsteroidFoldout(AsteroidSettings asteroidSettings)
        {
            // Foldout
            var asteroidFoldout = new Foldout()
                {viewDataKey = asteroidSettings.name, text = asteroidSettings.name};
            asteroidFoldout.AddToClassList("asteroids-foldout");
            
            // Label
            Label shipNameLabel = new Label("Asteroids");
            shipNameLabel.AddToClassList("asteroids-header");
            asteroidFoldout.Add(shipNameLabel);
            
            SerializedObject so = new SerializedObject(asteroidSettings);

            // Force
            var pfForce = CreateField(so, "Force");
            asteroidFoldout.Add(pfForce);
            
            // Size
            var pfSize = CreateField(so, "Size");
            asteroidFoldout.Add(pfSize);
            
            // Torque
            var pfTorque = CreateField(so, "Torque");
            asteroidFoldout.Add(pfTorque);

            return asteroidFoldout;
        }
        
        private VisualElement CreateAsteroidSpawnerFoldout(AsteroidSpawnerSettings asteroidSpawner)
        {   
            // Foldout
            var asteroidSpawnerFoldout = new Foldout()
                {viewDataKey = asteroidSpawner.name, text = asteroidSpawner.name};
            asteroidSpawnerFoldout.AddToClassList("asteroids-foldout");
            
            // Label
            Label asteroidSpawnerLabel = new Label("Asteroid Spawner");
            asteroidSpawnerLabel.AddToClassList("asteroids-header");
            asteroidSpawnerFoldout.Add(asteroidSpawnerLabel);
            
            SerializedObject so = new SerializedObject(asteroidSpawner);
            
            // Spawn Rate
            var pfSpawnRate = CreateField(so, "SpawnRate");
            asteroidSpawnerFoldout.Add(pfSpawnRate);
            
            // Spawn Amount
            var pfSpawnAmount = CreateField(so, "SpawnAmount");
            asteroidSpawnerFoldout.Add(pfSpawnAmount);
            
            // Spawn Directions
            var pfSpawnDirections = CreateBoolGroup(so);
            asteroidSpawnerFoldout.Add(pfSpawnDirections);

            return asteroidSpawnerFoldout;
        }

        // Create a group of bools for the spawn directions.
        private VisualElement CreateBoolGroup(SerializedObject so)
        {
            // Create new visual element and add style.
            VisualElement ve = new VisualElement();
            ve.AddToClassList("asteroids-bool-group");
            
            // Iterate over all visible properties in the serialized object and create a toggle for each bool field.
            SerializedProperty sp = so.GetIterator();
            while (sp.NextVisible(true))
            {
                if(sp.propertyType == SerializedPropertyType.Boolean)
                {
                    var boolField = new Toggle(sp.displayName);
                    boolField.BindProperty(sp);
                    ve.Add(boolField);
                }
            }
            return ve;
        }
        
        // Create a property field for a given property in a serialized object. 
        private VisualElement CreateField(SerializedObject so, string propertyName, string label = "")
        {
            // If so is null, return empty object.
            if(so is null)
                return new VisualElement();
            
            // If label is empty, use the property name as label.
            if(string.IsNullOrEmpty(label))
                label = propertyName;
            
            // Find the property in the serialized object. Return empty object if it doesn't exist.
            SerializedProperty sp = so.FindProperty(propertyName);
            if(sp is null)
                return new VisualElement();
            
            // Create new field and add style. The type of field created depends on the type of the property.
            VisualElement ve = new VisualElement();
            switch (sp.propertyType)
            {
                case SerializedPropertyType.Vector2:
                    ve.Add(new Label(label));
                    
                    CreateCustomMinMaxSlider(sp, ve);
                    ve.AddToClassList("asteroids-min-max-slider");
                    
                    break;
                
                case SerializedPropertyType.Vector2Int:
                    ve.Add(new Label(label));
                    
                    CreateCustomMinMaxIntSlider(sp, ve);
                    ve.AddToClassList("asteroids-int-min-max");

                    break;
                
                default:
                    ve = new PropertyField(sp, label);
                    ve.AddToClassList("asteroids-property-field");
                    
                    break;
            }
            ve.Bind(so);
            return ve;
        }

        // Create a custom min max slider for a Vector2Int property as the Toolkit had no native yet.
        private void CreateCustomMinMaxIntSlider(SerializedProperty sp, VisualElement ve)
        {
            // Find the min and max values of the property.
            var min = sp.FindPropertyRelative("x");
            var max = sp.FindPropertyRelative("y");

            // Create a slider and field for the min value and bind it.
            var minVisual = new VisualElement();
            minVisual.AddToClassList("asteroids-int-min-max-slider");
            minVisual.Add(new Label("Min"));
            var minSlider = new SliderInt(0, 10);
            minSlider.BindProperty(min);
            var minField = new IntegerField();
            minField.BindProperty(min);
            minVisual.Add(minSlider);
            minVisual.Add(minField);
            ve.Add(minVisual);

            // Create a slider and field for the max value and bind it.
            var maxVisual = new VisualElement();
            maxVisual.AddToClassList("asteroids-int-min-max-slider");
            maxVisual.Add(new Label("Max"));
            var maxSlider = new SliderInt(0, 10);
            maxSlider.BindProperty(max);
            var maxField = new IntegerField();
            maxField.BindProperty(max);
            maxVisual.Add(maxSlider);
            maxVisual.Add(maxField);
            ve.Add(maxVisual);
            
            // Sync values on callback so the min value can't be higher than the max value, and vice versa.
            maxField.RegisterValueChangedCallback(evt => Sync(sp, false));
            maxSlider.RegisterValueChangedCallback(evt => Sync(sp, false));
            minField.RegisterValueChangedCallback(evt => Sync(sp, true));
            minSlider.RegisterValueChangedCallback(evt => Sync(sp, true));
        }

        // Sync min and max values of a Vector2 or Vector2Int property.
        private void Sync(SerializedProperty sp, bool minChanged)
        {
            // Find the min and max values of the property.
            var min = sp.FindPropertyRelative("x");
            var max = sp.FindPropertyRelative("y");
            
            // Return if no value found.
            if( min is null || max is null)
                return;
            
            // Sync values so that min can't be higher than max, and vice versa. The value changes is based on the minChanged parameter.
            if (max.intValue < min.intValue)
            {
                if (minChanged)
                {
                    max.intValue = min.intValue;
                }
                else
                {
                    min.intValue = max.intValue;
                }
                sp.serializedObject.ApplyModifiedProperties();
            }
            
        }

        // Create a custom min max slider for a Vector2 property as the native won't show value fields.
        private void CreateCustomMinMaxSlider(SerializedProperty sp,VisualElement ve)
        {
            // Create and bind min value field.
            FloatField minField = new FloatField();
            minField.BindProperty(sp.FindPropertyRelative("x"));
            ve.Add(minField);

            // Create and bind slider.
            var slider = new MinMaxSlider();
            slider.lowLimit = 0;
            slider.highLimit = 10;
            slider.BindProperty(sp);
            ve.Add(slider);
            
            // Create and bind max value field.
            FloatField maxField = new FloatField();
            maxField.BindProperty(sp.FindPropertyRelative("y"));
            ve.Add(maxField);
        }

        // Clear and draw window again.
        private void ReDraw()
        {
            _root.Clear();
            CreateUI();
        }

        
        // Method GetAssetsOfType made by AntonHedlund.
        // Get all assets of a given type derived from ScriptableObject.
        public static T[] GetAssetsOfType<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets($"t: {typeof(T).Name}");
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                a[i] = (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(T));
            }
            return a;
        }
    }
}
