using UnityEngine;
using System.Collections;

public class SkillCardWorker : MonoBehaviour
{
    public IEnumerator Run()
    {
        Debug.Log("Is SkillCardWorker Checked. ");

        yield return null;
    }
}
