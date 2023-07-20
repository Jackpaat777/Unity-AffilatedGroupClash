using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    [Header("---------------[JiHaDol]")]
    public GameObject[] jiHa_prefabs;
    List<GameObject>[] jiHa_pools;

    [Header("---------------[JuPok]")]
    public GameObject[] juPok_prefabs;

    [Header("---------------[BackChiTheRock]")]
    public GameObject[] bakChi_prefabs;
    // 풀 담당 리스트
    List<GameObject>[] bakChi_pools;

    [Header("---------------[V_band]")]
    public GameObject[] vBand_prefabs;

    [Header("---------------[Bullet]")]
    public GameObject[] bullet_prefabs;
    List<GameObject>[] bullet_pools;

    void Awake()
    {
        instance = this;

        // 프리펩의 길이만큼 리스트 생성
        jiHa_pools = new List<GameObject>[jiHa_prefabs.Length];
        bakChi_pools = new List<GameObject>[bakChi_prefabs.Length];
        bullet_pools = new List<GameObject>[bullet_prefabs.Length];
        
        for (int i = 0; i < jiHa_pools.Length; i++)
        {
            // 각 리스트를 생성자를 통해 초기화
            jiHa_pools[i] = new List<GameObject>();
        }
        for (int i = 0; i < bakChi_pools.Length; i++)
        {
            // 각 리스트를 생성자를 통해 초기화
            bakChi_pools[i] = new List<GameObject>();
        }
        for (int i = 0; i < bullet_pools.Length; i++)
        {
            // 각 리스트를 생성자를 통해 초기화
            bullet_pools[i] = new List<GameObject>();
        }
    }

    public GameObject GetJiHa(int idx, Vector3 pos)
    {
        GameObject select = null;

        // 선택한 풀의 비활성화된 오브젝트 접근
        foreach (GameObject item in jiHa_pools[idx])
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
            select = Instantiate(jiHa_prefabs[idx], transform);

            // 오브젝트를 새로 생성했으니 풀에 등록
            jiHa_pools[idx].Add(select);
        }

        // 지정 위치에 소환
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

    public void BulletSetting(GameObject bulletObj, UnitDetail uDetail, int uLayer, int uDmg)
    {
        Bullet bulletLogic = bulletObj.GetComponent<Bullet>();

        bulletLogic.unitDetail = uDetail;
        bulletLogic.layer = uLayer;
        bulletLogic.dmg = uDmg;
    }
}
