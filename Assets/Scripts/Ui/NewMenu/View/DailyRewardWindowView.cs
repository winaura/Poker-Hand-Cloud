using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardWindowView : MonoBehaviour
{
    [SerializeField] private Button _claimButton;
    [SerializeField] private Text _chipsRewardText;
    [SerializeField] private Text _descriptionText;
    [SerializeField] private Text _dailyRewardText;
    [SerializeField] private Text _claimButtonText;
    


    public static void CreateWindow(string description, string rewardText, string claimButtonText, string chipsRewardText, int sortingOrder = 0)
    {
        DailyRewardWindowView rewardWindow = Instantiate(Resources.Load<DailyRewardWindowView>("DailyRewardWindow"));
        rewardWindow.GetComponent<Canvas>().sortingOrder = sortingOrder;
        rewardWindow._claimButton.onClick.AddListener(() => Destroy(rewardWindow.gameObject));
        rewardWindow._chipsRewardText.text = chipsRewardText;
        rewardWindow._descriptionText.text = description;
        rewardWindow._dailyRewardText.text = rewardText;
        rewardWindow._claimButtonText.text = claimButtonText;
    }
}