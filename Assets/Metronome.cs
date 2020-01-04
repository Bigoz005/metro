using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Metronome : MonoBehaviour
{
    public AudioSource tick;
    public AudioSource accentTick;

    private SpriteRenderer background;
    private Text textNumber;
    private Color backgroundColor;
    private Color textNumberColor;
    private double bpm = 0.0F;
    private int barLenght = 0;
    private int accent = 0;
    private int i = 1; 
    private double nextTick = 0.0F; // The next tick in dspTime
    private double sampleRate = 0.0F;
    private bool ticked = false;
    private bool playing = false;
    private bool vibrationsOn = true;

    public void setVibrationsOnOff()
    {
        if (vibrationsOn)
        {
            vibrationsOn = false;
        }else
        {
            vibrationsOn = true;
        }
    }

    public void startMetronome()
    {
        if (!playing)
        {
            playing = true;
            this.Start();
        }else
        {
            playing = false;
            this.textNumber.color = textNumberColor;
            i = 1;
            this.textNumber.text = i.ToString();
        }
    }

    public void setTempo(InputField inputBpm)
    {
        if(inputBpm.text != null && inputBpm.text != "") { 
        double tempBpm = double.Parse(inputBpm.text);
        
            if (tempBpm > 500)
            {
                tempBpm = 500;
            }

            if (tempBpm < 0)
            {
                tempBpm = 0;
            }

            this.bpm = tempBpm;
        }
        else
        {
            this.bpm = 0;
        }
    }

    public void setBarLenght(InputField inputBarLenght)
    {
        if (inputBarLenght.text != null && inputBarLenght.text != "") { 
            int tempBarLenght = int.Parse(inputBarLenght.text);

            if (tempBarLenght > 32)
            {
                tempBarLenght = 32;
            }

            if (tempBarLenght <= 0)
            {
                tempBarLenght = 1;
            }

            this.barLenght = tempBarLenght;
            i = 1;
        }
        else
        {
            this.barLenght = 1;
        }
    }

    public void setAccent(InputField inputAccent)
    {
        if(inputAccent.text != null && inputAccent.text != "") { 
            int tempAccent = int.Parse(inputAccent.text);

            if (tempAccent > 32)
            {
                tempAccent = 32;
            }

            if (tempAccent < 0)
            {
                tempAccent = 0;
            }

            this.accent = tempAccent;
            i = 1;
        }
        else
        {
            this.accent = 0;
        }
    }

    void Start()
    {
        this.background = GetComponentInChildren<SpriteRenderer>();
        this.backgroundColor = background.color;
        this.textNumber = GameObject.Find("Number").GetComponent<Text>();
        this.textNumberColor = textNumber.color;
        if (playing) { 
        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;

        nextTick = startTick + (60.0 / bpm);
        }
    }

    void LateUpdate()
    {
        this.background.color = backgroundColor;
        if (playing && !ticked && nextTick >= AudioSettings.dspTime)
        {             
            ticked = true;
            BroadcastMessage("OnTick");
        }
    }

    // Just an example OnTick here
    void OnTick()
    {
        this.textNumber.text = i.ToString();
        if (i == barLenght)
        {
            if (i == accent)
            {
                Debug.Log("Accent");
                accentTick.Play();
                this.background.color = new Color(1, 0, 0, 1);
                this.textNumber.color = new Color(0, 0, 1, 1);
                if (vibrationsOn) { 
                    Vibration.CreateOneShot(50, 5);
                }
                i = 1;
            }
            else
            { 
                Debug.Log("Tick");
                tick.Play();
                this.background.color = new Color(0, 1, 0, 1);
                this.textNumber.color = new Color(1, 0, 0, 1);
                i = 1;
            }
        }
        else
        {
            if (i == accent)
            {
                Debug.Log("Accent");
                accentTick.Play();
                this.background.color = new Color(1, 0, 0, 1);
                this.textNumber.color = new Color(0, 0, 1, 1);
                if (vibrationsOn)
                {
                    Vibration.CreateOneShot(50, 5);
                }
                i++;
            }
            else
            {
                Debug.Log("Tick");
                tick.Play();
                this.background.color = new Color(0, 1, 0, 1);
                this.textNumber.color = new Color(1, 0, 0, 1);
                i++;
            }
        }

    }

    void FixedUpdate()
    {
        double timePerTick = 60.0f / bpm;
        double dspTime = AudioSettings.dspTime;

        while (dspTime >= nextTick)
        {
            ticked = false;
            nextTick += timePerTick;
        }

    }
}