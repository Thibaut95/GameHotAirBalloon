using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TableScore : MonoBehaviour
{
    [SerializeField]
    private GameObject linePrefab;

    private List<Score> listScore;

    public void SortBy(string columnName)
    {
        switch (columnName)
        {
            case "fuel":
                listScore.Sort(
                    delegate (Score s1, Score s2)
                    {
                        return s1.fuel.CompareTo(s2.fuel)*-1;
                    }
                );
                break;
            case "time":
                listScore.Sort(
                    delegate (Score s1, Score s2)
                    {
                        return s1.time.CompareTo(s2.time);
                    }
                );
                break;
            case "distance":
                listScore.Sort(
                    delegate (Score s1, Score s2)
                    {
                        return s1.distance.CompareTo(s2.distance);
                    }
                );
                break;
            case "score":
                listScore.Sort(
                    delegate (Score s1, Score s2)
                    {
                        return s1.global.CompareTo(s2.global)*-1;
                    }
                );
                break;
        }

        UpdateTable();
    }

    public void SetScores(List<Score> list)
    {
        this.listScore = list;
        SortBy("score");
    }

    private void UpdateTable()
    {
        GameObject content = this.transform.Find("TableUserScore").Find("Scroll View").Find("Viewport").Find("Content").gameObject;
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
        RectTransform rect = content.GetComponent<RectTransform>();
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, listScore.Count * 30 + 50);

        int posY = (listScore.Count * 30) / 2 - 110;

        foreach (var item in listScore)
        {
            GameObject line = Instantiate(linePrefab, content.transform);

            for (int i = 0; i < 4; i++)
            {
                Vector3 pos = line.transform.GetChild(i).localPosition;
                pos.y += posY;
                line.transform.GetChild(i).localPosition = pos;
            }

            posY -= 30;

            line.transform.Find("TextFuel").GetComponent<Text>().text = string.Format("{0:0.00}", item.fuel);
            line.transform.Find("TextTime").GetComponent<Text>().text = string.Format("{0:0.00}", item.time);
            line.transform.Find("TextDistance").GetComponent<Text>().text = item.distance.ToString();
            line.transform.Find("TextScore").GetComponent<Text>().text = string.Format("{0:0.00}", item.global);
        }
    }
}
