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
                var shipFoldout = GetShipFoldout(ship);
                _root.Add(shipFoldout);
            }
            
            AsteroidSettings[] asteroidSettingsArray = GetAssetsOfType<AsteroidSettings>();

            foreach (var asteroid in asteroidSettingsArray)
            {
                var asteroidFoldout = GetAsteroidFoldout(asteroid);
                _root.Add(asteroidFoldout);
            }
        }
        
        private Foldout GetShipFoldout(ShipSettings shipSettings)
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

        private Foldout GetAsteroidFoldout(AsteroidSettings asteroidSettings)
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
            if(string.IsNullOrEmpty(label))
                label = propertyName;
            
            SerializedProperty sp = so.FindProperty(propertyName);
            VisualElement ve = new VisualElement();
            switch (sp.propertyType)
            {
                case SerializedPropertyType.Vector2:
                    ve = new VisualElement();
                    ve.Add(new Label(label));
                    
                    
                    FloatField minField = new FloatField();
                    minField.BindProperty(sp.FindPropertyRelative("x"));
                    ve.Add(minField);
                    
                    var slider = new MinMaxSlider();
                    slider.lowLimit = 0;
                    slider.highLimit = 100;
                    slider.BindProperty(sp);
                    ve.Add(slider);
                    
                    FloatField maxField = new FloatField();
                    maxField.BindProperty(sp.FindPropertyRelative("y"));
                    ve.Add(maxField);
                    
                    break;
                
                default:
                    ve = new PropertyField(sp, label);
                    break;
            }
            ve.Bind(so);
            return ve;
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
