using UnityEngine;
using System.Collections;

public class CharacterCard : MonoBehaviour {
    public enum CardStatus
    {
        Idle,
        InAction,
        TurnActionEnded,
    }

    public int m_HealthPoint = 0;
    public int m_AttackPoint = 0;
    public int m_Speed = 0;
    private int m_CurrentHealthPoint = 0;

    public SpriteRenderer m_PortraitRenderer = null;
    public TextMesh m_HealthPointText = null;
    public TextMesh m_AttackPointText = null;
    public TextMesh m_SpeedText = null;

    private bool m_IsPlayerTeam = false;
    private bool m_IsDefensiveCard = false;
    private CardVisualizer m_CharacterCardVisualizer = null;
    private CharacterCard m_TargetCard = null;
    private CardStatus m_CardStatus = CardStatus.Idle;

    public bool IsPlayerTeam
    {
        get { return m_IsPlayerTeam; }
        set { m_IsPlayerTeam = value; }
    }

    public bool IsDefensiveCard
    {
        get { return m_IsDefensiveCard; }
        set { m_IsDefensiveCard = value; }
    }

    public CardStatus CurrentCardStatus
    {
        get { return m_CardStatus; }
    }

    public int CurrentHealthPoint
    {
        get { return m_CurrentHealthPoint; }
        set { m_CurrentHealthPoint = value; }
    }

    public CharacterCard(ref CharacterCard sourceObject)
    {
        this.m_HealthPoint = sourceObject.m_HealthPoint;
        this.m_AttackPoint = sourceObject.m_AttackPoint;
        this.m_Speed = sourceObject.m_Speed;

        this.m_PortraitRenderer = sourceObject.m_PortraitRenderer;
        this.m_HealthPointText = sourceObject.m_HealthPointText;
        this.m_AttackPointText = sourceObject.m_AttackPointText;
        this.m_SpeedText = sourceObject.m_SpeedText;

        this.m_IsPlayerTeam = sourceObject.m_IsPlayerTeam;
    }

    void Awake()
    {
        m_CharacterCardVisualizer = gameObject.AddComponent<CardVisualizer>();
        m_CharacterCardVisualizer.RegisterOnActionActive((CardVisualizeEffect.EffectType effectType) =>
        {
            switch(effectType)
            {
                case CardVisualizeEffect.EffectType.AttackEffect:
                    {
                        if(m_TargetCard != null)
                        {
                            if(m_TargetCard.isActiveAndEnabled)
                            {
                                m_TargetCard.OnCardAttacked();
                                m_TargetCard.CurrentHealthPoint -= m_AttackPoint;
                                m_TargetCard.StartCoroutine(m_TargetCard.UpdateCardStatus());
                                m_TargetCard = null;
                            }
                        }
                    }
                    break;
                case CardVisualizeEffect.EffectType.HitEffect:
                    {
                        if (isActiveAndEnabled)
                        {
                            StartCoroutine(UpdateCardStatus());
                        }
                    }
                    break;
                default:
                    {}
                    break;
            }

        });

        m_CharacterCardVisualizer.RegisterOnActionEnded(() =>
        {
            m_CardStatus = CardStatus.TurnActionEnded;
        });
    }

    void Update()
    {
        StartCoroutine(m_CharacterCardVisualizer.Run());
    }

    public void AttackCard(CharacterCard targetCard)
    {
        this.OnCardAttacking();
        m_TargetCard = targetCard;
    }

    public IEnumerator UpdateCardStatus()
    {
        m_HealthPointText.text = m_CurrentHealthPoint.ToString();
        m_AttackPointText.text = m_AttackPoint.ToString();
        m_SpeedText.text = m_Speed.ToString();

        yield return null;
    }

    public void OnCardDoNothing()
    {
        m_CardStatus = CardStatus.TurnActionEnded;
    }

    public void OnCardAttacking()
    {
        m_CharacterCardVisualizer.OnCardAttacking();
        m_CardStatus = CardStatus.InAction;
    }

    public void OnCardAttacked()
    {
        m_CharacterCardVisualizer.OnCardAttacked();
        m_CardStatus = CardStatus.InAction;
    }

    public void ResetStatus()
    {
        m_CurrentHealthPoint = m_HealthPoint;

    }

    public void PhaseEndAction()
    {
        m_CardStatus = CardStatus.Idle;
    }
}
