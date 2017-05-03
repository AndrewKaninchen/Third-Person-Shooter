using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace StateMachineTry
{

	[CustomEditor(typeof(State), true)]
	public class StateEditor : Editor
	{
		private SerializedObject so;

		public static void DrawCustomEditor(State state, SerializedObject so)
		{
			SerializedProperty prop;

			#region Draw State's Properties		
			prop = so.GetIterator();
			prop.Next(true);

			while (prop.NextVisible(false))
			{
				EditorGUILayout.PropertyField(prop, true);
			}
			#endregion

			EditorUtility.SetDirty(state);
		}

		private void OnEnable()
		{
			if (so == null)
				so = new SerializedObject(target);
		}
	}
}