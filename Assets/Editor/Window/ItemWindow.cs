using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ItemWindow : EditorWindow
{
    #region Presets
    private static class WindowColors
    {
        static public Color box = Color.black;
        static public Color selected = new Color(1f, 0.5f, 0f, 1f);
        static public Color add = Color.green;
        static public Color remove = Color.red;
        static public Color grey = Color.grey;
        static public Color text = Color.white;
        static public Color transparent = new Color(0f, 0f, 0f, 0f);
    }
    private static class WindowPresets
    {
        public static GUILayoutOption[] itemsButton =
        {
            GUILayout.MaxHeight(50f),
            GUILayout.Height(30f),
            GUILayout.ExpandWidth(true),
        };

        public static GUILayoutOption[] title =
        {
            GUILayout.MaxHeight(50f),
            GUILayout.Height(30f),
            GUILayout.ExpandWidth(true),
        };

        public static GUILayoutOption[] searchBar =
        {
            GUILayout.ExpandHeight(true),
            GUILayout.Width(100f),
            GUILayout.MaxWidth(200f),
            GUILayout.MinWidth(10f),
        };

        public static GUILayoutOption[] border =
        {
            GUILayout.ExpandHeight(true),
            GUILayout.Width(20f),
            GUILayout.MaxWidth(40f),
            GUILayout.MinWidth(5f),
        };

        public static GUILayoutOption[] expand =
        {
            GUILayout.ExpandHeight(true),
            GUILayout.ExpandWidth(true),
        };

        public static GUILayoutOption[] visuals =
        {
            GUILayout.Width(150f),
            GUILayout.MaxWidth(150f),
            GUILayout.MinWidth(20f),

            GUILayout.Height(150f),
            GUILayout.MaxHeight(150f),
            GUILayout.MinHeight(20f),
        };
    }
    private static class LabelPresets
    {
        public static GUIStyle titleCenter = new GUIStyle(EditorStyles.boldLabel) { fontSize = 20, alignment = TextAnchor.MiddleCenter, };
        public static GUIStyle search = new GUIStyle(EditorStyles.toolbarPopup) { fontSize = 15, alignment = TextAnchor.MiddleLeft, };
        public static GUIStyle popup = new GUIStyle(EditorStyles.toolbarPopup) { fontSize = 15, alignment = TextAnchor.UpperCenter, };

        public static GUIStyle center = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter, };
        public static GUIStyle left = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleLeft, };
        public static GUIStyle rigth = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleRight, };
        public static GUIStyle down = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.LowerCenter, };
    }
    #endregion

    private SODatabase database;
    private static string pathIcon = "Assets/Sandbox/nrobert/UI/icons/classes/organization.png";

    [MenuItem("Tools/Item Manager")]
    public static void ShowWindow()
    {
        ItemWindow window = GetWindow<ItemWindow>("Item Manager");

        Texture icone = AssetDatabase.LoadAssetAtPath<Texture>(pathIcon);
        window.titleContent = new GUIContent("Item Manager", icone);

        SODatabase database = Resources.Load<SODatabase>("DataBase");
        window.SetDatabase(database);
    }

    private void SetDatabase(SODatabase database)
    {
        this.database = database;
    }

    private void Save()
    {
        EditorUtility.SetDirty(database);
        for (int i = 0; i < database.AllItems.Count; i++)
        {
            EditorUtility.SetDirty(database.AllItems[i]);
        }
        AssetDatabase.SaveAssets();
    }

    private void OnGUI()
    {
        Title();
        NoDatabase();
        using (new GUILayout.HorizontalScope(WindowPresets.expand))
        {
            SearchBar();
            Border();
            Content();
        }
    }

    #region Header
    private void Title()
    {
        GUI.backgroundColor = WindowColors.box;
        using (new GUILayout.HorizontalScope(GUI.skin.box))
        {
            GUI.color = WindowColors.text;
            EditorGUILayout.LabelField("Item Manager", LabelPresets.titleCenter, WindowPresets.title);
        }
    }
    private void NoDatabase()
    {
        if (database == null)
        {
            EditorGUILayout.LabelField("No database loaded in resources folder");
            return;
        }
    }
    #endregion

    #region SearchBar
    private string itemSearch = string.Empty;
    private ItemType selectedItemType = ItemType.All;
    private Vector2 scrollViewItem;
    private SOItems itemToDisplay = null;
    private string path = "Assets/ScriptableObjects/Item/";
    private void SearchBar()
    {
        using (new GUILayout.VerticalScope(WindowPresets.searchBar))
        {
            InputSearchBar();
            PopupTypeItem();
            ScrollViewItem();
            AddItem();
        }
    }
    private void InputSearchBar()
    {
        using (new GUILayout.HorizontalScope())
        {
            GUI.color = WindowColors.text;
            EditorGUILayout.LabelField("Search :", GUILayout.MaxWidth(45f));
            GUI.backgroundColor = WindowColors.text;
            itemSearch = EditorGUILayout.TextArea(itemSearch, GUILayout.ExpandWidth(true));
        }
    }
    private void PopupTypeItem()
    {
        GUI.backgroundColor = WindowColors.text;
        selectedItemType = (ItemType)EditorGUILayout.EnumPopup(selectedItemType, GUILayout.ExpandWidth(true));
    }
    private void ScrollViewItem()
    {
        using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(scrollViewItem, WindowPresets.expand))
        {
            scrollViewItem = scrollViewScope.scrollPosition;
            List<SOItems> searchItem = SearchItem();
            for (int i = 0; i < searchItem.Count; i++)
            {
                ButtonSoItems(searchItem[i]);
            }
        }
    }
    private List<SOItems> SearchItem()
    {
        List<SOItems> searchItem;

        if (selectedItemType == ItemType.All)
        {
            if (itemSearch == "")
            {
                searchItem = database.AllItems;
            }
            else
            {
                searchItem = database.AllItems.Where(items => items.name.Replace(" ", "").ToLower().Contains(itemSearch.ToLower().Replace(" ", ""))).ToList();
            }
        }
        else
        {
            searchItem = database.AllItems.Where(items => items.itemType == selectedItemType).ToList();

            if (itemSearch != "")
            {
                searchItem = searchItem.Where(items => items.name.Replace(" ", "").ToLower().Contains(itemSearch.ToLower().Replace(" ", ""))).ToList();
            }
        }

        return searchItem;
    }
    private void ButtonSoItems(SOItems item)
    {
        GUI.backgroundColor = itemToDisplay == item ? WindowColors.selected : WindowColors.text;
        if (GUILayout.Button(item.NameItem, WindowPresets.itemsButton))
        {
            itemToDisplay = item;
            GUI.FocusControl(null);
        }
    }
    private void AddItem()
    {
        GUI.backgroundColor = WindowColors.add;
        if (GUILayout.Button("+", WindowPresets.itemsButton))
        {
            if (CanAddItem())
            {
                CreateNewItem();
            }
        }
    }
    private void CreateNewItem()
    {
        SOItems newItem = ScriptableObject.CreateInstance<SOItems>();
        newItem.itemType = selectedItemType;
        newItem.name = "NewItem";
        newItem.NameItem = "NewItem";

        database.AllItems.Add(newItem);

        string path = this.path + selectedItemType.ToString() + "/" + newItem.name + ".asset";

        AssetDatabase.CreateAsset(newItem, path);
        Save();
    }
    private bool CanAddItem()
    {
        if (selectedItemType == ItemType.All)
        {
            ShowError("None item type selected");
            return false;
        }

        if (IsAssetAlreadyExist())
        {
            ShowError("A item with the name NewItem already exist");
            return false;
        }

        return true;
    }
    private bool IsAssetAlreadyExist()
    {
        return database.AllItems.Where(items => items.name == "NewItem").Select(items => items).ToList().Count > 0;
    }
    private void ShowError(string error)
    {
        Debug.LogError(error);
    }
    #endregion

    #region Border
    private void Border()
    {
        GUI.backgroundColor = WindowColors.box;
        using (new GUILayout.HorizontalScope(GUI.skin.box, WindowPresets.border))
        {

        }
    }
    #endregion

    #region Content
    private float space = 0f;
    private void Content()
    {
        if (itemToDisplay != null)
        {
            ShowContent();
        }
    }
    private void ShowContent()
    {
        using (new GUILayout.VerticalScope(WindowPresets.expand))
        {
            //Name
            using (new GUILayout.HorizontalScope())
            {
                GUI.backgroundColor = WindowColors.text;
                EditorGUILayout.LabelField("Name", GUILayout.Width(50f));
                itemToDisplay.NameItem = EditorGUILayout.TextField(itemToDisplay.NameItem);
                GUI.backgroundColor = WindowColors.add;
                if (GUILayout.Button("V", GUILayout.Width(50f)))
                {
                    string newItemName = itemToDisplay.NameItem; //magic
                    RenameAsset();
                    itemToDisplay.NameItem = newItemName; //magic
                }
            }

            //Description
            using (new GUILayout.HorizontalScope())
            {
                GUI.backgroundColor = WindowColors.text;
                EditorGUILayout.LabelField("Description", GUILayout.Width(70f));
                itemToDisplay.description = EditorGUILayout.TextArea(itemToDisplay.description, GUILayout.Height(50f));
            }

            //Render
            using (new GUILayout.HorizontalScope())
            {
                //Icon
                using (new GUILayout.HorizontalScope(LabelPresets.left))
                {
                    GUI.backgroundColor = WindowColors.text;
                    EditorGUILayout.LabelField("Icon", GUILayout.Width(25f));
                    itemToDisplay.Sprite = EditorGUILayout.ObjectField(itemToDisplay.Sprite, typeof(Sprite), false, WindowPresets.visuals) as Sprite;
                }
                //Mesh
                using (new GUILayout.HorizontalScope(LabelPresets.center))
                {
                    GUI.backgroundColor = WindowColors.text;
                    EditorGUILayout.LabelField("Mesh", GUILayout.Width(35f));
                    itemToDisplay.mesh = EditorGUILayout.ObjectField(itemToDisplay.mesh, typeof(Mesh), false, WindowPresets.visuals) as Mesh;
                }
                //Material
                using (new GUILayout.HorizontalScope(LabelPresets.rigth))
                {
                    GUI.backgroundColor = WindowColors.text;
                    EditorGUILayout.LabelField("Material", GUILayout.Width(50f));
                    itemToDisplay.material = EditorGUILayout.ObjectField(itemToDisplay.material, typeof(Material), false, WindowPresets.visuals) as Material;
                }
            }

            //Data
            using (new GUILayout.HorizontalScope())
            {
                //MaxStack
                using (new GUILayout.HorizontalScope(LabelPresets.left))
                {
                    GUI.backgroundColor = WindowColors.text;
                    EditorGUILayout.LabelField("Max Stack", GUILayout.Width(75f));
                    itemToDisplay.MaxStack = EditorGUILayout.IntField(itemToDisplay.MaxStack);
                }
                GUILayout.Space(100f);
                //IsDiscover
                using (new GUILayout.HorizontalScope(LabelPresets.left))
                {
                    GUI.backgroundColor = WindowColors.text;
                    EditorGUILayout.LabelField("Is Discover", GUILayout.Width(75f));
                    itemToDisplay.IsDiscover = EditorGUILayout.Toggle(itemToDisplay.IsDiscover);
                }
                GUILayout.Space(100f);
                //IsEquipable
                using (new GUILayout.HorizontalScope(LabelPresets.center))
                {
                    GUI.backgroundColor = WindowColors.text;
                    EditorGUILayout.LabelField("Is Equipable", GUILayout.Width(75f));
                    itemToDisplay.isEquipable = EditorGUILayout.Toggle(itemToDisplay.isEquipable);
                }
                GUILayout.Space(100f);
                //TypeItem
                using (new GUILayout.HorizontalScope(LabelPresets.rigth))
                {
                    GUI.backgroundColor = WindowColors.text;
                    EditorGUILayout.LabelField("Item Type", GUILayout.Width(75f));
                    ItemType newType = (ItemType)EditorGUILayout.EnumPopup(itemToDisplay.itemType);
                    if (newType != ItemType.All)
                    {
                        MoveAsset(newType);
                    }
                }
            }

            // Save/delete
            using (new GUILayout.HorizontalScope(LabelPresets.down))
            {
                //Save
                GUI.backgroundColor = WindowColors.add;
                if (GUILayout.Button("Save Modifications", GUILayout.ExpandWidth(true)))
                {
                    Save();
                }
                GUILayout.Space(space);

                //delete
                GUI.backgroundColor = WindowColors.remove;
                if (GUILayout.Button("Delete", GUILayout.ExpandWidth(true)))
                {
                    DeleteAsset();
                }
            }
        }
    }
    private void RenameAsset()
    {
        string nameAsset = ConvertFirstLetterToUpper(itemToDisplay.NameItem).Replace(" ", "");
        AssetDatabase.RenameAsset(path + itemToDisplay.itemType.ToString() + "/" + itemToDisplay.name + ".asset",
                                  nameAsset + ".asset");

        itemToDisplay.name = nameAsset;
    }
    private void DeleteAsset()
    {
        database.AllItems.Remove(itemToDisplay);

        AssetDatabase.DeleteAsset(path + itemToDisplay.itemType.ToString() + "/" + itemToDisplay.name + ".asset");

        Save();
    }
    private void MoveAsset(ItemType newType)
    {
        AssetDatabase.MoveAsset(path + itemToDisplay.itemType.ToString() + "/" + itemToDisplay.name + ".asset",
                                path + newType.ToString() + "/" + itemToDisplay.name + ".asset");
        itemToDisplay.itemType = newType;
    }
    private string ConvertFirstLetterToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        input = char.ToUpper(input[0]) + input.Substring(1);

        for (int i = 1; i < input.Length; i++)
        {
            if (input[i - 1] == ' ')
            {
                input = input.Substring(0, i) + char.ToUpper(input[i]) + input.Substring(i + 1);
            }
        }

        return input;
    }
    #endregion
}