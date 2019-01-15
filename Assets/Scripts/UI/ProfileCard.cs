using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileCard : MonoBehaviour
{
    public Text Name, Race, Job;


    public void SetValues(string pName, string pRace,string pJob) {
        Name.text = pName;
        Race.text = pRace;
        Job.text = pJob;
    }


    public void OnButtonClick() {
        SearchEngine searchEngine = GameObject.FindObjectOfType<SearchEngine>();
        searchEngine.CheckProfileCard(this);
    }


}
