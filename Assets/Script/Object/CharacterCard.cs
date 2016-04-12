using UnityEngine;
using System.Collections;

public class CharacterCard : MonoBehaviour {
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
    private int m_PlacedRowNumber = 0;
    public bool IsPlayerTeam
    {
        get { return m_IsPlayerTeam; }
        set { m_IsPlayerTeam = value; }
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

    public int CurrentHealthPoint
    {
        get { return m_CurrentHealthPoint; }
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
        ResetStatus();
        StartCoroutine(UpdateCardStatus());
    }

    public void AttackCard(CharacterCard targetCard)
    {
        targetCard.m_CurrentHealthPoint -= m_AttackPoint;
        targetCard.StartCoroutine(targetCard.UpdateCardStatus());
        Debug.Log("Health Point = " + targetCard.m_CurrentHealthPoint);
    }

    public IEnumerator UpdateCardStatus()
    {
        m_HealthPointText.text = m_CurrentHealthPoint.ToString();
        m_AttackPointText.text = m_AttackPoint.ToString();
        m_SpeedText.text = m_Speed.ToString();

        yield return null;
    }

    public void ResetStatus()
    {
        m_CurrentHealthPoint = m_HealthPoint;
    }
}
