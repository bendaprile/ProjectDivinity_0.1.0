using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeleteImplant : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionField = null;
    private ImplantUIPrefab implantUIPrefab = null;

    public void SetImplantUIPrefab(ImplantUIPrefab implantUIPrefab)
    {
        this.implantUIPrefab = implantUIPrefab;
        descriptionField.text = "ARE YOU SURE YOU WANT TO DESTROY " + implantUIPrefab.GetName() + "?";
    }

    public void ConfirmDelete()
    {
        implantUIPrefab.DeleteSelectedItem();
    }
}
