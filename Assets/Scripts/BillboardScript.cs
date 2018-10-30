using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
	public bool invertBillboard;

	void Update() {
		Vector3 lookDir = Camera.main.transform.position - transform.position;
		Quaternion lookRotation = Quaternion.LookRotation(invertBillboard ? -lookDir : lookDir);

		transform.rotation = lookRotation;
	}
}