using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    public GameObject[] bluePrefabs;
    public GameObject[] redPrefabs;
    public GameObject[] bulletPrefabs;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
    }
}
