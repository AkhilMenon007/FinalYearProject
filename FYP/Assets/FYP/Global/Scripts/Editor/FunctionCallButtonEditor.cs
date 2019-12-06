using UnityEditor;
using UnityEngine;

namespace FYP
{
    [CustomEditor(typeof(FunctionCallButton))]
    public class FunctionCallButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Call Functions"))
            {
                FunctionCallButton button = target as FunctionCallButton;
                button.FunctionList.Invoke();
            }
        }
    }
}