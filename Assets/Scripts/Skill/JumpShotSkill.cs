using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpShotSkill : BaseSkill
{
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private int spinCount = 3;
    [SerializeField] private float spinDuration= 2;
    public override void Activate(PlayerController playerController, EnemyController enemyController, CombatObjectPool combatObjectPool, CharacterType characterType,Transform targetPoint)
    {
        if (characterType == CharacterType.Player)
        {
            playerController.Rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            
            playerController.StartCoroutine(PlayerRotateAndShoot(playerController, targetPoint, combatObjectPool)); 
        }
        else if (characterType == CharacterType.Enemy)
        {
            enemyController.Rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            
            enemyController.StartCoroutine(EnemyRotateAndShoot(enemyController, targetPoint, combatObjectPool)); 
        }
    }
    
    private IEnumerator PlayerRotateAndShoot(PlayerController playerController, Transform targetPoint, CombatObjectPool combatObjectPool)
    {
        float singleDuration = spinDuration / spinCount; 

        for (int i = 0; i < spinCount; i++)
        {
          
            yield return RotateSpin(playerController.transform, 360f, singleDuration);
            GameObject straightArrow = combatObjectPool.Get(PoolType.JumpShotArrow);
            straightArrow.transform.position = playerController.ShootPoint.position;
            JumpShotArrow arrow = straightArrow.GetComponent<JumpShotArrow>();
            arrow.Initialize(targetPoint.position, combatObjectPool, OwnerType.Player);
        }
    }
    
    private IEnumerator EnemyRotateAndShoot(EnemyController enemyController, Transform targetPoint, CombatObjectPool combatObjectPool)
    {
        float singleDuration = spinDuration / spinCount; 

        for (int i = 0; i < spinCount; i++)
        {
          
            yield return RotateSpin(enemyController.transform, -360f, singleDuration);
            GameObject straightArrow = combatObjectPool.Get(PoolType.JumpShotArrow);
            straightArrow.transform.position = enemyController.ShootPoint.position;
            JumpShotArrow arrow = straightArrow.GetComponent<JumpShotArrow>();
            arrow.Initialize(targetPoint.position, combatObjectPool, OwnerType.Enemy);
        }
    }
    

    private IEnumerator RotateSpin(Transform target, float angle, float duration)
    {
        float elapsed = 0f;
        float startAngle = target.eulerAngles.z;
        float endAngle = startAngle + angle;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.Lerp(startAngle, endAngle, elapsed / duration);
            target.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }

        target.rotation = Quaternion.Euler(0, 0, endAngle);
    }
}
