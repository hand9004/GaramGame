using UnityEngine;
using System.Collections;

public class CharacterCard : MonoBehaviour {
    public enum CardStatus
    {
        Idle,
        InAction,
    }

    public int m_AttackRange = 0;
    public int m_HealthPoint = 0;
    public int m_AttackPoint = 0;
    public int m_Speed = 0;
    private int m_CurrentHealthPoint = 0;

    public SpriteRenderer m_PortraitRenderer = null;
    public TextMesh m_HealthPointText = null;
    public TextMesh m_AttackPointText = null;
    public TextMesh m_SpeedText = null;

    private bool m_IsPlayerTeam = false;
    private bool m_IsActionEnded = false;
    private int m_PlacedRowNumber = 0;
    private CardVisualizer m_CharacterCardVisualizer = null;
    private CharacterCard m_TargetCard = null;
    private CardStatus m_CardStatus = CardStatus.Idle;

    public bool IsPlayerTeam
    {
        get { return m_IsPlayerTeam; }
        set { m_IsPlayerTeam = value; }
    }

    public bool IsActionEnded
    {
        get { return m_IsActionEnded; }
        set { m_IsActionEnded = value; }
    }

    public int RowNumber
    {
        get
        {
            Debug.Assert(m_PlacedRowNumber > 0 && m_PlacedRowNumber <= 3, "RowNumber Should be Set. Range is 1 ~ 3.");
            return m_PlacedRowNumber;
        }
        set
        {
            Debug.Assert(value > 0 && value <= 3, "RowNumber Value Should be 1 ~ 3.");
            m_PlacedRowNumber = value;
        }
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
        this.m_AttackRange = sourceObject.m_AttackRange;
        this.m_HealthPoint = sourceObject.m_HealthPoint;
        this.m_AttackPoint = sourceObject.m_AttackPoint;
        this.m_Speed = sourceObject.m_Speed;

        this.m_PortraitRenderer = sourceObject.m_PortraitRenderer;
        this.m_HealthPointText = sourceObject.m_HealthPointText;
        this.m_AttackPointText = sourceObject.m_AttackPointText;
        this.m_SpeedText = sourceObject.m_SpeedText;

        this.m_IsPlayerTeam = sourceObject.m_IsPlayerTeam;
        this.m_PlacedRowNumber = sourceObject.m_PlacedRowNumber;
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
            m_IsActionEnded = true;
            m_CardStatus = CardStatus.Idle;
        });
    }

    void Update()
    {
        StartCoroutine(m_CharacterCardVisualizer.Run());
    }

    public void AttackCard(CharacterCard targetCard)
    {
        this.OnCardAttacking();
        targetCard.OnCardAttacked();
        m_TargetCard = targetCard;
    }

    public IEnumerator UpdateCardStatus()
    {
        m_HealthPointText.text = m_CurrentHealthPoint.ToString();
        m_AttackPointText.text = m_AttackPoint.ToString();
        m_SpeedText.text = m_Speed.ToString();

        yield return null;
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
}
