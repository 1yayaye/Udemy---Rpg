using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button craftButton;

    [SerializeField] private Image[] materialImage;
    private readonly List<InventoryItem> emptyMaterials = new List<InventoryItem>();

    public void SetupCraftWindow(ItemData_Equipment _data)
    {

        craftButton.onClick.RemoveAllListeners();

        if (_data == null)
        {
            ClearCraftWindow();
            return;
        }

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].sprite = null;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        List<InventoryItem> craftingMaterials = _data.craftingMaterials ?? emptyMaterials;

        for (int i = 0; i < craftingMaterials.Count && i < materialImage.Length; i++)
        {
            if (craftingMaterials.Count > materialImage.Length)
                Debug.LogWarning("You have more materials amount than you have material slots in craft window");

            if (craftingMaterials[i] == null || craftingMaterials[i].data == null)
                continue;

            materialImage[i].sprite = craftingMaterials[i].data.itemIcon;
            materialImage[i].color = Color.white;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            materialSlotText.text = craftingMaterials[i].stackSize.ToString();
            materialSlotText.color = Color.white;
        }


        itemIcon.sprite = _data.itemIcon;
        itemIcon.color = Color.white;
        itemName.text = LocalizationText.Translate(_data.itemName);
        itemDescription.text = _data.GetDescription();

        craftButton.onClick.AddListener(() => Inventory.instance.CanCraft(_data, craftingMaterials));
    }

    private void ClearCraftWindow()
    {
        itemIcon.sprite = null;
        itemIcon.color = Color.clear;
        itemName.text = "";
        itemDescription.text = "";

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].sprite = null;
            materialImage[i].color = Color.clear;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            if (materialSlotText != null)
            {
                materialSlotText.text = "";
                materialSlotText.color = Color.clear;
            }
        }
    }
}
