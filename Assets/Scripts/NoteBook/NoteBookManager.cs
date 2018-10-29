using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#region Temp for testing SQL implementation
using Mono.Data.Sqlite;
using System.Data;
using System;
#endregion

public enum NoteBookState {
    MainCategory = 0,
    SubCategory = 1,
    SubCategoryInfo = 2,
}

public class NoteBookManager : MonoBehaviour {

    [SerializeField]
    private List<Button> _topButtons = new List<Button>();
    [SerializeField]
    private List<RectTransform> _subCategory = new List<RectTransform>();

    [SerializeField]//temp for debug
    private List<Button> _subCategoryButtons = new List<Button>();

    [SerializeField]//temp for debug
    private int _currentButtonSelectedTop = 0;
    [SerializeField]
    private int _currentButtonSelectedCategory = 0;

    [SerializeField]
    private TextMeshProUGUI _categoryText;
    [SerializeField]
    private TextMeshProUGUI _informationDisplay;



    public NoteBookState CurrentState = NoteBookState.MainCategory;
    [SerializeField]
    private int _currentMainSelected = 0;
    [SerializeField]
    private int _currentSubSelected = 0;
    [SerializeField]
    private int _currentSubInfoSelected = 0;

    void Update() {
        UpdateState();

    }

    private void UpdateState() {
        switch(CurrentState) {
            case NoteBookState.MainCategory:
                SelectCategory(ref _currentMainSelected, _topButtons, KeyCode.RightArrow, KeyCode.D, KeyCode.LeftArrow, KeyCode.A);
                break;
            case NoteBookState.SubCategory:
                SelectCategory(ref _currentSubSelected, _subCategoryButtons, KeyCode.DownArrow, KeyCode.S, KeyCode.UpArrow, KeyCode.W);
                break;
            case NoteBookState.SubCategoryInfo:
                break;
            default:
                break;
        }
        if(Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(1)) {
            ChangeNoteBookState();
        }
       
    }

    private void SelectCategory(ref int pCurrentSelected, List<Button> pButtons, KeyCode pRightOrDown, KeyCode pRightOrDownSecondary, KeyCode pLeftOrUp, KeyCode pLeftOrUpSecondary) {

        if(Input.GetKeyDown(pRightOrDown) || Input.GetKeyDown(pRightOrDownSecondary)) {
            pCurrentSelected++;
            if(pCurrentSelected >= pButtons.Count) {
                pCurrentSelected = 0;
            }
        }

        if(Input.GetKeyDown(pLeftOrUp) || Input.GetKeyDown(pLeftOrUpSecondary)) {
            pCurrentSelected--;
            if(pCurrentSelected <= -1) {
                pCurrentSelected = pButtons.Count - 1;
            }
        }

        pButtons[pCurrentSelected].Select();
    }

    private void ChangeNoteBookState(bool GoUpInState = false) {
        int stateint = (int)CurrentState;
        if(!GoUpInState) {
            stateint--;
            if(stateint < 0) {
                stateint = 0;
            }
        } else {
            stateint++;
            if(stateint > System.Enum.GetValues(typeof(NoteBookState)).Length) {
                stateint = System.Enum.GetValues(typeof(NoteBookState)).Length-1;
            }
        }
        CurrentState = (NoteBookState)stateint;
        //clean up old states
        switch(CurrentState) {
            case NoteBookState.MainCategory:
                for(int i = 0; i < _subCategory.Count; i++) {
                    _subCategory[i].gameObject.SetActive(false);
                }
                _categoryText.text = "Select Category";
                break;
            case NoteBookState.SubCategory:
                break;
            case NoteBookState.SubCategoryInfo:
                break;
            default:
                break;
        }

    }

    public void ShowCategory(int pCategory) {//NoteBookCategory pCategory
        ChangeNoteBookState(true);
        switch(CurrentState) {
            case NoteBookState.MainCategory:
                for(int i = 0; i < _subCategory.Count; i++) {
                    _subCategory[i].gameObject.SetActive(false);
                }
                break;
            case NoteBookState.SubCategory:
                _categoryText.text = _subCategory[pCategory].gameObject.name;

                _subCategoryButtons.Clear();
                _currentSubSelected = 0;
                _subCategory[pCategory].gameObject.SetActive(true);
                Button[] buttons = _subCategory[pCategory].GetComponentsInChildren<Button>();
                for(int i = 0; i < buttons.Length; i++) {
                    _subCategoryButtons.Add(buttons[i]);
                }
                break;
            case NoteBookState.SubCategoryInfo:
                break;
        }

    }

    public void ShowInformation(string pInformation) {
       
        string[] split = pInformation.Split(',');
        _categoryText.text = split[0];
        Data data = DataBase.Instance.LoadData(split[0], split[1]);
        string textToDisplay = "";

        for(int i = 0; i < data.DataConstruct.Length; i++) {
            if(data.DataConstruct[i].CustomDictionary.DataSate != DataState.UnDiscoverd) {
                textToDisplay += data.DataConstruct[i].CustomDictionary.Information[(int)data.DataConstruct[i].CustomDictionary.DataSate];
                textToDisplay += "\n";

            }
        }
        _informationDisplay.gameObject.SetActive(true);
        _informationDisplay.text = textToDisplay;
    }





    public void ShowInfoDataBase(string pInformation) {
        string[] split = pInformation.Split(',');
        if(split[0] == "Veeq") {

            string conn = "URI=file:" + Application.dataPath + "/TestDatabase.s3db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * " + "FROM NoteBook_Veeq";

            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            string textToDisplay = "";
            while(reader.Read()) {
                string keyword = reader.GetString(0);
                DataState state = (DataState)reader.GetInt32(1);
                switch(state) {
                    case DataState.UnDiscoverd:
                        break;
                    case DataState.Discoverd:
                        textToDisplay += reader.GetString(2);
                        break;
                    case DataState.InProgress:
                        textToDisplay += reader.GetString(3);
                        break;
                    case DataState.Completed:
                        textToDisplay += reader.GetString(4);
                        break;
                }
                textToDisplay += "\n";
            }
            _informationDisplay.gameObject.SetActive(true);
            _informationDisplay.text = textToDisplay;
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

        } else {
            ShowInformation(pInformation);
        }

    }



}
