using UnityEngine;
using System.Collections;

public class MonoUpdatePassthrough : MonoBehaviour
{
	public delegate void OnUpdate();
	public OnUpdate onUpdate;

	private void Update() {
		if (onUpdate != null)
			onUpdate.Invoke();
	}
}