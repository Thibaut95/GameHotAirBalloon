using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using Firebase.Auth;

public class ScoresPanel : MonoBehaviour
{
    private FirebaseAuth auth;
    private DatabaseReference dbReference;
    private Dictionary<string, User> users;
    private List<List<Tuple<string, Score>>> bestScores;
    private List<Score> personalScores;
    private List<Score> otherScores;
    private string state = "";
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
        switch (state)
        {
            case "updateTableUserScore":
                this.transform.Find("UserScore").GetComponent<TableScore>().SetScores(personalScores);
                break;
            case "updateTableOtherScore":
                this.transform.Find("OtherUserScore").GetComponent<TableScore>().SetScores(otherScores);
                break;
            case "updateTableGeneralScore":
                this.transform.Find("GeneralScore").GetComponent<TableGeneral>().SetScores(bestScores);
                break;
        }
        state = "";
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
        state = "updateTableGeneralScore";
    }

    private void UpdateListUserScores(DataSnapshot snapshot)
    {
        personalScores.Clear();
        Debug.Log(snapshot.GetRawJsonValue());
        foreach (var item in snapshot.Children)
        {
            Score score = JsonUtility.FromJson<Score>(item.GetRawJsonValue());
            personalScores.Add(score);
        }
        state = "updateTableUserScore";
    }

    private void UpdateListOtherScores(DataSnapshot snapshot)
    {
        otherScores.Clear();
        foreach (var item in snapshot.Children)
        {
            Score score = JsonUtility.FromJson<Score>(item.GetRawJsonValue());
            otherScores.Add(score);
        }
        state = "updateTableOtherScore";
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
            }
        });
    }

    public void GetOtherUserScores()
    {
        string username = transform.Find("OtherUserScore").Find("InputFieldUserName").Find("Text").GetComponent<Text>().text;
        string id = "";
        foreach (var item in users)
        {
            if (item.Value.username == username)
            {
                id = item.Key;
                break;
            }
        }
        if (id != "")
        {
            transform.Find("OtherUserScore").Find("TextError").GetComponent<Text>().text = "";
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
            transform.Find("OtherUserScore").Find("TextError").GetComponent<Text>().text = "Le nom d'utilisateur n'existe pas";
            otherScores.Clear();
            state = "updateTableOtherScore";
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
            }
        });
    }

    public void UpdateScoresPanel(int race)
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        this.race = race;
        transform.Find("TextRace").GetComponent<Text>().text = "Scores pour la course " + (race + 1);
        GetUserScores();
        GetUsers();
    }
}

