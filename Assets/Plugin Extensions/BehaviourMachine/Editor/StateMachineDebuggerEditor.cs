using UnityEngine;
using UnityEditor;
using BehaviourMachine;

[CustomEditor(typeof(StateMachineDebugger))]
public class StateMachineDebuggerEditor : Editor
{		
	SerializedObject[] stateSOs;
	StateMachineDebugger debugger;
	bool[] fold;
	int oldStatesLenght;

	void OnEnable()
	{
		if (debugger == null) debugger = target as StateMachineDebugger;
		if (fold == null) fold = new bool[debugger.states.Length];
	}

	public override void OnInspectorGUI()
	{
		GUILayout.Space(5);
		serializedObject.Update();

		#region Updating State Lists
		debugger.states = debugger.GetComponents<StateBehaviour>();
		stateSOs = new SerializedObject[debugger.states.Length];

		for (int i = 0; i < debugger.states.Length; i++)
		{
			stateSOs[i] = new SerializedObject(debugger.states[i]);
		}
		#endregion

		#region Dealing with the Foldouts
		int curStatesLenght = debugger.states.Length;
		if (oldStatesLenght != curStatesLenght)
		{
			var temp = new bool[curStatesLenght];
			for (int i = 0; i < fold.Length && i < curStatesLenght; i++)
			{
				temp[i] = fold[i];
			}
			fold = temp;
		}
		oldStatesLenght = curStatesLenght;
		#endregion

		#region Sub-Editors
		for (int i = 0; i < curStatesLenght; i++)
		{			
			GUIStyle style = new GUIStyle(GUIStyle.none);
			style.padding.top += 5;
			style.padding.bottom += 5;		

			using (var v = new EditorGUILayout.VerticalScope(style))
			{
				GUI.Box(EditorGUI.IndentedRect(v.rect), "");
						
				stateSOs[i].Update();
				fold[i] = EditorGUILayout.InspectorTitlebar(fold[i], stateSOs[i].targetObject, true);
				if(fold[i])
					StateBehaviourEditor.DrawCustomEditor(debugger.states[i], stateSOs[i]);
				stateSOs[i].ApplyModifiedProperties();
			}
			EditorGUILayout.Space();
		}
		#endregion

		serializedObject.ApplyModifiedProperties();
	}
}