using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int runCoins { get; private set; }
    public static int bestRunCoins { get; private set; }

    public static event Action<int,int> CoinsChanged;

    void Awake()
    {
        runCoins = 0;
        bestRunCoins = PlayerPrefs.GetInt("BestRunCoins", 0);
        CoinsChanged?.Invoke(runCoins, bestRunCoins);
    }

    public static void AddCoin(int v = 1)
    {
        runCoins += v;

        if (runCoins > bestRunCoins)
        {
            bestRunCoins = runCoins;
            PlayerPrefs.SetInt("BestRunCoins", bestRunCoins);
            PlayerPrefs.Save();
        }

        CoinsChanged?.Invoke(runCoins, bestRunCoins);
    }
}
