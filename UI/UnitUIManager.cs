using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitUIManager : MonoBehaviour
{
    [SerializeField] private GameObject sidePanel;
    [SerializeField] private GameObject unitActionCanvasObject;
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject actionConfirmButton;
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private TextMeshProUGUI actionTooltipText;
    [SerializeField] private TextMeshProUGUI activeUnitQueueText;

    public bool IsActive => actionPanel.activeSelf;

    const int maxButtons = 10;
    Canvas unitActionCanvas;
    List<GameObject> actionButtonObjects;
    List<TextMeshProUGUI> actionButtonTexts;

    void Start()
    {
        unitActionCanvas = unitActionCanvasObject.GetComponent<Canvas>();
        actionButtonObjects = new List<GameObject>(maxButtons);
        actionButtonTexts = new List<TextMeshProUGUI>(maxButtons);

        foreach (Transform b in unitActionCanvasObject.transform)
        {
            var buttonObject = b.gameObject;
            var buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();

            actionButtonObjects.Add(buttonObject);
            actionButtonTexts.Add(buttonText);
        }
    }

    public void DisplayTooltip(Unit unit, UnitAction action)
    {
        sidePanel.SetActive(false);
        actionConfirmButton.SetActive(false);
        actionPanel.SetActive(true);

        if (action.SelectionType == SelectionTypes.Self)
        {
            actionConfirmButton.SetActive(true);
        }

        actionNameText.text = action.Name;
        actionTooltipText.text = action.Tooltip(unit);
    }

    public void HideTooltip()
    {
        sidePanel.SetActive(true);
        actionPanel.SetActive(false);
    }

    public void DisplayActiveUnit(Unit unit)
    {
        activeUnitQueueText.text = unit.DisplayName;

        if (unit.Team != UnitTeam.Player)
        {
            unitActionCanvas.enabled = false;
        }
        else
        {
            int actionButtonCount = unit.ActionIds.Count < maxButtons ? unit.ActionIds.Count : maxButtons - 1;

            for (int i = 0; i < actionButtonCount; i++)
            {
                var action = ActionRegister.Get(unit.ActionIds[i]);
                var button = actionButtonObjects[i].GetComponent<Button>();

                actionButtonObjects[i].SetActive(true);
                actionButtonTexts[i].text = action.Name;
                
                button.interactable = action.IsUsable(unit);
            }

            for (int i = actionButtonCount; i < maxButtons - 1; i++)
            {
                actionButtonObjects[i].SetActive(false);
            }

            if (actionButtonCount == maxButtons - 1)
            {
                actionButtonObjects[actionButtonCount].SetActive(true);
            }

            unitActionCanvas.enabled = true;
        }
    }
}
