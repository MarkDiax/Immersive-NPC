using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RogoDigital
{
	public static class ContinuationManager
	{
		public class Job
		{
			public Job(Func<bool> completed, System.Action continueWith) {
				Completed = completed;
				ContinueWith = continueWith;
			}

			public Func<bool> Completed { get; private set; }
			public System.Action ContinueWith { get; private set; }
		}

		private static readonly List<Job> jobs = new List<Job>();
		private static MonoUpdatePassthrough updatePass;

		public static void Add(Func<bool> completed, System.Action continueWith) {
			if (!jobs.Any()) {
				if (updatePass == null) {
					updatePass = GameObject.FindObjectOfType<MonoUpdatePassthrough>();
					
					if (updatePass == null) {
						GameObject obj = new GameObject("MonoUpdatePassthrough");
						updatePass = obj.AddComponent<MonoUpdatePassthrough>();
					}
				}

				updatePass.onUpdate += Update;
			}

			Job job = new Job(completed, continueWith);
			jobs.Add(job);
		}

		private static void Update() {
			for (int i = 0; i >= 0; --i) {
				var jobIt = jobs[i];
				if (jobIt.Completed()) {
					jobs.RemoveAt(i);
					jobIt.ContinueWith();
				}
			}
			if (!jobs.Any()) updatePass.onUpdate -= Update;
		}
	}
}