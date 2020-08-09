using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DefinitionDownloaderEditor : EditorWindow
{
    
    /// <summary>
    /// Predefined python enviroment const variable what program expected
    /// </summary>
    const string PYTHON_HOME = "PYTHON_HOME";
    /// <summary>
    /// Destination folder for download definitions
    /// </summary>
    private string _downloadFolder = "Assets/Definitions";
    /// <summary>
    /// Uniq root definition folder name from GoogleDrive
    /// </summary>
    private string _definitionsFolder = "Druzstvo_Remote_Definitions";
    /// <summary>
    /// Scripth Path - use backslash because its window command path
    /// </summary>
    private string _scriptPath = @"Assets\Plugin_DefinitionDownloader\DefinitionDownloader\main.py";
    /// <summary>
    /// Credential Path - google drive generated credential
    /// </summary>
    private string _credentialPath = "Assets/Plugin_DefinitionDownloader/DefinitionDownloader/credentials.json";
    
    public class Item
    {
        public string id;
        public string name;
        public string mimeType;
    }
    
    public class PythonParam
    {
        public string CommandParam;
        public string DefinitionFolder;
        public string FolderIdx;
        public string DownloadFolder;
    }

    [MenuItem("Custom/DefinitionDownloader")]
    static void Init()
    {
        var window = (DefinitionDownloaderEditor) EditorWindow.GetWindow(typeof(DefinitionDownloaderEditor));
        window.name = "DefinitionDownloader";
        window.Show();
    }
    
    private bool _settingsPanel = true;
    public List<Item> data = new List<Item>();

    private void OnGUI()
    {
        _settingsPanel = EditorGUILayout.Foldout(_settingsPanel, "Settings");
        if (_settingsPanel)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Check Properties");
                EditorGUILayout.BeginHorizontal("Box");
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Python enviroment path:",GUILayout.Width( 150 ) );
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(string.IsNullOrEmpty(GetPythonEnvPath()) ? "FAILED" : "OK");
                if(string.IsNullOrEmpty(GetPythonEnvPath())) Debug.LogError("Python Enviroment Fail");
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal("Box");
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Script path:",GUILayout.Width( 150 ) );
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(!File.Exists(_scriptPath) ? "FAILED" : "OK");
                if(!File.Exists(_scriptPath))Debug.LogError("Script Path FAIL");
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal("Box");
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Credential path:",GUILayout.Width( 150 ) );
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(!File.Exists(_credentialPath) ? "FAILED" : "OK");
                if(!File.Exists(_credentialPath))Debug.LogError("Credential Path FAIL");
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Paths:");
                EditorGUILayout.BeginHorizontal("Box");
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Download Folder:",GUILayout.Width( 130 ) );
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(_downloadFolder);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal("Box");
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Drive Uniq Folder:",GUILayout.Width( 130 ) );
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(_definitionsFolder);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal("Box");
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Downloader Script:",GUILayout.Width( 130 ) );
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(_scriptPath);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal("Box");
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Credential json:",GUILayout.Width( 130 ) );
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(_credentialPath);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                
            EditorGUILayout.EndVertical();
        }
        
        if (GUILayout.Button("Load Google Drive Definitions"))
        {
            data = new List<Item>();
            var result = RunPythonProcess(new PythonParam
            {
                CommandParam = "showDefinitions",
                DefinitionFolder = _definitionsFolder,
            });
            foreach (var r in result)
            {
                var item = ConvertToObject<Item>(r);
                data.Add(item);
            }
        }
        
        if(data.Count > 0){
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Label("Definitions");
            for (var i = 0; i < data.Count; i++)
            {
                EditorGUILayout.BeginHorizontal("box");
                if (GUILayout.Button($"Download ({data[i].name})"))
                {
                    Debug.Log(data[i].id);
                    RunPythonProcess(new PythonParam
                    {
                        CommandParam = "downloadDefinitions",
                        DefinitionFolder = _definitionsFolder,
                        FolderIdx = data[i].id,
                        DownloadFolder = _downloadFolder
                    });
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
    }

    string GetPythonEnvPath()
    {
        var pythonPath = Environment.GetEnvironmentVariable(PYTHON_HOME, EnvironmentVariableTarget.User);
        if (string.IsNullOrEmpty(pythonPath))
        {
            Debug.LogError("Python enviroment variable is not set properly!");
            return null;
        }
        return pythonPath.TrimEnd(';');
    }
    
    ProcessStartInfo CreatePythonProcessInfo(PythonParam param)
    {
        var pythonPath = Environment.GetEnvironmentVariable(PYTHON_HOME, EnvironmentVariableTarget.User);
        if(string.IsNullOrEmpty(pythonPath))
            throw new NullReferenceException("PythonPath is null!");
        
        pythonPath = pythonPath.TrimEnd(';');
        var process = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{_scriptPath}\" \"{param.CommandParam}\" \"{param.DefinitionFolder}\" \"{param.FolderIdx}\" \"{param.DownloadFolder}\"",  
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };
        return process;
    }

    Process CreatePythonProcess(PythonParam param)
    {
        var psi = CreatePythonProcessInfo(param);
        var process = Process.Start(psi);
        if(process == null)
            throw new NullReferenceException("Python process is null!");

        return process;
    }

    void DebugPythonProcess(Process process, bool isResultDebug = false, bool isErrorDebug = true)
    {
        if (isResultDebug)
        {
            
            string errors = process.StandardError.ReadToEnd();
            Debug.Log("Python Process Errors:");
            Debug.Log(errors);
        }

        if(isErrorDebug)
        {
            string results = process.StandardOutput.ReadToEnd();
            Debug.Log("Python Process Result:");
            Debug.Log(results);
        }
    }

    List<string> RunPythonProcess(PythonParam param)
    {
        var resultList = new List<string>();
        var process = CreatePythonProcess(param);
        
        while (!process.StandardOutput.EndOfStream)
        {
            var line = process.StandardOutput.ReadLine();
            resultList.Add(line);
        }

        DebugPythonProcess(process);
        AssetDatabase.Refresh();
        return resultList;
    }
    
    T ConvertToObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

}
