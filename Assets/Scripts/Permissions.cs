using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;


public class Permissions : MonoBehaviour
{
    void Start()
    {
        Permission.RequestUserPermission(Permission.ExternalStorageRead);
        Permission.RequestUserPermission(Permission.ExternalStorageWrite);
    }
}
