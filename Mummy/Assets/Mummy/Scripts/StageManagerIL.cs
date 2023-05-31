using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManagerIL : MonoBehaviour {

    public enum HINT_COLOR {
        BLACK, BLUE, GREEN, RED
    }

    public HINT_COLOR hintColor = HINT_COLOR.BLACK;

    public Material[] hintMat;
    public string[] hintTag;

    private Renderer renderer;
    private int prevTag = -1;

    private void Start() {
        renderer = transform.Find("Hint").GetComponent<Renderer>();
    }

    public void InitStage() {
        int idx = 0;
        do {
            idx = Random.Range(0, hintMat.Length);
        } while (idx == prevTag);
        prevTag = idx;

        renderer.material = hintMat[idx];

        hintColor = (HINT_COLOR)idx;
    }
}
