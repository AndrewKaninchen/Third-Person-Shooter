using UnityEngine;
using UnityEditor;

namespace MyFSM
{
	[CustomEditor(typeof(StateMachine))]
	public class StateMachineEditor : Editor
	{		
		SerializedObject[] stateSOs;
		StateMachine controller;
		bool[] fold;
		int oldStatesLenght;

		void OnEnable()
		{
			if (controller == null) controller = target as StateMachine;
			if (fold == null) fold = new bool[controller.states.Length];
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("startingState"));
			serializedObject.ApplyModifiedProperties();

			#region Updating State Lists
			controller.states = controller.GetComponents<State>();
			stateSOs = new SerializedObject[controller.states.Length];

			for (int i = 0; i < controller.states.Length; i++)
			{
				stateSOs[i] = new SerializedObject(controller.states[i]);
			}
			foreach (State state in controller.states)
			{
				state.hideFlags = HideFlags.HideInInspector;
			} 
			#endregion

			#region Dealing with the Foldouts
			int curStatesLenght = controller.states.Length;
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
						StateEditor.DrawCustomEditor(controller.states[i], stateSOs[i]);
					stateSOs[i].ApplyModifiedProperties();
				}
				EditorGUILayout.Space();
			}
			#endregion			
		}
	}
}