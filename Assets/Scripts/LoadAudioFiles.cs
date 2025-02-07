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
    static string[] fileTypes = {"ogg", "wav" }; // Valid file types
 
    static FileInfo[] files;
    public AudioSource audioSource;
    public Text text, fileTitle, fileTime, volumeText, panText, pitchText, reverbText;
    public Dropdown dropdown1, dropdown2, dropdown3;
    public AudioClipCombine audioClipCombine;
    public AudioClip metroClip;
    public AudioClip accentClip;

    private List<AudioClip> audioClips;
    private List<AudioClip> selectedAudioClips;

    private bool addMetronome = false;
    private int frequency = 44100;
    private int channels = 2;
    private string firstName;
    private string secondName;
    private float volume, pan, pitch, reverb;
    private int fullLenght, playTime, seconds, minutes;
    private int metronomeAccents = 4;
    private int metronomeBPM = 120;

    public void Start()
    {
        audioClips = new List<AudioClip>();
        selectedAudioClips = new List<AudioClip>();
        /*
        audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        audioSource = GetComponent<AudioSource>();
        */

        //audioClipCombine = gameObject.AddComponent<AudioClipCombine>();


        // If in editor the path is in Assets folder
        if (Application.isEditor) { 
            path = "/Assets";
            path = path.Substring(1);
            text.text = "";
            text.text = "Path: " + path;
        }
        else 
        {
            //path = Application.persistentDataPath;
            
            path = GetAndroidExternalStoragePath();
            //if (path == null) {
                //path = GetAndroidExternalFilesDir();    
                //AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
                //path = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").ToString();
                //path = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<String>("getAbsolutePath");
            //}
            //pathStart = "/mnt/sdcard/SongFiles";


            if (!Directory.Exists(path + "/FilesInz"))
            {
                Directory.CreateDirectory(path + "/FilesInz");
            }
            path += "/FilesInz";
            path = path.Substring(1);
            text.text = "";
            text.text = "Path: " + path;
        }

        dropdown1.ClearOptions();
        dropdown2.ClearOptions();
        dropdown3.ClearOptions();

        setDefaultAudioSourceParams();
        GetFilesInDirectory();
    }

    public void initializeVars()
    {
        
    }

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

    public string GetAndroidExternalStoragePath()
    {
        string tempPath;
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
            tempPath = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<string>("getAbsolutePath");
        }
        catch (Exception e)
        {
            tempPath = null;
            Debug.Log(e.Message);
        }
        return tempPath;
    }

    public void SaveMultipleSeparately(InputField fileNameInput)
    {
        String fileName = fileNameInput.text;
        if (fileName != null && fileName != "")
        { 

            setSelectedAudioClips(dropdown1, dropdown2);

            if (selectedAudioClips.Count > 1)
            {
                Debug.Log("SelectedAudioClips Capacity: " + selectedAudioClips.Capacity);
                AudioClip result = audioClipCombine.CombineMany(selectedAudioClips[0], selectedAudioClips[1]);
                SavWav.Save(fileName, result);
                selectedAudioClips.Clear();
                GetFilesInDirectory();
            }
            else
            {
                text.text = "Choose files to concatenate";
            }
        }
    }

    //musi byc ten sam sample rate i ilosc kanalow
    public void SaveSeparatelyButton(InputField fileNameInput)
    {
        
        String fileName = fileNameInput.text;
        if (fileName != null && fileName != "")
        {

            setSelectedAudioClips(dropdown1, dropdown2);
            if (selectedAudioClips.Count > 1)
            {
                Debug.Log("SelectedAudioClips: " + selectedAudioClips.Capacity);
                AudioClip result = audioClipCombine.Combine(selectedAudioClips[0], selectedAudioClips[1], this.metroClip, this.accentClip, this.channels, this.frequency, this.addMetronome, this.metronomeBPM, this.metronomeAccents);
                SavWav.Save(fileName, result);
                text.text = "File is saved as:" + fileName + ".wav";
                selectedAudioClips.Clear();
                GetFilesInDirectory();
            }
            else
            {
                text.text = "Choose files to concatenate";
            }
        }
        else
        {
            text.text = "Your File need to have a name";
        }
    }

    //musi byc ten sam sample rate i ilosc kanalow
    public void SaveMixedButton(InputField fileNameInput)
    {
        String fileName = fileNameInput.text;
        
        if (fileName != null && fileName !="")
        {
            setSelectedAudioClips(dropdown1, dropdown2);

            if (selectedAudioClips.Count > 1)
            {
                Debug.Log("SelectedAudioClips: " + selectedAudioClips.Capacity);
                AudioClip result = audioClipCombine.MixAudioFiles(selectedAudioClips[0], selectedAudioClips[1], this.metroClip, this.accentClip, this.channels, this.frequency, this.addMetronome, this.metronomeBPM, this.metronomeAccents);
                Debug.Log("Created result");
                SavWav.Save(fileName, result);
                text.text = "File is saved as: " + fileName + ".wav";
                selectedAudioClips.Clear();
                GetFilesInDirectory();
            }
            else
            {
                text.text = "Choose files to mix";
            }
        }
        else
        {
            text.text = "Your File need to have a name";
        }
    }

    private void setDefaultAudioSourceParams()
    {
        audioSource.volume = 1;
        audioSource.panStereo = 0;
        audioSource.pitch = 1;
        audioSource.reverbZoneMix = 0;
    }

    public void getVolumeValue(Slider slider)
    {
        volume = slider.value;
        volumeText.text = volume.ToString("F2");
        setVolume();
    }

    private void setVolume()
    {
        audioSource.volume = volume;
    }

    public void getPitchValue(Slider slider)
    {
        pitch = slider.value;
        pitchText.text = pitch.ToString("F2");
        SetPitch();
    }

    private void SetPitch()
    {
        audioSource.pitch = pitch;
    }

    public void getReverbValue(Slider slider)
    {
        reverb = slider.value;
        reverbText.text = reverb.ToString("F2");
        SetReverb();
    }

    private void SetReverb()
    {
        audioSource.reverbZoneMix = reverb;
    }

    public void getPanoramaValue(Slider slider)
    {
        pan = slider.value;
        panText.text = pan.ToString("F2");
        SetPan();
    }

    private void SetPan()
    {
        audioSource.panStereo = pan;  
    }

    public void addMetronomeButton()
    {
        this.addMetronome = !this.addMetronome;
        if (addMetronome)
        {
            Debug.Log(this.addMetronome);
            this.text.text = "Metronome will be added";
        }
        else
        {
            Debug.Log(this.addMetronome);
            this.text.text = "Metronome will not be added";
        }
    }

    public void SetTime(AudioClip audioClip, int time)
    {
        audioSource.clip = audioClip;
        if (time >= 0 && time <= audioClip.length)
        {
            audioSource.timeSamples = time;
        }
    }

    public void setTempo(InputField BPMInput)
    {
        if (BPMInput.text != "")
        {
            int tempBPM = int.Parse(BPMInput.text);
            if (tempBPM > 0 && tempBPM <= 200)
            {
                this.metronomeBPM = tempBPM;
                text.text = "You set BPM to: " + this.metronomeBPM;
                Debug.Log(this.metronomeBPM);
            }
            else
            {
                text.text = "You can set BPM from 1 to 200";
            }
        }
    }

    public void setAccents(InputField AccentInput)
    {
        if (AccentInput.text != "")
        {
            int tempAccent = int.Parse(AccentInput.text);
            if (tempAccent >= 0 && tempAccent <= 8)
            {
                this.metronomeAccents = tempAccent;
                text.text = "You set Accents to: " + this.metronomeAccents;
                Debug.Log(this.metronomeAccents);
            }
            else
            {
                text.text = "You can set Accents from 0 to 8";
            }
        }
    }

    public void setChannels(InputField channelsInput)
    {
        if(channelsInput.text != "") { 
            int tempChannels = int.Parse(channelsInput.text);
            if (tempChannels > 0 && tempChannels <= 2)
            {
                this.channels = tempChannels;
                text.text = "You set channels to: " + this.channels;
            }   
            else
            {
                text.text = "You can set 1 or 2 channels";            
            }
        }
    }

    public void setFrequency(InputField frequencyInput)
    {
        if(frequencyInput.text != "") { 
            int tempFrequency = int.Parse(frequencyInput.text);
            if (tempFrequency > 8000 && tempFrequency <= 192000)
            {
                this.frequency = tempFrequency;
                text.text = "You set frequency to: " + this.frequency;
            }
            else
            {
                text.text = "You can set frequency from 8000 to 192000 Hz";
            }
        }
    }
    
    public void loadItemsToDropdown(String fileName)
    {
       
        dropdown1.options.Add(new Dropdown.OptionData() { text = fileName });
        dropdown2.options.Add(new Dropdown.OptionData() { text = fileName });
        dropdown3.options.Add(new Dropdown.OptionData() { text = fileName });

        dropdown1.RefreshShownValue();
        dropdown2.RefreshShownValue();
        dropdown3.RefreshShownValue();
        
    }

    public void setSelectedAudioToPlay(Dropdown dropdown3)
    {
        String clipName = dropdown3.options[dropdown3.value].text;
        foreach (AudioClip audioClip in audioClips)
        {
            if (audioClip.name == clipName)
            {
                audioSource.clip = audioClip;
                break;
            }
        }
    }

    public void ShowTitle()
    {
        fileTitle.text = audioSource.clip.name;
        fullLenght = (int)audioSource.clip.length;
    }

    public void ShowTime()
    {
        seconds = playTime % 60;
        minutes = (playTime / 60) % 60;
        fileTime.text = minutes + ":" + seconds.ToString("D2") + "/" + ((fullLenght / 60) % 60) + ":" + (fullLenght % 60).ToString("D2");
    }

    public void PlayMusic()
    {
        
            if(audioSource.clip == null)
            {
                audioSource.clip = audioClips[0];
            }

            if (audioSource.isPlaying)
            {
                return;
            }
            else
            {
                setSelectedAudioToPlay(dropdown3);
                fullLenght = (int)audioSource.clip.length;
                fileTitle.text = audioSource.clip.name;
                audioSource.Play();
                StartCoroutine(WaitForMusicEnd());
            }
    }

    IEnumerator WaitForMusicEnd()
    {
        while (audioSource.isPlaying)
        {
            playTime = (int)audioSource.time;
            ShowTime();
            yield return null;
        }
    }

    public void StopMusic()
    {
        StopCoroutine("WaitForMusicEnd");
        fileTime.text = "00:00/00:00";
        audioSource.Stop();
    }

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
            StartCoroutine(WaitForMusicEnd());
        }
    }

    public void MuteSound()
    {
        if(audioSource.mute == false) { 
            audioSource.mute = true;
        }
        else
        {
            audioSource.mute = false;
        }
    }

    public void setSelectedAudioClips(Dropdown dropdown1, Dropdown dropdown2)
    { 

        firstName = dropdown1.options[dropdown1.value].text;
        secondName = dropdown2.options[dropdown2.value].text;

        Debug.Log(firstName);
        Debug.Log(secondName);
        
        int i = 0;
        foreach(AudioClip audioClip in audioClips) {
            
            if (audioClip.name == firstName)
            {
                if (i==0)
                {
                    selectedAudioClips.Add(audioClip);
                    Debug.Log("1 added to selected: " + selectedAudioClips[0]);
                    i++;
                }
            }
        }
        i = 0;
        foreach (AudioClip audioClip in audioClips)
        {
            if (selectedAudioClips[0].name != audioClip.name) { 
                if (audioClip.name == secondName)
                {
                    if (i == 0)
                    {
                        selectedAudioClips.Add(audioClip);        
                        Debug.Log("2 added to selected: " + selectedAudioClips[1]);
                        i++;
                    }
                }
            }
            else
            {
                text.text = "The second file to mix can't be same file like first";
            }
        }
    }

    public void GetFilesInDirectory()
    {
        dropdown1.ClearOptions();
        dropdown2.ClearOptions();
        dropdown3.ClearOptions();

        DirectoryInfo info = new DirectoryInfo(path);
        files = info.GetFiles();
        foreach (FileInfo file in files)
        {
            string extension = Path.GetExtension(file.FullName);
            if (ValidType(extension)) {
                loadItemsToDropdown(file.Name);
                LoadFile(file.FullName, file.Name);                
            }
        }
    }

    public bool ValidType(string extension){
     foreach (string validExtension in fileTypes)
         if (extension.IndexOf(validExtension) > -1)
             return true;
     return false;
    }

    public void LoadFile(string path, string fileName)
    {
        WWW www = new WWW("file://" + path);        
        AudioClip clip = www.GetAudioClip(false);
        while (clip.loadState != AudioDataLoadState.Loaded)
        {
            
        }
        if (Application.isEditor) { 
            string[] parts = path.Split("\\"[0]);
            clip.name = parts[parts.Length - 1];
        }
        else
        {
            clip.name = fileName;
        }
        
        audioClips.Add(clip);
    }

}


