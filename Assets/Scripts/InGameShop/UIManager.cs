using UnityEngine;
using TMPro;

public partial class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject skillExplantion;
    [SerializeField] private TextMeshProUGUI[] skillExplantionText;
    private bool isExplantionActive;
    public void OnEnter_Set_SkillExplantion(bool set,Vector3 pos,CardInfo cardInfo=null)
    {
        if (isExplantionActive) return;
        skillExplantion.SetActive(set);
        if (set)
        {

            skillExplantion.transform.position = pos;
            Camera.main.ScreenToWorldPoint(skillExplantion.transform.position);
            if (cardInfo.objName != skillExplantionText[0].text)
            {
                skillExplantionText[0].text = cardInfo.objName;
                skillExplantionText[1].text = cardInfo.GetSkillExplantion(1);
                skillExplantionText[2].text = cardInfo.GetSkillExplantion(2);
                skillExplantionText[3].text = cardInfo.GetSkillExplantion(3);
            }
        }
    }
    public void SetisExplantionActive(bool set)
    {
        isExplantionActive=set;
    }
    
}
