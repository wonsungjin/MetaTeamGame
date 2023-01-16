using UnityEngine;

public partial class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject skillExplantion;
    public void OnEnter_Set_SkillExplantion(Vector3 pos)
    {
        if(skillExplantion.activeSelf)
        {
            skillExplantion.SetActive(false);
            skillExplantion.transform.position = pos;
            Camera.main.ScreenToWorldPoint(skillExplantion.transform.position);

        }
        else skillExplantion.SetActive(true);

    }
    
}
