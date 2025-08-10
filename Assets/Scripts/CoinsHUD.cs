using TMPro;
using UnityEngine;

public class CoinsHUD : MonoBehaviour
{
    private TextMeshProUGUI coinsText;

    void Awake()
    {
        coinsText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        coinsText.text = $"Coins: {ScoreManager.runCoins}";
    }
}
