using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private List<BaseSkill> playerSkills; 
    [SerializeField] private List<Button> skillButtons;
    [SerializeField] private List<TextMeshProUGUI> cooldownTexts;
    
    
    private void Start()
    {
        for (int i = 0; i < skillButtons.Count; i++)
        {
            int skillNum = i; 
            skillButtons[i].onClick.AddListener(() => UsePlayerSkill(skillNum));
        }
    }

    private void Update()
    {
        for (int i = 0; i < playerSkills.Count; i++)
        {
            
            float remaining = playerSkills[i].GetRemainingCooldown();

            if (remaining > 0)
            {
                cooldownTexts[i].text = remaining.ToString("F1"); 
                skillButtons[i].interactable = false;          
            }
            else
            {
                cooldownTexts[i].text = "";
                skillButtons[i].interactable = true;         
            }
        }
    }

    private void UsePlayerSkill(int skillNum)
    {
        if (skillNum < 0 || skillNum >= playerSkills.Count) return;

        var skill = playerSkills[skillNum];
        if (skill != null)
        {
            skill.TryActivate(transform, null); 
        }
    }

    private void UseEnemySkill(int skillNum)
    {
        if (skillNum < 0 || skillNum >= playerSkills.Count) return;

        var skill = playerSkills[skillNum];
        if (skill != null)
        {
            skill.TryActivate(transform, null); 
        }
    }
}
