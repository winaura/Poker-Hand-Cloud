using PokerHand.Common.Dto;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Client;

public enum RatingTabType { TopPlayers, You }
public enum RatingCoverageType { Global }

[DisallowMultipleComponent]
public class RatingWindow : MonoBehaviour
{
    [SerializeField] private RatingTab defaultRatingTab;
    [SerializeField] private CoverageTab defaultCoverageTab;
    [SerializeField] private RectTransform tabsContent;
    [SerializeField] private PlayerTab playerTabPrefab;
    [SerializeField] private GameObject loadingRing;

    private RatingTabBase[] ratingTabs;
    private RatingTab selectedRatingTab;
    private CoverageTab selectedCoverageTab;
    private float playerTabHeight;
    private float playerTabSpacing;
    private LinkedList<PlayerTab> playerTabs = new LinkedList<PlayerTab>();

    private void Awake()
    {
        playerTabHeight = playerTabPrefab.GetComponent<RectTransform>().rect.height;
        playerTabSpacing = tabsContent.GetComponent<VerticalLayoutGroup>().spacing;
        ratingTabs = GetComponentsInChildren<RatingTabBase>();
    }

    private void Start()
    {
        OnReceiveRank += GetMyPlayerRank;
        OnReceivePlayersRank += GetPlayersRank;
        selectedRatingTab = defaultRatingTab;
        selectedCoverageTab = defaultCoverageTab;
        ActivateTabs(ratingTabs, false);
        SendRatingRequest();
    }

    private void OnDestroy()
    {
        OnReceiveRank -= GetMyPlayerRank;
        OnReceivePlayersRank -= GetPlayersRank;
    }

    public void SelectTab(RatingTabBase tab)
    {
        ActivateTabs(ratingTabs, false);
        switch (tab)
        {
            case RatingTab:
                selectedRatingTab.SelectTab(false);
                selectedRatingTab = (RatingTab)tab;
                break;
           case CoverageTab:
                selectedCoverageTab.SelectTab(false);
                selectedCoverageTab = (CoverageTab)tab;
                break;
        }
        SendRatingRequest();
    }

    private void ActivateTabs(IEnumerable<RatingTabBase> tabs, bool isActive)
    {
        foreach (RatingTabBase tab in tabs)
            tab.ActivateTab(isActive);
        loadingRing.SetActive(!isActive);
    }

    private void SendRatingRequest()
    {
        foreach (PlayerTab tab in playerTabs)
            Destroy(tab.gameObject);
        playerTabs.Clear();
        switch ((selectedRatingTab.TabType, selectedCoverageTab.TabType))
        {
            case (RatingTabType.TopPlayers, RatingCoverageType.Global):
                HTTP_GetTopPlayers();
                break;
            case (RatingTabType.You, RatingCoverageType.Global):
                HTTP_GetPlayerRank(PlayerProfileData.Instance.Id, PlayerProfileData.Instance.TotalMoney);
                break;
        }
    }

    private void CreatePlayerTab(PlayerWithRankDto playeyersRank)
    {
        PlayerTab playerTab = Instantiate(playerTabPrefab, tabsContent);
        playerTab.SetPlayerRank(playeyersRank);
        playerTabs.AddLast(playerTab);
    }

    private void CalculateTabContentHeight(int tabCount)
    {
        tabsContent.sizeDelta = new Vector2(tabsContent.sizeDelta.x, playerTabHeight * tabCount + playerTabSpacing * (tabCount - 1));
    }

    private void GetMyPlayerRank(RankDto rank)
    {
        CalculateTabContentHeight(1);
        CreatePlayerTab(rank.ExpandToPlayerRank());
        ActivateTabs(ratingTabs.Except(new RatingTabBase[] { selectedRatingTab, selectedCoverageTab }), true);
    }

    private void GetPlayersRank(IEnumerable<PlayerWithRankDto> playeyersRank)
    {
        CalculateTabContentHeight(playeyersRank.Count());
        foreach (var playerRank in playeyersRank)
            CreatePlayerTab(playerRank);
        ActivateTabs(ratingTabs.Except(new RatingTabBase[] { selectedRatingTab, selectedCoverageTab }), true);
    }

    public void Close() => Destroy(gameObject);
}