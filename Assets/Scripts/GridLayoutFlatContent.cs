using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), (typeof(GridLayoutGroup)))]
public class GridLayoutFlatContent : MonoBehaviour
{
    [SerializeField] RectTransform targetRect;
    [SerializeField] GridLayoutGroup gridLayout;
    [SerializeField, Min(1)] float aspectRatio;

    void OnEnable()
    {
        float width = (targetRect.rect.width - gridLayout.padding.left - gridLayout.padding.right - gridLayout.spacing.x * (gridLayout.constraintCount - 1)) / gridLayout.constraintCount;
        float height = width / aspectRatio;
        gridLayout.cellSize = new Vector2(width, height);
        int rowCount = gridLayout.transform.childCount / gridLayout.constraintCount;
        RectTransform gridLayoutRectTransform = gridLayout.GetComponent<RectTransform>();
        gridLayoutRectTransform.sizeDelta = new Vector2(gridLayoutRectTransform.sizeDelta.x, (height + gridLayout.spacing.y) * rowCount - gridLayout.spacing.y);
    }
}