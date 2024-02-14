using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using MongoDB.Driver;
using Unity.Profiling.Memory;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    public class DriverTestWithSnapshots : MonoBehaviour
    {
        const string LogFilePath = "_log.csv";
        const string SnapshotsDirectory = "Snapshots/";

        static readonly TimeSpan SnapshotDelay = TimeSpan.FromHours(2);

        static int id;

        Coroutine _co;
        Process _process;
        const string ConnectionString = "mongodb://localhost:27017,localhost:27019/?replicaSet=poolReplSet";

        void Awake()
        {
            Debug.Log("Initializing...");

            _process = Process.GetCurrentProcess();

            if (File.Exists(LogFilePath))
            {
                File.Delete(LogFilePath);
            }

            using (StreamWriter w = File.AppendText(LogFilePath))
            {
                w.WriteLine("GlobalTime,WorkingSet");
            }

            if (!Directory.Exists(SnapshotsDirectory))
            {
                Directory.CreateDirectory(SnapshotsDirectory);
            }

            var mongoClient = new MongoClient(MongoClientSettings.FromConnectionString(ConnectionString));

            Debug.Log("Initialized!");
        }

        void OnEnable()
        {
            _co = StartCoroutine(SnapshotsTakerProcess());
        }

        void OnDisable()
        {
            StopCoroutine(_co);
        }

        IEnumerator SnapshotsTakerProcess()
        {
            while (true)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                MemoryProfiler.TakeSnapshot($"{SnapshotsDirectory}snapshot{id++}.snap", finishCallback: null);

                _process.Refresh();

                using (StreamWriter w = File.AppendText(LogFilePath))
                {
                    w.WriteLine($"{((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()},{_process.WorkingSet64}");
                }

                yield return new WaitForSeconds((float)SnapshotDelay.TotalSeconds);
            }
        }
    }
}
