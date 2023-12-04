using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class MakeMenuItemECS : MonoBehaviour
{
    [MenuItem("Assets/Create/ECSFile")]
    static void OpenWindow()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(CreateECSFileWindow));
        window.Show();
    }
    public static void CreateECSClasses(string SystemName)
    {


        string FileTagPath = "Assets/DAT_NEWSCRIPTS/ECS_BASE/Base_Tag.cs";
        string FileIAspectPath = "Assets/DAT_NEWSCRIPTS/ECS_BASE/Base_InputAspect.cs";
        string FileAuthoringPath = "Assets/DAT_NEWSCRIPTS/ECS_BASE/Base_OwnerAuthoring.cs";
        string FileCollectSystemPath = "Assets/DAT_NEWSCRIPTS/ECS_BASE/Base_CollectSystem.cs";
        string FileWorkSystemPath = "Assets/DAT_NEWSCRIPTS/ECS_BASE/Base_WorkSystem.cs";
        string folderPath = "Assets/DAT_NEWSCRIPTS";

        string[] classNames = { SystemName + "_Tag", SystemName + "_InputAspect", SystemName + "_OwnerAuthoring", SystemName + "_CollectSystem", SystemName + "_WorkSystem" };
        string[] baseFilePath = { FileTagPath, FileIAspectPath, FileAuthoringPath, FileCollectSystemPath, FileWorkSystemPath };
        string folderName = SystemName + "Folder";
        if (!AssetDatabase.IsValidFolder(folderName))
        {
            AssetDatabase.CreateFolder("Assets/DAT_NEWSCRIPTS", folderName);
        }

        for (int i = 0; i < classNames.Length; i++)
        {
            string filePath = Path.Combine(folderPath + "/" + folderName, classNames[i] + ".cs");
            if (!File.Exists(filePath))
            {
                string templateContent = File.ReadAllText(baseFilePath[i]);
                templateContent = templateContent.Replace("Base", SystemName);
                File.WriteAllText(filePath, templateContent);
            }
        }
        AssetDatabase.Refresh();


    }

}
public class CreateECSFileWindow : EditorWindow
{
    private string SystemName = "";
    void OnGUI()
    {
        GUILayout.Label("Nhập tên System:");
        SystemName = EditorGUILayout.TextField(SystemName);

        if (GUILayout.Button("Create"))
        {
            MakeMenuItemECS.CreateECSClasses(SystemName);
            this.Close();
        }

        if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }
    }
}