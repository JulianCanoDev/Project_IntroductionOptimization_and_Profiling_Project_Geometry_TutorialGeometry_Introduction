using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public string[] scoreStrings =
    {
        "Green: ",
        "Red: ",
        "Yellow: ",
        "Purple: "
    };

    [SerializeField]
    Text[] scoreTexts;

    [SerializeField]
    Text avgFpsText;

    [SerializeField]
    Text fpsText;

    [SerializeField]
    Text deltaTimeText;

    [SerializeField]
    Toggle benchmarkModeToggle;

    [SerializeField]
    RawImage pause;

    public ArmyCollection[] Armies;

    public float minDeltaTime = Mathf.NegativeInfinity;
    public float maxDeltaTime = Mathf.Infinity;

    void Awake()
    {
        Transform canvas = transform.Find("Canvas");
        if (!canvas)
        {
            return;
        }

        Transform panel = canvas.Find("Panel");
        if (!panel)
        {
            return;
        }

        Transform scoreTransform = panel.Find("ScoreTexts");
        if (scoreTransform)
        {
            scoreTexts = scoreTransform.gameObject.GetComponentsInChildren<Text>();
        }

        Transform fpsTransform = panel.Find("FPSText");
        if (fpsTransform)
        {
            fpsText = fpsTransform.gameObject.GetComponent<Text>();
        }

        Transform avgFpsTransform = panel.Find("AvgFPSText");
        if (avgFpsTransform)
        {
            avgFpsText = avgFpsTransform.gameObject.GetComponent<Text>();
        }

        Transform deltaTimeTransform = panel.Find("DeltaTimeText");
        if (deltaTimeTransform)
        {
            deltaTimeText = deltaTimeTransform.gameObject.GetComponent<Text>();
        }

        Transform benchmarkToggleTransform = panel.Find("BenchmarkModeToggle");
        if (benchmarkToggleTransform)
        {
            benchmarkModeToggle = benchmarkToggleTransform.gameObject.GetComponent<Toggle>();
        }

        Transform pauseTransform = canvas.Find("Pause");
        if (pauseTransform)
        {
            pause = pauseTransform.gameObject.GetComponent<RawImage>();
        }

        GameMode gm = FindObjectOfType<GameMode>();
        if (gm && benchmarkModeToggle)
        {
            benchmarkModeToggle.onValueChanged.AddListener(delegate {
                if (gm)
                {
                    gm.SetBenchmarkMode(benchmarkModeToggle.isOn);
                }
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (deltaTimeText)
        {
            deltaTimeText.text = "ms: " + minDeltaTime.ToString("f2") + " to " + maxDeltaTime.ToString("f2");
        }
    }

    public void SetPause(bool active)
    {
        if (pause)
        {
            pause.gameObject.SetActive(active);
        }
    }

    public void SetFps(float fps)
    {
        if (fpsText)
        {
            string fpsStr = fps.ToString("f2");
            fpsText.text = "FPS: " + fpsStr;
        }
    }

    public void SetAvgFps(float fps)
    {
        if (avgFpsText)
        {
            string fpsStr = fps.ToString("f2");
            avgFpsText.text = "Avg FPS: " + fpsStr;
        }
    }
    public void SetAvgFps(string text)
    {
        if (avgFpsText)
        {
            avgFpsText.text = "Avg FPS: " + text;
        }
    }

    public void SetTeamScores()
    {
        int SoldierCount = 0;
        foreach (ArmyCollection i in Armies)
        {
            SoldierCount += i.Count();
        }

        scoreTexts[0].text = "Soldiers: " + SoldierCount.ToString();
    }

    public void SetBenchmarkModeToggle(bool newState)
    {
        if (benchmarkModeToggle)
        {
            benchmarkModeToggle.isOn = newState;
        }
    }
}
