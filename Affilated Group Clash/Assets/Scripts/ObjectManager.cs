using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    [Header("---------------[BackChiTheRock]")]
    public GameObject[] bakChiA_blue_Prefabs;
    public GameObject[] bakChiB_blue_Prefabs;
    public GameObject[] bakChiA_red_Prefabs;
    public GameObject[] bakChiB_red_Prefabs;

    [Header("---------------[Bullet]")]
    public GameObject[] bulletBPrefabs;
    public GameObject[] bulletRPrefabs;
    public GameObject[] bulletTPrefabs;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
    }
}
