using System.Collections;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤 패턴 : 인스턴스를 여러번 사용하지 않고 하나의 인스턴스로 사용하기

    [Header("---------------[InGame]")]
    public float gameTimer;
    public int maxCost;
    public int blueCost;
    public int redCost;
    public float costTimer;
    public TextMeshProUGUI costText;
    public GameObject allSensor;
    public bool isDevil;
    bool isGameStart;

    [Header("---------------[Unit Info]")]
    public bool isUnitClick;
    public GameObject unitObj;
    public Image unitImage;
    public Slider hpSlider;
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI atsText;
    public TextMeshProUGUI ranText;
    public TextMeshProUGUI spdText;

    [Header("---------------[Blue Team Setting]")]
    public string teamBlueName;
    public int startBlueIdx;
    public int groupBlueNum;
    public GameObject[] teamBluePrefabs;
    [Header("---------------[Red Team Setting]")]
    public string teamRedName;
    public int startRedIdx;
    public int groupRedNum;
    public GameObject[] teamRedPrefabs;
    public float spawnTimer;
    public int patternIdx;

    [Header("---------------[UI]")]
    public GameObject gameSet;
    public GameObject[] baseObject;
    public GameObject menuPanel;
    public GameObject selectPanel;
    public TextMeshProUGUI selectText;
    public int selectPageIdx;

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

    // ======================================================= 팀 세팅 함수
    public void SelectButton(string tmName)
    {
        if (selectPageIdx == 0)
            BlueTeamSetting(tmName);
        else if (selectPageIdx == 1)
            RedTeamSetting(tmName);

        selectPageIdx = 1;
    }
    public void BackButton()
    {
        selectText.text = "플레이할 그룹을 선택해주세요";
        if (selectPageIdx == 0)
        {
            menuPanel.SetActive(true);
            selectPanel.SetActive(false);
            return;
        }
        selectPageIdx = 0;
    }
    void BlueTeamSetting(string tmName)
    {
        selectText.text = "상대할 그룹을 선택해주세요";

        teamBlueName = tmName;
        lastButton.SetActive(false);
        switch (teamBlueName)
        {
            case "지하A":
                teamBluePrefabs = ObjectManager.instance.giHa_prefabs;
                startBlueIdx = 0;
                groupBlueNum = 5;
                break;
            case "지하B":
                teamBluePrefabs = ObjectManager.instance.giHa_prefabs;
                startBlueIdx = 10;
                groupBlueNum = 5;
                break;
            case "주폭":
                teamBluePrefabs = ObjectManager.instance.juPok_prefabs;
                startBlueIdx = 0;
                groupBlueNum = 6;
                lastButton.SetActive(true);
                break;
            case "박취A":
                teamBluePrefabs = ObjectManager.instance.bakChi_prefabs;
                startBlueIdx = 0;
                groupBlueNum = 6;
                lastButton.SetActive(true);
                break;
            case "박취B":
                teamBluePrefabs = ObjectManager.instance.bakChi_prefabs;
                startBlueIdx = 12;
                groupBlueNum = 5;
                break;
            case "V급":
                teamBluePrefabs = ObjectManager.instance.vBand_prefabs;
                startBlueIdx = 0;
                groupBlueNum = 5;
                break;
        }

        // Team Button Setting
        for (int i = startBlueIdx; i < startBlueIdx + groupBlueNum; i++)
        {
            Unit teamUnit = teamBluePrefabs[i].GetComponent<Unit>();
            SpriteRenderer spriteRen = teamUnit.GetComponent<SpriteRenderer>();
            // Image
            blueImage[i - startBlueIdx].sprite = spriteRen.sprite;
            // Type
            TypeTextSetting(blueTypeText[i - startBlueIdx], teamUnit.unitType);
            // Cost
            blueCostText[i - startBlueIdx].text = teamUnit.unitCost.ToString();
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
    void RedTeamSetting(string enName)
    {
        selectText.text = "플레이할 그룹을 선택해주세요";
        selectPanel.SetActive(false);

        teamRedName = enName;
        switch (teamRedName)
        {
            case "지하A":
                teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                startRedIdx = 0;
                groupRedNum = 5;
                break;
            case "지하B":
                teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                startRedIdx = 10;
                groupRedNum = 5;
                break;
            case "주폭":
                teamRedPrefabs = ObjectManager.instance.juPok_prefabs;
                startRedIdx = 0;
                groupRedNum = 6;
                break;
            case "박취A":
                teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                startRedIdx = 0;
                groupRedNum = 6;
                break;
            case "박취B":
                teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                startRedIdx = 12;
                groupRedNum = 5;
                break;
            case "V급":
                teamRedPrefabs = ObjectManager.instance.vBand_prefabs;
                startRedIdx = 0;
                groupRedNum = 5;
                break;
        }

        // Game Start
        GameStart();
        selectPageIdx = 0;
    }
    void GameStart()
    {
        isGameStart = true;
        // UI 세팅
        gameSet.SetActive(true);
        baseObject[0].SetActive(true);
        baseObject[1].SetActive(true);
    }

    // ======================================================= 인게임 버튼 함수
    public void CameraMoveButton(string type)
    {
        // Set Speed By Button
        if (type == "RightDown" || type == "LeftUp") // 오른쪽 버튼을 누르거나 왼쪽버튼을 떼면 2 더하기
            camSpeed += 3f;
        else if(type == "RightUp" || type == "LeftDown")
            camSpeed -= 3f;
    }
    public void MakeBlueUnit(int idx)
    {
        GameObject unitB = teamBluePrefabs[idx];
        Unit unitBLogic = unitB.GetComponent<Unit>();

        // 예외처리
        if (isDevil && unitBLogic.unitDetail == UnitDetail.Devil)
            return;

        // Cost 감소
        blueCost -= unitBLogic.unitCost;
        if (blueCost < 0)
        {
            blueCost += unitBLogic.unitCost;
            return;
        }
        // 생성
        GetUnit(teamBlueName, idx, unitB.transform.position);
    }
    public void MakeRedUnit(int idx)
    {
        GameObject unitR = teamRedPrefabs[idx];
        Unit unitRLogic = unitR.GetComponent<Unit>();

        // 예외처리
        if (isDevil && unitRLogic.unitDetail == UnitDetail.Devil)
            return;

        // Cost 감소
        redCost -= unitRLogic.unitCost;
        if (redCost < 0)
        {
            redCost += unitRLogic.unitCost;
            return;
        }
        // 생성
        GetUnit(teamRedName, idx, unitR.transform.position);

        // 생성 후에 패턴인덱스 변경
        patternIdx = Random.Range(0, 3);
    }
    void GetUnit(string teamName, int idx, Vector3 pos)
    {
        switch (teamName)
        {
            case "지하A":
            case "지하B":
                ObjectManager.instance.GetGiHa(idx, pos);
                break;
            case "주폭":
                ObjectManager.instance.GetJuPok(idx, pos);
                break;
            case "박취A":
            case "박취B":
                ObjectManager.instance.GetBakChi(idx, pos);
                break;
            case "V급":
                ObjectManager.instance.GetVBand(idx, pos);
                break;
        }
    }

    void Update()
    {
        if (!isGameStart)
            return;

        // Timer
        gameTimer += Time.deltaTime;

        // Camera Move
        CameraMove();
        // Cost
        CostUp();
        // KeyBoard
        KeyBoard();

        // Loop Pattern
        //StartCoroutine(Pattern(1f, patternIdx));

        // Unit Infomation
        if (isUnitClick)
        {
            Unit unitLogic = unitObj.GetComponent<Unit>();
            UnitInfo(unitLogic);
        }
    }

    void UnitInfo(Unit unitLogic)
    {
        unitImage.gameObject.SetActive(true);

        SpriteRenderer spriteRen = unitLogic.GetComponent<SpriteRenderer>();
        unitImage.sprite = spriteRen.sprite;
        unitNameText.text = unitLogic.unitName;
        hpSlider.value = (float)unitLogic.unitHp / unitLogic.unitMaxHp;
        hpText.text = $"HP : {unitLogic.unitHp} / {unitLogic.unitMaxHp}";
        atkText.text = $"ATK : {unitLogic.unitAtk}";
        string floatAts = unitLogic.unitAtkSpeed.ToString("F2");
        atsText.text = $"ATS : " + floatAts;
        ranText.text = $"RAN : {unitLogic.unitRange}";

        if (unitLogic.unitSpeed > 0)
            spdText.text = $"SPD : {unitLogic.unitSpeed}";
        else
            spdText.text = $"SPD : {unitLogic.unitSpeed * -1}";
    }

    // ======================================================= Update 함수
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
            MakeBlueUnit(0 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.S))
            MakeBlueUnit(1 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.D))
            MakeBlueUnit(2 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.F))
            MakeBlueUnit(3 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.G))
            MakeBlueUnit(4 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.H) && groupBlueNum == 6)
            MakeBlueUnit(5 + startBlueIdx);
        // red
        if (Input.GetKeyDown(KeyCode.Z))
            MakeRedUnit(0 + startRedIdx + groupRedNum);
        if (Input.GetKeyDown(KeyCode.X))
            MakeRedUnit(1 + startRedIdx + groupRedNum);
        if (Input.GetKeyDown(KeyCode.C))
            MakeRedUnit(2 + startRedIdx + groupRedNum);
        if (Input.GetKeyDown(KeyCode.V))
            MakeRedUnit(3 + startRedIdx + groupRedNum);
        if (Input.GetKeyDown(KeyCode.B))
            MakeRedUnit(4 + startRedIdx + groupRedNum);
        if (Input.GetKeyDown(KeyCode.N) && groupRedNum == 6)
            MakeRedUnit(5 + startRedIdx + groupRedNum);
    }

    // ======================================================= AI 함수
    // 패턴을 메모장을 이용해 직접 제작하기?
    // 랜덤값을 통해 알아서 소환하기?
    IEnumerator Pattern(float time, int typeNum)
    {
        yield return new WaitForSeconds(time);

        // random 결정용 변수
        int rand = Random.Range(0, 2);
        switch (typeNum)
        {
            // 3코스트 이상일 때, 0번유닛과 1번유닛을 랜덤하게 소환
            case 0:
                if (redCost >= 3)
                    MakeRedUnit(rand + startRedIdx + groupRedNum);
                break;
            // 6코스트 이상일 때, rand값이 0이면 0번패턴, 1이면 2번유닛과 3번유닛을 랜덤하게 소환
            case 1:
                if (redCost >= 6)
                {
                    int idx = Random.Range(2, 4);
                    if (rand == 0)
                        StartCoroutine(Pattern(0, 0));
                    else if (rand == 1)
                        MakeRedUnit(idx + startRedIdx + groupRedNum);
                }
                break;
            // 8코스트 이상일 때, rand값이 0이면 1번패턴, 1이면 4번유닛을 소환
            case 2:
                if (redCost >= 8)
                {
                    if (rand == 0)
                        StartCoroutine(Pattern(0, 1));
                    else if (rand == 1)
                    {
                        // 팀이 6명일 경우
                        if (groupRedNum == 6)
                        {
                            int idx = Random.Range(4, 6);
                            MakeRedUnit(idx + startRedIdx + groupRedNum);
                        }
                        // 팀이 5명일 경우
                        else
                        {
                            MakeRedUnit(4 + startRedIdx + groupRedNum);
                        }
                    }
                }
                break;
        }
    }
}
