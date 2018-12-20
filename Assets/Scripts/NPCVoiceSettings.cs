using UnityEngine;
using System.Collections;
using RogoDigital.Lipsync;

[CreateAssetMenu(fileName = "NPCVoiceSettings", menuName = "NPC/VoiceSettings", order = 1)]
public class NPCVoiceSettings : ScriptableObject
{
	public string maryttsVoiceName = "dfki-spike-hsmm";
	public LipSyncRuntimeGenerator.MaryXMLAttribute[] maryXMLAttributes = {
		new LipSyncRuntimeGenerator.MaryXMLAttribute() {
			attributeTitle = "rate",
			attributeValue = "-13%"
		},
		new LipSyncRuntimeGenerator.MaryXMLAttribute() {
			attributeTitle = "pitch",
			attributeValue = "+20%"
		},
		new LipSyncRuntimeGenerator.MaryXMLAttribute() {
			attributeTitle = "contour",
			attributeValue = "(10%,+10%)(30%, +0%)(50%,-10%)(80%,-5%)(100%,-20%)"
		}
	};
}