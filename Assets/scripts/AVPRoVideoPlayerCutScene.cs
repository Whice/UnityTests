using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerCutScene : MonoBehaviour
{
    [SerializeField] private string[] clips = new string[0];

    private MediaPlayer mediaPlayer;
    private void PlayerEvents(MediaPlayer mediaPlayer, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
    {
       if (eventType == MediaPlayerEvent.EventType.FinishedPlaying)
        {
            gameObject.SetActive(false);
            mediaPlayer.CloseVideo();
            float timeEnd = Time.time;
            Debug.LogError(timeEnd - timeStart);
        }
    }
    private void Awake()
    {
        mediaPlayer = GetComponent<MediaPlayer>();
        mediaPlayer.Events.AddListener(PlayerEvents);
    }

    private int currentIndex = 0;
    private float timeStart = 0;
    public void SetNextVideo()
    {
        gameObject.SetActive(true);
        ++currentIndex;
        if (currentIndex == clips.Length)
            currentIndex = 0;
        StartCoroutine(StartAnimation());
    }

    private IEnumerator StartAnimation()
    {
        yield return null;
        string currentClip = clips[currentIndex] + ".mp4";
        timeStart = Time.time;
        mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, currentClip);
    }
}
