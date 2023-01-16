using UnityEngine;

public partial class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject skillExplantion;
    public void OnEnter_Set_SkillExplantion(bool set,Vector3 pos)
    {
        skillExplantion.SetActive(set);
        if(set==true)
        {
            skillExplantion.transform.position = pos;

        }

    }
    
}
