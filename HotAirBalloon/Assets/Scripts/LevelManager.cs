using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject balloon;

    private bool endLevel = false;
    private float time = 0;
    private BalloonControler balloonControler;
    private WindController windController;
    private GameObject canvasFinish;
    DatabaseReference dbReference;
    Firebase.Auth.FirebaseAuth auth;

    // Start is called before the first frame update
    void Start()
    {
        

        Time.timeScale = 0;
        balloonControler = balloon.GetComponent<BalloonControler>();
        windController = balloon.GetComponent<WindController>();
        canvasFinish = this.transform.Find("CanvasFinish").gameObject;
        canvasFinish.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (balloonControler.Geth() > balloonControler.Geth0())
        {
            endLevel = true;
        }

        if (balloonControler.Geth() <= balloonControler.Geth0() && endLevel)
        {
            FinishGame();
        }
    }

    void FixedUpdate()
    {
        time += Time.deltaTime;
    }

    public void StartGame()
    {
        Time.timeScale = 1;
    }

    public void CreateBestScores(Score score, int index)
    {
        string json = JsonUtility.ToJson(score);

        dbReference.Child("generalscores").Child("race"+StaticClass.CrossSceneInformation).Child(StaticClass.GetHashString(auth.CurrentUser.Email)).Child("score" + index).SetRawJsonValueAsync(json).ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
                  Debug.LogError("error: " + task.Exception);
                  return;
              }
              Debug.LogFormat("Score inserted");
          });

    }

    public void CheckAndUpdateBestScores(DataSnapshot snapshot, Score score)
    {
        Score[] scoreTab = new Score[snapshot.ChildrenCount];
        for (int i = 0; i < snapshot.ChildrenCount; i++)
        {
            scoreTab[i] = JsonUtility.FromJson<Score>(snapshot.Child("score" + i).GetRawJsonValue());
            if (i == 0 && scoreTab[i].fuel < score.fuel)
            {
                scoreTab[i] = score;
                CreateBestScores(score, i);
            }
            else if (i == 1 && scoreTab[i].distance > score.distance)
            {
                scoreTab[i] = score;
                CreateBestScores(score, i);
            }
            else if (i == 2 && scoreTab[i].time > score.time)
            {
                scoreTab[i] = score;
                CreateBestScores(score, i);
            }
            else if (i == 3 && scoreTab[i].global < score.global)
            {
                scoreTab[i] = score;
            }
        }
    }
    public void FinishGame()
    {
        endLevel = false;
        Time.timeScale = 0;
        canvasFinish.SetActive(true);

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://gamehotairballoon-1.firebaseio.com/");
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        //TODO compute global score
        Score score = new Score(balloonControler.GetCurrentFuel(), windController.DistanceToTarget(), time, 0);
        string json = JsonUtility.ToJson(score);
        // auth.CurrentUser.Email
        dbReference.Child("usersscores").Child("race"+StaticClass.CrossSceneInformation).Child(StaticClass.GetHashString(auth.CurrentUser.Email)).Push().SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("error in inserting userscore: " + task.Exception);
                return;
            }
            Debug.LogFormat("Score inserted");
        });

        FirebaseDatabase.DefaultInstance.GetReference("generalscores/"+("race"+StaticClass.CrossSceneInformation)+"/" + StaticClass.GetHashString(auth.CurrentUser.Email)).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("error in getting generalscores : " + task.Exception);
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Debug.Log("best scores allready exist");
                    CheckAndUpdateBestScores(snapshot, score);
                }
                else
                {
                    Debug.Log("create best scores");
                    for (int i = 0; i < 4; i++)
                    {
                        CreateBestScores(score, i);
                    }
                }
            }
        });
    }
}
