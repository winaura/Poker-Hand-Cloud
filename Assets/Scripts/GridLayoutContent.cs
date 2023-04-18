using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), (typeof(GridLayoutGroup)))]
public class GridLayoutContent : MonoBehaviour
{
    [SerializeField] int totalCols;
    [SerializeField] int visibleRows;
    [SerializeField] RectTransform targetRect;
    [SerializeField] RectTransform contentRect;
    [SerializeField] GridLayoutGroup gridLayout;
    int totalRows = 0;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        var width = targetRect.rect.width / totalCols;
        var height = targetRect.rect.height / visibleRows;
        var size = width < height ? width : height;
        totalRows = (contentRect.childCount / totalCols) + (contentRect.childCount % totalCols > 0 ? 1 : 0);
        gridLayout.cellSize = new Vector2(size, size);
        contentRect.sizeDelta = new Vector2(size * totalCols, size * totalRows - contentRect.rect.height);
    }
}