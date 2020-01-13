﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

public class LoadAudioFiles : MonoBehaviour
{

    static string path  = "./"; // Is equal to where you have your executable
    static string[] fileTypes = { "ogg", "wav" }; // Valid file types
 
    static FileInfo[] files;
    static AudioSource audioSource;
    static List<AudioClip> audioClips = new List<AudioClip>();
    static Text text;
    AudioClipCombine audioClipCombine;
    public AudioClip result;

    public static string GetAndroidExternalFilesDir()
    {
        string[] potentialDirectories = new string[]
        {
        "/storage/emulated/0",
        "/sdcard",
        "/storage",
        "/mnt/sdcard",
        "/storage/sdcard0",
        "/storage/sdcard1"
        };

        if (Application.platform == RuntimePlatform.Android)
        {
            for (int i = 0; i < potentialDirectories.Length; i++)
            {
                if (Directory.Exists(potentialDirectories[i]))
                {
                    return potentialDirectories[i];
                }
            }
        }
        return "";
    }

    //    public void GetAndroidExternalStoragePath()
    //  {
    //    try
    //  {
    //    AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
    //  path = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<string>("getAbsolutePath");
    //}
    //catch (Exception e)
    //{
    //Debug.Log(e.Message);
    //}
    //}


    public void Start()
    {
        audioClipCombine = gameObject.AddComponent<AudioClipCombine>();
        text = GetComponentInChildren<Text>();
        // If in editor the path is in Assets folder
        if (Application.isEditor) { 
            path = "/Assets";
            path = path.Substring(1);
            text.text = "";
            text.text = "Path: " + path;
        }
        else
        {
            //AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
            //path = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").ToString();
            //path = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<String>("getAbsolutePath");

            path = GetAndroidExternalFilesDir();
            path += "/Music";
            path = path.Substring(1);
            text.text = "";
            text.text = "Path: " + path;
        }
    }
    //musi byc ten sam sample rate i ilosc kanalow
    public void SaveSeparatelyButton()
    {
        // Set an AudioSource to this object
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

        // Find files in directory        
        GetFilesInDirectory();

        // Play a clip found in directory
        if (audioClips.Count > 0)
        {
            //audioSource.clip = audioClips[0];
            //audioSource.Play();
            result = audioClipCombine.Combine(audioClips[0], audioClips[1]);
            SavWav.Save("mySeperatelyfile", result);
        //audioSource.clip = result;
        }
    }
    //musi byc ten sam sample rate i ilosc kanalow
    public void SaveMixedButton()
    {
        // Set an AudioSource to this object
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

        // Find files in directory        
        GetFilesInDirectory();

        // Play a clip found in directory
        if (audioClips.Count > 0)
        {
            //audioSource.clip = audioClips[0];
            //audioSource.Play();
            result = audioClipCombine.MixAudioFiles(audioClips[0], audioClips[1]);
            SavWav.Save("myMixedFile", result);
            //audioSource.clip = result;
        }
    }

    public void GetFilesInDirectory()
    {
        DirectoryInfo info = new DirectoryInfo(path);
        files = info.GetFiles();
        foreach (FileInfo file in files)
        {
            string extension = Path.GetExtension(file.FullName);
            if (ValidType(extension))
                LoadFile(file.FullName);
                Debug.Log(file.FullName);
        }
    }

    public bool ValidType(string extension){
     foreach (string validExtension in fileTypes)
         if (extension.IndexOf(validExtension) > -1)
             return true;
     return false;
    }

    public void LoadFile(string path)
    {
        WWW www = new WWW("file://" + path);
        AudioClip clip = www.GetAudioClip(false);
        while (!clip.isReadyToPlay)
        { 

        }
        string[] parts = path.Split("\\"[0]);
        clip.name = parts[parts.Length - 1];
        audioClips.Add(clip);
    }

}

