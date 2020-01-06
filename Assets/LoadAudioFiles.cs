using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadAudioFiles : MonoBehaviour
{

    static string path  = "./"; // Is equal to where you have your executable
    static string[] fileTypes = { "ogg", "wav" }; // Valid file types
 
    static FileInfo[] files;
    static AudioSource audioSource;
    static List<AudioClip> audioClips = new List<AudioClip>();
    static Text text;

    public void Start()
    {
        // If in editor the path is in Assets folder
        if (Application.isEditor)
            path = "Assets/";

        text = GetComponentInChildren<Text>();
        text.text = "";
        // Set an AudioSource to this object
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

        // Find files in directory        
        GetFilesInDirectory();

        // Play a clip found in directory
        if (audioClips.Count > 0)
        {
            audioSource.clip = audioClips[0];
            audioSource.Play();
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
            text.text += file.FullName + ' '; 
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

