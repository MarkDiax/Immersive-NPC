using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

using System.IO;

#region Database commands and tables
public sealed class CommandType {
    public const string Select = "SELECT";
    public const string Update = "UPDATE";
}

public sealed class Tables {
    public const string Keywords = "Keywords";
    public const string Milestone1 = "Milestone_1";
    public const string Subjects = "Subjects";
}

public sealed class TableKeywords {
    public const string KeywordID = "Keyword_ID";
    public const string Value = "Value";
    public const string Handled = "Handled";
}
#endregion

//_sqlQuery = "UPDATE Subjects SET Keyword_ID='AND.3,4,5' WHERE Subject_ID = '2'";


public class DatabaseFramework : MonoBehaviour {
  
    
    private string _playerDatabase = "URI=file:";

    private IDbConnection _databaseConnection = null;
    private IDbCommand _databaseCommand = null;

    private string _sqlQuery = "";
    [HideInInspector]
    public static DatabaseFramework Instance = null;

 

    void Start(){
        #region Instance And Dont Destroy On Load
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        #endregion

        _playerDatabase += Application.dataPath + "/playerDatabase.s3db";

        if(GameManger.Instance.ConfigFile.OverrideDatabase) {
            if(File.Exists(Application.dataPath + "/PlayerDatabase.s3db")) {
                Debug.Log("Delete file plz");
                File.Delete(Application.dataPath + "/PlayerDatabase.s3db");
            } else {
                Debug.Log("No file to delete");
            }
            File.Copy(Application.dataPath + "/Database/DatabaseDefault.s3db", Application.dataPath + "/PlayerDatabase.s3db");
        }

        UnlockKeyword("Name");

        #region OLD STUFF

        // IDbConnection playerdatabase = (IDbConnection)new SqliteConnection(_playerDatabase);
        // playerdatabase.Open();
        // IDbConnection databasecon = (IDbConnection)new SqliteConnection(_databasePath);
        // databasecon.Open();
        // //INSERT INTO DB1.dbo.TempTable SELECT* FROM DB2.dbo.TempTable

        // IDbCommand command = playerdatabase.CreateCommand();
        // _sqlQuery = "INSERT INTO Subjects SELECT * FROM databasecon.Subjects";
        //// _sqlQuery = "SELECT * FROM Keywords";
        // command.CommandText = _sqlQuery;
        // command.ExecuteNonQuery();
        // //IDataReader reader = command.ExecuteReader();
        // //while(reader.Read()) {
        // //    Debug.Log("reader : " + reader.GetValue(0));
        // //}


        // _databaseConnection = (IDbConnection)new SqliteConnection(_databasePath);
        //try {
        //    _databaseConnection.Open();
        //} catch(Exception e) {
        //    Debug.LogError("Database connection Failed with error : " + e);
        //    throw;
        //}

        //try {
        //    _databaseCommand = _databaseConnection.CreateCommand();


        //ExecuteCommand(CommandType.Update,Tables.Keywords,TableKeywords.Value, "Name");
        //UpdateDatabaseColum(Tables.Keywords, TableKeywords.Value, TableKeywords.Handled, "Name", "1");
        //CheckMilestone(Tables.Milestone1);
        //CheckSubject("1");
        //CheckKeywordWithCommand("OR.1,2");
        //CheckKeyword("1");


        //CreateDatabaseConnection();
        //_sqlQuery = "UPDATE Subjects Set Command = 'AND' , KeywordIDs WHERE SubjectID ='3' ";
        //_databaseCommand.CommandText = _sqlQuery;
        //_databaseCommand.ExecuteNonQuery();
        //CloseDatabaseConnection();
        #endregion

    }

    public void UnlockKeyword(string pKeyword) {
        CreateDatabaseConnection();
        //UpdateDatabaseColum(Tables.Keywords, TableKeywords.Value, TableKeywords.Handled, "Name", "1");
        _sqlQuery = "UPDATE Keywords SET Handled = '1' WHERE Value = '"+pKeyword+"' ";
        _databaseCommand.CommandText = _sqlQuery;
        _databaseCommand.ExecuteNonQuery();
        CloseDatabaseConnection();

        CheckMilestone("Milestone_1");
    }

    public void CheckMilestone(string pMilestoneTable) {
        CreateDatabaseConnection();
     
        _sqlQuery = "SELECT SubjectID FROM Milestone_1";
     
        _databaseCommand.CommandText = _sqlQuery;
        _databaseCommand.ExecuteNonQuery();
        IDataReader reader = _databaseCommand.ExecuteReader();
        List<int> subjectIDs = new List<int>();

        while(reader.Read()) {
            subjectIDs.Add(reader.GetInt32(0));
        }
        reader.Dispose();

        CloseDatabaseConnection();

        for(int i = 0; i < subjectIDs.Count; i++) {
            CheckSubject(subjectIDs[i].ToString());
        }

        UnlockMilestone("Milestone_1");
    }

    public void CheckSubject(string pSubjectID) {
        CreateDatabaseConnection();
        _sqlQuery = "SELECT Command,KeywordIDs FROM Subjects WHERE SubjectID = '" + pSubjectID+"' " ;
        
        _databaseCommand.CommandText = _sqlQuery;
        IDataReader reader = _databaseCommand.ExecuteReader();
        List<string> Commands = new List<string>();
        List<string> Keywords = new List<string>();
        while(reader.Read()) {
            Commands.Add(reader.GetString(0));
            Keywords.Add(reader.GetString(1));
        }
        reader.Dispose();
        CloseDatabaseConnection();

        for(int i = 0; i < Commands.Count; i++) {
            if(CheckKeywordWithCommand(Commands[i],Keywords[i])) {
                UnlockSubject(pSubjectID);
            } 
        }
       
    }

    public void UnlockSubject(string pSubjectID) {
        CreateDatabaseConnection();
        _sqlQuery = "UPDATE Subjects SET Unlocked ='1' WHERE SubjectID ='"+pSubjectID+"' ";
        _databaseCommand.CommandText = _sqlQuery;
        _databaseCommand.ExecuteNonQuery();
        _sqlQuery = "UPDATE Milestone_1 SET Unlocked ='1' WHERE SubjectID ='" + pSubjectID + "' ";
        _databaseCommand.CommandText = _sqlQuery;
        _databaseCommand.ExecuteNonQuery();

        CloseDatabaseConnection();
    }
   
    public void UnlockMilestone(string pSubjectID) {
        CreateDatabaseConnection();
     
        _sqlQuery = "SELECT Unlocked FROM Milestone_1";
        _databaseCommand.CommandText = _sqlQuery;
        _databaseCommand.ExecuteNonQuery();
        IDataReader reader = _databaseCommand.ExecuteReader();
        List<int> outcome = new List<int>();
        while(reader.Read()) {
            outcome.Add(reader.GetInt32(0));
        }
        reader.Dispose();
        CloseDatabaseConnection();

        for(int i = 0; i < outcome.Count; i++) {
            if(outcome[i] == 0) {
                Debug.Log("MileStone was not completed");
                return;
            }
            if(i == outcome.Count-1) {
                Debug.Log("Switch to new Milestone!");
            }
        }
    }

    public bool CheckKeywordWithCommand(string pCommand,string pKeywords) {

        string[] keywords = pKeywords.Split(new char[] { ',' });
        List<bool> booleans = new List<bool>();


        for(int i = 0; i < keywords.Length; i++) {
            booleans.Add(CheckKeyword(keywords[i]));
        }
        switch(pCommand) {
            case "OR":
                for(int i = 0; i < booleans.Count; i++) {
                    if(booleans[i]==true) {
                        return true;
                    }
                }
                break;
            case "AND":
                for(int i = 0; i < booleans.Count; i++) {
                    if(booleans[i] == false) {
                        return false;
                    }
                    if(i == booleans.Count-1) {
                        return true;
                    }
                }
                break;
        }
        return false;
    }

    public bool CheckKeyword(string pKeywordID) {
        CreateDatabaseConnection();
        _sqlQuery = "SELECT Handled FROM Keywords WHERE KeywordID = '"+pKeywordID+"' "; //_sqlQuery = "SELECT Handled FROM Keywords WHERE Keyword_ID = '" + pKeywordID + "' ";
        _databaseCommand.CommandText = _sqlQuery;
        _databaseCommand.ExecuteNonQuery();
        IDataReader reader = _databaseCommand.ExecuteReader();
        int outcome = -1;
        while(reader.Read()) {
            outcome = reader.GetInt32(0);
        }
        reader.Dispose();
        CloseDatabaseConnection();
        if(outcome == 1) {
            return true;
        } else {
            return false;
        }
    }

    private void CreateDatabaseConnection() {
        _databaseConnection = (IDbConnection)new SqliteConnection(_playerDatabase);
        try {
            _databaseConnection.Open();
        } catch(Exception e) {
            Debug.LogError("Database connection Failed with error : " + e);
            throw;
        }

        try {
            _databaseCommand = _databaseConnection.CreateCommand();
        } catch(Exception e) {
            Debug.LogError("Createing database command Failed with error : " + e);
            throw;
        }
    }

    private void CloseDatabaseConnection() {
        _databaseConnection.Dispose();
        _databaseConnection = null;
        _databaseCommand.Dispose();
        _databaseCommand = null;
        _sqlQuery = "";
    }


    #region OLD CODE STILL USEFULL
    //public void UpdateDatabaseColum(string pTable, string pColumID, string pColumToAlter, string pColumIDValue, string pValue) {
    //    CreateDatabaseConnection();
    //    //UPDATE Customers SET ContactName = 'Alfred Schmidt', City = 'Frankfurt' WHERE CustomerID = 1;
    //    _sqlQuery = "UPDATE " + pTable + " SET " + pColumToAlter + " = '" + pValue + "' WHERE " + pColumID + " =" + "'" + pColumIDValue + "'";
    //    _databaseCommand.CommandText = _sqlQuery;
    //    _databaseCommand.ExecuteNonQuery();
    //    CloseDatabaseConnection();

    //    CheckMilestone("Milestone_1");

    //}
    #endregion

}
