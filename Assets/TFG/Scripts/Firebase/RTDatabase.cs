// Copyright 2016 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFG.Events;
using TFG.Authentification;


// Handler for UI buttons on the scene.  Also performs some
// necessary setup (initializing the firebase app, etc) on
// startup.
namespace TFG.Database
{
    // Handler for UI buttons on the scene.  Also performs some
    // necessary setup (initializing the firebase app, etc) on
    // startup.
    public class RTDatabase : Singleton<RTDatabase>
    {
        Auth _auth;
        ArrayList leaderBoard = new ArrayList();
        public IEnumerable<DataSnapshot> questions;
        public IEnumerable<DataSnapshot> temas;
        public IEnumerable<DataSnapshot> puntuaciones;
        Vector2 scrollPosition = Vector2.zero;
        private Vector2 controlsScrollViewVector = Vector2.zero;
        long pastScore = 0;

        //private const int MaxScores = 5;
        private string logText = "";
        //private string email = "";
        private LeaderboardEntry userScore = new LeaderboardEntry();

        const int kMaxLogSize = 16382;
        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        protected bool isFirebaseInitialized = false;

        // When the app starts, check to make sure that we have
        // the required dependencies to use Firebase, and if not,
        // add them if possible.
        protected virtual void Start()
        {
            Debug.Log("Initializing Firebase Database");
            //leaderBoard.Add("Firebase Top " + MaxScores.ToString() + " Scores");
            //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://tfg-2019-20.firebaseio.com/");
            //DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

            _auth = AppManager.Instance.UserAuthentification;

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError(
                      "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }

        // Initialize the Firebase database:
        protected virtual void InitializeFirebase()
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            //StartListener();
            isFirebaseInitialized = true;
        }

        private void OnEnable()
        {
            FirebaseDatabase.DefaultInstance
              .GetReference("preguntas")
              .ValueChanged += HandledChangedQuestions;
            FirebaseDatabase.DefaultInstance
              .GetReference("temas")
              .ValueChanged
                += HandledChangedThemes;
            FirebaseDatabase.DefaultInstance
              .GetReference("puntuaciones")
              .OrderByChild("score").ValueChanged += HandledChangedScores;
              
        }

        private void OnDisable()
        {
            FirebaseDatabase.DefaultInstance
              .GetReference("preguntas")
              .ValueChanged -= HandledChangedQuestions;
            FirebaseDatabase.DefaultInstance
              .GetReference("temas")
              .ValueChanged -= HandledChangedThemes;
            FirebaseDatabase.DefaultInstance
              .GetReference("puntuaciones")
              .OrderByChild("score").ValueChanged -= HandledChangedScores;
              
        }
        /*protected void StartListener()
        {
           
        }*/

        // Output text to the debug log text field, as well as the console.
        public void DebugLog(string s)
        {
            Debug.Log(s);
            logText += s + "\n";

            while (logText.Length > kMaxLogSize)
            {
                int index = logText.IndexOf("\n");
                logText = logText.Substring(index + 1);
            }
        }

        void HandledChangedQuestions(object sender, ValueChangedEventArgs args)
        {
            string dataFetch = "preguntas";
            bool wrongData = false;

            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            Debug.Log("Received values for preguntas.");
            if (args.Snapshot != null && args.Snapshot.ChildrenCount > 0)
            {
                foreach (var childSnapshot in args.Snapshot.Children)
                {
                    if (childSnapshot == null
                      || childSnapshot.Value == null)
                    {
                        Debug.LogError("Bad data in sample.  Did you forget to call SetEditorDatabaseUrl with your project id?");
                        wrongData = true;
                        break;
                    }
                    else
                    {
                        //questions.Add(childSnapshot);
                        //var question = childSnapshot.Value as Dictionary<string, object>;
                        //Debug.Log("Pregunta: " + childSnapshot.Key + "; " + question["texto"] + ";");
                    }
                }
                if(!wrongData)
                {
                    questions = args.Snapshot.Children;
                }
            }
            else
            {
                Debug.Log("Nothing Received from Database");
            }
            if (DatabaseEvents.dataRetrieved != null)
            {
                DatabaseEvents.dataRetrieved.Invoke(dataFetch);
            }
        }

        
        void HandledChangedScores(object sender, ValueChangedEventArgs args)
        {
            string dataFetch = "puntuaciones";
            bool wrongData = false;

            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            Debug.Log("Received values for puntuaciones.");
            if (args.Snapshot != null && args.Snapshot.ChildrenCount > 0)
            {
                foreach (var childSnapshot in args.Snapshot.Children)
                {
                    if (childSnapshot == null
                      || childSnapshot.Value == null)
                    {
                        Debug.LogError("Bad data in sample.  Did you forget to call SetEditorDatabaseUrl with your project id?");
                        wrongData = true;
                        break;
                    }
                    else
                    {
                        if(childSnapshot.Key == _auth._userID)
                        {
                            var sDict = childSnapshot.Value as Dictionary<string, object>;
                            pastScore = (long)sDict["score"];
                        }
                        //var sDict = childSnapshot.Value as Dictionary<string, object>;
                        //var eText = sDict["email"] as string;
                        //long prueba = (long)sDict["score"];
                        //Debug.Log("Puntuacion: " + childSnapshot.Key + ", " + prueba.ToString() + "; " + eText + ";");

                    }
                }
                if (!wrongData)
                {
                    puntuaciones = args.Snapshot.Children;
                }
            }
            else
            {
                Debug.Log("Nothing Received from Database");
            }
            if (DatabaseEvents.dataRetrieved != null)
            {
                DatabaseEvents.dataRetrieved.Invoke(dataFetch);
            }
        }
        

        void HandledChangedThemes(object sender, ValueChangedEventArgs args)
        {
            string dataFetch = "temas";
            bool wrongData = false;

            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            Debug.Log("Received values for temas.");
            if (args.Snapshot != null && args.Snapshot.ChildrenCount > 0)
            {
                foreach (var childSnapshot in args.Snapshot.Children)
                {
                    if (childSnapshot == null
                      || childSnapshot.Value == null)
                    {
                        Debug.LogError("Bad data in sample.  Did you forget to call SetEditorDatabaseUrl with your project id?");
                        wrongData = true;
                        break;
                    }
                    else
                    {
                        //Debug.Log("Tema: " + childSnapshot.Key + "; " + childSnapshot.Value + ";");
                    }
                }
                if (!wrongData)
                {
                    temas = args.Snapshot.Children;
                }
            }
            else
            {
                Debug.Log("Nothing Received from Database");
            }
            if (DatabaseEvents.dataRetrieved != null)
            {
                DatabaseEvents.dataRetrieved.Invoke(dataFetch);
            }

        }


        // A realtime database transaction receives MutableData which can be modified
        // and returns a TransactionResult which is either TransactionResult.Success(data) with
        // modified data or TransactionResult.Abort() which stops the transaction with no changes.
        /*
        TransactionResult AddScoreTransaction(MutableData mutableData)
        {
            
            //List<object> leaders = mutableData.Value as List<object>;
            Dictionary<string, int> leaders = mutableData.Value as Dictionary<string, int>;

            if (leaders == null)
            {
                leaders = new Dictionary<string, int>();
            }
            else if (mutableData.ChildrenCount >= MaxScores)
            {
                // If the current list of scores is greater or equal to our maximum allowed number,
                // we see if the new score should be added and remove the lowest existing score.
                long minScore = long.MaxValue;
                object minVal = null;
                foreach (var child in leaders)
                {
                    if (!(child is Dictionary<string, object>))
                        continue;
                    long childScore = (long)((Dictionary<string, object>)child)["score"];
                    if (childScore < minScore)
                    {
                        minScore = childScore;
                        minVal = child;
                    }
                }
                // If the new score is lower than the current minimum, we abort.
                if (minScore > score)
                {
                    return TransactionResult.Abort();
                }
                // Otherwise, we remove the current lowest to be replaced with the new score.
                leaders.Remove(minVal);
            }
            // Now we add the new score as a new entry that contains the email address and score.
            
            if(!leaders.ContainsKey(userScore.uid)) // !mutableData.HasChild(userScore.uid))
            {
                leaders[userScore.uid] = userScore.score;
            }
            else
            {
                leaders[userScore.uid] = leaders[userScore.uid] + userScore.score;
            }
            // You must set the Value to indicate data at that location has changed.
            //leaders.Add(newScoreMap);
            //mutableData.Value = leaders;
            //return TransactionResult.Success(mutableData);

            //Dictionary<string, object> scores = mutableData; // as Dictionary<string, object>;
            mutableData.Value = leaders as object;
            return TransactionResult.Success(mutableData);
        }*/

        /*

        private void AddScoreToLeaders(string email,
                           long score,
                           DatabaseReference leaderBoardRef)
        {

            leaderBoardRef.RunTransaction(mutableData => {
                List<object> leaders = mutableData.Value as List<object>
        
                if (leaders == null)
                {
                    leaders = new List<object>();
                }
                else if (mutableData.ChildrenCount >= MaxScores)
                {
                    long minScore = long.MaxValue;
                    object minVal = null;
                    foreach (var child in leaders)
                    {
                        if (!(child is Dictionary<string, object>)) continue;
                        long childScore = (long)
                                    ((Dictionary<string, object>)child)["score"];
                        if (childScore < minScore)
                        {
                            minScore = childScore;
                            minVal = child;
                        }
                    }
                    if (minScore > score)
                    {
                        // The new score is lower than the existing 5 scores, abort.
                        return TransactionResult.Abort();
                    }

                    // Remove the lowest score.
                    leaders.Remove(minVal);
                }

                // Add the new high score.
                Dictionary<string, object> newScoreMap =
                                 new Dictionary<string, object>();
                newScoreMap["score"] = score;
                newScoreMap["email"] = email;
                leaders.Add(newScoreMap);
                mutableData.Value = leaders;
                return TransactionResult.Success(mutableData);
            });
        }
        */


        /*
        public void AddScore(LeaderboardEntry scoreEntry)
        {
            if (scoreEntry.score == 0 || string.IsNullOrEmpty(scoreEntry.uid))
            {
                DebugLog("invalid score or email.");
                return;
            }
            DebugLog(String.Format("Attempting to add score {0} {1}",
              scoreEntry.uid, scoreEntry.score.ToString()));
            userScore = scoreEntry;

            DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("puntuaciones");

            DebugLog("Running Transaction...");
            // Use a transaction to ensure that we do not encounter issues with
            // simultaneous updates that otherwise might create more than MaxScores top scores.
            reference.RunTransaction(AddScoreTransaction)
              .ContinueWithOnMainThread(task => {
                  if (task.Exception != null)
                  {
                      DebugLog(task.Exception.ToString());
                  }
                  else if (task.IsCompleted)
                  {
                      DebugLog("Transaction complete.");
                  }
              });
        }
        */

        public string AddQuestionToDatabase(QuestionEntry entry)
        {
            // Create new entry at /user-scores/$userid/$scoreid and at
            // /leaderboard/$scoreid simultaneously
            string key = FirebaseDatabase.DefaultInstance.RootReference.Child("preguntas").Push().Key;
            Dictionary<string, object> entryValues = entry.ToDictionary();

            Dictionary<string, object> childUpdates = new Dictionary<string, object>();
            childUpdates[key] = entryValues;

            FirebaseDatabase.DefaultInstance.RootReference.Child("preguntas").UpdateChildrenAsync(childUpdates);

            return key;
        }

        public void ModifyQuestionToDatabase(QuestionEntry entry, string key)
        {
            Dictionary<string, object> entryValues = entry.ToDictionary();

            Dictionary<string, object> childUpdates = new Dictionary<string, object>();
            childUpdates[key] = entryValues;

            FirebaseDatabase.DefaultInstance.RootReference.Child("preguntas").UpdateChildrenAsync(childUpdates);
        }

        public void AddScore(LeaderboardEntry scoreEntry)
        {
            /*var usuarios = puntuaciones as Dictionary<string, object>;
            if (usuarios != null && usuarios.ContainsKey(scoreEntry.uid))
            {
                DebugLog("Treying to check past score");
                var checkScore = usuarios[scoreEntry.uid] as Dictionary<string, object>;
                pastScore = (long)checkScore["score"];
            }*/

            scoreEntry.score += pastScore;
            Dictionary<string, object> entryValues = scoreEntry.ToDictionary();

            Dictionary<string, object> childUpdates = new Dictionary<string, object>();
            childUpdates[scoreEntry.uid] = entryValues;

            FirebaseDatabase.DefaultInstance.RootReference.Child("puntuaciones").UpdateChildrenAsync(childUpdates);
        }
        /*
        public void RetrieveScores()
        {
            string dataFetch = "puntuaciones";
            FirebaseDatabase.DefaultInstance
              .GetReference(dataFetch)
              .GetValueAsync().ContinueWith(task => {
                  if (task.IsFaulted)
                  {
                      // Handle the error...
                      Debug.Log("No Scores Received from Database");
                  }
                  else if (task.IsCompleted)
                  {
                      bool wrongData = false;
                      DataSnapshot snapshot = task.Result;
                      foreach (var childSnapshot in snapshot.Children)
                      {
                          if (childSnapshot == null
                            || childSnapshot.Value == null)
                          {
                              Debug.LogError("Bad data in sample.  Did you forget to call SetEditorDatabaseUrl with your project id?");
                              wrongData = true;
                              break;
                          }
                          else
                          {
                              var sDict = childSnapshot.Value as Dictionary<string, object>;
                              var eText = sDict["email"] as string;
                              long prueba = (long)sDict["score"];
                              Debug.Log("Puntuacion: " + childSnapshot.Key + ", " + prueba.ToString() + "; " + eText + ";");
                          }
                      }
                      if (!wrongData)
                      {

                          puntuaciones = snapshot.Children;
                      }
                      
                      Debug.Log("Scores Received from Database");
                      if (DatabaseEvents.dataRetrieved != null)
                      {
                          DatabaseEvents.dataRetrieved.Invoke(dataFetch);
                      }
                  }
              });
        }
        */
    }
}
