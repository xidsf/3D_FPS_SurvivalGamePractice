using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private int hp; //돌의 체력
    [SerializeField]
    private float destroyTime; //파편 제거 시간

    [SerializeField]
    private SphereCollider col;

    //필요한 게임 오브젝트
    [SerializeField]
    private GameObject go_Rock;
    [SerializeField]
    private GameObject go_Debris;
    [SerializeField]
    private GameObject go_effectPrefab;
    [SerializeField]
    private GameObject go_rockItemPrefab;

    [SerializeField]
    private int maxCount; //돌 아이템 등장 횟수

    //필요한 사운드의 이름
    [SerializeField]
    private string strikeSound;
    [SerializeField]
    private string destroySound;


    public void Mining()
    {
        SoundManager.instance.playSE(strikeSound);
        hp -= 1;
        GameObject clone =  Instantiate(go_effectPrefab, col.bounds.center, Quaternion.identity);
        Destroy(clone, 2.0f);
        if (hp <= 0)
        {
            Destruction();
        }
        
    }

    private void Destruction()
    {
        SoundManager.instance.playSE(destroySound);
        col.enabled = false;
        for (int i = 0; i < maxCount; i++)
        {
            Instantiate(go_rockItemPrefab, go_Rock.transform.position, Quaternion.identity);
        }
        Destroy(go_Rock);

        go_Debris.SetActive(true);
        Destroy(go_Debris, destroyTime);
        
    }

}
