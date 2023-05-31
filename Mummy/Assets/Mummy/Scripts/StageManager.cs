using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {

    public GameObject GoodItem;
    public GameObject BadItem;

    public int GoodItemCount = 30;
    public int BadItemCount = 20;

    public List<GameObject> GoodItemList = new List<GameObject>();
    public List<GameObject> BadItemList = new List<GameObject>();

    public void SetStageObject() {
        foreach (var obj in GoodItemList)
            Destroy(obj);

        foreach (var obj in BadItemList)
            Destroy(obj);

        GoodItemList.Clear();
        BadItemList.Clear();

        for (int i = 0; i < GoodItemCount; i++) {
            Vector3 pos = new Vector3(Random.Range(-23.0f, 23.0f), 0.05f, Random.Range(-23.0f, 23.0f));
            Quaternion rot = Quaternion.Euler(Vector3.up * Random.Range(0, 360f));

            GoodItemList.Add(Instantiate(GoodItem, transform.position + pos, rot, transform));
        }

        for (int i = 0; i < BadItemCount; i++) {
            Vector3 pos = new Vector3(Random.Range(-23.0f, 23.0f), 0.05f, Random.Range(-23.0f, 23.0f));
            Quaternion rot = Quaternion.Euler(Vector3.up * Random.Range(0, 360f));

            BadItemList.Add(Instantiate(BadItem, transform.position + pos, rot, transform));
        }
    }

}
