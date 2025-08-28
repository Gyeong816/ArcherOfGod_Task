using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
   
    [SerializeField] private List<Button> skillButtons;
    [SerializeField] private List<BaseSkill> skills;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private CombatObjectPool combatObjectPool;
    
     private List<TextMeshProUGUI> _cooldownTexts = new List<TextMeshProUGUI>();
     private List<BaseSkill> _playerSkills = new List<BaseSkill>(); 
     private List<BaseSkill> _enemySkills = new List<BaseSkill>();

     public int EnemySkillCount => _enemySkills.Count;

    private void Awake()
    {
        _cooldownTexts.Clear();
        foreach (var button in skillButtons)
        {
            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                _cooldownTexts.Add(text);
            }
        }
        
        for (int i = 0; i < skills.Count; i++)
        {
            _playerSkills.Add(skills[i]);
        }
        for (int i = 0; i < skills.Count; i++)
        {
            _enemySkills.Add(skills[i]);
        }
    }

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
        for (int i = 0; i < _playerSkills.Count; i++)
        {
            BaseSkill skill = _playerSkills[i];
            
            skill.TickCooldown(Time.deltaTime);
            
            float remaining = skill.GetRemainingCooldown();

            if (remaining > 0f)
            {
                _cooldownTexts[i].text = Mathf.CeilToInt(remaining).ToString();
                skillButtons[i].interactable = false;
            }
            else
            {
                _cooldownTexts[i].text = "";
                skillButtons[i].interactable = true;
            }
        }
    }

    private void UsePlayerSkill(int skillNum)
    {
        if (skillNum < 0 || skillNum >= _playerSkills.Count) return;

        var skill = _playerSkills[skillNum];
        if (skill != null && skill.CanUse())
        {
         
            var cmd = new SkillCommand(
                skill,
                CharacterType.Player,
                playerController,
                enemyController,
                combatObjectPool,
                skill.animName 
            );
            
            playerController.EnqueueSkill(cmd);
            
            skill.SetTimer();
        }
    }

    public BaseSkill GetEnemySkill(int skillNum)
    {
        if (skillNum < 0 || skillNum >= _enemySkills.Count) 
            return null;

        var skill = _enemySkills[skillNum];
        if (skill != null)
        {
            return skill;
        }
        return null;
    }
    
}
