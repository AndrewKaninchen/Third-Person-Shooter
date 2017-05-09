using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace MyFSM
{
	[CustomEditor(typeof(State), true)]
	public class StateEditor : Editor
	{
		public static void DrawCustomEditor(State state, SerializedObject so)
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
}