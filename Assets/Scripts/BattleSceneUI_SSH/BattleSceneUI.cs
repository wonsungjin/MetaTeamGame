using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneUI : MonoBehaviour
{
    [SerializeField] GameObject playerProfilPanel = null;
    GameObject enemyProfilPanel = null;

    private void Awake()
    {
        playerProfilPanel = GameObject.Find("PlayerProfilPanel");
        enemyProfilPanel = GameObject.Find("PlayerProfilPanel");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClickOptionButton()
    {

    }

    public void OnClickPlayerProfilButton()
    {

    }

    public void OnClickEnemyProfilButton()
    {

    }

    public void Init()
    {
        // option, profil panel 초기화 필요

        
    }
}
