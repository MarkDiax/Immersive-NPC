using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;




public class Profile : ScriptableObject {
    public string Name, Race, Job;

}
[System.Serializable]
public class JsonClass
{
    public List<string> Items = new List<string>();
}


public class SearchEngine : MonoBehaviour{


    private List<string> _names = new List<string>();
    private List<string> _races = new List<string>();
    private List<string> _jobs = new List<string>();


    [SerializeField]
    private Dropdown _dropDownNames, _dropdownRaces,_dropDownJobs;

    private List<Profile> _profiles = new List<Profile>();
    [SerializeField]//temp
    private List<Profile> _profilesToDisplay = new List<Profile>();

    [SerializeField]
    private GameObject _profileCardPrefab;
    [SerializeField]
    private Transform _profileCardParent;


    private Profile _criminalProfile;

    public string CriminalName, CriminalRace, CriminalJob;
    public int AmountOfNames = 100, AmountOfRaces = 4, AmountOfJobs = 10, AmountOfExtraProfliles = 20;
    [SerializeField]//temp
    public int CaptureTryes = 0;
    public List<string> DisplayTexts = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        _names.Add(CriminalName);
        _races.Add(CriminalRace);
        _jobs.Add(CriminalJob);

        #region loading Data from files
        string jsondata = File.ReadAllText(Application.dataPath + "/StreamingAssets/Names.txt");

        List<string> items = new List<string>();
        items = JsonUtility.FromJson<JsonClass>(jsondata).Items;
        for(int i = 0; i < AmountOfNames; i++) {
            if(i <= items.Count) {
                _names.Add(items[i]);
            }
           
        }
        items.Clear();

        jsondata = File.ReadAllText(Application.dataPath + "/StreamingAssets/Races.txt");
        items = JsonUtility.FromJson<JsonClass>(jsondata).Items;
        for(int i = 0; i < AmountOfRaces; i++) {
            if(i <= items.Count) {
                _races.Add(items[i]);
            }
        }
        items.Clear();

        jsondata = File.ReadAllText(Application.dataPath + "/StreamingAssets/Jobs.txt");
        items = JsonUtility.FromJson<JsonClass>(jsondata).Items;
        for(int i = 0; i < AmountOfJobs; i++) {
            if(i <= items.Count) {
                _jobs.Add(items[i]);
            }
        }
        items.Clear();
        #endregion

        CreateRandomNames(AmountOfExtraProfliles);
        _names = (List<string>)Shuffle(_names);
        _races = (List<string>)Shuffle(_races);
        _jobs = (List<string>)Shuffle(_jobs);

        _dropDownNames.AddOptions(_names);
        _dropdownRaces.AddOptions(_races);
        _dropDownJobs.AddOptions(_jobs);
    }
    
    
    private void CreateRandomNames(int pAmount) {

        for(int i = 0; i < pAmount; i++) {
            Profile profile = new Profile();
            profile.Name = _names[UnityEngine.Random.Range(1, _names.Count)];
            profile.Race = _races[UnityEngine.Random.Range(1, _races.Count)];
            profile.Job = _jobs[UnityEngine.Random.Range(1, _jobs.Count)];
            _profiles.Add(profile);
        }

        CreateCriminalProfiles(CriminalName, CriminalRace, CriminalJob);
        _profiles = (List<Profile>)Shuffle(_profiles);
        UpdateScrollView();

    }


    private void CreateCriminalProfiles(string pCriminalName, string pCirminalRace,string pCriminalJob) {
        _criminalProfile = new Profile();
        _criminalProfile.Name = pCriminalName;
        _criminalProfile.Race = pCirminalRace;
        _criminalProfile.Job = pCriminalJob;
        _profiles.Add(_criminalProfile);

        Profile fakecriminal = new Profile();
        fakecriminal.Name = _names[UnityEngine.Random.Range(1, _names.Count)];
        fakecriminal.Race = pCirminalRace;
        fakecriminal.Job = pCriminalJob;
        _profiles.Add(fakecriminal);

        Profile fakecriminal1 = new Profile();
        fakecriminal1.Name = pCriminalName;
        fakecriminal1.Race = _races[UnityEngine.Random.Range(1, _races.Count)];
        fakecriminal1.Job = pCriminalJob;
        _profiles.Add(fakecriminal1);

        Profile fakecriminal2 = new Profile();
        fakecriminal2.Name = pCriminalName;
        fakecriminal2.Race = pCirminalRace;
        fakecriminal2.Job = _jobs[UnityEngine.Random.Range(1, _jobs.Count)];
        _profiles.Add(fakecriminal2);

    }


    public IList<T> Shuffle<T>(IList<T> pList) {
        System.Random rng = new System.Random(Random.Range(0,1000));
        int n = pList.Count;
        while(n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = pList[k];
            pList[k] = pList[n];
            pList[n] = value;
        }
        return pList;
    }

    private void TempAddingStuffToList(List<string> pList,string pListname,int pAmount) {
        for(int i = 0; i < pAmount; i++) {
            pList.Add(pListname+ i);
        }
    }

    public void UpdateScrollView() {
        _profilesToDisplay.Clear();
        for(int i = _profileCardParent.childCount; i-- > 0;) {
            Destroy(_profileCardParent.GetChild(i).gameObject);
        }

        for(int i = 0; i < _profiles.Count; i++) {
            if((_dropDownNames.value ==0 || _dropDownNames.options[_dropDownNames.value].text == _profiles[i].Name) &
                (_dropdownRaces.value == 0 || _dropdownRaces.options[_dropdownRaces.value].text == _profiles[i].Race) &
                (_dropDownJobs.value == 0 || _dropDownJobs.options[_dropDownJobs.value].text == _profiles[i].Job)) {
                _profilesToDisplay.Add(_profiles[i]);
            }
        }

        for(int i = 0; i < _profilesToDisplay.Count; i++) {
            GameObject profilecard = GameObject.Instantiate(_profileCardPrefab);
           
            profilecard.transform.parent = _profileCardParent;
            profilecard.transform.localPosition = Vector3.zero;
            //profilecard.transform.Rotate(Vector3.up, -90);
            profilecard.transform.rotation = transform.rotation;
            profilecard.transform.localScale = Vector3.one;
            profilecard.GetComponent<ProfileCard>().SetValues(_profilesToDisplay[i].Name, _profilesToDisplay[i].Race, _profilesToDisplay[i].Job);
        }

    }

    public void CheckProfileCard(ProfileCard pCard) {
       
        if(pCard.Name.text == _criminalProfile.Name && pCard.Race.text == _criminalProfile.Race && pCard.Job.text == _criminalProfile.Job) {
            SceneHandler.Instance.ShowEndScreen(DisplayTexts[3], -1);
        }
        else {
            SceneHandler.Instance.ShowEndScreen(DisplayTexts[CaptureTryes], CaptureTryes+1);
        }
        CaptureTryes++;
    }


}
