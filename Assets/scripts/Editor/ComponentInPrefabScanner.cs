using UnityEngine;
using UnityEditor;
using TMPro;

/// <summary>
/// Скрипт для поиска компонентов во всех префабах в проекте.
/// </summary>
public class ComponentInPrefabScanner : EditorWindow
{
    [MenuItem("Custom Tools/Scan TMP Prefabs")]
    static void Init()
    {
        ComponentInPrefabScanner window = (ComponentInPrefabScanner)EditorWindow.GetWindow(typeof(ComponentInPrefabScanner));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Scan Prefabs for TextMeshProUGUI"))
        {
            ScanPrefabsForTMP();
        }
    }

    void ScanPrefabsForTMP()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab"); // Find all prefab assets
        foreach (string guid in guids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);


            if (prefab != null)
            {
                TextMeshProUGUI[] tmpComponents = prefab.GetComponentsInChildren<TextMeshProUGUI>(true); // Find all TextMeshProUGUI components in the prefab
                foreach (TextMeshProUGUI tmpComponent in tmpComponents)
                {
                    string message = "Prefab: " + prefab.name + " (" + prefabPath + ")" + "\n" +
                        "SkeletonGraphic Component: " + tmpComponent.name + " (" + AssetDatabase.GetAssetPath(tmpComponent) + ")";
                    Debug.Log(message);
                }
            }


        }
            Debug.Log("Scan Complete");
    }
}
