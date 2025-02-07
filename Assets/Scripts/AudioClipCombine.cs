﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipCombine : MonoBehaviour
{
    public AudioClip CombineMany(params AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return null;

        int length = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
                continue;

            length += clips[i].samples * clips[i].channels;
        }

        float[] data = new float[length];
        length = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
                continue;

            float[] buffer = new float[clips[i].samples * clips[i].channels];
            clips[i].GetData(buffer, 0);
            //System.Buffer.BlockCopy(buffer, 0, data, length, buffer.Length);
            buffer.CopyTo(data, length);
            length += buffer.Length;
        }

        if (length == 0)
            return null;



        AudioClip result = AudioClip.Create("Combined", length, 2, 44100, false, false);
        result.SetData(data, 0);

        return result;
    }


    public AudioClip Combine(AudioClip clipA, AudioClip clipB, AudioClip clipC, AudioClip clipD, int channels, int frequency, bool addMetronome, int bpm, int accent)
    {
        if (clipA == null || clipB == null)
            return null;
 
        int length = 0;
        length += clipA.samples * clipA.channels;
        length += clipB.samples * clipB.channels;

        float[] data = new float[length];
        length = 0;

        float[] buffer1 = new float[clipA.samples * clipA.channels];
        clipA.GetData(buffer1, 0);
        buffer1.CopyTo(data, length);
        length += buffer1.Length;

        float[] buffer2 = new float[clipB.samples * clipB.channels];
        clipB.GetData(buffer2, 0);
        buffer2.CopyTo(data, length);
        length += buffer2.Length;

        double timeOfResult = clipB.length + clipA.length;

        if (length == 0)
            return null;

        if (addMetronome)
        {
            float[] floatSamplesC = new float[clipC.samples * clipC.channels];
            clipC.GetData(floatSamplesC, 0);

            float[] floatSamplesD = new float[clipD.samples * clipD.channels];
            clipD.GetData(floatSamplesD, 0);

            float[] floatSampleMetro = new float[length];// pusta tablica dla metronomu, dlugosc wyjsciowego audioclipu

            Debug.Log("bpm: " + bpm);
            double tick = 60.0 / bpm;// ile sekund trwa 1 tick
            Debug.Log("tick: " + tick);

            double ticksAmount;
            ticksAmount = (int)(timeOfResult / tick);// ilosc tickow mieszaca się w utworze

            Debug.Log("ticksAmount: " + ticksAmount);

            
            //Debug.Log("difference: " + difference);

            int j = 0;
            int addedTicksAmount = 0;
            int innerTicks = 1;
            int samplesInTime = clipC.frequency / (int)(1 / tick);

            AudioClip resultClip = AudioClip.Create("Temp", length, channels, frequency, false); 

            for (int g = 0; g < floatSampleMetro.Length; g++)
            {
                if (g % samplesInTime == 0)
                {
                    if (addedTicksAmount < ticksAmount)
                    {
                        if (innerTicks == 1)
                        {
                            resultClip.SetData(floatSamplesD, (int)(0 + addedTicksAmount * samplesInTime));
                            innerTicks++;
                            addedTicksAmount++;
                        }
                        else
                        {
                            resultClip.SetData(floatSamplesC, (int)(0 + addedTicksAmount * samplesInTime));
                            innerTicks++;
                            addedTicksAmount++;
                        }
                        if (innerTicks > accent)
                        {
                            innerTicks = 1;
                        }
                    }
                }
            }
            resultClip.GetData(floatSampleMetro, 0);

            Debug.Log("Ticks Amount: " + ticksAmount + " Added amount: " + addedTicksAmount);
            float[] mixedFloatArrayWithMetronome = MixAndClampFloatBuffers(data, floatSampleMetro); //polaczenie obu sampli z samplem metronomu

            AudioClip result = AudioClip.Create("Mixed", mixedFloatArrayWithMetronome.Length, channels, frequency, false);
            result.SetData(mixedFloatArrayWithMetronome, 0);
            return result;
        }
        else
        {
            AudioClip result = AudioClip.Create("Combined", length, channels, frequency, false, false);
            result.SetData(data, 0);
            return result;
        }
    }

    public AudioClip MixAudioFiles(AudioClip clipA, AudioClip clipB, AudioClip clipC, AudioClip clipD, int channels, int frequency, bool addMetronome, int bpm, int accent)
    {
        float[] floatSamplesA = new float[clipA.samples * clipA.channels];
        clipA.GetData(floatSamplesA, 0);
       
        float[] floatSamplesB = new float[clipB.samples * clipB.channels];
        clipB.GetData(floatSamplesB, 0);

        float timeOfResult;
        
        if (clipA.length > clipB.length){
            timeOfResult = clipA.length;
            
        }
        else
        {
            timeOfResult = clipB.length;

        }

        float[] mixedFloatArray = MixAndClampFloatBuffers(floatSamplesA, floatSamplesB);

        AudioClip resultClip = AudioClip.Create("Temp", mixedFloatArray.Length, channels, frequency, false); ;

        if (addMetronome)
        {
            float[] floatSamplesC = new float[clipC.samples * clipC.channels];
            clipC.GetData(floatSamplesC, 0);
            
            float[] floatSamplesD = new float[clipD.samples * clipD.channels];
            clipD.GetData(floatSamplesD, 0);

            float[] floatSampleMetro = new float[mixedFloatArray.Length];// pusta tablica dla metronomu, dlugosc wyjsciowego audioclipu

            Debug.Log("timeOfResult: " + timeOfResult);

            Debug.Log("bpm: " + bpm);
            double tick = 60.0 / bpm;// ile sekund trwa 1 tick
            Debug.Log("tick: " + tick);

            double ticksAmount;
            ticksAmount = (int)(timeOfResult / tick);// ilosc tickow mieszaca się w utworze

            Debug.Log("ticksAmount: " + ticksAmount);
            
            //Debug.Log("difference: " + difference);

            int j = 0;
            int addedTicksAmount = 0;
            int innerTicks = 1;
            int samplesInTime = clipC.frequency / (int)(1/tick);
            
            for (int g = 0; g < floatSampleMetro.Length; g++)
            {
                if (g % samplesInTime == 0)
                {
                    if (addedTicksAmount < ticksAmount)
                    {
                        if(innerTicks == 1){
                            resultClip.SetData(floatSamplesD, (int)(0 + addedTicksAmount * samplesInTime));
                            innerTicks++;
                            addedTicksAmount++;
                        }else{
                            resultClip.SetData(floatSamplesC, (int)(0 + addedTicksAmount * samplesInTime));
                            innerTicks++;
                            addedTicksAmount++;
                        }
                        if(innerTicks > accent){
                            innerTicks = 1;
                        }
                    }
                }
            }
            resultClip.GetData(floatSampleMetro, 0);
          
            Debug.Log("Ticks Amount: " + ticksAmount + " Added amount: " + addedTicksAmount);
            float[] mixedFloatArrayWithMetronome = MixAndClampFloatBuffers(mixedFloatArray, floatSampleMetro); //polaczenie obu sampli z samplem metronomu

            AudioClip result = AudioClip.Create("Mixed", mixedFloatArrayWithMetronome.Length, channels, frequency, false);
            result.SetData(mixedFloatArrayWithMetronome, 0); 
            return result;
        }
        else
        { 
            AudioClip result = AudioClip.Create("Mixed", mixedFloatArray.Length, channels, frequency, false);
            result.SetData(mixedFloatArray, 0);
            return result;
        }
    }

    private float[] MixAndClampFloatBuffers(float[] bufferA, float[] bufferB)
    {
        int minLength = Math.Min(bufferA.Length, bufferB.Length);
        int maxLength = Math.Max(bufferA.Length, bufferB.Length);

        float[] mixedFloatArray = new float[maxLength];

        for (int i = 0; i < minLength; i++)
        {
            mixedFloatArray[i] = ClampToValidRange((bufferA[i] + bufferB[i]) / 2);
        }

        if (minLength < maxLength)
        {
            if (bufferA.Length > bufferB.Length)
            {
                for (int i = minLength; i < maxLength; i++)
                {
                    mixedFloatArray[i] = ClampToValidRange(bufferA[i]);
                }
            }
            else if (bufferA.Length < bufferB.Length)
            {
                for (int i = minLength; i < maxLength; i++)
                {
                    mixedFloatArray[i] = ClampToValidRange(bufferB[i]);
                }
            }
        }

        return mixedFloatArray;
    }

    private float ClampToValidRange(float value)
    {
        float min = -1.0f;
        float max = 1.0f;
        return (value < min) ? min : (value > max) ? max : value;
    }

    /*
    private byte[] MixBuffers(byte[] bufferA, byte[] bufferB)
    {
        byte[] array = new byte[bufferA.Length];
        for (int i = 0; i < bufferA.Length; i++)
        {
            byte byteA = bufferA[i];
            byte byteB = bufferB[i];
            byte byteC = (byte)(((int)byteA + (int)byteB >> 1));
            array[i] = byteC;
        }
        return array;
    }
    */

    /*
    private byte[] floatToByte(float[] floatArray)
    {
        byte[] byteArray = new byte[floatArray.Length * 4];

        for (int i = 0; i < floatArray.Length; i++)
        {
            float currentFloat = floatArray[i];

            byte[] float2byte = BitConverter.GetBytes(currentFloat);
            Assert.IsTrue(float2byte.Length == 4);

            int offset = 4 * i;
            byteArray[0 + offset] = float2byte[0];
            byteArray[1 + offset] = float2byte[1];
            byteArray[2 + offset] = float2byte[2];
            byteArray[3 + offset] = float2byte[3];
        }
        return byteArray;
    }
    */

    /*
    private float[] byteToFloat(byte[] byteArray)
    {
        Assert.IsTrue(byteArray.Length % 4 == 0);
        float[] floatArray = new float[byteArray.Length / 4];

        for (int i = 0; i < floatArray.Length; i++)
        {
            int offset = 4 * i;
            byte[] byteArrayChunk = new byte[]
            {byteArray[0 + offset], byteArray[1 + offset], byteArray[2 + offset], byteArray[3 + offset]};
            floatArray[i] = BitConverter.ToSingle(byteArrayChunk, 0);
        }

        return floatArray;
    }
    */

}
