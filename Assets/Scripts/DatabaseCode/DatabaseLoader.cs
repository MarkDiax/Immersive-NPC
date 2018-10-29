using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;


public class DatabaseLoader : MonoBehaviour {

    void Start() {
        LoadSomeData();
    }


    private void LoadSomeData() {

        string conn = "URI=file:" + Application.dataPath + "/TestDatabase.s3db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        //string sqlQueryInsert = "INSERT INTO PlaceSequence(Value,Name,RandomSequence) VALUES ('4', 'Insertvalue2', '6');";
        //dbcmd.CommandText = sqlQueryInsert;
        //dbcmd.ExecuteNonQuery();
        string sqlQueryUpdate = "UPDATE PlaceSequence SET Value='5', Name='Updated', RandomSequence='7' WHERE Value = '4';";
        dbcmd.CommandText = sqlQueryUpdate;
        dbcmd.ExecuteNonQuery();

        string sqlQuery = "SELECT Value,Name, RandomSequence " + "FROM PlaceSequence";
        


        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while(reader.Read()) {
            int value = reader.GetInt32(0);
            string name = reader.GetString(1);
            int rand = reader.GetInt32(2);

            Debug.Log("Value= " + value + "  Name =" + name + "  Random ="+ rand);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }
}
