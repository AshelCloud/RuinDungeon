using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public MonsterData data;

    public Animator ani; // 0 - 가만히, 1 - 걷기, 2 - 공격
    private NavMeshAgent navi;

    Player player;

    public Canvas canvas;
    private Transform hud;

    private bool isUpdate = false;

    private bool isStart = false;

    private int hp;
    private Vec3 position;
    private quaternion rotation;
    [HideInInspector] public int Damage;
    private float delay;
    private float detectRange;
    private float attackRange;

    private Slider hpBar;

    private Image clear;

    public List<AudioSource> walk = new List<AudioSource>();
    private bool walkSound = false;
    public List<AudioSource> BossAttak = new List<AudioSource>();
    public AudioSource BossBreath;
    public AudioSource BossJump;
    public AudioSource BossBGM;
    public AudioSource ClearSound;
    public List<AudioSource> MonsterAttack = new List<AudioSource>();

    public ParticleSystem dust;
    public ParticleSystem death;

    [HideInInspector] public int attackCount;
    [HideInInspector] public bool attackAble;
    [HideInInspector] public float bossCombo;

    public GameObject sword;

    public void Initialize(Player player, Slider HpSlider, Image Clear)
    {
        clear = Clear;

        Damage = data.Damage;
        hp = data.hp;
        transform.position = data.position;
        transform.rotation = data.rotation;
        delay = data.delay;
        detectRange = data.detectRange;
        attackRange = data.attackRange;

        data.itemList = DataManager.BinaryDeserialize<List<ItemData>>("ItemListData" + "_" + data.classLevel); // ItemList_레벨

        for (int i = 0; i < transform.childCount; i++)
        {
            hud = transform.GetChild(i);
            if (hud.name == "HUD") { break; }
        }

        navi = GetComponent<NavMeshAgent>();
        this.player = player;
        ani = GetComponent<Animator>();

        isUpdate = true;

        hpBar = HpSlider;
        hpBar.transform.position = hud.position;
        hpBar.GetComponent<Slider>().maxValue = data.hp;
        hpBar.GetComponent<Slider>().value = data.hp;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 50f, Color.white);
        if (hp <= 0 || ani.GetInteger("Condition") == 4)
        {
            if (navi != null && navi.enabled)
                navi.SetDestination(transform.position);
            return;
        }
        SetHpBar(hud.position);
        if (!isUpdate || !navi.isOnNavMesh || (transform.CompareTag("Boss") && !isStart) || !navi.enabled || player == null) { return; }
        Movement();
        if (transform.CompareTag("Boss") && BossHpBar != null && player.hp <= 0)
        {
            player.BGM.Play();
            BossBGM.Stop();
            Destroy(BossHpBar.gameObject);
        }
        if (Input.GetKey(KeyCode.Alpha0))
        {
            ReduceHp(400);
        }
    }

    private void Movement()
    {
        bool result;

        result = Tracking();

        if (!result && transform.CompareTag("Monster"))
        {
            Return();
        }
    }

    private Coroutine jump;
    private bool Tracking()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < data.attackRange)
        {
            Attack();
            return true;
        }
        else if (distance < data.detectRange && !ani.GetBool("Attack"))
        {
            Vector3 direction = player.transform.position - transform.position;

            Vector3 bodyPos = transform.position + new Vector3(0.0f, 8.0f, 0.0f);

            RaycastHit hit;
            Debug.DrawRay(bodyPos, direction, Color.red, Time.deltaTime);
            Physics.Raycast(bodyPos, direction, out hit, 100.0f);
            bool hasPlayer = false;
            if (hit.transform != null)
                hasPlayer = hit.transform.gameObject.GetComponent<Player>();

            if (!hasPlayer)
            {
                return false;
            }
            navi.SetDestination(player.transform.position);

            ani.SetInteger("Condition", 1);
            Debug.DrawRay(transform.position, transform.forward * 9f, Color.white);
            if (transform.CompareTag("Boss") && distance > 50f && distance < 60f)
            {
                int num = Random.Range(0, 100);
                if (num < 50 && ani.GetInteger("Condition") == 1)
                {
                    jump = StartCoroutine(JumpAttack());
                }
                ani.SetInteger("Combo", 0);
            }

            return true;
        }
        return false;
    }

    private void Return()
    {
        if (transform.position == navi.destination || !navi.enabled)
        {
            for (int i = 0; i < walk.Count; i++)
                walk[i].Stop();
            if (ani.GetInteger("Condition") == 2)
                ani.SetInteger("Condition", 0);
            return;
        }
        navi.SetDestination(transform.position);
        ani.SetInteger("Condition", 0);
    }

    bool jumpcheck;
    Vector3 JumpPos;
    IEnumerator JumpAttack()
    {
        jumpcheck = false;
        look = StartCoroutine(Look());
        yield return new WaitForEndOfFrame();
        navi.enabled = false;
        for (int i = 0; i < walk.Count; i++)
            walk[i].Stop();
        ani.SetInteger("Condition", 5);
        
        while (true)
        {
            if (jumpcheck)
            {
                transform.position = Vector3.MoveTowards(transform.position, JumpPos, Time.deltaTime * 130f);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void Attack()
    {
        navi.enabled = false;
        for (int i = 0; i < walk.Count; i++)
            walk[i].Stop();
        ani.SetInteger("Condition", 0);
        if (!ani.GetBool("Attack"))
        {
            if (transform.CompareTag("Monster"))
                StartCoroutine(Delay());
            else if (transform.CompareTag("Boss"))
                StartCoroutine(BossAttack());
        }
    }

    private IEnumerator BossAttack()
    {
        ani.SetBool("Attack", true);
        Coroutine looking = StartCoroutine(Look());
        Vector3 pos = transform.position + transform.up;
        RaycastHit hit;
        Debug.DrawRay(pos, transform.forward * data.attackRange, Color.green, 10f);
        Physics.Raycast(pos, transform.forward, out hit , data.attackRange);
        while (hit.transform != null && !hit.transform.GetComponent<Player>())
        {
            Physics.Raycast(pos, transform.forward, out hit, data.attackRange);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        StopCoroutine(looking);
        ani.SetInteger("Condition", 2);
        ani.SetInteger("Combo", Random.Range(1, 8));
        yield return null;
    }

    private IEnumerator Delay()
    {
        ani.SetBool("Attack", true);
        Coroutine look = StartCoroutine(Look());
        yield return new WaitForSeconds(delay);
        StopCoroutine(look);
        ani.SetInteger("Condition", 2);
        yield return null;
    }

    private IEnumerator Look()
    {
        while (hp >= 0 && player != null)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            dir -= new Vector3(0, dir.y, 0);
            quaternion q = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, data.RotationSpeed);
            //transform.LookAt(player.transform);
            yield return new WaitForEndOfFrame();
        }
    }

    private void SetHpBar(Vector3 HUDPos)
    {
        if (hpBar != null)
        {
            hpBar.transform.position = HUDPos;
            hpBar.transform.LookAt(Camera.main.transform.position);
        }
    }

    private void Die()
    {
        if (hp <= 0)
        {
            ani.SetInteger("Condition", 4);
            DropItem();
            if (transform.CompareTag("Boss"))
                Invoke("DieParticle", 2f);
            else
                Destroy(hpBar.gameObject);
            GetComponent<BoxCollider>().enabled = false;
            Invoke("DieDestroy", 4f);
        }
    }

    IEnumerator BGMFade()
    {
        while (BossBGM.volume >= 0)
        {
            BossBGM.volume -= 0.05f;
            yield return new WaitForSeconds(0.25f);
        }
        yield return null;
    }

    private void DieParticle()
    {
        death.Play();
        StartCoroutine(BGMFade());
    }

    private void DieDestroy()
    {
        if (transform.CompareTag("Boss"))
        {
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            Destroy(sword);
            Destroy(transform.GetChild(0).GetChild(1).GetChild(1).gameObject);
            ClearSound.Play();
            clear.gameObject.SetActive(true);
            StartCoroutine(ImageFade());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ImageFade()
    {
        yield return new WaitForSeconds(2.5f);
        while (clear.color.a >= 0)
        {
            clear.color = new Color(clear.color.r, clear.color.g, clear.color.b, clear.color.a - 0.05f);
            yield return new WaitForEndOfFrame();
        }
        player.BGM.Play();
        yield return null;
    }

    //몬스터마다 레벨있음(data.level)
    //case level : 아이템_level.htf 리스트 순회해서 랜덤으로 아이템 떨굼
    private void DropItem()
    {
        int classLevel = data.classLevel;
        float kind = Random.Range(0, 1f);
        int count = Random.Range(0, data.itemList.Count);

        switch (classLevel)
        {
            case 1:

                if (kind >= 0.7f)
                {
                    while (data.itemList[count].itemType == ItemType.Potion && data.itemList.Count > count + 1)
                    {
                        count++;
                    }
                }
                else if (0.3f >= kind)
                {
                    while (data.itemList[count].itemType != ItemType.Potion && data.itemList.Count > count + 1)
                    {
                        count++;
                    }
                }
                GameObject item = Resources.Load<GameObject>(Define.ModelPath + data.itemList[count].modelPath);
                //if (item.GetComponent<MeshRenderer>() == null) { Debug.Log(data.itemList[count].modelPath + "Noting"); return; }
                GameObject go = Instantiate(item, transform.position, Quaternion.identity);
                Item goItem = go.AddComponent<Item>();
                goItem.data = data.itemList[count];
                Debug.Log(goItem.data.name + ", " + goItem.data.discription);

                DropGround(go);
                //레이로 바닥에 딱붙혀라
                //만약 레이로 붙혓는데 물체의 절반이 바닥에 빠지면 sclae.y / 2f 해서 position.y 에 + 해주면된다
                //중복되는 코드 쓰지마라
                break;

            case 2:

                break;

            case 3:

                break;
        }
    }

    public void DropGround(GameObject go)
    {
        RaycastHit hit;

        int savelayer = go.layer;
        go.layer = 2;

        Vector3 bodyPos = transform.position + new Vector3(0.0f, 8.0f, 0.0f);
        Debug.DrawRay(bodyPos, Vector3.down * 20f, Color.green, 100f);
        Physics.Raycast(bodyPos, Vector3.down, out hit, 20f);

        //Debug.Log(go.GetComponent<Renderer>().bounds.size.y / 2.0f);

        if (hit.transform.CompareTag("Ground"))
        {
            Vector3 size = go.GetComponentInChildren<Renderer>().bounds.size;
            Vector3 pos = new Vector3(0f, size.y / 2.0f + 0.005f, 0f);
            go.transform.position = hit.point + pos;
        }
        go.layer = savelayer;
    }

    public void ReduceHp(int Damage)
    {
        hp -= Damage;
        hpBar.value = hp;
        if (transform.CompareTag("Boss"))
            BossHpBar.value = hp;
        Die();
    }

    //애니메이션 event들
    private void BossCombo()
    {
        if (ani.GetInteger("Combo") < 5)
        {
            bossCombo += 3;
        }
        else if (ani.GetInteger("Combo") == 5)
        {
            bossCombo += 1.5f;
        }
        else if (ani.GetInteger("Combo") > 5)
        {
            bossCombo += 1;
        }

        if (bossCombo >= 3)
        {
            ani.SetInteger("Combo", 0);
        }
    }

    private Coroutine stop;
    private Coroutine look;
    private void SlowAni(float speed)
    {
        if (!isUpdate) { return; }
        ani.speed = speed;
    }

    private void StopAni(float time)
    {
        AttackSpeed = 0;
        if (look != null)
        {
            StopCoroutine(look);
        }
        if (jump != null)
        {
            jumpcheck = false;
            StopCoroutine(jump);
        }

        ani.speed = 0;
        attackAble = false;
        Invoke("AniInvoke", time);
    }

    private void AniInvoke()
    {
        ani.speed = 1;
    }

    private void StopSlowAni(float time)
    {
        if (look != null)
        {
            StopCoroutine(look);
        }
        if (jump != null)
        {
            jumpcheck = false;
            StopCoroutine(jump);
        }
        AttackSpeed = 0;
        attackAble = false;
        stop = StartCoroutine(StopBossAni(time));
    }

    private void AniLook()
    {
        look = StartCoroutine(Look());
    }

    [HideInInspector] public float AttackSpeed;
    private void AttackMove(float speed)
    {
        AttackSpeed = speed;
    }

    IEnumerator StopBossAni(float time)
    {
        attackAble = false;
        ani.speed = 0;
        yield return new WaitForSeconds(time);
        ani.speed = 0.25f;
        yield return null;
    }

    private void BossJumpCheck()
    {
        JumpPos = player.transform.position;
        jumpcheck = true;
    }

    private void BossStart()
    {
        if (!isUpdate) { return; }
        StartCoroutine(BossStartAni());
    }

    private Slider BossHpBar;
    IEnumerator BossStartAni()
    {
        if (hpBar != null)
            Destroy(hpBar.gameObject);
        Vector3 direction = player.transform.position - transform.position;
        Vector3 bodyPos = transform.position + new Vector3(0.0f, 8.0f, 0.0f);

        RaycastHit hit;
        Physics.Raycast(bodyPos, direction, out hit, 80.0f);
        ani.speed = 0;
        while (!BossBreath.isPlaying)
        {
            direction = player.transform.position - transform.position;
            Physics.Raycast(bodyPos, direction, out hit, 140.0f);
            if (hit.transform != null && hit.transform.GetComponent<Player>())
            {
                Physics.Raycast(bodyPos, direction, out hit, 100.0f);
                BossBreath.Play();
                Debug.Log("stop");
            }
            yield return new WaitForEndOfFrame();
        }
        while (player != null && hit.transform == null || (hit.transform != null && !hit.transform.GetComponent<Player>()))
        {
            direction = player.transform.position - transform.position;
            Debug.DrawRay(bodyPos, direction, Color.green, 1f);

            Physics.Raycast(bodyPos, direction, out hit, 80.0f, 1 << 9);
            yield return new WaitForEndOfFrame();
        }
        BossHpBar = Instantiate(Resources.Load<Slider>(Define.PrefabFilePath + "BossHpBar"));
        GameSystem.instance.Removelist.Add(BossHpBar.gameObject);
        BossHpBar.transform.SetParent(GameObject.Find("Canvas").transform);
        BossHpBar.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 130f, 0f);
        BossHpBar.maxValue = hp;
        player.Wall.GetComponent<BoxCollider>().isTrigger = false;
        StartCoroutine(BossHpSet());
        ani.speed = 1;
    }

    IEnumerator BossHpSet()
    {
        int num = 0;
        while (BossHpBar.value < BossHpBar.maxValue)
        {
            num += 1;
            BossHpBar.value += 1 * num;
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    private void IsStart()
    {
        if (BossBreath != null)
        {
            BossBGM.Play();
            player.BGM.Stop();
            BossBreath.Stop();
        }
        isStart = true;
    }

    private void BossDamage(int damage)
    {
        Damage = damage;
    }

    private void IsAttack(int check)
    {
        if (check == 1)
            attackAble = true;
        else
            attackAble = false;
    }

    public void BossNavi()
    {
        StartCoroutine(NaviOn());
    }

    IEnumerator NaviOn()
    {
        yield return new WaitForFixedUpdate();
        navi.enabled = true;
    }

    private void WalkSound()
    {
        walk[Random.Range(0, walk.Count)].Play();
    }

    private void AttackSound(int count)
    {
        if (count == 1)
            BossAttak[Random.Range(0, 5)].Play();
        if (count != 1)
            BossAttak[Random.Range(5, 8)].Play();
    }

    private void JumpSounnd()
    {
        player.GetComponentInChildren<CameraShake>().ShakeCam();
        BossJump.Play();
        dust.Play();
    }

    private void MonsterAttackSound()
    {
        MonsterAttack[Random.Range(0, MonsterAttack.Count)].Play();
    }
}