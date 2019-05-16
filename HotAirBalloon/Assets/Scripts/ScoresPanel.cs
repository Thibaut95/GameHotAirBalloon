using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;

public class ScoresPanel : MonoBehaviour
{
    private Firebase.Auth.FirebaseAuth auth;
    private DatabaseReference dbReference;

    private Dictionary<string, User> users;

    private List<List<Tuple<string, Score>>> bestScores;

    private List<Score> personalScores;
    private List<Score> otherScores;

    private bool updateTableUserScore = false;
    private bool updateTableOtherScore = false;
    private bool updateTableGeneralScore = false;
    private int race;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://gamehotairballoon-1.firebaseio.com/");
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        

        bestScores = new List<List<Tuple<string, Score>>>();
        users = new Dictionary<string, User>();
        personalScores = new List<Score>();
        otherScores = new List<Score>();

        for (int i = 0; i < 4; i++)
        {
            bestScores.Add(new List<Tuple<string, Score>>());
        }
    }

    void Update()
    {
        if (updateTableUserScore)
        {
            updateTableUserScore = false;
            this.transform.Find("UserScore").GetComponent<TableScore>().SetScores(personalScores);
        }
        if (updateTableOtherScore)
        {
            updateTableOtherScore = false;
            this.transform.Find("OtherUserScore").GetComponent<TableScore>().SetScores(otherScores);
        }
        if (updateTableGeneralScore)
        {
            updateTableGeneralScore = false;
            this.transform.Find("GeneralScore").GetComponent<TableGeneral>().SetScores(bestScores);
        }
    }

    private void UpdateListBestScores(DataSnapshot snapshot)
    {
        for (int i = 0; i < 4; i++)
        {
            bestScores[i].Clear();
        }
        foreach (var item in snapshot.Children)
        {
            for (int i = 0; i < item.ChildrenCount; i++)
            {
                Score score = JsonUtility.FromJson<Score>(item.Child("score" + i).GetRawJsonValue());
                bestScores[i].Add(new Tuple<string, Score>(users[item.Key].username, score));
            }
        }
        updateTableGeneralScore = true;
    }

    private void UpdateListUserScores(DataSnapshot snapshot)
    {
        personalScores.Clear();
        foreach (var item in snapshot.Children)
        {
            Score score = JsonUtility.FromJson<Score>(item.GetRawJsonValue());
            personalScores.Add(score);
        }
        Debug.Log("usersscores update");
        updateTableUserScore = true;

    }

    private void UpdateListOtherScores(DataSnapshot snapshot)
    {
        otherScores.Clear();
        foreach (var item in snapshot.Children)
        {
            Score score = JsonUtility.FromJson<Score>(item.GetRawJsonValue());
            otherScores.Add(score);
        }
        Debug.Log("otherscores update");
        updateTableOtherScore = true;

    }

    private void UpdateListUsers(DataSnapshot snapshot)
    {
        users.Clear();
        foreach (var item in snapshot.Children)
        {
            User user = JsonUtility.FromJson<User>(item.GetRawJsonValue());
            users.Add(item.Key, user);
        }
    }

    private void GetGeneralScores()
    {
        FirebaseDatabase.DefaultInstance.GetReference("generalscores/" + ("race" + race)).OrderByKey().GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("error in getting generalscores : " + task.Exception);
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                UpdateListBestScores(snapshot);
                if (snapshot.Exists)
                {
                    Debug.Log("generalscores get");
                    
                }
                else
                {
                    Debug.Log("no data");
                }
            }
        });
    }

    public void GetAutherUserScores()
    {
        string username = transform.Find("OtherUserScore").Find("InputFieldUserName").Find("Text").GetComponent<Text>().text;
        string id="";
        foreach (var item in users)
        {
            if(item.Value.username==username)
            {
                id=item.Key;
                break;
            }
        }
        if(id!="")
        {
            FirebaseDatabase.DefaultInstance.GetReference("usersscores/" + ("race" + race) + "/" + id).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("error in getting userscores : " + task.Exception);
                    return;
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    UpdateListOtherScores(snapshot);
                    if (snapshot.Exists)
                    {
                        Debug.Log("userscores get");
                    }
                    else
                    {
                        Debug.Log("no data");
                    }
                }
            });
        }
        else
        {
            // TODO error message
        }
    }
    private void GetUserScores()
    {
        FirebaseDatabase.DefaultInstance.GetReference("usersscores/" + ("race" + race) + "/" + StaticClass.GetHashString(auth.CurrentUser.Email)).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("error in getting userscores : " + task.Exception);
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                UpdateListUserScores(snapshot);
                if (snapshot.Exists)
                {
                    Debug.Log("userscores get");
                }
                else
                {
                    Debug.Log("no data");
                }
            }
        });
    }

    private void GetUsers()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users/").OrderByKey().GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("error in getting users : " + task.Exception);
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                UpdateListUsers(snapshot);
                GetGeneralScores();
                if (snapshot.Exists)
                {
                    Debug.Log("users get");
                }
                else
                {
                    Debug.Log("no data");
                }
            }
        });
    }

    // Update is called once per frame
    public void UpdateScoresPanel(int race)
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        this.race = race;
        transform.Find("TextRace").GetComponent<Text>().text = "Scores pour la course "+(race+1);
        GetUserScores();
        GetUsers(); 
    }
}
