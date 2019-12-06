using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif

/// <summary>
/// Scriptable Objects with OnBegin and OnEnd Callbacks which can be called without an instance of the object running in the scene
/// </summary>

public abstract class MonoScriptableobject : ScriptableObject
{
    abstract protected void OnBegin();
    abstract protected void OnEnd();

#if UNITY_EDITOR
    protected void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayStateChange;
    }

    protected void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayStateChange;
    }

    void OnPlayStateChange(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            OnBegin();
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            OnEnd();
        }
    }
#else
        protected void OnEnable()
        {
            OnBegin();
        }
 
        protected void OnDisable()
        {
            OnEnd();
        }
#endif
}