using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SpotType {
    None = -1,
    Chair = 0,
    Food = 1,
    Trashcan = 2,
    Random = 3,
}


public class SpotManager : MonoBehaviour {

    public static SpotManager Instance;

    private Dictionary<SpotType, List<Spot>> _collection = new Dictionary<SpotType, List<Spot>>();

	// Use this for initialization
	void Awake () {
        if(Instance != null) {
            Destroy(this);
        }
        Instance = this;

        Spot[] Spots = GameObject.FindObjectsOfType<Spot>();
        for(int i = 0; i < Spots.Length; i++) {
            AddSpot(Spots[i]);
        }
	}

    public void AddSpot(Spot pSpot) {
        if(_collection.ContainsKey(pSpot.SpotType)) {
            _collection[pSpot.SpotType].Add(pSpot);
        } else {
            _collection.Add(pSpot.SpotType, new List<Spot>());
            _collection[pSpot.SpotType].Add(pSpot);
        }
    }


    public Spot GetSpot(SpotType pSpotType, int pNPCID) {

        for(int i = 0; i < _collection[pSpotType].Count; i++) {
            if(_collection[pSpotType][i].ClaimendBy == pNPCID) {
               return  _collection[pSpotType][i];
            }
        }
        for(int i = 0; i < _collection[pSpotType].Count; i++) {
            if(_collection[pSpotType][i].ClaimendBy ==-1) {
                return _collection[pSpotType][i];
            }
        }



        return null;
    }



}
