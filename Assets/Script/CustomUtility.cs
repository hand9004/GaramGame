using UnityEngine;
using System.Collections.Generic;

namespace CustomUtility
{
    class UtilityFunctions
    {
        // 배열에 넣은 확률에 따라 어느 영역이 랜덤에 의해 선택됬는지 알려준다.
        public static int GetWhereIsCorrect(int min, int max, float[] percentageDistributeArr)
        {
            // 배열 무결성 검사
            float totalPercentage = 0.0f;
            for (int i = 0; i < percentageDistributeArr.Length; ++i)
            {
                totalPercentage += percentageDistributeArr[i];
            }
            Debug.Assert(totalPercentage == 1.0f, "Total Percentage Should be 1.0f.");

            int selectedArea = -1;
            int pickedValue = UnityEngine.Random.Range(min, max);
            int randomValueLength = max - min;

            if (percentageDistributeArr.Length <= 2)
            {
                if (selectedArea <= min + (randomValueLength * percentageDistributeArr[0]))
                {
                    selectedArea = 0;
                }
                else
                {
                    selectedArea = 1;
                }
            }
            else
            {
                float prevPercentageCheckVal = 0.0f;
                float percentageCheckVal = randomValueLength * percentageDistributeArr[0];
                for (int i = 0; i < percentageDistributeArr.Length - 1; ++i)
                {
                    if (pickedValue < percentageCheckVal && pickedValue >= prevPercentageCheckVal)
                    {
                        selectedArea = i;
                        break;
                    }
                    else
                    {
                        prevPercentageCheckVal = percentageCheckVal;
                        percentageCheckVal += randomValueLength * percentageDistributeArr[i + 1];
                    }
                }
            }

            return selectedArea;
        }
    }
}
