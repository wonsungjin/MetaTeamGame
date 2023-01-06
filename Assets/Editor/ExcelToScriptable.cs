using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System;
using CreateScriptable;

public class ExcelToScriptable : EditorWindow
{
    string csv = "";
    string folder = "";
    string __GUID = "";//858ed67e0d64b72429e8c773f1903334
    string __name = "";


    [MenuItem("MyTools/ExcelToScriptable")]   
    public static void ShowWindow()
    {
        ExcelToScriptable ets = (ExcelToScriptable)EditorWindow.GetWindow(typeof(ExcelToScriptable));
        ets.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Label("Input CSV Name");
        csv = GUILayout.TextField(csv, 1000);

        GUILayout.Label("Input Save Folder");
        folder = GUILayout.TextField(folder, 1000);

        GUILayout.Label("Input GUID");
        __GUID = GUILayout.TextField(__GUID, 1000);

        if (GUILayout.Button("START CONVERT"))
        {
            //FileManageMent fileManageMent = new FileManageMent("C:/Users/User/Desktop/"+ csv + ".csv", folder, __GUID, __name);
            //fileManageMent.Run();
            
        }

        GUILayout.EndVertical();
    }
}
