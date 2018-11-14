using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class NPCManager : MonoSingleton<NPCManager>
{
	public InteractionPrompt npcInteractionPrompt;

	public delegate void OnInteractWithNPC(NPC pNPC);
	public OnInteractWithNPC onInteractWithNPC;

	[HideInInspector]
	public NPC currentInteractingNPC;
	public UnityAction onPlayerOutOfRange;

	private Player _player;
	private List<NPC> _NPCs = new List<NPC>();

	private string _interactionPromptText = "Press MB1 to interact"; //TODO: move to a better location
	private string _interactionStopText = "Press MB2 to stop interaction";

	private Coroutine _npcTrackerRoutine;
	private bool _isNPCInRange;

	private void Start() {
		_player = Player.Instance;
		_player.onInteractWithNPCRequest += OnInteractRequest;
	}

	private void Update() {
		for (int i = 0; i < _NPCs.Count; i++) {
			bool inRange = _NPCs[i].InInteractRange;
			if (inRange != _isNPCInRange) {
				_isNPCInRange = inRange;
				npcInteractionPrompt.EnableText(_isNPCInRange);
				npcInteractionPrompt.SetText(_isNPCInRange ? _interactionPromptText : _interactionStopText);
				break;
			}
		}
	}

	private IEnumerator NPCTrackRoutine(NPC pNPC) {
		yield return new WaitWhile(() => pNPC.InInteractRange);

		onPlayerOutOfRange.Invoke();
	}

	private void OnInteractRequest(bool pRequest) {
		if (pRequest && currentInteractingNPC == null)
			InteractStart();
		else
			InteractStop();
	}

	private void InteractStart() {
		NPC npc = GetClosestInteractableNPC();
		if (npc != null) {
			npcInteractionPrompt.SetText(_interactionStopText);
			currentInteractingNPC = npc;
			currentInteractingNPC.Interact();
			onInteractWithNPC.Invoke(currentInteractingNPC);
			_npcTrackerRoutine = StartCoroutine(NPCTrackRoutine(currentInteractingNPC));
		}
	}

	private void InteractStop() {
		StopCoroutine(_npcTrackerRoutine);
		npcInteractionPrompt.SetText(_interactionPromptText);
		currentInteractingNPC = null;
		onInteractWithNPC.Invoke(null);
	}

	public NPC GetClosestInteractableNPC() {
		NPC closestNPC = null;
		float closestDistance = -1f;

		for (int i = 0; i < _NPCs.Count; i++) {
			float dist = Vector3.Distance(_NPCs[i].transform.position, _player.transform.position);
			if (dist < _NPCs[i].playerInteractionRange) {

				if (dist < closestDistance || closestDistance == -1) {
					closestDistance = dist;
					closestNPC = _NPCs[i];
				}
			}
		}
		return closestNPC;
	}

	public void RegisterNPC(NPC pNPC) {
		if (_NPCs.Contains(pNPC)) {
			Debug.LogWarning("NPC has already been added to the registry!");
			return;
		}

		_NPCs.Add(pNPC);
	}
}
