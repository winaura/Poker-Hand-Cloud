using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.SnappingDemo
{
    public class SlotCellView : EnhancedScrollerCellView
    {
        /// <summary>
        /// These are the UI elements that will be updated when the data changes
        /// </summary>
        public Text slotText;

        /// <summary>
        /// This function sets up the data for the cell view
        /// </summary>
        /// <param name="data">The data to use</param>
        public void SetData(SlotData data)
        {
            slotText.text = data.intData.IntoCluttered();

            if (data.rarityOfButtons == RarityMenuButtons.White) slotText.color = Color.white;
            else if (data.rarityOfButtons == RarityMenuButtons.Blue) slotText.color = new Color(0, 0.5882f, 1);
            else if (data.rarityOfButtons == RarityMenuButtons.Orange) slotText.color = new Color(1, 0.4509f, 0);
            else if (data.rarityOfButtons == RarityMenuButtons.Pink) slotText.color = new Color(0.8745f, 0, 1);
        }
    }
}