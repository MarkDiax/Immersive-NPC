using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JobType {
    None = -1,
    TalkToPerson = 0,
    SitOnChair = 1,
    PickUpFood = 2,
    WalkToPoint = 3,
    MoveObject = 4
}

public enum Tasks {
    TalkToPerson = 1,
    MoveObject = 2,
    PickUpObjectAndFinishAtLocation = 3
    
}


public class JobManager : MonoBehaviour {

    public static JobManager Instance;

    public Dictionary<JobType, List<Job>> Jobs = new Dictionary<JobType, List<Job>>();


    // Use this for initialization
    void Start() {
        if(Instance != null) {
            Destroy(this);
        }
        Instance = this;

        Job[] jobs = GetComponentsInChildren<Job>();
        for(int i = 0; i < jobs.Length; i++) {
            AddJob(jobs[i]);
        }
        
        //for(int i = 0; i < spots.Length; i++) {
        //    if(Destinations.ContainsKey(spots[i].SpotType)) {
        //        Destinations[spots[i].SpotType].Add(spots[i]);
        //    } else {
        //        Destinations.Add(spots[i].SpotType, new List<Spot>());
        //        Destinations[spots[i].SpotType].Add(spots[i]);
        //    }
        //}
    }

    public void AddJob(Job pJob) {
            if(Jobs.ContainsKey(pJob.JobType)) {
                Jobs[pJob.JobType].Add(pJob);
            } else {
                Jobs.Add(pJob.JobType, new List<Job>());
                Jobs[pJob.JobType].Add(pJob);
            }
    }

    public Job GetJob() {

        //for test
        return Jobs[JobType.PickUpFood][0];


        int JobIndex = Random.Range(0, Jobs.Keys.Count);
        for(int i = 0; i < Jobs[(JobType)JobIndex].Count; i++) {
            Debug.Log(Jobs[(JobType)JobIndex][i].Taken + " " + Jobs[(JobType)JobIndex][i].Claimend);
            if(!Jobs[(JobType)JobIndex][i].Taken && !Jobs[(JobType)JobIndex][i].Claimend) {
                Debug.Log("Return : " + Jobs[(JobType)JobIndex][i].gameObject.name);
                return  Jobs[(JobType)JobIndex][i];
            }
        }
        return null;
    }

    public void Update() {
        if(Input.GetKeyDown(KeyCode.S)) {
            Job newJob = GetJob();
            Debug.Log("Name obj " +  newJob.gameObject.name);
        }
    }

}
