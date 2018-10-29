using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public enum DataState {
    UnDiscoverd = -1,
    Discoverd = 0,
    InProgress = 1,
    Completed = 2,
}

[Serializable]
public class CustomDictionary {
    public DataState DataSate = DataState.UnDiscoverd;
    public string[] Information =  new string[]{"stuff1", "stuff2", "stuff3" };
}

[Serializable]
public class CustomData {
    public string DataKey = "Null";
    public CustomDictionary CustomDictionary = new CustomDictionary(); 
}

[Serializable]
public class Data {
    public CustomData[] DataConstruct = new CustomData[2];
}


public class DataBase : MonoBehaviour {

    public static DataBase Instance;

    public void Start() {
        Instance = this;

        //Data data = new Data();
        //CustomData CustomData = new CustomData();
        //CustomData.DataKey = "test1";
        //data.DataConstruct[0] = CustomData;
        //data.DataConstruct[1] = CustomData;

        //CustomDictionary Customdic = new CustomDictionary();
        //Customdic.DataSate = DataState.UnDiscoverd;
        //Customdic.Information = new string[]{"overstuff1", "overstuff2", "overstuff3" };
        //data.DataConstruct[0].CustomDictionary = Customdic;
        //data.DataConstruct[1].CustomDictionary = Customdic;

        //string objToString = JsonUtility.ToJson(data);
        //SaveData("Null", objToString);

        //Data dataLoaded = LoadData("Null");

        //foreach(var item in dataLoaded.DataConstruct) {
        //    Debug.Log("DataKey : " +  item.DataKey + " with State : " + item.CustomDictionary.DataSate.ToString() + " to show the text : " + item.CustomDictionary.Information[(int)item.CustomDictionary.DataSate]);
        //}

    }

    public void SaveData(string pSubject,string pInformationToSave) {
        File.WriteAllText(Application.dataPath + "/StreamingAssets/"+pSubject + ".txt" , pInformationToSave);
    }

    public Data LoadData(string pSubject, string pFolder) {
        string text = File.ReadAllText(Application.dataPath + "/StreamingAssets/" +pFolder+"/"  + pSubject + ".txt");

        Data data = JsonUtility.FromJson<Data>(text);
        return data;
    }

}
