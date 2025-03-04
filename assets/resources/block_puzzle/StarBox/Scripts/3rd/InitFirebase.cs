using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitFirebase : OneSoftGame.Tools.PersistentSingleton<InitFirebase> {

    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    public static bool IsFirebaseInitialized = false;
    
    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    protected void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                IsFirebaseInitialized = true;
                Debug.Log("Firebase Initialized");
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies");
            }
        });
    }

}
