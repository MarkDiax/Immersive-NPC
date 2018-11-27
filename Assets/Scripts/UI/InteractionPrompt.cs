using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class InteractionPrompt : MonoBehaviour
{
	Text _text;

	public void SetText(string pText) {
		if (_text == null)
			_text = GetComponent<Text>();

		_text.text = pText;
	}
}
