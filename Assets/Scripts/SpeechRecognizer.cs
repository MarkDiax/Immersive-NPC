using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class SpeechRecognizer : Singleton<SpeechRecognizer>
{
	private string textRecognition;

	private DictationRecognizer _dictationRecognizer;

	public delegate void OnSpeechRecognized(string pText);
	public OnSpeechRecognized onSpeechRecognized;

	public void StartListen() {
		_dictationRecognizer.Start();
	}

	public void StopListen() {
		_dictationRecognizer.Stop();
		textRecognition = "";
	}

	public override void Init() {
		_dictationRecognizer = new DictationRecognizer();
		_dictationRecognizer.InitialSilenceTimeoutSeconds = 0.8f;

		_dictationRecognizer.DictationResult += (text, confidence) => {
			Debug.LogFormat("Dictation result: {0}", text);
			textRecognition += text + "\n";
			onSpeechRecognized.Invoke(text);
		};

		_dictationRecognizer.DictationComplete += (completionCause) => {
			if (completionCause != DictationCompletionCause.Complete)
				Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
		};

		_dictationRecognizer.DictationError += (error, hresult) => {
			Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
		};
	}
}