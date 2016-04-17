using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIControlWorker : MonoBehaviour
{
    public delegate void SkillButtonListener(int skillIndex);
    public delegate void BattleStyleChangeListener(bool isBattleAutomatic);

    private bool m_IsBattleAutomatic = false;

    private Button m_Skill1Button = null;
    private Button m_Skill2Button = null;
    private Button m_BattleStyleButton = null;

    private Text m_PhaseTitleText = null;
    private Text m_PhaseCountText = null;
    private Text m_BattleStyleText = null;

    private RectTransform m_CrystalBackground = null;

    private SkillButtonListener m_SkillButtonListener = null;
    private BattleStyleChangeListener m_BattleStyleChangeListener = null;

    private void Awake()
    {
        m_Skill1Button = GameObject.Find("Skill1_Button").GetComponent<Button>();
        m_Skill2Button = GameObject.Find("Skill2_Button").GetComponent<Button>();
        m_BattleStyleButton = GameObject.Find("BattleStyleButton").GetComponent<Button>();

        m_PhaseTitleText = GameObject.Find("PhaseTitleText").GetComponent<Text>();
        m_PhaseCountText = GameObject.Find("PhaseCountText").GetComponent<Text>();
        m_BattleStyleText = GameObject.Find("BattleStyleText").GetComponent<Text>();

        m_CrystalBackground = GameObject.Find("CrystalBackgroundUI").GetComponent<RectTransform>();

        m_Skill1Button.onClick.AddListener(() =>
        {
            Debug.Assert(m_SkillButtonListener != null, "Please Register SkillButtonListener.");
            m_SkillButtonListener(1);
        });
        m_Skill2Button.onClick.AddListener(() =>
        {
            Debug.Assert(m_SkillButtonListener != null, "Please Register SkillButtonListener.");
            m_SkillButtonListener(2);
        });
        m_BattleStyleButton.onClick.AddListener(() =>
        {
            if(m_IsBattleAutomatic)
            {
                m_IsBattleAutomatic = false;
                m_BattleStyleText.text = "수동";
                m_BattleStyleChangeListener(m_IsBattleAutomatic);
            }
            else
            {
                m_IsBattleAutomatic = true;
                m_BattleStyleText.text = "자동";
                m_BattleStyleChangeListener(m_IsBattleAutomatic);
            }
        });
    }

    public void RegisterSkillButtonListener(SkillButtonListener skillBtnListener)
    {
        m_SkillButtonListener = skillBtnListener;
    }

    public void RegisterBattleStyleChangeListener(BattleStyleChangeListener battleStyleChangeListener)
    {
        m_BattleStyleChangeListener = battleStyleChangeListener;
    }

    public void SetPhaseText(int phaseCount)
    {
        m_PhaseCountText.text = phaseCount.ToString();
    }

    public IEnumerator Run()
    {
        yield return null;
    }
}
