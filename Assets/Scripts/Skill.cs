using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public string skillType;
    
    public virtual void AttackSkill() // 공격시
    {

    }
    public virtual void HitSkill() // 피격시
    {     
          
    }     
    public virtual void DeathSkill() // 사망시
    {     
          
    }     
    public virtual void KillSkill() // 처치시
    {      
           
    }      
    public virtual void BuySkill() // 구매시
    {      
           
    }      
    public virtual void SellSkill() // 판매시
    {      
           
    }      
    public virtual void GameStartSkill() //전투 시작시 
    {     
           
    }      
    public virtual void StoreExitSkill() //
    {

    }
    /////////
    public void GetGold(int value)
    {

    }
    public void ThrowMissile()
    {

    }
    public void UnitCreate()
    {

    }
    public void AddStatus(Card card,string key)
    {

    }

}