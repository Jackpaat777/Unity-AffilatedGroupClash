using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    [Header("---------------[JiHaDol]")]
    public GameObject[] jiHaA_blue_prefabs;
    public GameObject[] jiHaB_blue_prefabs;
    public GameObject[] jiHaA_red_prefabs;
    public GameObject[] jiHaB_red_prefabs;

    [Header("---------------[JuPok]")]
    public GameObject[] juPok_blue_prefabs;
    public GameObject[] juPok_red_prefabs;

    [Header("---------------[BackChiTheRock]")]
    public GameObject[] bakChi_prefabs;
    // 풀 담당 리스트
    List<GameObject>[] bakChi_pools;

    [Header("---------------[V_band]")]
    public GameObject[] vBand_blue_prefabs;
    public GameObject[] vBand_red_prefabs;

    [Header("---------------[Bullet]")]
    public GameObject[] bullet_prefabs;
    //public GameObject[] bulletR_prefabs;
    //public GameObject[] bulletT_prefabs;
    List<GameObject>[] bullet_pools;

    void Awake()
    {
        instance = this;

        // 프리펩의 길이만큼 리스트 생성
        bakChi_pools = new List<GameObject>[bakChi_prefabs.Length];
        bullet_pools = new List<GameObject>[bullet_prefabs.Length];

        for (int i = 0; i < bakChi_pools.Length; i++)
        {
            // 각 리스트를 생성자를 통해 초기화
            bakChi_pools[i] = new List<GameObject>();
        }
    }

    public GameObject GetBakChi(int index)
    {
        GameObject select = null;

        // 선택한 풀의 비활성화된 오브젝트 접근
        foreach (GameObject item in bakChi_pools[index])
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
            select = Instantiate(bakChi_prefabs[index], transform);

            // 오브젝트를 새로 생성했으니 풀에 등록
            bakChi_pools[index].Add(select);
        }

        return select;
    }
}
