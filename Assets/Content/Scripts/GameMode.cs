using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class GameMode : MonoBehaviour
{

    public ArmyCollection[] Armies;

    public bool constantRespawning = false;
    public uint startingSoldiers = 50;
    public bool dumpFPS = false;
    public float minAndMaxFpsInterval = 2.0f;
    public float autoPauseInterval = 10.0f;

    [SerializeField]
    int[] teamScores;

    float autoPauseTime = 0.0f;
    bool paused = false;
    bool respawning = false;
    bool benchmarkMode = true;
    float originalMaximumDeltaTime = 0.0f;
    float avgDeltaTime = 0.0f;

    float deltaTimeSum = 0.0f;
    int deltaTimeSamples = 0;
    float lastTimestampForAvg = 0.0f;
    public float avgDeltaTimeInterval = 10.0f;

    SortedList<float, float> deltaTimeList = new SortedList<float, float>();

    UI ui;

    // File log.
    private string filePath = "";
    private string[] fileLines;
    private int frameNo = 0;

    private void Awake()
    {
        Physics.autoSimulation = false;

        int prefBenchmarkMode = PlayerPrefs.GetInt("benchmarkMode", -1);
        if (prefBenchmarkMode != -1)
        {
            benchmarkMode = (prefBenchmarkMode != 0);
        }

        originalMaximumDeltaTime = Time.maximumDeltaTime;
        if (benchmarkMode)
        {
            Time.maximumDeltaTime = Time.fixedDeltaTime;
        }
    }

    // Use this for initialization
    void Start()
    {
        uint maxTeamNumber = 0;
        foreach (SoldierSpawner soldierSpawner in FindObjectsOfType<SoldierSpawner>())
        {
            soldierSpawner.respawn = constantRespawning;
            soldierSpawner.soldierCount = startingSoldiers;

            if (Armies.Contains(soldierSpawner.team))
            {
                maxTeamNumber++;
            }
        }

        ui = FindObjectOfType<UI>();
        if (ui)
        {
            // Make the checkbox reflect the initial state
            ui.SetBenchmarkModeToggle(benchmarkMode);
        }

        if (autoPauseInterval > 0.0f)
        {
            autoPauseTime = Time.unscaledTime + autoPauseInterval;
        }
    }

    private void OnDestroy()
    {
        // Reset the maximum delta time parameter
        Time.maximumDeltaTime = originalMaximumDeltaTime;
    }

    void Update()
    {
        // Auto-pause
        if (!paused)
        {
            if (autoPauseTime > 0.0f && Time.unscaledTime > autoPauseTime)
            {
                paused = true;
                Time.timeScale = 0;
                QualitySettings.vSyncCount = 4;
                if (ui)
                {
                    ui.gameObject.SetActive(true);
                    ui.SetPause(true);
                }
            }
        }

        // Unpause on tap
        if (paused)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    paused = false;
                    Time.timeScale = 1;
                    QualitySettings.vSyncCount = 1;
                    if (ui)
                    {
                        ui.SetPause(false);
                    }

                    if (autoPauseInterval > 0.0f)
                    {
                        autoPauseTime = Time.unscaledTime + autoPauseInterval;
                    }
                }

                // Reset avg deltaTime
                deltaTimeSum = 0.0f;
                deltaTimeSamples = 0;
                lastTimestampForAvg = Time.unscaledTime;
            }

            return;
        }

        // Hide the UI on tap
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (ui)
                {
                    ui.gameObject.SetActive(!ui.gameObject.activeSelf);
                }
            }
        }

        float currentTime = Time.unscaledTime;

        List<float> removeList = new List<float>();
        foreach (KeyValuePair<float, float> dt in deltaTimeList)
        {
            // Remove old samples from the queue
            if (dt.Value < (currentTime - minAndMaxFpsInterval))
            {
                removeList.Add(dt.Key);
            }
        }

        // We can't iterate and remove on deltaTimeList in one go,
        // so we split the two operations
        foreach (float key in removeList)
        {
            deltaTimeList.Remove(key);
        }

        if (deltaTimeList.ContainsKey(Time.unscaledDeltaTime))
        {
            deltaTimeList.Remove(Time.unscaledDeltaTime);
        }

        deltaTimeList.Add(Time.unscaledDeltaTime, currentTime);
        IList<float> sortedDeltaTimes = deltaTimeList.Keys;

        if (sortedDeltaTimes.Count > 0)
        {
            ui.minDeltaTime = sortedDeltaTimes[0] * 1000;
            ui.maxDeltaTime = sortedDeltaTimes[sortedDeltaTimes.Count - 1] * 1000;
        }

        if (Time.unscaledTime - lastTimestampForAvg > avgDeltaTimeInterval)
        {
            avgDeltaTime = deltaTimeSum / deltaTimeSamples;
            deltaTimeSum = 0.0f;
            deltaTimeSamples = 0;
            lastTimestampForAvg = Time.unscaledTime;
        }

        deltaTimeSum += Time.unscaledDeltaTime;
        deltaTimeSamples += 1;

        float fps = 1.0f / Time.unscaledDeltaTime;
        float avgFps = 1.0f / avgDeltaTime;
        if (ui)
        {
            ui.SetFps(fps);
            if (avgDeltaTime == 0.0f)
            {
                ui.SetAvgFps("-");
            }
            else
            {
                ui.SetAvgFps(avgFps);
            }
        }


        frameNo++;
    }

    void FixedUpdate()
    {
        uint aliveTeams = 0;
        foreach (ArmyCollection a in Armies)
        {
            if (a.Count() > 0)
            {
                aliveTeams++;
            }
        }

        if (ui)
        {
            ui.SetTeamScores();
        }

        if (aliveTeams < 2 && !constantRespawning && !respawning)
        {
            respawning = true;
            StartCoroutine(StartRespawning(3));
        }
    }

    IEnumerator StartRespawning(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (SoldierSpawner soldierSpawner in FindObjectsOfType<SoldierSpawner>())
        {
            soldierSpawner.respawn = true;
        }

        StartCoroutine(StopRespawning(5));
    }

    IEnumerator StopRespawning(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (SoldierSpawner soldierSpawner in FindObjectsOfType<SoldierSpawner>())
        {
            soldierSpawner.respawn = false;
        }
        respawning = false;
    }

    public void SetBenchmarkMode(bool newState)
    {
        PlayerPrefs.SetInt("benchmarkMode", newState ? 1 : 0);
        benchmarkMode = newState;

        if (benchmarkMode)
        {
            Time.maximumDeltaTime = Time.fixedDeltaTime;
        }
        else
        {
            Time.maximumDeltaTime = originalMaximumDeltaTime;
        }
    }
}
