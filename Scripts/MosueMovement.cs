using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;



public class MosueMovement : MonoBehaviour
{

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    private struct POINT
    {
        public int X;
        public int Y;
    }

    public static MosueMovement Instance { get; private set; }
    private List<MouseRecordData> mouseMoventRecorded = new List<MouseRecordData>();
    private Vector2 originalPosition;
    private bool recordingMouse;
    private bool playingRecording;
    private bool notFirstLoad;
    private float startTime;
    private float timeTillNextMovement;
    private string saveFile;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SaveData.Load();
    }
    private void Start()
    {
        if (notFirstLoad)
        {
            OpeningStartLogic();
            UIManager.Instance.DebugMessage("Program loaded with recorded information");
        }
        else UIManager.Instance.firstTimeLoadObject.gameObject.SetActive(true);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Get out of jail
        {
            StopAllCoroutines();
            UIManager.Instance.DebugMessage("Stopped All");
            UIManager.Instance.StopEverything();
        }

        if (recordingMouse)
        {
            Vector3 mousePos = Input.mousePosition;
            float timestamp = Time.time - startTime;
            mouseMoventRecorded.Add(new MouseRecordData(mousePos, timestamp));
        }
    }


    //Mouse Jiggle ------------------------------------------------------------------------
    private IEnumerator MouseJiggle()
    {
        while (true)
        {
            POINT point;
            GetCursorPos(out point);
            originalPosition = new Vector2(point.X, point.Y);
            SetCursorPos(point.X + ReturnRandomNumber(), point.Y + ReturnRandomNumber());
            yield return new WaitForSeconds(ReturnRandomNumber());
        }
    }

    private int ReturnRandomNumber() //Mouse jiggle and time recursion. 
    {
        int number = UnityEngine.Random.Range(-90, 90);
        UIManager.Instance.DebugMessage(number.ToString() + "s - Time till playing again");
        return number;
    }


    //Mouse Recorded movement --------------------------------------------------------------
    private IEnumerator PlayBack()
    {
        if (playingRecording)
        {
            
            POINT point;
            int worldPosX;
            int worldPoxY;
            int distance = TravelDistance();
            GetCursorPos(out point);
            float plabackStartTime = Time.time;
            foreach (MouseRecordData movement in mouseMoventRecorded)
            {
                while (Time.time - plabackStartTime < movement.timestamp)
                    yield return null;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(movement.postion);
                worldPosX = (int)Math.Round(worldPos.x);
                worldPoxY = (int)Math.Round(worldPos.y);
                SetCursorPos(point.X + worldPosX * distance, point.Y + worldPoxY * distance);
            }
        }
        playingRecording = false;
        UIManager.Instance.DebugMessage("Finished");
        yield return new WaitForSeconds(30);
        OpeningStartLogic();
    }
    private int TravelDistance()
    {
        return UnityEngine.Random.Range(-10, 10);
    }


    //Opening Logic ------------------------------------------------------------------------
    private void OpeningStartLogic()
    {
        if (!playingRecording)
        {
            StartCoroutine(TimerRecursion());
            UIManager.Instance.SwitchColorGreen();
            timeTillNextMovement = ReturnRandomNumber();
        }
    }

    private IEnumerator TimerRecursion()
    {
        yield return new WaitForSeconds(timeTillNextMovement);
        playingRecording = true;
        StartCoroutine(PlayBack());
    }

    //UI Buttons ---------------------------------------------------------------------------
    public void GiggleStart() // not doing anything left for doc's
    {
        StartCoroutine(MouseJiggle());
    }

    public void GiggleStop()
    {
        StopAllCoroutines();
        recordingMouse = false;
        UIManager.Instance.DebugMessage("Stop everything");

    }
    public void RecordingStart()
    {
        mouseMoventRecorded.Clear();
        startTime = Time.time;
        recordingMouse = true;
        Debug.Log("Recroding");
        UIManager.Instance.firstTimeLoadObject.SetActive(false);
    }
    public void StopRecrodingMouse()
    {
        notFirstLoad = true;
        recordingMouse = false;
        playingRecording = false;
        UIManager.Instance.DebugMessage("Programed saved");
        SaveData.Save();
    }
    public void PlayRecording()
    {
        playingRecording = false;
        OpeningStartLogic();
        UIManager.Instance.DebugMessage("Program started again");
    }


    //Save and Load ----------------------------------------------------------------------------------------------------------------
    public void Save(ref SceneRecords data)
    {
        List<RecordingsOfMouse> sceneRecords = new List<RecordingsOfMouse>();
        for (int i = mouseMoventRecorded.Count - 1; i >= 0; i--)
        {
            if (mouseMoventRecorded[i] != null)
            {
                RecordingsOfMouse saveData = new RecordingsOfMouse
                {
                    postion = mouseMoventRecorded[i].postion,
                    timestamp = mouseMoventRecorded[i].timestamp
                };

                sceneRecords.Add(saveData);
            }
        }
        data.recordingsOfMice = sceneRecords.ToArray();
        data.firstTimeLoaded = notFirstLoad;
    }

    public void Load(ref SceneRecords data)
    {
        if (data.recordingsOfMice != null && data.recordingsOfMice.Length > 0) 
        {
            for (int i = data.recordingsOfMice.Length - 1; i >= 0; i--)
            {
                mouseMoventRecorded.Add(new MouseRecordData(data.recordingsOfMice[i].postion, data.recordingsOfMice[i].timestamp));
            }
            notFirstLoad = data.firstTimeLoaded;
            if (notFirstLoad)
            {
                UIManager.Instance.firstTimeLoadObject.SetActive(false);
            }
        }
    }

}
[System.Serializable]
public class MouseRecordData
{
    public Vector3 postion;
    public float timestamp;

    public MouseRecordData(Vector3 pos, float time)
    {
        postion = pos;
        timestamp = time;
    }
}
[System.Serializable]
public struct SceneRecords
{
    public RecordingsOfMouse[] recordingsOfMice;
    public bool firstTimeLoaded;
}

[System.Serializable]
public struct RecordingsOfMouse
{
    public Vector3 postion;
    public float timestamp;
}