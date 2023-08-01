using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    [Header("---------------[GiHaDol]")]
    public GameObject[] giHa_prefabs;
    List<GameObject>[] giHa_pools;  // 풀 담당 리스트

    [Header("---------------[JuFok]")]
    public GameObject[] juFok_prefabs;
    List<GameObject>[] juFok_pools;

    [Header("---------------[BackChiTheRock]")]
    public GameObject[] bakChi_prefabs;
    List<GameObject>[] bakChi_pools;

    [Header("---------------[V_band]")]
    public GameObject[] vBand_prefabs;
    List<GameObject>[] vBand_pools;

    [Header("---------------[Bullet]")]
    public GameObject[] bullet_prefabs;
    List<GameObject>[] bullet_pools;

    void Awake()
    {
        instance = this;

        // 프리펩의 길이만큼 리스트 생성
        giHa_pools = new List<GameObject>[giHa_prefabs.Length];
        juFok_pools = new List<GameObject>[juFok_prefabs.Length];
        bakChi_pools = new List<GameObject>[bakChi_prefabs.Length];
        vBand_pools = new List<GameObject>[vBand_prefabs.Length];
        bullet_pools = new List<GameObject>[bullet_prefabs.Length];

        // 각 리스트를 생성자를 통해 초기화
        for (int i = 0; i < giHa_pools.Length; i++)
            giHa_pools[i] = new List<GameObject>();
        for (int i = 0; i < juFok_pools.Length; i++)
            juFok_pools[i] = new List<GameObject>();
        for (int i = 0; i < bakChi_pools.Length; i++)
            bakChi_pools[i] = new List<GameObject>();
        for (int i = 0; i < vBand_pools.Length; i++)
            vBand_pools[i] = new List<GameObject>();
        for (int i = 0; i < bullet_pools.Length; i++)
            bullet_pools[i] = new List<GameObject>();
    }

    // ======================================================= 오브젝트 생성 함수
    public GameObject GetGiHa(int idx, Vector3 pos)
    {
        GameObject select = null;

        // 선택한 풀의 비활성화된 오브젝트 접근
        foreach (GameObject item in giHa_pools[idx])
        {
            if (!item.activeSelf)
            {
                // 발견하면 select에 할당
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // select == null이라 해도 되지만 데이터가 없는 경우를 의미하는 !select로 써도 됨
        if (!select)
        {
            // 못 찾았을 경우 새롭게 생성하여 select에 할당
            // Hierarchy창이 아닌 PoolManager 아래에 할당하기 위해 transform이라 지정
            select = Instantiate(giHa_prefabs[idx], transform);

            // 오브젝트를 새로 생성했으니 풀에 등록
            giHa_pools[idx].Add(select);
        }

        // 지정 위치에 소환
        select.transform.position = pos;

        return select;
    }
    public GameObject GetJuFok(int idx, Vector3 pos)
    {
        GameObject select = null;

        foreach (GameObject item in juFok_pools[idx])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(juFok_prefabs[idx], transform);
            juFok_pools[idx].Add(select);
        }

        select.transform.position = pos;

        return select;
    }
    public GameObject GetBakChi(int idx, Vector3 pos)
    {
        GameObject select = null;

        foreach (GameObject item in bakChi_pools[idx])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(bakChi_prefabs[idx], transform);
            bakChi_pools[idx].Add(select);
        }

        select.transform.position = pos;

        return select;
    }
    public GameObject GetVBand(int idx, Vector3 pos)
    {
        GameObject select = null;

        foreach (GameObject item in vBand_pools[idx])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(vBand_prefabs[idx], transform);
            vBand_pools[idx].Add(select);
        }

        select.transform.position = pos;

        return select;
    }
    public GameObject GetBullet(int idx, Vector3 pos)
    {
        GameObject select = null;
        foreach (GameObject item in bullet_pools[idx])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(bullet_prefabs[idx], transform);
            bullet_pools[idx].Add(select);
        }

        select.transform.position = pos;

        return select;
    }
}
