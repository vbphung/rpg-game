using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Combat
{
    public class WeaponEditor : EditorWindow
    {
        private static Weapon editedWeapon;
        private TextField newWeaponName;

        [MenuItem("Window/Editors/Weapon")]
        private static void ShowWindow()
        {
            var window = GetWindow<WeaponEditor>();
            window.titleContent = new GUIContent("Weapon Editor");
            window.Show();
        }

        [OnOpenAssetAttribute(1)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            Weapon weaponInstance = EditorUtility.InstanceIDToObject(instanceID) as Weapon;
            if (weaponInstance != null)
            {
                editedWeapon = weaponInstance;
                ShowWindow();
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            LoadTreeAsset();
            LoadAllWeapons(out Weapon[] weapons);

            var weaponList = rootVisualElement.Query<ListView>("weapon-list").First();
            weaponList.makeItem = () => new Label();
            weaponList.bindItem = (element, i) => (element as Label).text = weapons[i].name;
            weaponList.itemsSource = weapons;
            weaponList.itemHeight = 16;
            weaponList.selectionType = SelectionType.Single;
            weaponList.onSelectionChange += ShowWeaponInfo;
            weaponList.selectedIndex = 0;

            if (editedWeapon != null)
                weaponList.selectedIndex = Array.FindIndex(weapons, weapon => weapon.Equals(editedWeapon));

            ShowWeaponInfo(new Weapon[] { weapons[weaponList.selectedIndex] });

            var searchField = rootVisualElement.Query<TextField>("weapon-search-field").First();
            searchField.RegisterValueChangedCallback(changeEvent =>
            {
                if (String.IsNullOrWhiteSpace(changeEvent.newValue))
                    weaponList.itemsSource = weapons;
                else
                    weaponList.itemsSource = (from weapon in weapons
                                              where Regex.IsMatch(weapon.name, Regex.Escape(changeEvent.newValue), RegexOptions.IgnoreCase)
                                              select weapon).ToList();

                weaponList.Refresh();
            });

            newWeaponName = rootVisualElement.Query<TextField>("new-weapon-name").First();

            var createWeaponButton = rootVisualElement.Query<Button>("weapon-create-button").First();
            createWeaponButton.text = "New Weapon";
            createWeaponButton.clicked += () =>
            {
                if (String.IsNullOrWhiteSpace(newWeaponName.value))
                    return;

                Weapon newWeapon = ScriptableObject.CreateInstance<Weapon>();
                AssetDatabase.CreateAsset(newWeapon, "Assets/Game/Weapons/" + newWeaponName.value + ".asset");
                AssetDatabase.SaveAssets();

                LoadAllWeapons(out weapons);
                weaponList.itemsSource = weapons;
                weaponList.Refresh();
            };
        }

        #region Show
        private void ShowWeaponInfo(IEnumerable<object> weaponObjects)
        {
            foreach (var weaponObject in weaponObjects)
            {
                editedWeapon = weaponObject as Weapon;
                var serializedWeapon = new SerializedObject(editedWeapon);
                SerializedProperty weaponProperty = serializedWeapon.GetIterator();
                weaponProperty.Next(true);

                var propertiesRoot = rootVisualElement.Query<ScrollView>("weapon-properties").First();
                propertiesRoot.Clear();

                while (weaponProperty.NextVisible(false))
                {
                    var propertyField = new PropertyField(weaponProperty);

                    propertyField.SetEnabled(weaponProperty.name != "m_Script");
                    propertyField.Bind(serializedWeapon);
                    propertiesRoot.Add(propertyField);

                    if (weaponProperty.name == "icon")
                        propertyField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvent) => ShowWeaponIcon(editedWeapon.GetIcon()));
                }
            }
        }

        private void ShowWeaponIcon(Texture2D weaponIcon)
        {
            var icon = rootVisualElement.Query<Image>("weapon-icon").First();
            icon.image = weaponIcon;
        }
        #endregion

        #region Load
        private VisualElement LoadTreeAsset()
        {
            rootVisualElement.Add(Resources.Load<VisualTreeAsset>("WeaponEditor").CloneTree());
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("WeaponEditor_Style"));
            return rootVisualElement;
        }

        private void LoadAllWeapons(out Weapon[] weaponList)
        {
            var guids = AssetDatabase.FindAssets("t:Weapon");
            weaponList = new Weapon[guids.Length];
            for (int i = 0; i < guids.Length; ++i)
                weaponList[i] = AssetDatabase.LoadAssetAtPath<Weapon>(AssetDatabase.GUIDToAssetPath(guids[i]));
        }
        #endregion
    }
}