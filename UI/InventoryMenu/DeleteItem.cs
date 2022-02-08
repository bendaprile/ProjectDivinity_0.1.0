using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeleteItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI descriptionField = null;
    private InventoryUIPrefab inventoryUIPrefab = null;

    public void SetInventoryUIPrefab(InventoryUIPrefab inventoryUIPrefab)
    {
        this.inventoryUIPrefab = inventoryUIPrefab;
        descriptionField.text = "ARE YOU SURE YOU WANT TO DESTROY " + inventoryUIPrefab.GetItemName() + "?";
    }

    public void ConfirmDelete()
    {
        inventoryUIPrefab.DeleteSelectedItem();
    }
}
