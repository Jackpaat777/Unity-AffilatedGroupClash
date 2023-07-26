using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("---------------[InGame]")]
    public int maxCost;
    public int blueCost;
    public int redCost;
    public float costTimer;
    public TextMeshProUGUI costText;
    public GameObject allSensor;
    public bool isDevil;
    bool isGameStart;

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

    [Header("---------------[UI]")]
    public GameObject gameSet;
    public GameObject[] baseObject;
    public GameObject menuPanel;
    public GameObject selectPanel;
    public TextMeshProUGUI selectText;
    public int pageIndex;

    [Header("---------------[Button UI]")]
    public Image[] blueImage;
    public TextMeshProUGUI[] blueTypeText;
    public TextMeshProUGUI[] blueCostText;
    public GameObject lastButton;

    [Header("---------------[Camera]")]
    public Transform camTrans;
    public float camSpeed;
    public bool isMove;

    void Awake()
    {
        instance = this;
        isGameStart = false;
    }

    // ======================================================= �� ���� �Լ�
    public void SelectButton(string tmName)
    {
        if (pageIndex == 0)
            TeamSetting(tmName);
        else if (pageIndex == 1)
            EnemySetting(tmName);

        pageIndex = 1;
    }
    public void BackButton()
    {
        selectText.text = "�÷����� �׷��� �������ּ���";
        if (pageIndex == 0)
        {
            menuPanel.SetActive(true);
            selectPanel.SetActive(false);
            return;
        }
        pageIndex = 0;
    }
    void TeamSetting(string tmName)
    {
        selectText.text = "����� �׷��� �������ּ���";

        teamName = tmName;
        lastButton.SetActive(false);
        switch (teamName)
        {
            case "����A":
                teamPrefabs = ObjectManager.instance.giHa_prefabs;
                startidx = 0;
                groupNum = 5;
                break;
            case "����B":
                teamPrefabs = ObjectManager.instance.giHa_prefabs;
                startidx = 10;
                groupNum = 5;
                break;
            case "����":
                teamPrefabs = ObjectManager.instance.juPok_prefabs;
                startidx = 0;
                groupNum = 6;
                lastButton.SetActive(true);
                break;
            case "����A":
                teamPrefabs = ObjectManager.instance.bakChi_prefabs;
                startidx = 0;
                groupNum = 6;
                lastButton.SetActive(true);
                break;
            case "����B":
                teamPrefabs = ObjectManager.instance.bakChi_prefabs;
                startidx = 12;
                groupNum = 5;
                break;
            case "V��":
                teamPrefabs = ObjectManager.instance.vBand_prefabs;
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
    void TypeTextSetting(TextMeshProUGUI text, UnitType typeName)
    {
        switch (typeName)
        {
            case UnitType.Tanker:
                text.text = "��Ŀ";
                text.color = new Color(0, 255, 0);
                break;
            case UnitType.Warrior:
                text.text = "����";
                text.color = new Color(255, 0, 0);
                break;
            case UnitType.Ranger:
                text.text = "����";
                text.color = new Color(0, 200, 255);
                break;
            case UnitType.Buffer:
                text.text = "����";
                text.color = new Color(255, 255, 0);
                break;
            case UnitType.Special:
                text.text = "Ư��";
                text.color = new Color(255, 0, 255);
                break;
        }
    }
    void EnemySetting(string enName)
    {
        selectText.text = "�÷����� �׷��� �������ּ���";
        selectPanel.SetActive(false);

        enemyName = enName;
        switch (enemyName)
        {
            case "����A":
                enemyPrefabs = ObjectManager.instance.giHa_prefabs;
                startidx2 = 0;
                groupNum2 = 5;
                break;
            case "����B":
                enemyPrefabs = ObjectManager.instance.giHa_prefabs;
                startidx2 = 10;
                groupNum2 = 5;
                break;
            case "����":
                enemyPrefabs = ObjectManager.instance.juPok_prefabs;
                startidx2 = 0;
                groupNum2 = 6;
                break;
            case "����A":
                enemyPrefabs = ObjectManager.instance.bakChi_prefabs;
                startidx2 = 0;
                groupNum2 = 6;
                break;
            case "����B":
                enemyPrefabs = ObjectManager.instance.bakChi_prefabs;
                startidx2 = 12;
                groupNum2 = 5;
                break;
            case "V��":
                enemyPrefabs = ObjectManager.instance.vBand_prefabs;
                startidx2 = 0;
                groupNum2 = 5;
                break;
        }

        // Game Start
        GameStart();
        pageIndex = 0;
    }
    void GameStart()
    {
        isGameStart = true;
        // UI ����
        gameSet.SetActive(true);
        baseObject[0].SetActive(true);
        baseObject[1].SetActive(true);
    }

    // ======================================================= �ΰ��� ��ư �Լ�
    public void CameraMoveButton(string type)
    {
        // Set Speed By Button
        if (type == "RightDown" || type == "LeftUp") // ������ ��ư�� �����ų� ���ʹ�ư�� ���� 2 ���ϱ�
            camSpeed += 3f;
        else if(type == "RightUp" || type == "LeftDown")
            camSpeed -= 3f;
    }
    public void MakeBlueUnit(int idx)
    {
        GameObject unitB = teamPrefabs[idx];
        Unit unitBLogic = unitB.GetComponent<Unit>();

        // ����ó��
        if (isDevil && unitBLogic.unitDetail == UnitDetail.Devil)
            return;

        // Cost ����
        blueCost -= unitBLogic.unitCost;
        if (blueCost < 0)
        {
            blueCost += unitBLogic.unitCost;
            return;
        }
        // ����
        GetUnit(teamName, idx, unitB.transform.position);
    }
    public void MakeRedUnit(int idx)
    {
        GameObject unitR = enemyPrefabs[idx];
        Unit unitRLogic = unitR.GetComponent<Unit>();

        // ����ó��
        if (isDevil && unitRLogic.unitDetail == UnitDetail.Devil)
            return;

        // Cost ����
        redCost -= unitRLogic.unitCost;
        if (redCost < 0)
        {
            redCost += unitRLogic.unitCost;
            return;
        }
        // ����
        GetUnit(enemyName, idx, unitR.transform.position);
    }
    void GetUnit(string teamName, int idx, Vector3 pos)
    {
        switch (teamName)
        {
            case "����A":
            case "����B":
                ObjectManager.instance.GetGiHa(idx, pos);
                break;
            case "����":
                ObjectManager.instance.GetJuPok(idx, pos);
                break;
            case "����A":
            case "����B":
                ObjectManager.instance.GetBakChi(idx, pos);
                break;
            case "V��":
                ObjectManager.instance.GetVBand(idx, pos);
                break;
        }
    }

    void Update()
    {
        if (!isGameStart)
            return;

        // Camera Move
        CameraMove();
        // Cost
        CostUp();
        // KeyBoard
        KeyBoard();
    }

    // ======================================================= Update �Լ�
    void CameraMove()
    {
        // ȭ�� ������ �̵��Ϸ��� �ϸ� Move ����
        if ((camSpeed == -3f && camTrans.position.x > -5) || (camSpeed == 3f && camTrans.position.x < 5))
            isMove = true;
        else
            isMove = false;

        if (isMove)
        {
            // ���� ���Ͱ���ŭ �̵�
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
        // Ű���带 ���� �̵�
        if (Input.GetKey(KeyCode.RightArrow))
            camSpeed = 3f;
        if (Input.GetKey(KeyCode.LeftArrow))
            camSpeed = -3f;
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            camSpeed = 0;
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            camSpeed = 0;

        // Ű���带 ���� ���� ����
        // 2���� ���������ִ� �׷��� �����鿡�� Blue�� Red�� + Blue�� Red������ ��������
        // teamNum�� ���� Blue/ Red�� �������־�����, startIdx�� �� ���� ������ �������� ����Ͽ���
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
        if (Input.GetKeyDown(KeyCode.H) && groupNum == 6)
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
        if (Input.GetKeyDown(KeyCode.N) && groupNum2 == 6)
            MakeRedUnit(5 + startidx2 + groupNum2);
    }
}
