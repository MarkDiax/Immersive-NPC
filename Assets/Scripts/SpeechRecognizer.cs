using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechRecognizer : Singleton<SpeechRecognizer>
{
	private DictationRecognizer _dictationRecognizer;
	private KeywordRecognizer _keywordRecognizer;

	public List<string> npcKeywords = new List<string>();

	private List<string> _akemazPhrases = new List<string> {
		"akemaz","akkemaz", "akkemas", "akemas", "acemaz", "acemas", "acamas", "akim's",
		"akron", "achema", "acromas", "a conos", "a kamos", "a cummers", "a camaz", "aacomas", "arcanus", "i kemas"
	};

	private List<string> _veeqPhrases = new List<string> {
		"veeq", "veek", "zeke", "peak", "reek", "fique", "vic", "v"
	};

	private List<string> _rinnolPhrases = new List<string> {
		"rinnol", "rental"
	};

	public delegate void OnSpeechRecognized(string pText);
	public OnSpeechRecognized onSpeechRecognized;

	public void StartListen(float pSpeechTimeoutSeconds) {
		_dictationRecognizer.InitialSilenceTimeoutSeconds = pSpeechTimeoutSeconds;

		StartListen();
	}

	public void StartListen() {
		_dictationRecognizer.Start();
		//_keywordRecognizer.Start();
	}

	public void StopListen() {
		/**/
		_dictationRecognizer.Stop();
		_dictationRecognizer.Dispose();

		/**
		_keywordRecognizer.Stop();
		_keywordRecognizer.Dispose();
		/**/
	}

	private void ConstructNPCKeywords() {
		for (int i = 0; i < _akemazPhrases.Count; i++)
			npcKeywords.Add(_akemazPhrases[i]);

		for (int i = 0; i < _veeqPhrases.Count; i++)
			npcKeywords.Add(_veeqPhrases[i]);

		for (int i = 0; i < _rinnolPhrases.Count; i++)
			npcKeywords.Add(_rinnolPhrases[i]);
	}

	public override void Init() {
		/**
		 * 
		for (int i = 0; i < _akemazPhrases.Count; i++) {
			if (!_keywords.Contains(_akemazPhrases[i]))
				_keywords.Add(_akemazPhrases[i]);
		}

		_keywordRecognizer = new KeywordRecognizer(_keywords.ToArray(), ConfidenceLevel.Rejected);
		_keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
		
		/**/

		ConstructNPCKeywords();

		int dictationIndex = 0;
		_dictationRecognizer = new DictationRecognizer(ConfidenceLevel.Rejected, DictationTopicConstraint.Dictation);

		_dictationRecognizer.DictationResult += (text, confidence) => {
			if (confidence != ConfidenceLevel.High && !CheckForKeywords(ref text)) {
				Debug.LogFormat("Not sending text with no keywords found in text '{0}' with confidence level '{1}'!", text, confidence);
			}
			else {
				Debug.LogFormat("Dictation #{0}" + ": {1}", dictationIndex, text + " with confidence level " + confidence.ToString());

				CheckForKeywords(ref text);
				Debug.LogFormat("Dictation #{0}" + " After keyword check: {1}", dictationIndex, text);

				onSpeechRecognized.Invoke(text);
				dictationIndex++;
			}
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

	private bool CheckForKeywords(ref string pText) {
		pText = pText.ToLower();
		string textCopy = pText;

		//TODO
		//split text into seperate words. then check each word if it exist in the akemaz list.
		//if it does, remove that part from the wordd. 
		//check if the word string is now empty. if so, the word can be safely changed in the sentence.

		for (int i = 0; i < _akemazPhrases.Count; i++) {
			if (pText.Contains(_akemazPhrases[i])) {
				pText = pText.Replace(_akemazPhrases[i], "Akemaz");
				Debug.LogFormat("Replacing '{0}' with '{1}'", _akemazPhrases[i], "Akemaz");
				break;
			}
		}

		for (int i = 0; i < _veeqPhrases.Count; i++) {
			if (pText.Contains(_veeqPhrases[i])) {
				pText = pText.Replace(_veeqPhrases[i], "Veeq");
				Debug.LogFormat("Replacing '{0}' with '{1}'", _veeqPhrases[i], "Veeq");
				break;
			}
		}

		for (int i = 0; i < _rinnolPhrases.Count; i++) {
			if (pText.Contains(_rinnolPhrases[i])) {
				pText = pText.Replace(_rinnolPhrases[i], "Rinnol");
				Debug.LogFormat("Replacing '{0}' with '{1}'", _rinnolPhrases[i], "Rinnol");
				break;
			}
		}

		//A check for whether the text has changed in this method.
		return textCopy != pText;
	}

	private void OnPhraseRecognized(PhraseRecognizedEventArgs args) {
		StringBuilder builder = new StringBuilder();
		builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
		builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
		builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
		Debug.Log(builder.ToString());
	}
}