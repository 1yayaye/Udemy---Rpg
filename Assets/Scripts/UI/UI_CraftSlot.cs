using UnityEngine.EventSystems;
using UnityEngine;

public class UI_CraftSlot : UI_ItemSlot
{

    protected override void Start()
    {
        base.Start();
    }

    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if (_data == null)
        {
            CleanUpSlot();
            return;
        }

        item = new InventoryItem(_data);
        item.data = _data;

        itemImage.sprite = _data.itemIcon;
        itemImage.color = Color.white;
        itemText.text = LocalizationText.Translate(_data.itemName);

        if (itemText.text.Length > 12)
            itemText.fontSize = itemText.fontSize * .7f;
        else
            itemText.fontSize = 24;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.data == null)
            return;

        ItemData_Equipment equipment = item.data as ItemData_Equipment;

        if (equipment == null)
            return;

        ui.craftWindow.SetupCraftWindow(equipment);
    }
}
