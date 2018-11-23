using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class SpeechRecognizer : Singleton<SpeechRecognizer>
{
	private DictationRecognizer _dictationRecognizer;

	public delegate void OnSpeechRecognized(string pText);
	public OnSpeechRecognized onSpeechRecognized;

	private float _speechTimeOut = 0.5f;

	public void StartListen(float pSpeechTimeoutSeconds) {
		_speechTimeOut = pSpeechTimeoutSeconds;
		_dictationRecognizer.InitialSilenceTimeoutSeconds = pSpeechTimeoutSeconds;
		StartListen();
	}

	public void StartListen() {
		_dictationRecognizer.Start();
	}

	public void StopListen() {
		_dictationRecognizer.Stop();
	}

	public override void Init() {
		_dictationRecognizer = new DictationRecognizer();
		_dictationRecognizer.AutoSilenceTimeoutSeconds = 300f;

		_dictationRecognizer.DictationResult += (text, confidence) => {
			Debug.LogFormat("Dictation result: {0}", text);
			onSpeechRecognized.Invoke(text);
		};

		_dictationRecognizer.DictationComplete += (completionCause) => {
			if (completionCause != DictationCompletionCause.Complete) {
				Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
				StartListen(_speechTimeOut);
			}
		};

		_dictationRecognizer.DictationError += (error, hresult) => {
			Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
		};
	}
}