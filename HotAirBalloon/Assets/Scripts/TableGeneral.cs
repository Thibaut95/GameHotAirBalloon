using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class TableGeneral : MonoBehaviour
{
    [SerializeField]
    private GameObject linePrefab;

    private List<List<Tuple<string, Score>>> listScore;
    private int index = 0;

    public void ChangeList()
    {
        index=this.transform.Find("Dropdown").GetComponent<Dropdown>().value;
        SortBy("score");
    }

    public void SortBy(string columnName)
    {
        switch (columnName)
        {
            case "fuel":
                listScore[index].Sort(
                    delegate (Tuple<string, Score> s1, Tuple<string, Score> s2)
                    {
                        return s1.Item2.fuel.CompareTo(s2.Item2.fuel)*-1;
                    }
                    );
                break;
            case "time":
                listScore[index].Sort(
                    delegate (Tuple<string, Score> s1, Tuple<string, Score> s2)
                    {
                        return s1.Item2.time.CompareTo(s2.Item2.time);
                    }
                    );
                break;
            case "distance":
                listScore[index].Sort(
                    delegate (Tuple<string, Score> s1, Tuple<string, Score> s2)
                    {
                        return s1.Item2.distance.CompareTo(s2.Item2.distance);
                    }
                    );
                break;
            case "score":
                listScore[index].Sort(
                    delegate (Tuple<string, Score> s1, Tuple<string, Score> s2)
                    {
                        return s1.Item2.global.CompareTo(s2.Item2.global)*-1;
                    }
                    );
                break;
        }
        UpdateTable();
    }

    public void SetScores(List<List<Tuple<string, Score>>> list)
    {
        this.listScore = list;
        SortBy("score");
    }

    private void UpdateTable()
    {
        GameObject content = this.transform.Find("TableGeneralScore").Find("Scroll View").Find("Viewport").Find("Content").gameObject;
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
        RectTransform rect = content.GetComponent<RectTransform>();
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, listScore.Count * 30 + 50);

        int posY = (listScore.Count * 30) / 2 - 110;


        foreach (var item in listScore[index])
        {
            GameObject line = Instantiate(linePrefab, content.transform);

            for (int i = 0; i < 5; i++)
            {
                Vector3 pos = line.transform.GetChild(i).localPosition;
                pos.y += posY;
                line.transform.GetChild(i).localPosition = pos;
            }

            posY -= 30;

            line.transform.Find("TextUserName").GetComponent<Text>().text = item.Item1;
            line.transform.Find("TextFuel").GetComponent<Text>().text = string.Format("{0:0.00}", item.Item2.fuel);
            line.transform.Find("TextTime").GetComponent<Text>().text = string.Format("{0:0.00}", item.Item2.time);
            line.transform.Find("TextDistance").GetComponent<Text>().text = item.Item2.distance.ToString();
            line.transform.Find("TextScore").GetComponent<Text>().text = string.Format("{0:0}", item.Item2.global);
        }
    }
}
