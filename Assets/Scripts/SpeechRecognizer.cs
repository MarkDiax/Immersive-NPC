using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechRecognizer : Singleton<SpeechRecognizer>
{
	private DictationRecognizer _dictationRecognizer;
	private KeywordRecognizer _keywordRecognizer;

	private List<string> _keywords = new List<string> {
		"veeq", "discoball", "veek", "reek",
	};

	private List<string> _akemazPhrases = new List<string> {
		"akemaz","akkemaz", "akkemas", "akemas", "acemaz", "acemas", "acamas"
	};

	public delegate void OnSpeechRecognized(string pText);
	public OnSpeechRecognized onSpeechRecognized;

	public void StartListen(float pSpeechTimeoutSeconds) {
		_dictationRecognizer.InitialSilenceTimeoutSeconds = pSpeechTimeoutSeconds;

		StartListen();
	}

	public void StartListen() {
		//_dictationRecognizer.Start();
		_keywordRecognizer.Start();
	}

	public void StopListen() {
		/**
		_dictationRecognizer.Stop();
		_dictationRecognizer.Dispose();

		/**/
		_keywordRecognizer.Stop();
		_keywordRecognizer.Dispose();
		/**/
	}

	public override void Init() {
		/**/
		for (int i = 0; i < _akemazPhrases.Count; i++) {
			if (!_keywords.Contains(_akemazPhrases[i]))
				_keywords.Add(_akemazPhrases[i]);
		}

		_keywordRecognizer = new KeywordRecognizer(_keywords.ToArray(), ConfidenceLevel.Rejected);
		_keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
		

		/**/
		int dictationIndex = 0;
		_dictationRecognizer = new DictationRecognizer(ConfidenceLevel.Rejected, DictationTopicConstraint.Dictation);

		_dictationRecognizer.DictationResult += (text, confidence) => {
			Debug.LogFormat("Dictation #{0}" + ": {1}", dictationIndex, text + " with confidence level " + confidence.ToString());
			onSpeechRecognized.Invoke(text);
			dictationIndex++;
		};

		_dictationRecognizer.DictationComplete += (pCompletionCause) => {
			if (pCompletionCause != DictationCompletionCause.Complete) {
				Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", pCompletionCause);
				Debug.LogWarning("Restarting Speech Recognizer due to error.");
				StartListen();
			}
			Debug.Log("Dictation completed by " + pCompletionCause);
		};

		_dictationRecognizer.DictationHypothesis += (pText) => {
			Debug.Log("Dictation n" + dictationIndex + " : " + pText);
		};

		_dictationRecognizer.DictationError += (error, hresult) => {
			Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
		};

		/**/
	}
	
	private void OnPhraseRecognized(PhraseRecognizedEventArgs args) {
		StringBuilder builder = new StringBuilder();
		builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
		builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
		builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
		Debug.Log(builder.ToString());
	}
}