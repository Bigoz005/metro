using System.Collections;
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
    static AudioSource audioSource;
    static List<AudioClip> audioClips = new List<AudioClip>();
    static Text text, fileTitle, fileTime;
    private List<AudioClip> selectedAudioClips = new List<AudioClip>();
    AudioClipCombine audioClipCombine;
    private AudioClip result;
    private Dropdown dropdown1, dropdown2, dropdown3;
    private int frequency = 44100;
    private int channels = 2;
    private string firstName;
    private string secondName;
    private int fullLenght, playTime, seconds, minutes;

    public void Start()
    {
        dropdown1 = GameObject.Find("SelectFile1").GetComponent<Dropdown>();
        dropdown1.ClearOptions();
        dropdown2 = GameObject.Find("SelectFile2").GetComponent<Dropdown>();
        dropdown2.ClearOptions();
        dropdown3 = GameObject.Find("SelectFileToPlay").GetComponent<Dropdown>();
        dropdown3.ClearOptions();
        text = GetComponentInChildren<Text>();
        fileTitle = GameObject.Find("FileName").GetComponent<Text>();
        fileTime = GameObject.Find("FileTime").GetComponent<Text>();

        audioClipCombine = gameObject.AddComponent<AudioClipCombine>();
        
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null) { 
            audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        }

        // If in editor the path is in Assets folder
        if (Application.isEditor) { 
            path = "/Assets";
            path = path.Substring(1);
            text.text = "";
            text.text = "Path: " + path;
        }
        else
        {
            path = GetAndroidExternalStoragePath();
            if (path == null) {
                path = GetAndroidExternalFilesDir();
                //AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
                //path = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").ToString();
                //path = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<String>("getAbsolutePath");
            }
            if (!Directory.Exists(path + "/assetsFiles"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path + "/assetsFiles"));
            }
            path += "/assetsFiles";
            path = path.Substring(1);
            text.text = "";
            text.text = "Path: " + path;
        }

        GetFilesInDirectory();
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
                result = audioClipCombine.CombineMany(selectedAudioClips[0], selectedAudioClips[1]);
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
                result = audioClipCombine.Combine(selectedAudioClips[0], selectedAudioClips[1], this.channels, this.frequency);
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
            // Set an AudioSource to this object
            // audioSource = GetComponent<AudioSource>();
            //if (audioSource == null)
            // audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

            setSelectedAudioClips(dropdown1, dropdown2);

            if (selectedAudioClips.Count > 1)
            {
                // Play a clip found in directory
                //audioSource.clip = audioClips[0];
                //audioSource.Play();
                result = audioClipCombine.MixAudioFiles(selectedAudioClips[0], selectedAudioClips[1], this.channels, this.frequency);
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

    public void setVolume(AudioClip audioClip, float vol)
    {
        audioSource.clip = audioClip;
        if (vol >= 0 && vol <= 1)
        {
            audioSource.volume = vol;
        }
    }

    public void SetPitch(AudioClip audioClip, float pitch)
    {
        audioSource.clip = audioClip;
        if(pitch >= -3 && pitch <= 3) { 
            audioSource.pitch = pitch;
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

    public void SetReverb(AudioClip audioClip, float reverb)
    {
        audioSource.clip = audioClip;
        if (reverb >= 0 && reverb <= 1.1)
        {
            audioSource.reverbZoneMix = reverb;
        }
    }

    public void SetPan(AudioClip audioClip, float pan)
    {
        audioSource.clip = audioClip;
        if (pan >= -1 && pan <= 1)
        {
            audioSource.panStereo = pan;
        }
    }

    public void setChannels(InputField channelsInput)
    {
        if(channelsInput.text != "") { 
            int tempChannels = int.Parse(channelsInput.text);
            if (tempChannels > 0 && tempChannels != null && tempChannels <= 2)
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
            if (tempFrequency > 8000 && tempFrequency != null && tempFrequency <= 192000)
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
            Debug.Log(audioClip.name);
            if (audioClip.name == firstName)
            {
                if (i==0)
                {
                    selectedAudioClips.Add(audioClip);
                    Debug.Log("added to selected: " + selectedAudioClips[0]);
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
                        Debug.Log("added to selected: " + selectedAudioClips[1]);
                        i++;
                    }
                }
            }
        }
    }

    public void GetFilesInDirectory()
    {
        dropdown1.ClearOptions();
        dropdown2.ClearOptions();

        DirectoryInfo info = new DirectoryInfo(path);
        files = info.GetFiles();
        foreach (FileInfo file in files)
        {
            string extension = Path.GetExtension(file.FullName);
            if (ValidType(extension)) { 
                loadItemsToDropdown(file.Name);
                LoadFile(file.FullName);
                Debug.Log(file.FullName);
            }
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


