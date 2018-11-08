using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace RogoDigital.Lipsync
{
	[CreateAssetMenu(fileName = "New RT Language Model", menuName = "LipSync Pro/AutoSync RT Language Model")]
	public class AutoSyncRTLanguageModel : ScriptableObject
	{
#pragma warning disable 618

		[SerializeField]
		public string language;
		[SerializeField]
		public string recommendedPhonemeSet;

		[SerializeField]
		public string hmmDir;
		[SerializeField]
		public string dictFile;
		[SerializeField]
		public string allphoneFile;
		[SerializeField]
		public string lmFile;
		[SerializeField]
		public PhonemeMapping[] phonemeMapper = new PhonemeMapping[0];

		public string GetBasePath() {
			string path = Application.dataPath + '/' + LipSyncRuntimeManager.Instance.languageModelPath;
			return path;
		}

		public static AutoSyncRTLanguageModel Load(string languageName) {
			AutoSyncRTLanguageModel model = LipSyncRuntimeManager.Instance.languageModel;
			if (model.language == languageName) {
				return model;
			}

			Debug.LogWarning("AutoSyncRTLanguageModel::Language not supported!");
			return null;
		}

		[System.Serializable]
		public struct PhonemeMapping
		{
			public string label;
			[System.Obsolete("Use AutoSyncLanguageModel.PhonemeMapping.phonemeName instead.")]
			public Phoneme phoneme;
			public string phonemeName;

			public PhonemeMapping(string label, string phonemeName) {
				this.label = label;
				this.phonemeName = phonemeName;
				this.phoneme = Phoneme.AI;
			}

			[System.Obsolete("Use string constructor instead.")]
			public PhonemeMapping(string label, Phoneme phoneme) {
				this.label = label;
				this.phoneme = phoneme;
				this.phonemeName = phoneme.ToString();
			}
		}
	}
}