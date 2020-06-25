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
using TFG.UI;


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
        UI_Manager _uiManager;
        ArrayList leaderBoard = new ArrayList();
        public IEnumerable<DataSnapshot> questions;
        public IEnumerable<DataSnapshot> temas;
        public IEnumerable<DataSnapshot> puntuaciones;
        public IEnumerable<DataSnapshot> stats;

        public Dictionary<string, long> questionStats;
        public List<string> questionCercanas = new List<string>();

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
            _uiManager = UI_Manager.Instance;

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
            //FirebaseApp app = FirebaseApp.DefaultInstance;
            //StartListener();
            isFirebaseInitialized = true;
        }

        private void OnEnable()
        {
            FirebaseDatabase.DefaultInstance
              .GetReference("temas")
              .OrderByValue().ValueChanged += HandledChangedThemes;
            FirebaseDatabase.DefaultInstance
              .GetReference("puntuaciones")
              .OrderByChild("score").ValueChanged += HandledChangedScores;
            FirebaseDatabase.DefaultInstance
              .GetReference("stats")
              .ValueChanged += HandleChangedStats;
            FirebaseDatabase.DefaultInstance
              .GetReference("preguntas")
              .ValueChanged += HandledChangedQuestions;

            UserEvents.userSignIn.AddListener(HandleChangedRoles);
        }

        private void OnDisable()
        {
            FirebaseDatabase.DefaultInstance
              .GetReference("temas")
              .OrderByValue().ValueChanged -= HandledChangedThemes;
            FirebaseDatabase.DefaultInstance
              .GetReference("puntuaciones")
              .OrderByChild("score").ValueChanged -= HandledChangedScores;
            FirebaseDatabase.DefaultInstance
              .GetReference("stats")
              .ValueChanged -= HandleChangedStats;
            FirebaseDatabase.DefaultInstance
              .GetReference("preguntas")
              .OrderByChild("tema").ValueChanged -= HandledChangedQuestions;

            UserEvents.userSignIn.RemoveListener(HandleChangedRoles);
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

        void HandleChangedRoles()
        {
            FirebaseDatabase.DefaultInstance
              .GetReference("roles/" + _auth._userID)
              .GetValueAsync().ContinueWith(task => {
                  if (task.IsFaulted)
                  {
                      // Handle the error...
                      Debug.Log("Cant Access roles");
                  }
                  else if (task.IsCompleted)
                  {
                      DataSnapshot snapshot = task.Result;
                      // Do something with snapshot...
                      if(snapshot.Value != null)
                      {
                          string readRole = snapshot.Value as string;
                          _auth._userRole = readRole;
                          Debug.Log("Valor de rol Leido: " + readRole);
                          if (UserEvents.userRoleChanged != null)
                          {
                              UserEvents.userRoleChanged.Invoke(_auth._userRole);
                          }
                      }
                  }
              });
        }

        void HandleChangedStats(object sender, ValueChangedEventArgs args)
        {
            string dataFetch = "stats";
            bool wrongData = false;

            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            Debug.Log("Received values for stats.");
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
                        //Debug.Log("Stat: " + childSnapshot.Key.ToString() + ";");
                    }
                }
                if (!wrongData)
                {
                    stats = args.Snapshot.Children;
                    //Debug.Log("Stats correct;");
                }
            }
            else
            {
                Debug.Log("No stats Received from Database");
            }
            if (DatabaseEvents.dataRetrieved != null)
            {
                DatabaseEvents.dataRetrieved.Invoke(dataFetch);
            }

        }

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

        public void DeleteQuestionToDatabase(string key)
        {
            //FirebaseDatabase.DefaultInstance.RootReference.Child("preguntas/" + key).Dele;
            FirebaseDatabase.DefaultInstance.RootReference.Child("preguntas").Child(key).RemoveValueAsync();
        }

        public void GetQuestionStatistics(string key)
        {
            FirebaseDatabase.DefaultInstance
              .GetReference("stats/" + key)
              .GetValueAsync().ContinueWith(task => {
                  if (task.IsFaulted)
                  {
                      // Handle the error...
                      Debug.Log("Cant Access roles");
                  }
                  else if (task.IsCompleted)
                  {
                      DataSnapshot snapshot = task.Result;
                      // Do something with snapshot...
                      if (snapshot.Value != null)
                      {
                          var sDict = snapshot.Value as Dictionary<string, object>;
                          questionStats["correcta"] = (long)sDict["correcta"];
                          questionStats["incorrecta"] = (long)sDict["incorrecta"];
                      }
                      else
                      {
                          questionStats["correcta"] = 0;
                          questionStats["incorrecta"] = 0;
                      }
                  }
              });
        }

        public void GetQuestionCercanas(string key)
        {
            FirebaseDatabase.DefaultInstance
              .GetReference("preguntas/" + key + "/cercanas")
              .GetValueAsync().ContinueWith(task => {
                  if (task.IsFaulted)
                  {
                      // Handle the error...
                      Debug.Log("Cant Access cercanas");
                  }
                  else if (task.IsCompleted)
                  {
                      DataSnapshot snapshot = task.Result;
                      // Do something with snapshot...
                      questionCercanas.Clear();
                      if (snapshot.Value != null)
                      {
                          var sDict = snapshot.Value as Dictionary<string, string>;
                          foreach (var cercana in sDict.Values)
                          {
                              questionCercanas.Add(cercana);
                              Debug.Log("Cercana " + cercana);
                          }
                          Debug.Log("Recividas Cercanas");
                      }
                      if(DatabaseEvents.cercanasRetrieved != null)
                      {
                          DatabaseEvents.cercanasRetrieved.Invoke();
                      }
                  }
              });
        }

        public void UpdateQuestionStats(string key, bool correct)
        {
            long correctTimes = 0, incorrectTimes = 0;
            Dictionary<string, object> statsQuestion;
            if(correct && _uiManager._statsIDs.Contains(key))
            {
                statsQuestion = _uiManager._statsValues[_uiManager._statsIDs.IndexOf(key)] as Dictionary<string, object>;
                correctTimes = (long)statsQuestion["correcta"];
                correctTimes++;
                UpdateQuestionCorrect(key, correctTimes);
            }
            else if (!correct && _uiManager._statsIDs.Contains(key))
            {
                statsQuestion = _uiManager._statsValues[_uiManager._statsIDs.IndexOf(key)] as Dictionary<string, object>;
                incorrectTimes = (long)statsQuestion["incorrecta"];
                incorrectTimes++;
                UpdateQuestionIncorrect(key, incorrectTimes);
            }
            else
            {
                StartQuestionStats(key, correct);
            }
        }

        void UpdateQuestionCorrect(string key, long number)
        {
            Dictionary<string, object> childUpdates = new Dictionary<string, object>();
            childUpdates["correcta"] = number;

            FirebaseDatabase.DefaultInstance.RootReference.Child("stats/" + key).UpdateChildrenAsync(childUpdates);
        }

        void UpdateQuestionIncorrect(string key, long number)
        {
            Dictionary<string, object> childUpdates = new Dictionary<string, object>();
            childUpdates["incorrecta"] = number;

            FirebaseDatabase.DefaultInstance.RootReference.Child("stats/" + key).UpdateChildrenAsync(childUpdates);
        }

        void StartQuestionStats(string key, bool correct)
        {
            Dictionary<string, object> childUpdates = new Dictionary<string, object>();
            childUpdates["correcta"] = correct ? 1 : 0;
            childUpdates["incorrecta"] = correct ? 0 : 1;

            FirebaseDatabase.DefaultInstance.RootReference.Child("stats/" + key).UpdateChildrenAsync(childUpdates);
        }
    }
}
