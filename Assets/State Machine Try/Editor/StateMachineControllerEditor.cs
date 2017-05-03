using UnityEngine;
using UnityEditor;

namespace StateMachineTry
{
	[CustomEditor(typeof(StateMachineController))]
	public class StateMachineControllerEditor : Editor
	{
		SerializedObject so;
		SerializedObject[] stateSOs;
		StateMachineController controller;
		bool[] fold;
		int oldStatesLenght;

		void OnEnable()
		{
			controller = target as StateMachineController;
			fold = new bool[controller.states.Length];
			if (so == null) so = new SerializedObject(controller);
			if (stateSOs == null) stateSOs = new SerializedObject[controller.states.Length];
			for (int i = 0; i < controller.states.Length; i++)
			{
				stateSOs[i] = new SerializedObject(controller.states[i]);
			}
			controller.states = controller.GetComponents<State>();
			foreach (State state in controller.states)
			{
				state.hideFlags = HideFlags.None;
				//state.hideFlags = HideFlags.HideInInspector;
			}
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(so.FindProperty("startingState"));

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

			EditorGUI.indentLevel++;
			for (int i = 0; i < controller.states.Length; i++)
			{
				fold[i] = EditorGUILayout.Foldout(fold[i], controller.states[i].GetType().Name);
				if (fold[i])
				{
					GUIStyle style = new GUIStyle(GUIStyle.none);
					style.padding.top += 5;
					style.padding.bottom += 5;
					using (var v = new EditorGUILayout.VerticalScope(style))
					{
						GUI.Box(EditorGUI.IndentedRect(v.rect), "");
						StateEditor.DrawCustomEditor(controller.states[i], stateSOs[i]);
					}
				}
			}
			EditorGUI.indentLevel--;
		}
	}
}