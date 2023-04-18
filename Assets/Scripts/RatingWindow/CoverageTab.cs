using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverageTab : RatingTabBase
{
    [SerializeField] private RatingCoverageType tabType;

    public RatingCoverageType TabType { get => tabType; }

    protected override string LocalizationKey => tabType.ToString();
}
