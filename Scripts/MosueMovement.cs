using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;



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

    public static MosueMovement instace { get; private set; }
    private List<MouseRecordData> mouseMoventRecorded = new List<MouseRecordData>();
    private Vector2 originalPosition;
    private bool recordingMouse;
    private bool playingRecording;
    private float startTime;
    private string saveFile;

    private void Awake()
    {
        if (instace == null)
        {
            instace = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SaveData.Load();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Get out of jail
        {
            StopAllCoroutines();
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

    private int ReturnRandomNumber() //Mouse giggle return value. 
    {
        return UnityEngine.Random.Range(-3, 3);
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
    }
    private int TravelDistance()
    {
        return UnityEngine.Random.Range(-10, 10);
    }


    //UI Buttons ---------------------------------------------------------------------------
    public void GiggleStart()
    {
        StartCoroutine(MouseJiggle());
    }

    public void GiggleStop()
    {
        StopAllCoroutines();
        recordingMouse = false;
        Debug.Log("Stop Everything!");
    }
    public void RecordingStart()
    {
        mouseMoventRecorded.Clear();
        startTime = Time.time;
        recordingMouse = true;
        Debug.Log("Recroding");
    }
    public void StopRecrodingMouse()
    {
        recordingMouse = false;
        Debug.Log("We Saved");
        SaveData.Save();
    }
    public void PlayRecording()
    {
        playingRecording = true;
        StartCoroutine(PlayBack());
        Debug.Log("Playing");
    }



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
    }

    public void Load(ref SceneRecords data)
    {
        for (int i = data.recordingsOfMice.Length - 1; i >= 0; i--)
        {
            mouseMoventRecorded.Add(new MouseRecordData(data.recordingsOfMice[i].postion, data.recordingsOfMice[i].timestamp));
            Debug.Log(data.recordingsOfMice[i].timestamp);
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
}

[System.Serializable]
public struct RecordingsOfMouse
{
    public Vector3 postion;
    public float timestamp;
}