using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    Animator animator;
    private Player player;

    private ParticleSystem Part;
    
	void Start () 
    {
        player = GetComponentInParent<Player>();
        animator = player.GetComponent<Animator>();
    }
    //8 10 3
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (transform.CompareTag("Weapon") && player.AttackAble)
        {
            Debug.Log(other.tag);
            if (animator.GetInteger("PlayerCondition") == 3 || animator.GetInteger("PlayerCondition") == 7 || animator.GetInteger("PlayerCondition") == 8)
            {
                Debug.Log(other.tag);
                if (other.CompareTag("Monster") || other.CompareTag("Boss"))
                {
                    if (Part == null)
                    {
                        Part = Instantiate(Resources.Load <ParticleSystem>(Define.PrefabFilePath + "MonsterHit"));
                    }
                    Part.transform.position = gameObject.transform.position + gameObject.transform.forward * Random.Range(1f, 5f);
                    Part.Play();
                    StartCoroutine(SetTimeScale());
                    other.GetComponent<Monster>().ReduceHp(player.Player_Damage);
                    player.GetComponentInChildren<CameraShake>().ShakeCam();
                    player.PlayerHitSound();
                }
            }
        }
    }

    IEnumerator SetTimeScale()
    {
        float delayTime = 0;
        Time.timeScale = 0;
        while (delayTime < 0.1f)
        {
            delayTime += Time.unscaledDeltaTime;
            yield return 0;
        }
        Time.timeScale = 1;
        yield return 0;
    }
}
