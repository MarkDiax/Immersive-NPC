using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class RegistationFile : MonoBehaviour
{

    public Image DisplayImage;
    public Text DisplayText, PageNumbers;
    public List<Button> Buttons = new List<Button>();

    public Button NextPageButton, BackPageButton;

    private string fileLoctaion;
    private Dictionary<KeyValuePair<string,int>, Dictionary<int, Sprite>> DictinoaryInfo = new Dictionary<KeyValuePair<string, int>, Dictionary<int, Sprite>>();

    private string _currentSubjectSeleceted = "";

	public static RegistationFile Instance = null;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}


	// Start is called before the first frame update
	void Start()
    {
        fileLoctaion = Application.dataPath;
        for(int i = 0; i < Buttons.Count; i++) {
            CreateDictionaryData(Buttons[i].GetComponentInChildren<Text>().text);
        }


        _currentSubjectSeleceted = Buttons[0].GetComponentInChildren<Text>().text;
        UpdateDisplay();
    }


    private void CreateDictionaryData(string pSubject) {
        int index = 0;
      
        Object[] files = Resources.LoadAll("FileRegistartion/" + pSubject);
        Dictionary<int, Sprite> tempDictionary = new Dictionary<int, Sprite>();
      
        for(int i = 0; i < files.Length; i++) {
            if(files[i].GetType() ==  typeof(Sprite)) {
                tempDictionary.Add(index, (Sprite)files[i]);
                index++;
            }
        }
        DictinoaryInfo.Add(new KeyValuePair<string, int>(pSubject,0), tempDictionary);

    }


    private void UpdateDisplay() {

        DisplayText.text = _currentSubjectSeleceted;
        for(int i = 0; i < DictinoaryInfo.Count; i++) {
            if(DictinoaryInfo.ElementAt(i).Key.Key == _currentSubjectSeleceted) {
                DisplayImage.sprite = DictinoaryInfo.ElementAt(i).Value.ElementAt(DictinoaryInfo.ElementAt(i).Key.Value).Value;

                PageNumbers.text = DictinoaryInfo.ElementAt(i).Key.Value+1 + "/" + DictinoaryInfo.ElementAt(i).Value.Values.Count();
            }
        }
    }


    public void UpdatePage(int pUpOrDown) {
        KeyValuePair<string, int> tempkeyValuePair;
        Dictionary<int, Sprite> tempDic = new Dictionary<int, Sprite>();
        for(int i = DictinoaryInfo.Count; i-- > 0;) {
            if(DictinoaryInfo.ElementAt(i).Key.Key == _currentSubjectSeleceted) {
                int tempindex = DictinoaryInfo.ElementAt(i).Key.Value;
                if((tempindex <= 0  && pUpOrDown == -1) || (tempindex >= DictinoaryInfo.ElementAt(i).Value.Values.Count   && pUpOrDown ==1 )) {
                    return;
                }
                else {
                    tempkeyValuePair = new KeyValuePair<string, int>(_currentSubjectSeleceted, tempindex += pUpOrDown);
                    tempDic = DictinoaryInfo.ElementAt(i).Value;
                    if((tempindex + pUpOrDown) <= -1) {
                        BackPageButton.interactable = false;
                    }
                    else {
                        BackPageButton.interactable = true;

                    }
                    if((tempindex + pUpOrDown) >= DictinoaryInfo.ElementAt(i).Value.Values.Count) {
                        NextPageButton.interactable = false;
                    }
                    else {
                        NextPageButton.interactable = true;
                    }
                    DictinoaryInfo.Remove(DictinoaryInfo.ElementAt(i).Key);
                }
            }
        }
        DictinoaryInfo.Add(tempkeyValuePair, tempDic);
        UpdateDisplay();
    }

    public void ChangeSubjectTo(string pSubject) {
        _currentSubjectSeleceted = pSubject;
        UpdatePage(0);
    }
    
}
