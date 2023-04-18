using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatingTab : RatingTabBase
{
    [SerializeField] private RatingTabType tabType;

    public RatingTabType TabType { get => tabType; }

    protected override string LocalizationKey => tabType.ToString();
}
