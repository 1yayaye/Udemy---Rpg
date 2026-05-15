using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftList : MonoBehaviour , IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private GameObject craftSlotPrefab;

    [SerializeField] private List<ItemData_Equipment> craftEquipment;


    void Start()
    {
        transform.parent.GetChild(0).GetComponent<UI_CraftList>().SetupCraftList();
        SetupDefaultCraftWindow();
    }

    public void SetupCraftList()
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < craftEquipment.Count; i++)
        {
            if (craftEquipment[i] == null)
            {
                Debug.LogWarning("Craft list contains a missing equipment reference.");
                continue;
            }

            GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);
            newSlot.GetComponent<UI_CraftSlot>().SetupCraftSlot(craftEquipment[i]);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();
        SetupDefaultCraftWindow();
    }

    public void SetupDefaultCraftWindow()
    {
        UI ui = GetComponentInParent<UI>();

        if (ui == null || ui.craftWindow == null)
            return;

        foreach (ItemData_Equipment equipment in craftEquipment)
        {
            if (equipment != null)
            {
                ui.craftWindow.SetupCraftWindow(equipment);
                return;
            }
        }

        ui.craftWindow.SetupCraftWindow(null);
    }
}
