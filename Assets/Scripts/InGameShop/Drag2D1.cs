using MongoDB.Driver;
using System.Collections;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public partial class Drag2D : MonoBehaviour
{

    public Color color = Color.white;

    [Range(0, 16)]
    public int outlineSize = 1;

    void UpdateOutline(bool outline)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);
        spriteRenderer.SetPropertyBlock(mpb);
    }


    private void OnMouseEnter()
    {
        if (isClickBool)
        {
            UIManager.Instance.OnEnter_Set_SkillExplantion(Camera.main.WorldToScreenPoint(transform.position));
        }
    }
    private void OnMouseExit()
    {
        UIManager.Instance.OnEnter_Set_SkillExplantion(Vector3.zero);

    }
}