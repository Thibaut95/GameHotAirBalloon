using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




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
    // Start is called before the first frame update
    void Start()
    {
        currentMapNumber = 0;
    }

    private void UpdateMaps()
    {
        for (int i = 2; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < 4; i++)
        {
            if (currentMapNumber + i < mapNumber)
            {
                GameObject map = Instantiate(Resources.Load("Map"+(currentMapNumber+i))) as GameObject;
                //GameObject map = Instantiate(Resources.Load("Map1")) as GameObject;
                map.transform.parent = this.transform;
                map.layer = 10;
                map.GetComponent<RectTransform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);
                map.GetComponent<RectTransform>().localPosition = new Vector3(i % 2 * 300 - 150, -((i / 2) * 120) + 60, 0);

                Rect rect =  map.GetComponent<RectTransform>().rect;
                GameObject button = Instantiate(buttonPrefab);
                button.transform.parent=this.transform;
                button.GetComponent<RectTransform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);
                button.GetComponent<RectTransform>().localPosition = new Vector3(i % 2 * 300 - 150, -((i / 2) * 120) + 60, 0);
                button.GetComponent<RectTransform>().rect.Set(rect.x,rect.y,rect.width,rect.height);
                
                int j = i;
                if(startGame)
                {
                    button.GetComponent<Button>().onClick.AddListener(() => {LoadScene(currentMapNumber+j); });
                }
                else
                {
                    button.GetComponent<Button>().onClick.AddListener(() => {LoadScoresMenu(currentMapNumber+j); });
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

        if (currentMapNumber + 4 < mapNumber)
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
        this.startGame=startGame;
        UpdateMaps();
    }

    public void LoadScene(int level)
    {
        StaticClass.CrossSceneInformation = level;
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
        if (currentMapNumber + 4 < mapNumber) currentMapNumber += 4;
        UpdateMaps();
    }

    public void PreviousPage()
    {
        if (currentMapNumber > 0) currentMapNumber -= 4;
        UpdateMaps();
    }

}
