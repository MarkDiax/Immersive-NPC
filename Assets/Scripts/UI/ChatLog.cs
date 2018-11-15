using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ChatLog : MonoBehaviour
{
	[Header("UI")]
	public GameObject chatMessage;

	public float scrollSpeed = 2f;
	public int maxMessageCount = 16;

	private Transform _contentArea;
	private ScrollRect _scrollRect;

	private void Start() {
		_scrollRect = GetComponent<ScrollRect>();
		_contentArea = _scrollRect.viewport.GetChild(0).transform;

		ClearScrollArea();
	}

	private void Update() {
		if (_contentArea.childCount > maxMessageCount)
			Destroy(_contentArea.GetChild(0).gameObject);

		UpdateScrollView();
	}

	private void ClearScrollArea() {
		for (int i = 0; i < _contentArea.childCount; i++)
			Destroy(_contentArea.GetChild(i).gameObject);
	}

	private void UpdateScrollView() {
		if (_scrollRect.verticalNormalizedPosition > 0)
			_scrollRect.verticalNormalizedPosition -= Time.deltaTime * scrollSpeed;
	}

	public void AddChatMessage(string pMessage) {
		GameObject message = GameObject.Instantiate(chatMessage, _contentArea.transform);
		message.GetComponent<Text>().text = pMessage;
	}
}