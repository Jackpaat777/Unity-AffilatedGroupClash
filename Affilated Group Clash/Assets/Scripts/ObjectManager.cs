using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    [Header("---------------[GiHaDol]")]
    public GameObject[] giHa_prefabs;
    List<GameObject>[] giHa_pools;  // Ǯ ��� ����Ʈ

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

        // �������� ���̸�ŭ ����Ʈ ����
        giHa_pools = new List<GameObject>[giHa_prefabs.Length];
        juFok_pools = new List<GameObject>[juFok_prefabs.Length];
        bakChi_pools = new List<GameObject>[bakChi_prefabs.Length];
        vBand_pools = new List<GameObject>[vBand_prefabs.Length];
        bullet_pools = new List<GameObject>[bullet_prefabs.Length];

        // �� ����Ʈ�� �����ڸ� ���� �ʱ�ȭ
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

    // ======================================================= ������Ʈ ���� �Լ�
    public GameObject GetGiHa(int idx, Vector3 pos)
    {
        GameObject select = null;

        // ������ Ǯ�� ��Ȱ��ȭ�� ������Ʈ ����
        foreach (GameObject item in giHa_pools[idx])
        {
            if (!item.activeSelf)
            {
                // �߰��ϸ� select�� �Ҵ�
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // select == null�̶� �ص� ������ �����Ͱ� ���� ��츦 �ǹ��ϴ� !select�� �ᵵ ��
        if (!select)
        {
            // �� ã���� ��� ���Ӱ� �����Ͽ� select�� �Ҵ�
            // Hierarchyâ�� �ƴ� PoolManager �Ʒ��� �Ҵ��ϱ� ���� transform�̶� ����
            select = Instantiate(giHa_prefabs[idx], transform);

            // ������Ʈ�� ���� ���������� Ǯ�� ���
            giHa_pools[idx].Add(select);
        }

        // ���� ��ġ�� ��ȯ
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
