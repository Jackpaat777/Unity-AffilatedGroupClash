using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("---------------[InGame]")]
    public GameObject menuSet;
    public GameObject gameSet;
    public GameObject[] baseObject;
    public Transform titleTrans;

    void Awake()
    {
        instance = this;
    }

    public void GameStart()
    {
        menuSet.SetActive(false);
        gameSet.SetActive(true);
        baseObject[0].SetActive(true);
        baseObject[1].SetActive(true);
    }


    void Update()
    {

    }
}
