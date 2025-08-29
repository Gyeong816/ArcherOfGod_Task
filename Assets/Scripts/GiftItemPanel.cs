using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftItemPanel : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private EnemyController enemy; 
    [SerializeField] private GameObject giftPanel;
    [SerializeField] private Transform giftPlayerIconPanel;
    [SerializeField] private Transform giftEnemyIconPanel;

    [Header("방어구 선택 버튼")]
    [SerializeField] private Button damageReductionButton;
    [SerializeField] private Button fireImmunityButton;
    [SerializeField] private Button iceImmunityButton;
    
    [Header("공격력 선택 버튼")]
    [SerializeField] private Button physicalAttackButton;
    [SerializeField] private Button fireAttackButton;
    [SerializeField] private Button iceAttackButton;
    
    [Header("확정 버튼")]
    [SerializeField] private Button claimButton;
    
    [Header("아이콘 스프라이트 배열")]
    [SerializeField] private GameObject[] armorIcons;   // 0=Armor, 1=ArmorFire, 2=ArmorIce
    [SerializeField] private GameObject[] attackIcons;  // 0=Attack, 1=AttackFire, 2=AttackIce

    private ArmorType _selectedArmor = ArmorType.None;
    private AttackBuffType _selectedAttack = AttackBuffType.None;
    private void Awake()
    {
        damageReductionButton.onClick.AddListener(() => OnArmorSelected(ArmorType.DamageReduction, damageReductionButton));
        fireImmunityButton.onClick.AddListener(() => OnArmorSelected(ArmorType.FireImmunity, fireImmunityButton));
        iceImmunityButton.onClick.AddListener(() => OnArmorSelected(ArmorType.IceImmunity, iceImmunityButton));

        physicalAttackButton.onClick.AddListener(() => OnAttackSelected(AttackBuffType.Physical, physicalAttackButton));
        fireAttackButton.onClick.AddListener(() => OnAttackSelected(AttackBuffType.Fire, fireAttackButton));
        iceAttackButton.onClick.AddListener(() => OnAttackSelected(AttackBuffType.Ice, iceAttackButton));
        
        claimButton.onClick.AddListener(ClaimSelection);

        claimButton.interactable = false; 
    }

    private void OnArmorSelected(ArmorType type, Button clickedButton)
    {
        _selectedArmor = type;
        
        damageReductionButton.interactable = clickedButton == damageReductionButton;
        fireImmunityButton.interactable = clickedButton == fireImmunityButton;
        iceImmunityButton.interactable = clickedButton == iceImmunityButton;

        CheckClaimReady();
    }

    private void OnAttackSelected(AttackBuffType type, Button clickedButton)
    {
        _selectedAttack = type;
        
        physicalAttackButton.interactable = clickedButton == physicalAttackButton;
        fireAttackButton.interactable = clickedButton == fireAttackButton;
        iceAttackButton.interactable = clickedButton == iceAttackButton;

        CheckClaimReady();
    }

    private void CheckClaimReady()
    {
        claimButton.interactable = (_selectedArmor != ArmorType.None && _selectedAttack != AttackBuffType.None);
    }

    private void ClaimSelection()
    {
        if (_selectedArmor != ArmorType.None)
        {
            GameManager.Instance.SetPlayerArmor(_selectedArmor);

            int playerArmorIndex = (int)_selectedArmor - 1;
            Instantiate(armorIcons[playerArmorIndex], giftPlayerIconPanel);
            
            ArmorType randomArmor = GetRandomArmor();
            GameManager.Instance.SetEnemyArmor(randomArmor);
        }
        
        if (_selectedAttack != AttackBuffType.None)
        {
            GameManager.Instance.SetPlayerBuffAttack(_selectedAttack);

            int playerAttackIndex = (int)_selectedAttack - 1;
            Instantiate(attackIcons[playerAttackIndex], giftPlayerIconPanel);
            
            AttackBuffType randomAttack = GetRandomAttack();
            GameManager.Instance.SetEnemyBuffAttack(randomAttack);
        }

        giftPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private ArmorType GetRandomArmor()
    {
        int randomIndex = Random.Range(0, armorIcons.Length);
        
        Instantiate(armorIcons[randomIndex], giftEnemyIconPanel);

        return (ArmorType)(randomIndex + 1); 
    }

    private AttackBuffType GetRandomAttack()
    {
        int randomIndex = Random.Range(0, attackIcons.Length);
        
        Instantiate(attackIcons[randomIndex], giftEnemyIconPanel);

        return (AttackBuffType)(randomIndex + 1); 
    }
}
