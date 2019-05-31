using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class LevelPanel : MonoBehaviour
{
    [SerializeField]
    private int mapNumber;
    [SerializeField]
    private GameObject previousButton;
    [SerializeField]
    private GameObject nextButton;
    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private GameObject scoresPanel;

    private int currentMapNumber;
    private bool startGame;
    private List<RacePositions> listRacePositions;
    // Start is called before the first frame update
    void Start()
    {
        currentMapNumber = 0;

        
        
    }

    private void GetRaceFromFile()
    {
        listRacePositions = new List<RacePositions>();
        string path = "Assets/Resources/races.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path); 
        string values=reader.ReadToEnd();

        string[] valueSplit = values.Split('{','}');
        for (int i = 0; i < valueSplit.Length; i++)
        {
            if(i%2==0 && i!=0 && i != valueSplit.Length-1)
            {
                RacePositions race = JsonUtility.FromJson<RacePositions>("{"+valueSplit[i]+"}");
                listRacePositions.Add(race);
            }
        }
        reader.Close();

        Debug.Log(listRacePositions.Count);
    }

    private void UpdateMaps()
    {
        if(listRacePositions==null)
        {
            GetRaceFromFile();
        }
        for (int i = 2; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < 4; i++)
        {
            if (currentMapNumber + i < listRacePositions.Count)
            {
                GameObject map = Instantiate(Resources.Load("Map")) as GameObject;
                //GameObject map = Instantiate(Resources.Load("Map1")) as GameObject;
                map.transform.parent = this.transform;
                map.layer = 10;
                map.GetComponent<RectTransform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);
                map.GetComponent<RectTransform>().localPosition = new Vector3(i % 2 * 300 - 150, -((i / 2) * 120) + 60, 0);
                map.GetComponent<MapInfo>().UpdateStartAndTarget(listRacePositions[currentMapNumber + i]);

                Rect rect = map.GetComponent<RectTransform>().rect;
                GameObject button = Instantiate(buttonPrefab);
                button.transform.parent = this.transform;
                button.GetComponent<RectTransform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);
                button.GetComponent<RectTransform>().localPosition = new Vector3(i % 2 * 300 - 150, -((i / 2) * 120) + 60, 0);
                button.GetComponent<RectTransform>().rect.Set(rect.x, rect.y, rect.width, rect.height);

                int j = i;
                if (startGame)
                {
                    button.GetComponent<Button>().onClick.AddListener(() => { LoadScene(currentMapNumber + j); });
                }
                else
                {
                    button.GetComponent<Button>().onClick.AddListener(() => { LoadScoresMenu(currentMapNumber + j); });
                }

            }
        }

        if (currentMapNumber == 0)
        {
            previousButton.SetActive(false);
        }
        else
        {
            previousButton.SetActive(true);
        }

        if (currentMapNumber + 4 < listRacePositions.Count)
        {
            nextButton.SetActive(true);
        }
        else
        {
            nextButton.SetActive(false);
        }
    }

    public void SetMenuType(bool startGame)
    {
        this.startGame = startGame;
        UpdateMaps();
    }

    public void LoadScene(int level)
    {
        StaticClass.CrossSceneInformation = level;
        StaticClass.racePositions = listRacePositions[level];
        SceneManager.LoadScene(1);
        Debug.Log("Level " + level);
    }

    public void LoadScoresMenu(int level)
    {
        scoresPanel.GetComponent<ScoresPanel>().UpdateScoresPanel(level);
        gameObject.SetActive(false);
        scoresPanel.SetActive(true);
    }

    public void NextPage()
    {
        if (currentMapNumber + 4 < listRacePositions.Count) currentMapNumber += 4;
        UpdateMaps();
    }

    public void PreviousPage()
    {
        if (currentMapNumber > 0) currentMapNumber -= 4;
        UpdateMaps();
    }

    public int GetNbLevel()
    {
        return listRacePositions.Count;
    }
}
