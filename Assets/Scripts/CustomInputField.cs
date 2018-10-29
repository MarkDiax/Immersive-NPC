using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomInputField : InputField
{

	protected override void Start() {

		onEndEdit.AddListener(Enter);
	}

	void Enter(string str) {
		text = "";
		ActivateInputField();
	}
}
