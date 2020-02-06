using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TFG.UI
{
    public class TFG_UI_Menus : MonoBehaviour
    {
        [MenuItem("TFG/UI Tools/Create UI Group")]
        public static void CreateUIGroup()
        {
            Debug.Log("Creating UI group.");
            GameObject uiGroup = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG/Prefabs/UI/UI_Group.prefab");
            if(uiGroup)
            {
                GameObject createdGroup = (GameObject)Instantiate(uiGroup);
                createdGroup.name = "UI_Group";

            }
            else
            {
                EditorUtility.DisplayDialog("UI Tools Wraning", "Cannot find UI Group Prefabs", "OK");
            }
        }
    }
}
