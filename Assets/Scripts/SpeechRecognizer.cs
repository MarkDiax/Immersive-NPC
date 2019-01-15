using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechRecognizer : Singleton<SpeechRecognizer>
{
	private DictationRecognizer _dictationRecognizer;

	private List<string> _akemazPhrases = new List<string> {
		"akemaz","akkemaz", "akkemas", "akemas", "acemaz", "acemas", "acamas", "akim's", "acronis",
		"akron", "achema", "acromas", "a conos", "a kamos", "a cummers", "a camaz", "aacomas", "arcanus",
		"i kemas", "our cameras", "hokiness", "aquilus", //TODO: repair multiple words like "a camos"
	};

	private List<string> _veeqPhrases = new List<string> {
		"veeq", "veek", "zeke", "peak", "reek", "fique", "vic", "v", "feet", "fee", "week", "weak"
	};

	private List<string> _rinnolPhrases = new List<string> {
		"rinnol", "rental"
	};

	public delegate void OnSpeechRecognized(string pText);
	public OnSpeechRecognized onSpeechRecognized;

	private int _dictationIndex;
	private bool _stopOnNextResult;

	public void StartListen() {
		AddListeners();
		Internal_StartListen();
	}

	private void Internal_StartListen() {
		_dictationRecognizer.Start();
		Debug.Log("SpeechRecognizer has started listening.");
	}

	public void StopListen() {
		_stopOnNextResult = true;

	}

	public override void Init() {
		_dictationRecognizer = new DictationRecognizer(ConfidenceLevel.Rejected, DictationTopicConstraint.Dictation);
	}

	private void AddListeners() {
		_dictationRecognizer.DictationResult += OnDictationResult;
		_dictationRecognizer.DictationComplete += OnDictationComplete;
		_dictationRecognizer.DictationHypothesis += OnDictationHypothesis;
		_dictationRecognizer.DictationError += OnDictationError;
	}
	private void RemoveListeners() {
		_dictationRecognizer.DictationResult -= OnDictationResult;
		_dictationRecognizer.DictationComplete -= OnDictationComplete;
		_dictationRecognizer.DictationHypothesis -= OnDictationHypothesis;
		_dictationRecognizer.DictationError -= OnDictationError;
	}

	private void OnDictationResult(string pText, ConfidenceLevel pConfidence) {
		Debug.LogFormat("Dictation #{0}" + ": {1}", _dictationIndex, pText + " with confidence level " + pConfidence.ToString());
		bool keywordsFound = CheckForKeywords(ref pText); //optimization

		if ((pConfidence == ConfidenceLevel.Rejected || pConfidence == ConfidenceLevel.Low) && !keywordsFound) {
			Debug.LogFormat("Not sending text with no keywords found in text '{0}' with confidence level '{1}'!", pText, pConfidence);
		}
		else {
			Debug.LogFormat("Dictation #{0}" + " After keyword check: {1}", _dictationIndex, pText);

			//hacky
			if (pText.Contains("mall"))
				pText = pText.Replace("mall", "mole");

			//replace "on" with "uun"

			if (_stopOnNextResult)
			{
				_stopOnNextResult = false;
				RemoveListeners();
				_dictationRecognizer.Stop();
				Debug.Log("SpeechRecognizer has stoppped listening.");
			}

			onSpeechRecognized.Invoke(pText);
			_dictationIndex++;
		}
	}

	private void OnDictationComplete(DictationCompletionCause pCompletionCause) {
		Debug.Log("Dictation completed by " + pCompletionCause);

		if (_dictationRecognizer.Status != SpeechSystemStatus.Running) {

			Debug.LogWarning("Restarting Speech Recognizer due to error.");
			Internal_StartListen();
		}
	}

	private void OnDictationHypothesis(string pText) {
		Debug.Log("Dictation n" + _dictationIndex + " : " + pText);
	}

	private void OnDictationError(string pError, int pResult) {
		Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", pError, pResult);
	}

	private bool CheckForKeywords(ref string pText) {
		pText = pText.ToLower();
		string textCopy = pText;
		string[] words = pText.Split(' ');

		/*
		//todo:
		//check entire sentence
		//then if there's "a kamaz" in whole sentence
		//split "a kamaz" in 2 strings
		//check if "a" is seen in a word
		//if so, remove "a" from word. if word is now empty or null, its correct

		for (int i = 0; i < _akemazPhrases.Count; i++) {
			if (pText.Contains(_akemazPhrases[i])) {
				string[] phrases = _akemazPhrases[i].Split(' ');
				//"a"
				//"kamaz"

				for (int j = 0; j < words.Length; j++) {

					int correctPhraseCount = 0;
					for (int x = 0; x < phrases.Length; x++) {
						string wordCopy = words[j];
						//word = "apple"

						words[j].Replace(phrases[x], "");
						//word = "pple"
						//WATCH OUT: if second word doesn't fit, we damaged the sentence

						if (string.IsNullOrEmpty(words[j]) || string.IsNullOrWhiteSpace(words[j])) {
							correctPhraseCount++;

							if (correctPhraseCount == phrases.Length) {
								//insert "Akemaz"

							}
						}
						else {
							//word is not null, means it doesn't fit
							words[j] = wordCopy;
							break;
						}
					}
				}


			}
		}

		*/

		for (int i = 0; i < words.Length; i++) {
			for (int j = 0; j < _akemazPhrases.Count; j++) {
				if (words[i] == _akemazPhrases[j]) {
					words[i] = "Akemaz";
					Debug.LogFormat("Replacing '{0}' with '{1}'", _akemazPhrases[j], words[i]);
					break;
				}

			}
			if (words[i] == "Akemaz")
				continue;

			for (int j = 0; j < _veeqPhrases.Count; j++) {
				if (words[i] == _veeqPhrases[j]) {
					words[i] = "Veeq";
					Debug.LogFormat("Replacing '{0}' with '{1}'", _veeqPhrases[j], words[i]);
					break;
				}
			}
			if (words[i] == "Veeq")
				continue;

			for (int j = 0; j < _rinnolPhrases.Count; j++) {
				if (words[i] == _rinnolPhrases[j]) {
					words[i] = "Rinnol";
					Debug.LogFormat("Replacing '{0}' with '{1}'", _rinnolPhrases[j], words[i]);
					break;
				}
			}
		}

		pText = "";

		for (int i = 0; i < words.Length; i++) {
			pText += words[i];
			if (i < words.Length - 1) //add no whitespace at the end of the sentence
				pText += ' ';
		}

		//A check for whether the text has changed in this method.
		return textCopy != pText;
	}
	

	public void ToggleSpeechRecognizer() {
		if (IsListening)
			StopListen();
		else
			StartListen();
	}

	/// <summary>
	/// Returns true if the SpeechRecognizer is listening to audio input.
	/// </summary>
	public bool IsListening => _dictationRecognizer.Status == SpeechSystemStatus.Running;
}