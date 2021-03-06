﻿using UnityEditor;
using BehaviourMachine;

[CustomEditor(typeof(StateBehaviour), true)]
public class StateBehaviourEditor : Editor
{
	public static void DrawCustomEditor(StateBehaviour state, SerializedObject so)
	{
		SerializedProperty prop;

		#region Draw State's Properties		
		prop = so.GetIterator();
		prop.Next(true);
		prop.NextVisible(false);

		while (prop.NextVisible(false))
		{
			EditorGUILayout.PropertyField(prop, true);
		}
		#endregion

		EditorUtility.SetDirty(state);
	}
}