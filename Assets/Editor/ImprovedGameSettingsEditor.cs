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
        }
        
        private Foldout GetShipFoldout(ShipSettings shipSettings)
        {
            var shipFoldout = new Foldout();
            shipFoldout.text = shipSettings.name;
            
            Label shipNameLabel = new Label("Ship");
            shipNameLabel.AddToClassList("header");
            shipFoldout.Add(shipNameLabel);

            SerializedObject shipSO = new SerializedObject(shipSettings);

            PropertyField pfThrottle = CreatePropertyField(shipSO, "Throttle");
            shipFoldout.Add(pfThrottle);
            
            PropertyField pfRotation = CreatePropertyField(shipSO, "Rotation");
            shipFoldout.Add(pfRotation);
           
            SerializedObject so = new SerializedObject(shipSettings.Health);
            
            PropertyField pfHealth = CreatePropertyField(so, "IntValue", "Starting Health");
            shipFoldout.Add(pfHealth);
            
            return shipFoldout;
            
        }

        private PropertyField CreatePropertyField(SerializedObject so, string propertyName, string label = "")
        {
            if(string.IsNullOrEmpty(label))
                label = propertyName;
            
            SerializedProperty sp = so.FindProperty(propertyName);
            PropertyField pf = new PropertyField(sp, label);
            pf.Bind(so);
            return pf;
        }

        private Foldout GetAsteroidFoldout(AsteroidSettings asteroidSettings)
        {
            var asteroidFoldout = new Foldout();
            asteroidFoldout.text = asteroidSettings.name;
            
            Label shipNameLabel = new Label("Asteroids");
            shipNameLabel.AddToClassList("header");
            asteroidFoldout.Add(shipNameLabel);

            //MinMax Force
            //MinMax Size
            //MinMax Torque
            //MinMax SpawnRate
            
            
            

            return asteroidFoldout;
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
