using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    [Header("---------------[InGame]")]
    public bool isGameLive;
    public int blueMaxCost;
    public int redMaxCost;
    public int blueCost;
    public int redCost;
    public int patternIdx;
    public float gameTimer;
    public float costBTimer;
    public float costRTimer;
    public float costBlueUp;
    public float costRedUp;
    public float redUpgradeTime;
    public float spawnTimer;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI costText;
    public List<GameObject> blueUnitList;
    public List<GameObject> redUnitList;

    [Header("---------------[Base]")]
    public int baseBLevel;
    public int baseRLevel;
    public int blueHP;
    public int redHP;
    public Unit baseB;
    public Unit baseR;
    public Sprite[] baseBSprite;
    public Sprite[] baseRSprite;
    public SpriteRenderer baseBSpriteRen;
    public SpriteRenderer baseRSpriteRen;
    public Slider baseBSlider;
    public Slider baseRSlider;
    public TextMeshProUGUI blueHpText;
    public TextMeshProUGUI redHpText;
    public TextMeshProUGUI blueHpShadowText;
    public TextMeshProUGUI redHpShadowText;
    public TextMeshProUGUI blueUpgradeCostText;
    public TextMeshProUGUI upgradeText;
    public GameObject blueDestroyEffect;
    public GameObject redDestroyEffect;
    public GameObject[] blueUpgradeyEffect;
    public GameObject[] redUpgradeyEffect;


    [Header("---------------[Top UI]")]
    public Animator fadeAc;
    public GameObject optionPanel;
    public TextMeshProUGUI levelText;
    public Image blueLogo;
    public Image redLogo;
    public Sprite[] logoSprites;
    [Header("---------------[Button UI]")]
    public Image[] blueButtonImage;
    public TextMeshProUGUI[] blueTypeText;
    public TextMeshProUGUI[] blueCostText;
    public GameObject lastButton;
    [Header("---------------[Unit Info UI]")]
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

    [Header("---------------[Result]")]
    public GameObject overSet;
    public GameObject controlSet;
    public GameObject victoryObj;
    public GameObject defeatObj;
    public GameObject nextBtn;
    public GameObject resetBtn;

    [Header("---------------[Devil]")]
    public bool isDevilB;
    public bool isDevilR;
    public float devilBTimer;
    public float devilRTimer;
    public bool isDevilBAttack;
    public bool isDevilRAttack;

    [Header("---------------[Description]")]
    public int descriptionPage;
    public Button optionButton;
    public TextMeshProUGUI descriptionText;
    public GameObject descriptionPanel;
    public GameObject desPrevButton;
    public GameObject desNextButton;
    public GameObject[] descriptionRect;


    [Header("---------------[Camera]")]
    public Transform camTrans;
    public float camSpeed;
    public bool isMove;

    void Awake()
    {
        instance = this;
        isGameLive = true;

        // Fade In
        fadeAc.SetTrigger("fadeIn");
        // Fade 없앤 뒤 게임설명 띄우기
        StartCoroutine(DisableFade(0.5f));

        // Game Setting
        GameSetting();
    }
    void GameSetting()
    {
        // BlueTeam Button Setting
        for (int i = Variables.startBlueIdx; i < Variables.startBlueIdx + Variables.groupBlueNum; i++)
        {
            Unit teamUnit = Variables.teamBluePrefabs[i].GetComponent<Unit>();
            SpriteRenderer spriteRen = teamUnit.GetComponent<SpriteRenderer>();
            // Image
            blueButtonImage[i - Variables.startBlueIdx].sprite = spriteRen.sprite;
            // Type
            TypeTextSetting(blueTypeText[i - Variables.startBlueIdx], teamUnit.unitType);
            // Cost
            blueCostText[i - Variables.startBlueIdx].text = teamUnit.unitCost.ToString();
        }

        // 5명팀이면 마지막버튼 끄기
        if (Variables.groupBlueNum == 5)
            lastButton.SetActive(false);

        // Base
        baseBSpriteRen.gameObject.SetActive(true);
        baseRSpriteRen.gameObject.SetActive(true);
        blueHpText.text = blueHP.ToString();
        redHpText.text = redHP.ToString();
        blueHpShadowText.text = blueHP.ToString();
        redHpShadowText.text = redHP.ToString();

        // RedTeam Level
        switch (Variables.gameLevel)
        {
            case 0:
                // 코스트 증가 시간
                costRedUp = 3.5f;
                // 업그레이드 시간
                redUpgradeTime = 120;
                // 몇초에 한명씩 나오는지
                spawnTimer = 3f;
                // 레벨 텍스트 변경
                levelText.text = "매우쉬움";
                break;
            case 1:
                costRedUp = 3f;
                redUpgradeTime = 90;
                spawnTimer = 2f;
                levelText.text = "쉬움";
                break;
            case 2:
                costRedUp = 2.5f;
                redUpgradeTime = 60;
                spawnTimer = 1f;
                levelText.text = "보통";
                break;
            case 3:
                costRedUp = 2f;
                redUpgradeTime = 50;
                spawnTimer = 0.75f;
                levelText.text = "어려움";
                break;
            case 4:
                costRedUp = 1.75f;
                redUpgradeTime = 30;
                spawnTimer = 0.75f;
                levelText.text = "매우어려움";
                break;
        }

        // Logo
        blueLogo.sprite = logoSprites[Variables.teamBlueNum];
        redLogo.sprite = logoSprites[Variables.teamRedNum];

        // BGM
        SoundManager.instance.BgmPlay("Game");
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
    IEnumerator DisableFade(float time)
    {
        yield return new WaitForSeconds(time);

        fadeAc.gameObject.SetActive(false);

        // 게임 설명 띄우기
        if (!Variables.isFirstGame)
        {
            DescriptionButton();
            Variables.isFirstGame = true;
        }
    }

    void Update()
    {
        if (!isGameLive)
            return;

        // Timer
        GameTimer();
        // Devil
        DevilTimer();
        // Camera Move
        CameraMove();
        // Red Upgrade;
        RedUpgrade();
        // Cost
        BlueCostUp();
        RedCostUp();
        // KeyBoard
        KeyBoard();
        // Unit Infomation
        UnitInfo();

        // Loop Pattern
        StartCoroutine(Pattern(1f, patternIdx));
    }
    // ======================================================= Update 함수
    void GameTimer()
    {
        gameTimer += Time.deltaTime;
        timeText.text = ((int)gameTimer / 60).ToString("D2") + ":" + ((int)gameTimer % 60).ToString("D2");
    }
    void DevilTimer()
    {
        if (isDevilB)
        {
            devilBTimer += Time.deltaTime;
            if (devilBTimer > 2.5f)
            {
                isDevilBAttack = true;
                devilBTimer = 0;
            }
            else
                isDevilBAttack = false;
        }
        if (isDevilR)
        {
            devilRTimer += Time.deltaTime;
            if (devilRTimer > 2f)
            {
                isDevilRAttack = true;
                devilRTimer = 0;
            }
            else
                isDevilRAttack = false;
        }
    }
    void CameraMove()
    {
        // 화면 밖으로 이동하려고 하면 Move 중지
        if ((camSpeed == -4f && camTrans.position.x > -6) || (camSpeed == 4f && camTrans.position.x < 6))
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
    void RedUpgrade()
    {
        // 업그레이드
        if (gameTimer > redUpgradeTime &&  baseRLevel == 0)
            UpgradeRed();
        else if (gameTimer > redUpgradeTime * 2.5f && baseRLevel == 1)
            UpgradeRed();
    }
    void BlueCostUp()
    {
        costText.text = $"{blueCost}/{blueMaxCost}";

        costBTimer += Time.deltaTime;
        if (costBTimer > costBlueUp)
        {
            blueCost += 1;
            blueCost = blueCost > blueMaxCost ? blueMaxCost : blueCost;

            costBTimer = 0;
        }
    }
    void RedCostUp()
    {
        costRTimer += Time.deltaTime;
        if (costRTimer > costRedUp)
        {
            redCost += 1;
            redCost = redCost > redMaxCost ? redMaxCost : redCost;

            costRTimer = 0;
        }
    }
    void KeyBoard()
    {
        // 키보드를 통한 이동
        if (Input.GetKey(KeyCode.RightArrow))
            camSpeed = 4f;
        if (Input.GetKey(KeyCode.LeftArrow))
            camSpeed = -4f;
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            camSpeed = 0;
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            camSpeed = 0;

        // 키보드를 통한 유닛 생성
        // 2조로 나뉘어져있는 그룹의 프리펩에는 Blue팀 Red팀 + Blue팀 Red팀으로 섞여있음
        // teamNum에 따라 Blue/ Red로 나누어주었으며, startIdx가 두 팀을 나누는 기준으로 사용하였음
        if (Input.GetKeyDown(KeyCode.Q))
            MakeBlueUnit(0 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.W))
            MakeBlueUnit(1 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.E))
            MakeBlueUnit(2 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.A))
            MakeBlueUnit(3 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.S))
            MakeBlueUnit(4 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.D) && Variables.groupBlueNum == 6)
            MakeBlueUnit(5 + Variables.startBlueIdx);
        // red
        if (Input.GetKeyDown(KeyCode.I))
            MakeRedUnit(0 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.O))
            MakeRedUnit(1 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.P))
            MakeRedUnit(2 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.J))
            MakeRedUnit(3 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.K))
            MakeRedUnit(4 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.L) && Variables.groupRedNum == 6)
            MakeRedUnit(5 + Variables.startRedIdx + Variables.groupRedNum);

        // 키보드로 업그레이드
        if (Input.GetKeyDown(KeyCode.B))
            UpgradeBlueButton();
    }
    void UnitInfo()
    {
        if (!isUnitClick)
            return;

        // Unit Setting
        Unit unitLogic = unitObj.GetComponent<Unit>();
        unitImage.gameObject.SetActive(true);

        // Image
        SpriteRenderer spriteRen = unitLogic.GetComponent<SpriteRenderer>();
        unitImage.sprite = spriteRen.sprite;
        // Name
        unitNameText.text = unitLogic.unitName;
        // Hp
        hpSlider.value = (float)unitLogic.unitHp / unitLogic.unitMaxHp;
        hpText.text = $"{unitLogic.unitHp} / {unitLogic.unitMaxHp}";
        // Atk
        atkText.text = $"ATK : {unitLogic.unitAtk}";
        // Atk Speed
        string floatAts = unitLogic.unitAtkSpeed.ToString("F1");
        atsText.text = $"ATS : " + floatAts;
        // Range
        ranText.text = $"RAN : {unitLogic.unitRange}";
        // Speed
        if (unitLogic.unitSpeed > 0)
            spdText.text = $"SPD : {unitLogic.unitSpeed * 10}";
        else
            spdText.text = $"SPD : {unitLogic.unitSpeed * -10}";
    }

    // ======================================================= 인게임 버튼 함수
    public void CameraMoveButton(string type)
    {
        // Set Speed By Button
        if (type == "RightDown" || type == "LeftUp") // 오른쪽 버튼을 누르거나 왼쪽버튼을 떼면 2 더하기
            camSpeed += 4f;
        else if (type == "RightUp" || type == "LeftDown")
            camSpeed -= 4f;
    }
    // 업그레이드
    public void UpgradeBlueButton()
    {
        // 코스트 사용
        if (blueCost < blueMaxCost)
            return;
        if (baseBLevel == 2)
        {
            blueUpgradeCostText.text = "-";
            upgradeText.text = "Max";
            return;
        }


        blueCost -= blueMaxCost;

        // 이펙트
        blueUpgradeyEffect[baseBLevel].SetActive(true);
        // Level Up
        baseBLevel++;
        // 스프라이트 변경 (초가집, 돌집, 빌딩)
        baseBSpriteRen.sprite = baseBSprite[baseBLevel];

        // Max 코스트 증가
        blueMaxCost += 5;
        // cost 텍스트 변경
        blueUpgradeCostText.text = blueMaxCost.ToString();
        //코스트 증가 속도 상승
        costBlueUp -= 0.25f;
        // 베이스 강화
        baseB.unitAtk += 5;
        baseB.unitAtkSpeed -= 0.2f;
    }
    void UpgradeRed()
    {
        redUpgradeyEffect[baseRLevel].SetActive(true);
        baseRLevel++;
        redMaxCost += 5;
        costRedUp -= 0.25f;
        baseRSpriteRen.sprite = baseRSprite[baseRLevel];

        baseB.unitAtk += 5;
        baseB.unitAtkSpeed -= 0.2f;

    }
    // 유닛 소환
    public void MakeBlueUnit(int idx)
    {
        GameObject unitB = Variables.teamBluePrefabs[idx];
        Unit unitBLogic = unitB.GetComponent<Unit>();

        // 예외처리
        if (isDevilB && unitBLogic.unitDetail == UnitDetail.Devil)
            return;

        // Cost 감소
        blueCost -= unitBLogic.unitCost;
        if (blueCost < 0)
        {
            blueCost += unitBLogic.unitCost;
            return;
        }
        // 생성
        GetUnitObject(Variables.teamBlueNum, idx, unitB.transform.position);
        blueUnitList.Add(unitB);
    }
    public void MakeRedUnit(int idx)
    {
        GameObject unitR = Variables.teamRedPrefabs[idx];
        Unit unitRLogic = unitR.GetComponent<Unit>();

        // 예외처리
        if (isDevilR && unitRLogic.unitDetail == UnitDetail.Devil)
            return;

        // Cost 감소
        redCost -= unitRLogic.unitCost;
        if (redCost < 0)
        {
            redCost += unitRLogic.unitCost;
            return;
        }
        // 생성
        GetUnitObject(Variables.teamRedNum, idx, unitR.transform.position);
        redUnitList.Add(unitR);

        // 생성 후에 패턴인덱스 변경
        PatternRatio();
    }
    void PatternRatio()
    {
        int rand = Random.Range(0, 100);

        // 패턴 인덱스 결정
        if (rand < 40)
            patternIdx = 0;
        else if (rand < 70)
            patternIdx = 1;
        else
            patternIdx = 2;
    }
    void GetUnitObject(int teamNum, int idx, Vector3 pos)
    {
        switch (teamNum)
        {
            case 0:
            case 1:
                ObjectManager.instance.GetGiHa(idx, pos);
                break;
            case 2:
                ObjectManager.instance.GetJuFok(idx, pos);
                break;
            case 3:
            case 4:
                ObjectManager.instance.GetBakChi(idx, pos);
                break;
            case 5:
                ObjectManager.instance.GetVBand(idx, pos);
                break;
        }
    }
    // 옵션 버튼
    public void OptionButton()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void OptionOut()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1;
    }
    // 설명 버튼
    public void DescriptionButton()
    {
        if (!descriptionPanel.activeSelf)
        {
            // 멈춤
            Time.timeScale = 0;
            // 패널 켜기
            descriptionPanel.SetActive(true);
            Description("None");
            // 다른 버튼 작동중지
            optionButton.interactable = false;
        }
        else if (descriptionPanel.activeSelf)
        {
            // 이미 켜져있는 경우 다시 끄기
            Time.timeScale = 1;
            descriptionPage = 0;
            descriptionPanel.SetActive(false);
            optionButton.interactable = true;
            for (int i = 0; i < 6; i++)
            {
                // 사각형 다 끄기
                descriptionRect[i].SetActive(false);
            }
        }
    }
    public void PrevDescriptionButton()
    {
        descriptionPage--;

        if (descriptionPage == 0)
            desPrevButton.SetActive(false);
        else
            desNextButton.SetActive(true);

        Description("Prev");
    }
    public void NextDescriptionButton()
    {
        descriptionPage++;

        // 다봤으면 나가기
        if (descriptionPage > 8)
        {
            DescriptionButton();
            return;
        }

        desPrevButton.SetActive(true);

        Description("Next");
    }
    void Description(string btnType)
    {
        // 사각형 키고 끄기
        if (descriptionPage == 0 || descriptionPage == 1)
        {
            // 첫 페이지와 두번째에는 사각형이 안나와야함
            descriptionRect[0].SetActive(false);
        }
        else if (descriptionPage > 1)
        {
            // 사각형 키기 (마지막 페이지에는 사각형이 없으므로 못킴)
            if (descriptionPage < 8)
                descriptionRect[descriptionPage - 2].SetActive(true);

            // 다음 버튼을 눌렀을 경우
            if (btnType == "Next")
            {
                // 이전 사각형 끄기
                if (descriptionPage > 2)
                    descriptionRect[descriptionPage - 3].SetActive(false);
            }
            // 이전버튼을 눌렀을 경우
            else if (btnType == "Prev")
            {
                // 다음 사각형 끄기
                if (descriptionPage < 7)
                    descriptionRect[descriptionPage - 1].SetActive(false);
            }
        }

        // 설명란
        switch (descriptionPage)
        {
            case 0:
                descriptionText.text = "침하잎하~ 산하대격돌에 오신것을 환영합니다!\n\n그럼 바로 게임설명을 해드리도록 하겠습니다!\n\n(현재 게임은 정지된 상태입니다.)";
                break;
            case 1:
                descriptionText.text = "가장 먼저 화면 이동입니다.\n\n키보드의 화살표키를 통해 좌우로 화면을 이동하실 수 있습니다.";
                break;
            case 2:
                descriptionText.text = "두번째는 멤버 소환입니다.\n\n이곳에는 해당 멤버을 뽑기 위한 코인, 멤버의 타입이 적혀있습니다.\n\n" +
                    "멤버를 소환하기 위해서는 해당하는 키보드를 눌러서 소환하실 수 있습니다.\n\n같은 유닛을 중복하여 소환하실 수도 있습니다!";
                break;
            case 3:
                descriptionText.text = "이 곳은 멤버 상세정보 란입니다.\n\n현재 소환된 멤버를 직접 클릭하면 해당멤버의 실시간 정보를 확인하실 수 있습니다!";
                break;
            case 4:
                descriptionText.text = "다음으로 현재 코인입니다.\n\n코인이 부족하면 멤버를 소환하실 수 없으며, 코인은 자동으로 1씩 증가합니다.\n\n" +
                    "또한 최대보유량이 있기 때문에 최대보유량을 넘어서 코인을 보유하실 수 없습니다.";
                break;
            case 5:
                descriptionText.text = "Base 업그레이드입니다.\n\nBase는 스스로 일정 범위 내에서 적을 인식해 공격합니다.\n\n" +
                    "아래 Upgrade를 통해 코인을 소모하여 Base의 공격력, 공격속도를 증가시킬 수 있으며, 최대 2번까지 가능합니다.";
                break;
            case 6:
                descriptionText.text = "이곳에서는 양 팀의 로고와 현재 Base의 체력을 확인하실 수 있습니다.\n\n" +
                    "양 팀 모두 1000으로 시작하며 가장 먼저 체력이 0이 되는 팀이 패배합니다.\n\n그 아래에는 현재 게임 플레이 시간이 나옵니다.";
                break;
            case 7:
                descriptionText.text = "마지막으로 일시정지 버튼을 눌러 소리를 조절하거나 게임을 종료하실 수 있으며\n\n물음표 버튼을 누르면 게임설명을 다시 확인하실 수 있습니다.";
                break;
            case 8:
                descriptionText.text = "그럼 재밌게 즐겨주세요!\n\n감사합니다!";
                break;
        }
    }
    // 씬 이동 버튼
    public void NextButton()
    {
        // 레벨 증가
        Variables.gameLevel++;

        // 레드팀 번호 설정
        int rand = Random.Range(0, 6);
        while (Variables.isSelectTeam[rand])
        {
            rand = Random.Range(0, 6);
        }
        // 다음 상대 설정
        StageRedTeamSetting(rand);

        Time.timeScale = 1;
        // Fade Out
        fadeAc.gameObject.SetActive(true);
        fadeAc.SetTrigger("fadeOut");
        StartCoroutine(LoadScene(1f, "InGame"));
    }
    public void ResetButton()
    {
        Time.timeScale = 1;
        // Fade Out
        fadeAc.gameObject.SetActive(true);
        fadeAc.SetTrigger("fadeOut");
        StartCoroutine(LoadScene(1f, "Game"));
    }
    void StageRedTeamSetting(int teamNum)
    {
        switch (teamNum)
        {
            case 0:
                Variables.teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 5;
                break;
            case 1:
                Variables.teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startRedIdx = 10;
                Variables.groupRedNum = 5;
                break;
            case 2:
                Variables.teamRedPrefabs = ObjectManager.instance.juFok_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 6;
                break;
            case 3:
                Variables.teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 6;
                break;
            case 4:
                Variables.teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startRedIdx = 12;
                Variables.groupRedNum = 5;
                break;
            case 5:
                Variables.teamRedPrefabs = ObjectManager.instance.vBand_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 5;
                break;
        }

        // Team Num
        Variables.teamRedNum = teamNum;
        // Random Team List
        Variables.isSelectTeam[teamNum] = true;
    }
    IEnumerator LoadScene(float time, string sceneName)
    {
        yield return new WaitForSeconds(time);

        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    // ======================================================= Base 함수
    public void BaseHit(int dmg, int layer)
    {
        // 게임이 끝났으면 Base Hit 금지
        if (!isGameLive)
            return;

        if (layer == 8)
        {
            // 데미지만큼 감소
            blueHP -= dmg;

            // Death
            if (blueHP <= 0)
            {
                blueHP = 0;
                baseBSpriteRen.gameObject.SetActive(false);
                blueDestroyEffect.SetActive(true);
                isGameLive = false;
                GameOver("Lose");
            }
            // Alive
            else
            {
                baseBSpriteRen.color = new Color(1, 0.5f, 0.5f);
                StartCoroutine(SpriteWhite(0.1f, baseBSpriteRen, blueHP > 0));
            }

            // Text
            baseBSlider.value = blueHP;
            blueHpText.text = blueHP.ToString();
            blueHpShadowText.text = blueHP.ToString();
        }
        else if (layer == 9)
        {
            redHP -= dmg;

            if (redHP <= 0)
            {
                redHP = 0;
                baseRSpriteRen.gameObject.SetActive(false);
                redDestroyEffect.SetActive(true);
                isGameLive = false;
                GameOver("Win");
            }
            else
            {
                baseRSpriteRen.color = new Color(1, 0.5f, 0.5f);
                StartCoroutine(SpriteWhite(0.1f, baseRSpriteRen, redHP > 0));
            }

            baseRSlider.value = redHP;
            redHpText.text = redHP.ToString();
            redHpShadowText.text = redHP.ToString();
        }
    }
    IEnumerator SpriteWhite(float time, SpriteRenderer spriteRen, bool isLive)
    {
        yield return new WaitForSeconds(time);

        // 0.1초 사이에 게임이 끝나버리면 함수를 실행하면 안됨
        if (isLive)
            spriteRen.color = Color.white;
    }
    void GameOver(string result)
    {
        // 이겼을 경우
        if (result == "Win")
        {
            // Victory
            victoryObj.SetActive(true);

            // Button
            if (Variables.isStage)  // Stage 모드
            {
                if (Variables.gameLevel < 4)
                    nextBtn.SetActive(true);
                else
                {
                    // 클리어
                    Variables.isStageClear[Variables.teamBlueNum] = true;
                    // 저장
                    //PlayerPrefs.SetInt("Clear" + Variables.teamBlueNum, 1);

                    // 업적획득 창


                    resetBtn.SetActive(true);
                }
            }
            else                   // Normal 모드
                resetBtn.SetActive(true);
        }
        // 졌을 경우
        else if (result == "Lose")
        {
            // Defeat
            defeatObj.SetActive(true);

            // Button
            resetBtn.SetActive(true);
        }

        // 게임 작동 중지 (시간, 코스트, 카메라이동, 소환버튼, 유닛클릭)
        isGameLive = false;
        camSpeed = 0;
        controlSet.SetActive(false);
        overSet.SetActive(true);
    }

    // ======================================================= COM 함수
    // 패턴을 메모장을 이용해 직접 제작하기?
    // 랜덤값을 통해 알아서 소환하기?
    IEnumerator Pattern(float time, int typeNum)
    {
        yield return new WaitForSeconds(time);

        // 유닛 결정용 변수
        int rand = Random.Range(0, 2);
        switch (typeNum)
        {
            // 3코스트 이상일 때, 0번유닛과 1번유닛을 랜덤하게 소환
            case 0:
                if (redCost >= 3)
                    MakeRedUnit(rand + Variables.startRedIdx + Variables.groupRedNum);
                break;
            // 6코스트 이상일 때, rand값이 0이면 0번패턴, 1이면 2번유닛과 3번유닛을 랜덤하게 소환
            case 1:
                if (redCost >= 6)
                    MakeRedUnit(rand + 2 + Variables.startRedIdx + Variables.groupRedNum);
                break;
            // 9코스트 이상일 때, rand값이 0이면 1번패턴, 1이면 4번유닛 (+5번유닛)을 소환
            case 2:
                if (redCost >= 9)
                {
                    // 팀이 6명일 경우
                    if (Variables.groupRedNum == 6)
                        MakeRedUnit(rand + 4 + Variables.startRedIdx + Variables.groupRedNum);
                    // 팀이 5명일 경우
                    else
                        MakeRedUnit(4 + Variables.startRedIdx + Variables.groupRedNum);
                }
                break;
            //case 3:
            //    if (redCost >= 10)
            //    {
            //        if (rand == 0)
            //            StartCoroutine(Pattern(0, 2));
            //        else if (rand == 1)
            //        {
            //            // Upgrade
            //            UpgradeRed();
            //        }
            //    }
            //    break;
        }
    }
}
