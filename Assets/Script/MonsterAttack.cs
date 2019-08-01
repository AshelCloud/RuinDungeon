using UnityEngine;

public class MonsterAttack : MonoBehaviour {
    public GameObject Go;
    private Monster monster;
    private Player player;
    private void Start()
    {
        monster = Go.GetComponent<Monster>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (monster.attackAble && col.CompareTag("Player") && gameObject.GetComponent<BoxCollider>().isTrigger && !col.isTrigger 
            && monster.data.MaxAttackCount > monster.attackCount)
        {
            if (col.GetComponentInParent<Animator>().GetInteger("PlayerCondition") != 6 && !col.GetComponentInParent<Player>().isDead)
            {
                player = col.GetComponentInParent<Player>();
                monster.attackCount += 1;
                monster.attackAble = false;
                if (col.GetComponentInParent<Animator>().GetInteger("PlayerCondition") != 4)//방어하지 않을때
                {
                    player.ReduceHp(monster.Damage, Go.CompareTag("Boss"));
                    Blood(col);
                }
                else if (col.GetComponentInParent<Animator>().GetInteger("PlayerCondition") == 4)//방어할때 
                {
                    Vector3 Dir = col.gameObject.transform.position - GetComponentInParent<Monster>().transform.position;
                    Vector3 Pos = GetComponentInParent<Monster>().transform.position + new Vector3(0.0f, 8.0f, 0.0f);
                    RaycastHit hit;
                    Debug.DrawRay(Pos, Dir, Color.yellow, 0.5f);
                    Physics.Raycast(Pos, Dir, out hit, 100f);
                    player.ShieldBlockSound.Play();
                    if (hit.collider != null && hit.collider.name == "Back") //뒷통수
                    {
                        player.ReduceHp(monster.Damage, Go.CompareTag("Boss"));
                        Blood(col);
                    }
                    else//방어할때 맞을때
                    {
                        player.DefensAttack();
                        player.PlayerParticle[2].transform.position = player.Weapon[0].transform.position;
                        player.PlayerParticle[2].Play();
                    }
                }
                if (col.GetComponentInParent<CameraShake>())
                    col.GetComponentInParent<CameraShake>().ShakeCam();
            }
        }
    }

    private void Blood(Collider col)
    {
        player = col.GetComponentInParent<Player>();
        player.PlayerParticle[1].gameObject.transform.rotation = monster.transform.rotation;
        player.PlayerParticle[1].Play();
        player.PlayerHitSound();
    }
}
