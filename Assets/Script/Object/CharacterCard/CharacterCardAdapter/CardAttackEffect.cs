using UnityEngine;
using System.Collections;

public class CardAttackEffect : CardVisualizeEffect
{
    public override IEnumerator UpdateVisualizeEffect()
    {
        if (m_EffectTarget != null)
        {
            if (m_CurrentElapsedTime < m_VisualizeEffectData.m_EffectTime)
            {
                float actionExecuteThreshold = m_VisualizeEffectData.m_EffectTime * 0.3f;
                if (m_CurrentElapsedTime < actionExecuteThreshold)
                {
                    m_VisualizeStatus = VisualizeEffectStatus.Start;
                    float lerpTime = m_CurrentElapsedTime / actionExecuteThreshold;
                    float lerpedPosition_Y = Mathf.Lerp(m_EffectStartPosition.y, m_EffectStartPosition.y + m_VisualizeEffectData.m_DistanceToSource, lerpTime);

                    m_EffectTarget.transform.position = new Vector3(m_EffectStartPosition.x, lerpedPosition_Y, m_EffectStartPosition.z);
                }
                else
                {
                    switch (m_VisualizeStatus)
                    {
                        case VisualizeEffectStatus.Start:
                            {
                                m_VisualizeStatus = VisualizeEffectStatus.Execute;
                            }
                            break;
                        case VisualizeEffectStatus.Execute:
                            {
                                m_VisualizeStatus = VisualizeEffectStatus.Progress;
                            }
                            break;
                        default:
                            { }
                            break;

                    }

                    float lerpTime = m_CurrentElapsedTime / (m_VisualizeEffectData.m_EffectTime - actionExecuteThreshold);
                    float lerpedPosition_Y = Mathf.Lerp(m_EffectStartPosition.y + m_VisualizeEffectData.m_DistanceToSource, m_EffectStartPosition.y, lerpTime);

                    m_EffectTarget.transform.position = new Vector3(m_EffectStartPosition.x, lerpedPosition_Y, m_EffectStartPosition.z);
                }
                m_CurrentElapsedTime += Time.deltaTime;
            }
            else
            {
                m_EffectTarget = null;
                m_CurrentElapsedTime = 0.0f;
                m_VisualizeStatus = VisualizeEffectStatus.End;
            }
        }

        yield return null;
    }

    public override EffectType GetVisualizeEffectType()
    {
        return EffectType.AttackEffect;
    }
}
