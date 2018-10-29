using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class InteractionPrompt : MonoBehaviour
{
	Text _text;

	private void Start() {
		_text = GetComponent<Text>();
	}

	public void SetText(string pText) {
		_text.text = pText;
	}

	public void EnableText(bool pEnable) {
		_text.enabled = pEnable;
	}
}
