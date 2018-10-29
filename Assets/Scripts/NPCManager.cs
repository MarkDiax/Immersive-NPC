using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : Singleton<NPCManager>
{
	private List<NPC> _NPCs;
	private Player _player;

	/// <summary>
	/// Functions as Awake() for this Singleton class.
	/// </summary>
	public override void Init() {
		_NPCs = new List<NPC>();

		_player = Player.Instance;
		_player.OnInteractRequest.AddListener(OnInteractRequest);
	}

	private void OnInteractRequest() {
		NPC npc = GetClosestInteractableNPC();
		if (npc != null) {
			npc.InteractWithPlayer();
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