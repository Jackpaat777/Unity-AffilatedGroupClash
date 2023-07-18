using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("---------------[Start]")]
    public GameObject selectSet;
    public GameObject gameSet;
    public GameObject[] baseObject;
    bool isGameStart;

    [Header("---------------[InGame]")]
    public int maxCost;
    public int blueCost;
    public int redCost;
    public float costTimer;
    public TextMeshProUGUI costText;

    [Header("---------------[Button UI]")]
    public Image[] blueImage;
    public TextMeshProUGUI[] blueTypeText;
    public TextMeshProUGUI[] blueCostText;
    public GameObject lastButton;

    [Header("---------------[Team Setting]")]
    public string teamName;
    public int startidx;
    public int groupNum;
    public GameObject[] teamPrefabs;
    [Header("---------------[Enemy Setting]")]
    public string enemyName;
    public int startidx2;
    public int groupNum2;
    public GameObject[] enemyPrefabs;

    [Header("---------------[Camera]")]
    public Transform camTrans;
    public float camSpeed;
    public bool isMove;

    void Awake()
    {
        instance = this;
        isGameStart = false;
    }

    public void GameStart()
    {
        isGameStart = true;
        // UI 세팅
        //selectSet.SetActive(false);
        gameSet.SetActive(true);
        baseObject[0].SetActive(true);
        baseObject[1].SetActive(true);
    }
    public void TeamSetting(string tmName)
    {
        teamName = tmName;
        lastButton.SetActive(false);
        switch (teamName)
        {
            case "지하A":
                teamPrefabs = ObjectManager.instance.jiHaA_blue_prefabs;
                startidx = 0;
                groupNum = 5;
                break;
            case "지하B":
                teamPrefabs = ObjectManager.instance.jiHaB_blue_prefabs;
                startidx = 10;
                groupNum = 5;
                break;
            case "주폭":
                teamPrefabs = ObjectManager.instance.juPok_blue_prefabs;
                startidx = 0;
                groupNum = 6;
                lastButton.SetActive(true);
                break;
            case "박취A":
                teamPrefabs = ObjectManager.instance.bakChi_prefabs;
                startidx = 0;
                groupNum = 6;
                lastButton.SetActive(true);
                break;
            case "박취B":
                teamPrefabs = ObjectManager.instance.bakChi_prefabs;
                startidx = 12;
                groupNum = 5;
                break;
            case "비급":
                teamPrefabs = ObjectManager.instance.vBand_blue_prefabs;
                startidx = 0;
                groupNum = 5;
                break;
        }

        // Team Button Setting
        for (int i = startidx; i < startidx + groupNum; i++)
        {
            Unit teamUnit = teamPrefabs[i].GetComponent<Unit>();
            SpriteRenderer spriteRen = teamUnit.GetComponent<SpriteRenderer>();
            // Image
            blueImage[i - startidx].sprite = spriteRen.sprite;
            // Type
            TypeTextSetting(blueTypeText[i - startidx], teamUnit.unitType);
            // Cost
            blueCostText[i - startidx].text = teamUnit.unitCost.ToString();
        }
    }
    public void EnemySetting(string enName)
    {
        enemyName = enName;
        switch (enemyName)
        {
            case "지하A":
                enemyPrefabs = ObjectManager.instance.jiHaA_blue_prefabs;
                startidx2 = 0;
                groupNum2 = 5;
                break;
            case "지하B":
                enemyPrefabs = ObjectManager.instance.jiHaB_blue_prefabs;
                startidx2 = 10;
                groupNum2 = 5;
                break;
            case "주폭":
                enemyPrefabs = ObjectManager.instance.juPok_blue_prefabs;
                startidx2 = 0;
                groupNum2 = 6;
                break;
            case "박취A":
                enemyPrefabs = ObjectManager.instance.bakChi_prefabs;
                startidx2 = 0;
                groupNum2 = 6;
                break;
            case "박취B":
                enemyPrefabs = ObjectManager.instance.bakChi_prefabs;
                startidx2 = 12;
                groupNum2 = 5;
                break;
            case "비급":
                enemyPrefabs = ObjectManager.instance.vBand_blue_prefabs;
                startidx2 = 0;
                groupNum2 = 5;
                break;
        }
    }
    void TypeTextSetting(TextMeshProUGUI text, UnitType typeName)
    {
        switch (typeName)
        {
            case UnitType.Tanker:
                text.text = "탱커";
                text.color = new Color(0, 255, 0);
                break;
            case UnitType.Warrior:
                text.text = "전사";
                text.color = new Color(255, 0, 0);
                break;
            case UnitType.Ranger:
                text.text = "원딜";
                text.color = new Color(0, 200, 255);
                break;
            case UnitType.Buffer:
                text.text = "버프";
                text.color = new Color(255, 255, 0);
                break;
            case UnitType.Special:
                text.text = "특수";
                text.color = new Color(255, 0, 255);
                break;
        }
    }


    // 버튼을 통한 이동
    public void CameraMoveButton(string type)
    {
        // Set Speed By Button
        if (type == "RightDown" || type == "LeftUp") // 오른쪽 버튼을 누르거나 왼쪽버튼을 떼면 2 더하기
            camSpeed += 3f;
        else if(type == "RightUp" || type == "LeftDown")
            camSpeed -= 3f;
    }

    // 버튼을 통한 유닛 생성
    public void MakeBlueUnit(int idx)
    {
        GameObject unitB = teamPrefabs[idx];
        Unit unitBLogic = unitB.GetComponent<Unit>();
        // Cost 감소
        blueCost -= unitBLogic.unitCost;
        if (blueCost < 0)
        {
            blueCost += unitBLogic.unitCost;
            return;
        }
        // 생성
        Instantiate(unitB);    
    }
    public void MakeRedUnit(int idx)
    {
        GameObject unitR = enemyPrefabs[idx];
        Unit unitRLogic = unitR.GetComponent<Unit>();
        // Cost 감소
        redCost -= unitRLogic.unitCost;
        if (redCost < 0)
        {
            redCost += unitRLogic.unitCost;
            return;
        }
        // 생성
        Instantiate(unitR);
    }

    void Update()
    {
        if (!isGameStart)
            return;

        // KeyBoard
        KeyBoard();
        // Camera Move
        CameraMove();
        // Cost
        CostUp();
    }


    void CameraMove()
    {
        // 화면 밖으로 이동하려고 하면 Move 중지
        if ((camSpeed == -3f && camTrans.position.x > -5) || (camSpeed == 3f && camTrans.position.x < 5))
            isMove = true;
        else
            isMove = false;

        if (isMove)
        {
            // 다음 벡터값만큼 이동
            Vector3 nextMove = Vector3.right * camSpeed * Time.deltaTime;
            camTrans.Translate(nextMove);
        }
    }
    void CostUp()
    {
        costText.text = blueCost.ToString();

        costTimer += Time.deltaTime;
        if (costTimer > 2f)
        {
            blueCost += 1;
            redCost += 1;
            blueCost = blueCost > maxCost ? maxCost : blueCost;
            redCost = redCost > maxCost ? maxCost : redCost;

            costTimer = 0;
        }
    }
    void KeyBoard()
    {
        // 키보드를 통한 이동
        if (Input.GetKey(KeyCode.RightArrow))
            camSpeed = 3f;
        if (Input.GetKey(KeyCode.LeftArrow))
            camSpeed = -3f;
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            camSpeed = 0;
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            camSpeed = 0;

        // 키보드를 통한 유닛 생성
        // 2조로 나뉘어져있는 그룹의 프리펩에는 Blue팀 Red팀 + Blue팀 Red팀으로 섞여있음
        // teamNum에 따라 Blue/ Red로 나누어주었으며, startIdx가 두 팀을 나누는 기준으로 사용하였음
        if (Input.GetKeyDown(KeyCode.A))
            MakeBlueUnit(0 + startidx);
        if (Input.GetKeyDown(KeyCode.S))
            MakeBlueUnit(1 + startidx);
        if (Input.GetKeyDown(KeyCode.D))
            MakeBlueUnit(2 + startidx);
        if (Input.GetKeyDown(KeyCode.F))
            MakeBlueUnit(3 + startidx);
        if (Input.GetKeyDown(KeyCode.G))
            MakeBlueUnit(4 + startidx);
        if (Input.GetKeyDown(KeyCode.H) && teamPrefabs.Length == 6)
            MakeBlueUnit(5 + startidx);
        // red
        if (Input.GetKeyDown(KeyCode.Z))
            MakeRedUnit(0 + startidx2 + groupNum2);
        if (Input.GetKeyDown(KeyCode.X))
            MakeRedUnit(1 + startidx2 + groupNum2);
        if (Input.GetKeyDown(KeyCode.C))
            MakeRedUnit(2 + startidx2 + groupNum2);
        if (Input.GetKeyDown(KeyCode.V))
            MakeRedUnit(3 + startidx2 + groupNum2);
        if (Input.GetKeyDown(KeyCode.B))
            MakeRedUnit(4 + startidx2 + groupNum2);
        if (Input.GetKeyDown(KeyCode.N))
            MakeRedUnit(5 + startidx2 + groupNum2);
    }
}
