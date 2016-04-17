using UnityEngine;
using System.Collections;

public class CardHitEffect : CardVisualizeEffect
{
    private int m_CurrentVibrateCount = 0;
    private CardVisualizeEffectData.CardVisualizeEffectHit m_HitEffectData = null;

    public override IEnumerator UpdateVisualizeEffect()
    {
        if(m_EffectTarget != null)
        {
            if(m_HitEffectData == null)
            {
                m_HitEffectData = (CardVisualizeEffectData.CardVisualizeEffectHit)m_VisualizeEffectData;
            }

            if (m_CurrentVibrateCount < m_HitEffectData.m_VibrationCount)
            {
                m_VisualizeStatus = VisualizeEffectStatus.Progress;
                if (m_CurrentVibrateCount % 2 == 0)
                {
                    m_EffectTarget.transform.position = new Vector3(m_EffectStartPosition.x,
                        m_EffectStartPosition.y + m_HitEffectData.m_DistanceToSource, m_EffectStartPosition.z);
                }
                else
                {
                    m_EffectTarget.transform.position = new Vector3(m_EffectStartPosition.x,
                        m_EffectStartPosition.y - m_HitEffectData.m_DistanceToSource, m_EffectStartPosition.z);
                }

                ++m_CurrentVibrateCount;
            }
            else
            {
                m_EffectTarget.transform.position = new Vector3(m_EffectStartPosition.x,
                    m_EffectStartPosition.y, m_EffectStartPosition.z);
                m_VisualizeStatus = VisualizeEffectStatus.End;
                m_EffectTarget = null;
            }
        }

        yield return null;
    }

    public override EffectType GetVisualizeEffectType()
    {
        return EffectType.HitEffect;
    }

    public override void ForceQuitVisualizeEffect()
    {
        m_EffectTarget.transform.position = new Vector3(m_EffectStartPosition.x, m_EffectStartPosition.y, m_EffectStartPosition.z);
    }
}
