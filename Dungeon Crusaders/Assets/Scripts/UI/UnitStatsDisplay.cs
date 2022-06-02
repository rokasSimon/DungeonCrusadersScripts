using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitStatsDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI unitName;

    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI manaText;
    [SerializeField] Slider manaBar;

    public void UpdateDisplay(Unit unit)
    {
        unitName.text = unit.DisplayName;

        healthBar.maxValue = unit.Stats.MaxHealth();
        healthText.text = unit.Stats.MaxHealth().ToString();
        healthBar.value = unit.Stats.CurrentHealth;

        manaBar.maxValue = unit.Stats.MaxMana();
        manaText.text = unit.Stats.MaxMana().ToString();
        manaBar.value = unit.Stats.CurrentMana;
    }
}
