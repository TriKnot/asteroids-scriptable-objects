using System;
using Asteroids;
using SettingsScripts;
using Ship;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Variables;

//[CustomEditor(typeof(GameSettings))]
namespace Editor
{
    public class ImprovedGameSettingsEditor : EditorWindow
    {

        [SerializeField] private VisualTreeAsset visualTreeAsset;
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
            _root = rootVisualElement;

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
        }

        private VisualElement CreateAsteroidSpawnerFoldout(AsteroidSpawnerSettings asteroidSpawner)
        {   
            var asteroidSpawnerFoldout = new Foldout();
            asteroidSpawnerFoldout.text = asteroidSpawner.name;
            
            Label asteroidSpawnerLabel = new Label("Asteroid Spawner");
            asteroidSpawnerLabel.AddToClassList("header");
            asteroidSpawnerFoldout.Add(asteroidSpawnerLabel);
            
            SerializedObject so = new SerializedObject(asteroidSpawner);
            //Spawn Rate
            var pfSpawnRate = CreateField(so, "SpawnRate");
            asteroidSpawnerFoldout.Add(pfSpawnRate);
            
            //Spawn Amount
            var pfSpawnAmount = CreateField(so, "SpawnAmount");
            asteroidSpawnerFoldout.Add(pfSpawnAmount);
            
            //Spawn Directions
            var pfSpawnDirections = CreateBoolGroup(so);
            asteroidSpawnerFoldout.Add(pfSpawnDirections);

            
            return asteroidSpawnerFoldout;
        }

        private VisualElement CreateBoolGroup(SerializedObject so)
        {
            SerializedProperty sp = so.GetIterator();
            VisualElement ve = new VisualElement();
            
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

        private Foldout CreateShipFoldout(ShipSettings shipSettings)
        {
            var shipFoldout = new Foldout();
            shipFoldout.text = shipSettings.name;
            
            Label shipNameLabel = new Label("Ship");
            shipNameLabel.AddToClassList("header");
            shipFoldout.Add(shipNameLabel);

            SerializedObject shipSO = new SerializedObject(shipSettings);

            var pfThrottle = CreateField(shipSO, "Throttle");
            shipFoldout.Add(pfThrottle);
            
            var pfRotation = CreateField(shipSO, "Rotation");
            shipFoldout.Add(pfRotation);
           
            SerializedObject so = new SerializedObject(shipSettings.Health);
            
            var pfHealth = CreateField(so, "IntValue", "Starting Health");
            shipFoldout.Add(pfHealth);
            
            return shipFoldout;
            
        }

        private Foldout CreateAsteroidFoldout(AsteroidSettings asteroidSettings)
        {
            var asteroidFoldout = new Foldout();
            asteroidFoldout.text = asteroidSettings.name;
            
            Label shipNameLabel = new Label("Asteroids");
            shipNameLabel.AddToClassList("header");
            asteroidFoldout.Add(shipNameLabel);
            
            SerializedObject so = new SerializedObject(asteroidSettings);

            //MinMax Force
            var pfForce = CreateField(so, "Force");
            asteroidFoldout.Add(pfForce);
            //MinMax Size
            var pfSize = CreateField(so, "Size");
            asteroidFoldout.Add(pfSize);
            //MinMax Torque
            var pfTorque = CreateField(so, "Torque");
            asteroidFoldout.Add(pfTorque);

            return asteroidFoldout;
        }
        
        private VisualElement CreateField(SerializedObject so, string propertyName, string label = "")
        {
            if(so is null)
                return new VisualElement();
            
            if(string.IsNullOrEmpty(label))
                label = propertyName;
            
            SerializedProperty sp = so.FindProperty(propertyName);
            
            if(sp is null)
                return new VisualElement();
            
            
            VisualElement ve = new VisualElement();
            switch (sp.propertyType)
            {
                case SerializedPropertyType.Vector2:
                    ve = new VisualElement();
                    ve.Add(new Label(label));
                    
                    AddOwnMinMaxSlider(sp, ve);
                    
                    break;
                
                case SerializedPropertyType.Vector2Int:
                    ve = new VisualElement();
                    ve.Add(new Label(label));
                    
                    AddOwnMinMaxIntSlider(sp, ve);

                    break;
                
                default:
                    ve = new PropertyField(sp, label);
                    break;
            }
            ve.Bind(so);
            return ve;
        }

        private void AddOwnMinMaxIntSlider(SerializedProperty sp, VisualElement ve)
        {
            var min = sp.FindPropertyRelative("x");
            var max = sp.FindPropertyRelative("y");

            var minVisual = new VisualElement();
            var minSlider = new SliderInt(0, 10);
            minSlider.BindProperty(min);
            var minField = new IntegerField();
            minField.BindProperty(min);
            minVisual.Add(minSlider);
            minVisual.Add(minField);
            ve.Add(minVisual);

            var maxVisual = new VisualElement();
            var maxSlider = new SliderInt(0, 10);
            maxSlider.BindProperty(max);
            var maxField = new IntegerField();
            maxField.BindProperty(max);
            maxVisual.Add(maxSlider);
            maxVisual.Add(maxField);
            ve.Add(maxVisual);
            
            maxField.RegisterValueChangedCallback(evt => Sync(sp, false));
            maxSlider.RegisterValueChangedCallback(evt => Sync(sp, false));
            minField.RegisterValueChangedCallback(evt => Sync(sp, true));
            minSlider.RegisterValueChangedCallback(evt => Sync(sp, true));
        }

        private void Sync(SerializedProperty sp, bool minChanged)
        {
            var min = sp.FindPropertyRelative("x");
            var max = sp.FindPropertyRelative("y");
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

        private void AddOwnMinMaxSlider(SerializedProperty sp,VisualElement ve)
        {
            FloatField minField = new FloatField();
            minField.BindProperty(sp.FindPropertyRelative("x"));
            ve.Add(minField);

            var slider = new MinMaxSlider();
            slider.lowLimit = 0;
            slider.highLimit = 10;
            slider.BindProperty(sp);
            ve.Add(slider);
                    
            FloatField maxField = new FloatField();
            maxField.BindProperty(sp.FindPropertyRelative("y"));
            ve.Add(maxField);
        }


        
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
