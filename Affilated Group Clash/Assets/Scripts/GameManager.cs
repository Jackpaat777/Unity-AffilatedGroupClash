using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤 패턴 : 인스턴스를 여러번 사용하지 않고 하나의 인스턴스로 사용하기

    [Header("---------------[InGame]")]
    public bool isGameLive;
    public float gameTimer;
    public int maxCost;
    public int blueCost;
    public int redCost;
    public float costTimer;
    public TextMeshProUGUI costText;
    public List<GameObject> blueUnitList;
    public List<GameObject> redUnitList;

    [Header("---------------[Base]")]
    public int blueHP;
    public int redHP;
    public Slider blueBaseSlider;
    public Slider redBaseSlider;
    public TextMeshProUGUI blueHpText;
    public TextMeshProUGUI redHpText;
    public TextMeshProUGUI blueHpShadowText;
    public TextMeshProUGUI redHpShadowText;
    public SpriteRenderer blueBaseSpriteRen;
    public SpriteRenderer redBaseSpriteRen;
    public Sprite[] blueBaseSprite;
    public Sprite[] redBaseSprite;
    public GameObject blueDestroyEffect;
    public GameObject redDestroyEffect;

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
    public GameObject optionPanel;
    public TextMeshProUGUI selectText;
    int selectPageIdx;
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

    [Header("---------------[Book]")]
    public GameObject[] groupPrefabsInBook;
    public int startIdxInBook;
    public int groupNumInBook;
    public GameObject lastButtonInBook;
    public Image[] buttonImageInBook;
    public TextMeshProUGUI[] buttonNameInBook;
    public int infoPageIdx;
    public string groupNameInInfo;
    public GameObject unitInfoSet;
    public Image infoImage;
    public TextMeshProUGUI infoUnitNameText;
    public TextMeshProUGUI infoUnitTypeText;
    public TextMeshProUGUI infoCostText;
    public TextMeshProUGUI infoHpText;
    public TextMeshProUGUI infoAtkText;
    public TextMeshProUGUI infoAtsText;
    public TextMeshProUGUI infoRanText;
    public TextMeshProUGUI infoSpdText;
    public TextMeshProUGUI infoSkillText;
    public TextMeshProUGUI infoDetailText;
    public Sprite[] unitSprites;
    float scaleNum;

    [Header("---------------[Result]")]
    public GameObject overSet;
    public GameObject controlSet;
    public GameObject victoryObj;
    public GameObject defeatObj;

    [Header("---------------[Devil]")]
    public bool isDevilB;
    public bool isDevilR;
    public float devilBTimer;
    public float devilRTimer;
    public bool isDevilBAttack;
    public bool isDevilRAttack;

    [Header("---------------[Camera]")]
    public Transform camTrans;
    public float camSpeed;
    public bool isMove;

    void Awake()
    {
        instance = this;
        isGameLive = false;

        // Base
        blueHpText.text = blueHP.ToString();
        blueHpShadowText.text = blueHP.ToString();
        redHpText.text = redHP.ToString();
        redHpShadowText.text = redHP.ToString();
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
                teamBluePrefabs = ObjectManager.instance.juFok_prefabs;
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
            blueButtonImage[i - startBlueIdx].sprite = spriteRen.sprite;
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
                teamRedPrefabs = ObjectManager.instance.juFok_prefabs;
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
        isGameLive = true;
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
        GetUnitObject(teamBlueName, idx, unitB.transform.position);
        blueUnitList.Add(unitB);
    }
    public void MakeRedUnit(int idx)
    {
        GameObject unitR = teamRedPrefabs[idx];
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
        GetUnitObject(teamRedName, idx, unitR.transform.position);
        redUnitList.Add(unitR);

        // 생성 후에 패턴인덱스 변경
        patternIdx = Random.Range(0, 3);
    }
    void GetUnitObject(string teamName, int idx, Vector3 pos)
    {
        switch (teamName)
        {
            case "지하A":
            case "지하B":
                ObjectManager.instance.GetGiHa(idx, pos);
                break;
            case "주폭":
                ObjectManager.instance.GetJuFok(idx, pos);
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
    public void ResetButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }

    void Update()
    {
        if (!isGameLive)
            return;

        // Timer
        gameTimer += Time.deltaTime;

        // Devil
        DevilTimer();
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


    // ======================================================= Update 함수
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
        if (Input.GetKeyDown(KeyCode.Q))
            MakeBlueUnit(0 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.W))
            MakeBlueUnit(1 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.E))
            MakeBlueUnit(2 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.A))
            MakeBlueUnit(3 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.S))
            MakeBlueUnit(4 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.D) && groupBlueNum == 6)
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
    void UnitInfo(Unit unitLogic)
    {
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

    // ======================================================= Base 함수
    public void BaseHit(int dmg, int layer)
    {
        if (layer == 8)
        {
            // 데미지만큼 감소
            blueHP -= dmg;

            // Death
            if (blueHP <= 0)
            {
                blueHP = 0;
                blueBaseSpriteRen.sprite = blueBaseSprite[2];
                blueDestroyEffect.SetActive(true);
                isGameLive = false;
                GameOver("Lose");
            }
            // Alive
            else
            {
                blueBaseSpriteRen.sprite = blueBaseSprite[1];
                StartCoroutine(SpriteChange(0.1f, blueBaseSpriteRen, blueBaseSprite[0]));
            }

            // Text
            blueBaseSlider.value = blueHP;
            blueHpText.text = blueHP.ToString();
            blueHpShadowText.text = blueHP.ToString();
        }
        else if (layer == 9)
        {
            redHP -= dmg;

            if (redHP <= 0)
            {
                redHP = 0;
                redBaseSpriteRen.sprite = redBaseSprite[2];
                redDestroyEffect.SetActive(true);
                GameOver("Win");
            }
            else
            {
                redBaseSpriteRen.sprite = redBaseSprite[1];
                StartCoroutine(SpriteChange(0.1f, redBaseSpriteRen, redBaseSprite[0]));
            }

            redBaseSlider.value = redHP;
            redHpText.text = redHP.ToString();
            redHpShadowText.text = redHP.ToString();
        }
    }
    IEnumerator SpriteChange(float time, SpriteRenderer spriteRen, Sprite sprite)
    {
        yield return new WaitForSeconds(time);

        spriteRen.sprite = sprite;
    }
    void GameOver(string result)
    {
        // 이겼을 경우
        if (result == "Win")
        {
            victoryObj.SetActive(true);
        }
        // 졌을 경우
        else if (result == "Lose")
        {
            defeatObj.SetActive(true);
        }

        // 게임 작동 중지 (시간, 코스트, 카메라이동, 소환버튼, 유닛클릭)
        isGameLive = false;
        controlSet.SetActive(false);
        overSet.SetActive(true);
    }

    // ======================================================= Book 함수
    public void TeamSelectInBook(string teamName)
    {
        groupNameInInfo = teamName;
        switch (teamName)
        {
            case "지하A":
                groupPrefabsInBook = ObjectManager.instance.giHa_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 5;
                lastButtonInBook.SetActive(false);
                break;
            case "지하B":
                groupPrefabsInBook = ObjectManager.instance.giHa_prefabs;
                startIdxInBook = 10;
                groupNumInBook = 5;
                lastButtonInBook.SetActive(false);
                break;
            case "주폭":
                groupPrefabsInBook = ObjectManager.instance.juFok_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 6;
                lastButtonInBook.SetActive(true);
                break;
            case "박취A":
                groupPrefabsInBook = ObjectManager.instance.bakChi_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 6;
                lastButtonInBook.SetActive(true);
                break;
            case "박취B":
                groupPrefabsInBook = ObjectManager.instance.bakChi_prefabs;
                startIdxInBook = 12;
                groupNumInBook = 5;
                lastButtonInBook.SetActive(false);
                break;
            case "V급":
                groupPrefabsInBook = ObjectManager.instance.vBand_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 5;
                lastButtonInBook.SetActive(false);
                break;
        }

        for (int i= startIdxInBook; i< startIdxInBook + groupNumInBook; i++)
        {
            Unit bookUnit = groupPrefabsInBook[i].GetComponent<Unit>();
            SpriteRenderer spriteRen = bookUnit.GetComponent<SpriteRenderer>();
            buttonImageInBook[i - startIdxInBook].sprite = spriteRen.sprite;
            buttonNameInBook[i - startIdxInBook].text = bookUnit.unitName;
        }

    }
    public void UnitInfoButton(int idx)
    {
        infoPageIdx = idx;
        // groupPrefabsInBook은 미리 생성됨
        Unit infoUnit = groupPrefabsInBook[startIdxInBook + infoPageIdx].GetComponent<Unit>();
        SpriteRenderer spriteRen = infoUnit.GetComponent<SpriteRenderer>();

        // Unit 정보 업데이트
        //infoImage.sprite = spriteRen.sprite;
        infoImage.sprite = UnitImage(idx);
        infoImage.SetNativeSize();
        infoImage.transform.localScale = Vector3.one * scaleNum;
        infoUnitNameText.text = infoUnit.unitName;
        TypeTextSetting(infoUnitTypeText, infoUnit.unitType);
        infoCostText.text = $"코인 : {infoUnit.unitCost}";
        infoHpText.text = $"체력 : {infoUnit.unitMaxHp}";
        infoAtkText.text = $"공격 : {infoUnit.unitAtk}";
        infoAtsText.text = $"공격속도 : {infoUnit.unitAtkSpeed}";
        infoRanText.text = $"공격범위 : {infoUnit.unitRange}";
        infoSpdText.text = $"이동속도 : {infoUnit.unitSpeed * 10}";
        infoSkillText.text = UnitSkillText(infoPageIdx);
        infoDetailText.text = UnitDetailText(infoPageIdx);

        unitInfoSet.SetActive(true);
    }
    Sprite UnitImage(int idx)
    {
        Sprite image = null;

        switch (groupNameInInfo)
        {
            case "지하A":
                switch (idx)
                {
                    case 0:
                        image = unitSprites[0];
                        break;
                    case 1:
                        image = unitSprites[1];
                        break;
                    case 2:
                        image = unitSprites[2];
                        break;
                    case 3:
                        image = unitSprites[3];
                        break;
                    case 4:
                        image = unitSprites[4];
                        break;
                }
                scaleNum = 0.18f;
                break;
            case "지하B":
                switch (idx)
                {
                    case 0:
                        image = unitSprites[5];
                        break;
                    case 1:
                        image = unitSprites[6];
                        break;
                    case 2:
                        image = unitSprites[7];
                        break;
                    case 3:
                        image = unitSprites[8];
                        break;
                    case 4:
                        image = unitSprites[9];
                        break;
                }
                scaleNum = 0.18f;
                break;
            case "주폭":
                switch (idx)
                {
                    case 0:
                        image = unitSprites[10];
                        break;
                    case 1:
                        image = unitSprites[11];
                        break;
                    case 2:
                        image = unitSprites[12];
                        break;
                    case 3:
                        image = unitSprites[13];
                        break;
                    case 4:
                        image = unitSprites[14];
                        break;
                    case 5:
                        image = unitSprites[15];
                        break;
                }
                scaleNum = 0.5f;
                break;
            case "박취A":
                switch (idx)
                {
                    case 0:
                        image = unitSprites[16];
                        break;
                    case 1:
                        image = unitSprites[17];
                        break;
                    case 2:
                        image = unitSprites[18];
                        break;
                    case 3:
                        image = unitSprites[19];
                        break;
                    case 4:
                        image = unitSprites[20];
                        break;
                    case 5:
                        image = unitSprites[21];
                        break;
                }
                scaleNum = 0.25f;
                break;
            case "박취B":
                switch (idx)
                {
                    case 0:
                        image = unitSprites[22];
                        break;
                    case 1:
                        image = unitSprites[23];
                        break;
                    case 2:
                        image = unitSprites[24];
                        break;
                    case 3:
                        image = unitSprites[25];
                        break;
                    case 4:
                        image = unitSprites[26];
                        break;
                }
                scaleNum = 0.25f;
                break;
            case "V급":
                switch (idx)
                {
                    case 0:
                        image = unitSprites[27];
                        break;
                    case 1:
                        image = unitSprites[28];
                        break;
                    case 2:
                        image = unitSprites[29];
                        break;
                    case 3:
                        image = unitSprites[30];
                        break;
                    case 4:
                        image = unitSprites[31];
                        break;
                }
                scaleNum = 0.5f;
                break;
        }
        return image;
    }
    string UnitSkillText(int idx)
    {
        string skillText = "";

        switch (groupNameInInfo)
        {
            case "지하A":
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 탱커.";
                        break;
                    case 1:
                        skillText = "공격에 적중당한 적 2초간 공격력 5 감소.\n(중첩불가)";
                        break;
                    case 2:
                        skillText = "HP가 적어질수록 공격속도 증가.\n(최대치 : 0.3)";
                        break;
                    case 3:
                        skillText = "범위 내의 아군 전체에게 공격속도 2배 증가.\n(중첩불가)";
                        break;
                    case 4:
                        skillText = "적 전체에게 피해를 입힘.\n(공격속도 고정)\n(중복 소환 불가)";
                        break;
                }
                break;
            case "지하B":
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 전사.";
                        break;
                    case 1:
                        skillText = "적에게 피해받을 때마다 적의 공격력의 절반만큼 피해를 되돌려줌.\n(근접 적만 해당)";
                        break;
                    case 2:
                        skillText = "공격에 적중당한 적 2초간 공격속도 2배 감소.\n(중첩불가)";
                        break;
                    case 3:
                        skillText = "범위 내 아군 한명에게 10만큼 힐.";
                        break;
                    case 4:
                        skillText = "적을 킬하면 최대체력 10, 공격력 5, 공격속도 0.1 증가\n(공격속도 최대치 : 0.3)";
                        break;
                }
                break;
            case "주폭":
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 전사.";
                        break;
                    case 1:
                        skillText = "범위 내 아군 전체에게 이동속도 10 증가.\n(중첩불가)";
                        break;
                    case 2:
                        skillText = "자신이 받는 모든 피격데미지 3 감소.";
                        break;
                    case 3:
                        skillText = "광역 원거리 공격.";
                        break;
                    case 4:
                        skillText = "적이 본인과 근접범위까지 오면 백스탭.";
                        break;
                    case 5:
                        skillText = "본인이 처음 공격하기 전까지 무적.";
                        break;
                }
                break;
            case "박취A":
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 탱커.";
                        break;
                    case 1:
                        skillText = "광역 근접 공격.";
                        break;
                    case 2:
                        skillText = "3.5초마다 1코인씩 증가.";
                        break;
                    case 3:
                        skillText = "공격 시 본인의 공격력만큼 체력 흡혈.";
                        break;
                    case 4:
                        skillText = "일반적인 원딜.\n(가장 긴 사거리 보유)";
                        break;
                    case 5:
                        skillText = "일정 범위 내에 적이 있다면 빠르게 적에게 이동 후 광역으로 자폭.";
                        break;
                }
                break;
            case "박취B":
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 탱커.";
                        break;
                    case 1:
                        skillText = "3번째 공격마다 추가 데미지 +5.";
                        break;
                    case 2:
                        skillText = "공격할 때마다 공격속도 증가.\n(최대 0.3)\n(이동 시 초기화)";
                        break;
                    case 3:
                        skillText = "범위 내 아군 전체에게 공격력 5 증가.\n(중첩불가)";
                        break;
                    case 4:
                        skillText = "공격 시 체력 50이하 적은 확정 킬.";
                        break;
                }
                break;
            case "V급":
                switch (idx)
                {
                    case 0:
                        skillText = "일반적인 전사.";
                        break;
                    case 1:
                        skillText = "공격 시 적을 밀쳐냄.";
                        break;
                    case 2:
                        skillText = "공격에 적중당한 적 2초간 이동속도 4 감소.\n(중첩불가)";
                        break;
                    case 3:
                        skillText = "범위 내 원거리 공격 무효.";
                        break;
                    case 4:
                        skillText = "원거리유닛만 공격 가능. 근접유닛은 공격할 수 없음. 피격당해도 멈추지 않음.";
                        break;
                }
                break;
        }

        return skillText;
    }
    string UnitDetailText(int idx)
    {
        string detailText = "";

        switch (groupNameInInfo)
        {
            case "지하A":
                switch (idx)
                {
                    case 0:
                        detailText = "머리에 제트플립이 달린 여성. 종족은 인간이 아니고 핸드폰이라고 한다. 특이사항은 말할때마다 핸드폰 화면이 열리는 모습.";
                        break;
                    case 1:
                        detailText = "지하돌의 취지에 가장 적합한 메인보컬. 특수한 발성덕분에 모두의 이목을 집중시킨다. 실제 하와와 여고생이 맞다고 한다.";
                        break;
                    case 2:
                        detailText = "말솜씨가 뛰어나지만 비밀이모와 비슷한 말투덕에 고모라는 별명으로 불린다. 가끔 폭주하는 모습때문에 주변 사람들이 놀라곤 한다.";
                        break;
                    case 3:
                        detailText = "개성이 강한 지하돌에서 일반인으로 특수하게 합격했다. 모두에게 상냥하며 노력하는 리더의 모습으로 매력을 뽐내고 있다.";
                        break;
                    case 4:
                        detailText = "자칭 마계에서 온 마왕님으로 특이한 말투와 허당끼가 있다. 본인의 마음에 들지 않는 사람에겐 저주를 내린다.";
                        break;
                }
                break;
            case "지하B":
                switch (idx)
                {
                    case 0:
                        detailText = "공대아싸녀. 많은 사람들을 만나는 걸 힘들어하며 체력이 좋지 않다. 항상 오브젝트라는 비둘기와 함께 다닌다.";
                        break;
                    case 1:
                        detailText = "띠뜨나라의 띠뜨공주. 그녀만의 억센 발음을 알아듣기 힘든 점이 특징. 피지컬이 뛰어나고 탱커지만 받아치는 능력이 좋다.";
                        break;
                    case 2:
                        detailText = "매우 힘빠지는 공기 100% 창법을 가지고 있다. 하지만 딜을 넣을 때는 강하게 말하는 한 방을 가지고 있다.";
                        break;
                    case 3:
                        detailText = "대한외국인으로 어눌하지만 한국어를 잘 구사한다. 모두에게 밝은 기운을 불어넣는 치유계열 멤버이며 생각보다 키가 크다.";
                        break;
                    case 4:
                        detailText = "이중인격 얀데레 봇치. 평소에는 조용하고 소심한 모습이지만 돌변하면 매우 무서운 얀데레의 모습을 보이는 특징이 있다.";
                        break;
                }
                break;
            case "주폭":
                switch (idx)
                {
                    case 0:
                        detailText = "주폭소년단의 리더. 정열적이며 활기찬 성격덕분에 팀원들을 조율하거나 여러 활동에 참여하는 모습을 보인다.";
                        break;
                    case 1:
                        detailText = "연하남 계열의 아이돌이지만 많이 연하인 것이 특징. 상당히 많은 여자들이 오이쿤을 노리고 있다는 소문이 존재한다.";
                        break;
                    case 2:
                        detailText = "쾌활한 학생회장 속성과 웃을 때 \"윤하하하\"하며 호탕하게 웃는 모습이 특징이다. 의외로 카사노바 기질이 있는 것 같다.";
                        break;
                    case 3:
                        detailText = "몸이 안좋은 병약미소년. 빈혈이 심해서 갑자기 쓰러지기도 하지만 이런 모습덕에 팬들에게서 모성애를 자극시킨다.";
                        break;
                    case 4:
                        detailText = "사연이 많아보이는 남자. 여자가 다가오면 소스라치게 놀라며 도망친다. 썰렁한 개그를 치거나 가끔 이상한 말을 할 때가 있다.";
                        break;
                    case 5:
                        detailText = "버터음색과 자상한 목소리로 많은 여심을 홀리는 모습을 보여준다. 현재는 군대이슈로 인해 잠시 활동을 중단한 상태이다.";
                        break;
                }
                break;
            case "박취A":
                switch (idx)
                {
                    case 0:
                        detailText = "억울한 듯한 말투를 가지고 있으며 눈물이 많은 소녀. 반응이 좋아 놀림받을 때가 많지만 가끔씩 날카로운 일침을 날리기도 한다.";
                        break;
                    case 1:
                        detailText = "평소에는 과묵하며 소심하지만 실제로는 근력과 체력이 매우 강하다. 또한 가면을 쓰면 성격이 거칠어지는 모습을 보인다.";
                        break;
                    case 2:
                        detailText = "재벌집 아가씨. 사립학교는 재미가 없어서 친구들을 만들기 위해 전학왔다. 은근 싸이코패스같은 면모를 보이기도 한다.";
                        break;
                    case 3:
                        detailText = "박취더락의 광대 포지션으로 가끔씩 약 처방이 필요하다. 상대방을 흡수하고 표절하는 것과 배신의 아이콘이라는 특징이 있다.";
                        break;
                    case 4:
                        detailText = "본명은 베르노아르시온느브리취더루왁. 나라의 공주이며 말끝에 \"노라\"를 붙이는 특이한 말투를 쓴다. 총을 굉장히 잘 쏜다.";
                        break;
                    case 5:
                        detailText = "잔잔하고 안정감있는 목소리를 가지고 있지만 한순간에 폭주할 때가 있다. 자타공인 데드 아티스트.";
                        break;
                }
                break;
            case "박취B":
                switch (idx)
                {
                    case 0:
                        detailText = "무려 8년을 유급한 복학생 언니/누님. 연륜이 느껴지는 여유있는 말투와 행동이 특징이다. 항상 체력이 부족해 힘들어한다.";
                        break;
                    case 1:
                        detailText = "지방에서 상경한 시골소녀로 구수한 사투리를 쓴다. 자칭 섹시호소인이지만 아무도 인정해주지 않는다.";
                        break;
                    case 2:
                        detailText = "Say-No Crystal(줄여서 크짱)이라는 이름의 기타를 가진 소녀. 귀여움 뒤에는 파워풀한 연주실력을 가지고 있다.";
                        break;
                    case 3:
                        detailText = "봇치처럼 소심하지만 의외로 할말은 하는 성격이다. 인싸처럼 보이고 싶다는 이유로 고글을 착용하고 다닌다.";
                        break;
                    case 4:
                        detailText = "노는걸 좋아하는 인싸 여고생. SNS를 즐겨하며 춤을 좋아한다. 노래도 좋아하는 것 같지만 잘하는 편은 아닌 것 같다.";
                        break;
                }
                break;
            case "V급":
                switch (idx)
                {
                    case 0:
                        detailText = "나긋나긋한 기타리스트. 실용음악과를 나왔으며 뛰어난 실력때문에 V급밴드에 위장취업한 것이 아니냐는 말을 자주 듣는다.";
                        break;
                    case 1:
                        detailText = "캐나다에서 온 드러머. 해외파답게 영어실력 좋다. 호기심이 많으며 어디로 튈지 모르는 매력을 가지고 있다.";
                        break;
                    case 2:
                        detailText = "비충베(비챤의 충실한 베이시스트). 겁도 많고 억까도 많지만 V급밴드에서 비타민같이 활기를 넣어주는 역할을 한다.";
                        break;
                    case 3:
                        detailText = "성장형 초보 피아니스트. 실력파 부원들이 많아 열심히 노력하며 실력을 키우고 있다. 이름덕에 대상현이라고 불리고 있다.";
                        break;
                    case 4:
                        detailText = "V급밴드의 마스코트. 고양이지만 매우 똑똑하며 사람말을 한다는 소문이 있다. 예민한 성격때문에 갑자기 다가가면 도망간다.";
                        break;
                }
                break;
        }

        return detailText;
    }
    public void LeftInfoButton()
    {
        // 인덱스 증가
        infoPageIdx--;
        // 0보다 작아질 경우 그룹의 최대값-1 으로 변경
        infoPageIdx = infoPageIdx < 0 ? groupNumInBook - 1 : infoPageIdx;
        // 정보 업데이트
        UnitInfoButton(infoPageIdx);
    }
    public void RightInfoButton()
    {
        // 인덱스 감소
        infoPageIdx++;
        // 그룹의 최대값-1보다 커질 경우 0 으로 변경
        infoPageIdx = infoPageIdx > groupNumInBook - 1 ? 0 : infoPageIdx;
        // 정보 업데이트
        UnitInfoButton(infoPageIdx);
    }

    // ======================================================= COM 함수
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
            // 9코스트 이상일 때, rand값이 0이면 1번패턴, 1이면 4번유닛을 소환
            case 2:
                if (redCost >= 9)
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

    IEnumerator PatternBA(float time, int typeNum)
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
            // 9코스트 이상일 때, rand값이 0이면 1번패턴, 1이면 4번유닛을 소환
            case 2:
                if (redCost >= 9)
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
