using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/*
[CustomEditor(typeof(Transform))]
public class BoardEditor : Editor {
	private void OnSceneGUI() {
		var t = target as Transform;

		var tScreen = GUIUtility.ScreenToGUIPoint(new Vector2(t.position.x, t.position.y));
		var size = new Vector2(1, 1);
		if (Handles.Button(t.position, Quaternion.Euler(30, -45, 0), 0.25f, 0.3f, Handles.CircleHandleCap)) {
			Undo.RecordObject(target, "Rotate 90");
			t.Rotate(Vector3.up, 90);
		}
	}
}
*/