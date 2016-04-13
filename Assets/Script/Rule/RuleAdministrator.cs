using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Constants;

public class RuleAdministrator : MonoBehaviour
{
    [System.Serializable]
    public struct CardDeckSettingData
    {
        public int RowNumber;
        public GameObject CardPrefab;
    };

    public Transform m_FirstPlayerRowMidPosition = null;
    public Transform m_SecondPlayerRowMidPosition = null;
    public Transform m_ThirdPlayerRowMidPosition = null;

    public Transform m_FirstEnemyRowMidPosition = null;
    public Transform m_SecondEnemyRowMidPosition = null;
    public Transform m_ThirdEnemyRowMidPosition = null;

    public Transform m_FirstEnemyCharacterRowMidPosition = null;
    public Transform m_SecondEnemyCharacterRowMidPosition = null;
    public Transform m_ThirdEnemyCharacterRowMidPosition = null;

    public List<CardDeckSettingData> m_PlayerSideCardSettingList = new List<CardDeckSettingData>();
    public List<CardDeckSettingData> m_EnemySideCardSettingList = new List<CardDeckSettingData>();

    private List<CharacterCard> m_PlayerCardCharacterList = new List<CharacterCard>();
    private List<CharacterCard> m_EnemyCardCharacterList = new List<CharacterCard>();

    private CardStatusCheckWorker m_CardStatusCheckWorker = null;
    private CardMovementWorker m_CardMovementWorker = null;
    private RuleCheckWorker m_RuleCheckWorker = null;
    private SkillCardWorker m_SkillCardWorker = null;

    private bool isGameFinished = false;

    void Awake()
    {
        Debug.Assert(m_PlayerSideCardSettingList.Count <= ConstantDefine.MAX_CARD_DECK_COUNT, "List Size Should be Under 6.");
        Debug.Assert(m_EnemySideCardSettingList.Count <= ConstantDefine.MAX_CARD_DECK_COUNT, "List Size Should be Under 6.");

        m_CardStatusCheckWorker = gameObject.AddComponent<CardStatusCheckWorker>();
        m_CardMovementWorker = gameObject.AddComponent<CardMovementWorker>();
        m_RuleCheckWorker = gameObject.AddComponent<RuleCheckWorker>();
        m_SkillCardWorker = gameObject.AddComponent<SkillCardWorker>();

        initCards();

        InitCardMovementWorker();
        InitRuleCheckWorker();
    }

    void Update()
    {
        StartCoroutine(m_RuleCheckWorker.Run());
        StartCoroutine(m_CardStatusCheckWorker.Run());

        if(!isGameFinished)
        {
            StartCoroutine(m_CardMovementWorker.Run());
            StartCoroutine(m_SkillCardWorker.Run());
        }
    }

    void OnDestroy()
    {
        m_CardStatusCheckWorker = null;
        m_CardMovementWorker = null;
        m_RuleCheckWorker = null;
        m_SkillCardWorker = null;
    }

    private void initCards()
    {
        initCard(m_PlayerSideCardSettingList, m_PlayerCardCharacterList, true);
        initCard(m_EnemySideCardSettingList, m_EnemyCardCharacterList, false);
    }

    private void initCard(List<CardDeckSettingData> settingDataList, List<CharacterCard> inGameDataList, bool isPlayerTeam)
    {
        ObjectPoolManager objectPoolMgr = GameObject.Find("CharacterCardPool").GetComponent<ObjectPoolManager>();

        for (int i = 0; i < settingDataList.Count; ++i)
        {
            CharacterCard settingCardPrefab = settingDataList[i].CardPrefab.GetComponent<CharacterCard>();
            CharacterCard inGameCardObject = objectPoolMgr.GetObject().GetComponent<CharacterCard>();

            // 카드에 나오는 캐릭터 스프라이트를 바꿔준다.
            SpriteRenderer settingPortraitSpr = settingCardPrefab.m_PortraitRenderer;
            SpriteRenderer inGamePortraitSpr = inGameCardObject.m_PortraitRenderer;
            inGamePortraitSpr.sprite = settingPortraitSpr.sprite;

            inGameCardObject.IsPlayerTeam = isPlayerTeam;
            inGameCardObject.RowNumber = m_PlayerSideCardSettingList[i].RowNumber;
            inGameCardObject.m_HealthPoint = settingCardPrefab.m_HealthPoint;
            inGameCardObject.m_AttackPoint = settingCardPrefab.m_AttackPoint;
            inGameCardObject.m_Speed = settingCardPrefab.m_Speed;
            inGameCardObject.m_AttackRange = settingCardPrefab.m_AttackRange;
            inGameCardObject.ResetStatus();

            StartCoroutine(inGameCardObject.UpdateCardStatus());

            inGameDataList.Add(inGameCardObject);
        }
    }

    private void InitCardMovementWorker()
    {
        Vector3[] playerCardPos = { m_FirstPlayerRowMidPosition.transform.position,
            m_SecondPlayerRowMidPosition.transform.position,
            m_ThirdPlayerRowMidPosition.transform.position };

        Vector3[] EnemyCardPos = { m_FirstEnemyRowMidPosition.transform.position,
            m_SecondEnemyRowMidPosition.transform.position,
            m_ThirdEnemyRowMidPosition.transform.position };

        Vector3[] EnemyCharacterPos = { m_FirstEnemyCharacterRowMidPosition.transform.position,
            m_SecondEnemyCharacterRowMidPosition.transform.position,
            m_ThirdEnemyCharacterRowMidPosition.transform.position };

        m_CardMovementWorker.PlayerCardPosition = playerCardPos;
        m_CardMovementWorker.EnemyCardPosition = EnemyCardPos;
        m_CardMovementWorker.EnemyCharacterPosition = EnemyCharacterPos;

        m_CardMovementWorker.InitCardPositionSlots();

        m_CardMovementWorker.AttachPlayerCardToSlot(ref m_PlayerCardCharacterList);
        m_CardMovementWorker.AttachEnemyCardToSlot(ref m_EnemyCardCharacterList);
    }

    private void InitRuleCheckWorker()
    {
        m_RuleCheckWorker.InitRuleCheckWorker(ref m_PlayerCardCharacterList, ref m_EnemyCardCharacterList);
        m_RuleCheckWorker.RegisterGameWinListener(
            (RuleCheckWorker.GameRuleState GameState, bool IsPlayerWin) =>
            {
                if (RuleCheckWorker.GameRuleState.GameEnd == GameState)
                {
                    if (!IsPlayerWin)
                    {
                        isGameFinished = true;
                    }
                    else
                    {
                        isGameFinished = true;
                    }
                }
            });
    }
}
