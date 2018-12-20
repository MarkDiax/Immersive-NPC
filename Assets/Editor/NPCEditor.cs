using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(NPC))]
public class NPCEditor : Editor
{
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		NPC npc = (NPC)target;
		if (GUILayout.Button("Save Voice Settings")) {
			npc.SaveVoiceSettings();
		}
	}
}