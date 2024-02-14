using MongoDB.Driver;
using UnityEngine;

namespace DefaultNamespace
{
    public class DriverTest : MonoBehaviour
    {
        const string ConnectionString = "mongodb://localhost:27017,localhost:27019/?replicaSet=poolReplSet";

        void Awake()
        {
            Debug.Log("Initializing...");

            var mongoClient = new MongoClient(MongoClientSettings.FromConnectionString(ConnectionString));

            Debug.Log("Initialized!");
        }
    }
}
