using UnityEngine;
using System.Collections;

namespace RogoDigital.Lipsync
{
	public class LipSyncRuntimeManager : MonoBehaviour
	{
		[Header("RTVoice/Audio Settings")]
		public AutoSyncRTLanguageModel languageModel;
		public string languageModelPath = "StreamingAssets/Language Models/EN-US/";
		public string rtVoiceModel = "Microsoft Zira Desktop";

		public string StreamingAssetsAudioPath {
			get{ return Application.streamingAssetsPath + "/Audio/"; }
		}

		[Header("LipSync Settings")]
		public LipSyncData lipSyncData;
		public LipSyncProject projectSettings;

		private static LipSyncRuntimeManager _instance;
		private static bool _initialized;

		private void Awake() {
			if (!_initialized)
				Init();
		}

		public void Init() {
			_initialized = true;
			_instance = this;
		}

		public static LipSyncRuntimeManager Instance {
			get {
				if (_instance == null) {
					GameObject obj = new GameObject("LipSyncRuntimeManager");
					_instance = obj.AddComponent<LipSyncRuntimeManager>();

					if (!_initialized)
						_instance.Init();
				}
				return _instance;
			}
		}
	}
}