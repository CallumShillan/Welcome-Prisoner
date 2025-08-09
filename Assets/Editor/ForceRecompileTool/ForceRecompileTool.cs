using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.Compilation;

public static class ForceRecompileTool
{
    [MenuItem("Tools/Force Script Recompile")]
    public static void ForceRecompile()
    {
        CompilationPipeline.RequestScriptCompilation();
        Debug.Log("Requested script recompilation.");
    }
}