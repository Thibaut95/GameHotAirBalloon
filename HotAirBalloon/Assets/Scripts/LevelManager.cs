using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject balloon;
    [SerializeField]
    private GameObject canvasUI;

    private bool endLevel = false;
    private float time = 0;
    private BalloonController balloonController;
    private WindController windController;
    private GameObject canvasFinish;
    private GameObject littleNeedle;
    private GameObject bigNeedle;
    DatabaseReference dbReference;
    Firebase.Auth.FirebaseAuth auth;

    // Start is called before the first frame update
    void Start()
    {


        Time.timeScale = 0;
        balloonController = balloon.GetComponent<BalloonController>();
        windController = balloon.GetComponent<WindController>();
        canvasFinish = this.transform.Find("CanvasFinish").gameObject;
        canvasFinish.SetActive(false);
        littleNeedle = canvasUI.transform.Find("NeedleLittle").gameObject;
        bigNeedle = canvasUI.transform.Find("NeedleBig").gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        if (balloonController.Geth() > balloonController.Geth0())
        {
            endLevel = true;
        }

        if (balloonController.Geth() <= balloonController.Geth0() && endLevel)
        {
            FinishGame();
        }
    }

    void FixedUpdate()
    {
        time += Time.deltaTime;
        float sec = time % 60;
        float min = (time - sec) / 60;
        littleNeedle.transform.eulerAngles = new Vector3(0, 0, -sec * 6);
        bigNeedle.transform.eulerAngles = new Vector3(0, 0, -min * 6);
    }

    public void StartGame()
    {
        Time.timeScale = 1;
    }

    public void CreateBestScores(Score score, int index)
    {
        string json = JsonUtility.ToJson(score);

        dbReference.Child("generalscores").Child("race" + StaticClass.CrossSceneInformation).Child(StaticClass.GetHashString(auth.CurrentUser.Email)).Child("score" + index).SetRawJsonValueAsync(json).ContinueWith(task =>
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
                CreateBestScores(score, i);
            }
        }
    }


    public void BreakOnGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void FinishGame()
    {
        endLevel = false;
        Time.timeScale = 0;
        canvasFinish.SetActive(true);

        int score_dist = windController.DistanceStartToTarget() - windController.DistanceToTarget();
        if (score_dist < 0) score_dist = 0;

        float maxStrength = windController.GetMaxStrength();
        int minTime = (int)(windController.DistanceToStart() / (maxStrength));
        float scoreTime = minTime / time;

        float score_new = (float)score_dist + scoreTime;


        transform.Find("CanvasFinish").Find("TextDistance").GetComponent<Text>().text = string.Format("{0:0}", windController.DistanceToTarget()) + " M";
        transform.Find("CanvasFinish").Find("TextFuel").GetComponent<Text>().text = string.Format("{0:0.0}", balloonController.GetCurrentFuel()) + " L";
        float sec = time % 60;
        float min = (time - sec) / 60;
        transform.Find("CanvasFinish").Find("TextTime").GetComponent<Text>().text = string.Format("{0:0}", min) + " MIN " + string.Format("{0:0}", sec) + " SEC";
        transform.Find("CanvasFinish").Find("TextScore").GetComponent<Text>().text = string.Format("{0:0.00}", score_new);

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://gamehotairballoon-1.firebaseio.com/");
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            Score score = new Score(balloonController.GetCurrentFuel(), windController.DistanceToTarget(), time, score_new);
            string json = JsonUtility.ToJson(score);

            dbReference.Child("usersscores").Child("race" + StaticClass.CrossSceneInformation).Child(StaticClass.GetHashString(auth.CurrentUser.Email)).Push().SetRawJsonValueAsync(json).ContinueWith(task =>
              {
                  if (task.IsFaulted)
                  {
                      Debug.LogError("error in inserting userscore: " + task.Exception);
                      return;
                  }
                  Debug.LogFormat("Score inserted");
              });

            FirebaseDatabase.DefaultInstance.GetReference("generalscores/" + ("race" + StaticClass.CrossSceneInformation) + "/" + StaticClass.GetHashString(auth.CurrentUser.Email)).GetValueAsync().ContinueWith(task =>
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
}
