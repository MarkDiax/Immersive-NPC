using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class NPCManager : MonoSingleton<NPCManager>
{
	private List<NPC> _NPCs = new List<NPC>();
	private Player _player;

	private Coroutine NPCTracker;

	private void Start() {
		_player = Player.Instance;
		_player.onInteractRequest.AddListener(OnInteractRequest);
	}

	private IEnumerator NPCTrackRoutine(NPC pNPC) {
		yield return new WaitWhile(() => pNPC.InInteractRange);

		_player.onInteractStop.Invoke();
	}

	private void OnInteractRequest() {
		NPC npc = GetClosestInteractableNPC();
		if (npc != null) {
			npc.Interact();
			NPCTracker = StartCoroutine(NPCTrackRoutine(npc));
		}
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