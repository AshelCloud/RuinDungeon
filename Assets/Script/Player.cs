using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private static Player _instance;

    public static Player instance
    {
        get
        {
            if (_instance == null) { _instance = FindObjectOfType(typeof(Player)) as Player; }

            return _instance;
        }
    }

    public PlayerData data;

    //unityEngine

    Rigidbody myRb;
    Animator myAnimation;

    public float hp;
    private float Back_hp;
    private float stamina;
    private float speed;
    private float rotationSpeed;

    private int MoveCode = 0;

    private bool increaseHp;
    private bool CanAction;
    public bool isSpecial;
    private bool ShakeCameraTime;
    public bool FinishDash;
    private bool FinishSting;
    public bool Allmovement;
    public int Player_Damage;
    public bool isDead;

    private Vector3 Playermovement;

    public Transform mainCamTransform;
    public GameObject[] Weapon;             //0 shield 1 Sword
    public ParticleSystem[] PlayerParticle;
    public GameObject PotionPre;
    public int AttackNum;


    /// <summary>
    /// 0.Idle 1.Walk 2.Run 3.Jump 4.Attack
    /// </summary>

    private bool isUpdate;


    Slider hpSlider;
    Slider staminaSlider;
    public Image PotionImage;
    private List<Image> PlayerDieImage = new List<Image>();


    List<Item> filedItems;
    public GameObject itemText;

    int selectItemCount;

    public int attackCount;

    private bool isNearAltar = false;
    private bool isNearStageSelect = false;
    private bool isNearChest = false;
    private bool isNearTreasureBox = false;
    private bool invincible = false;

    public List<AudioSource> WalkSound = new List<AudioSource>();
    public List<AudioSource> HitSound = new List<AudioSource>();
    public List<AudioSource> SwingSound = new List<AudioSource>();
    public List<AudioSource> DeathSound = new List<AudioSource>();

    public AudioSource PickupSound;
    public AudioSource UpgradeSound;
    public AudioSource ShieldBlockSound;
    public AudioSource MapSelectSound;
    public AudioSource heal;
    public AudioSource Chest;

    public AudioSource DeadBGM;
    public AudioSource BGM;

    public PlayerStat stats;
    public int maxHp;
    public int maxStamina;
    

    private int selectCount
    {
        get
        {
            return selectItemCount;
        }

        set
        {
            if (filedItems.Count > 0)
            {
                selectItemCount = value;
            }

            if (selectItemCount >= filedItems.Count)
            {
                selectItemCount = 0;
            }
        }
    }

    public void Initialize(Slider hpSlider, Slider staminaSlider, Image Potion_image, Image fadeinout, Image Dietext, PlayerCamera camera)
    {
        //In Game에서 수치가 변할 수 있는 Data들 따로 옮겨담기
        //MaxHp 경우 게임내에서 시시때때로 변하지않기 때문에 옮겨담지 않는다.
        //게임종료 or 세이브시 옮겨담아놓은 변수를 Data로 다시 옮겨담은뒤 저장한다.
        hp = data.hp;
        stamina = data.stamina;
        speed = data.speed;
        rotationSpeed = data.rotationSpeed;
        transform.position = data.position;
        transform.rotation = data.rotation;

        //나머지 변수 초기화부분
        myRb = GetComponent<Rigidbody>();
        myAnimation = GetComponentInChildren<Animator>();
        data.WeaponCode = 1;
        ChangeWeapon(data.WeaponCode);
        this.hpSlider = hpSlider;
        this.staminaSlider = staminaSlider;
        PlayerDieImage.Add(fadeinout);
        PlayerDieImage.Add(Dietext);
        for (int i = 0; i < 2; i++)
        {
            PlayerDieImage[i].gameObject.SetActive(false);
        }
        PotionImage = Potion_image;
        PotionImage.gameObject.SetActive(false);
        if (myAnimation == null) { Debug.LogError("Animator is NULL!"); }
        Allmovement = true;
        CanAction = true;
        FinishDash = true;
        FinishSting = true;
        isSpecial = false;
        isDead = false;
        mainCamTransform = camera.transform;
        //커서 부분 ( System으로 옮겨질 예정 임시로 여기다 놔둠)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        filedItems = new List<Item>();
        selectItemCount = 0;
        stats = data.stat;
        Player_Damage = (stats.str + 8);       
        maxHp = data.maxHp;
        maxStamina = data.maxStamina;

        itemText = Instantiate(Resources.Load<GameObject>(Define.PrefabFilePath + "PickUpItemText"), transform.position, transform.rotation);
        isUpdate = true;
    }

    void GetPlayerWeaPon()
    {
        for (int i = 0; i < 2; i++)
        {
            Weapon[i] = transform.GetComponentsInChildren<BoxCollider>()[i].gameObject;
        }
    }

    public void ReduceHp(int Damage, bool isboss)// 플레이어가 맞을때
    {
        if (hp - Damage <= 0 && !isDead && !invincible)
        {
            PlayerDie();
        }
        else if (isboss)
        {
            if (Damage < 11)
            {
                SetPlayerAnimation(Random.Range(9, 11));
            }
            if (Damage == 18 || Damage == 25)
            {
                SetPlayerAnimation(11);
            }
            if (Damage == 13)
            {
                SetPlayerAnimation(12);
            }
        }
        else if (Damage < 11)
        {
            SetPlayerAnimation(Random.Range(9, 11));
        }
        increaseHp = false;
        if (!invincible)
        {
            hp -= Damage;
        }
        hpSlider.value = hp;
    }

    void FixedUpdate()
    {
        if (!isUpdate) { return; }

        Movement();
    }

    void Update()
    {
        if (!isUpdate) { return; }
        if (hpSlider.maxValue != maxHp)
        {
            hpSlider.maxValue = maxHp;
            hp = maxHp;
            data.maxHp = maxHp;
        }
        if (staminaSlider.maxValue != maxStamina)
        {
            staminaSlider.maxValue = maxStamina;
            stamina = maxStamina;
            data.maxStamina = maxStamina;
        }
        ControlSpeed();
        Stamina_Hp_Mng();
        if (Input.GetKeyDown(KeyCode.I) && !Chest_UI.instance.open)
        {
            Inventory.instance.OpenAndClose(!Inventory.instance.inventoryImage.enabled);
            Equipments.instance.OpenAndClose();
        }

        if (Inventory.instance.inventoryImage.enabled || UpgradeLayout.instance.open)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (filedItems.Count != 0)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                selectCount++;
            }

            if (Input.GetKeyDown(KeyCode.F) && Inventory.instance.data.weight + filedItems[selectCount].data.weight < 50)
            {
                Inventory.instance.AddItem(filedItems[selectCount]);

                Item curItem = filedItems[selectCount];

                filedItems.Remove(curItem);
                Destroy(curItem.gameObject);
                itemText.SetActive(false);
                PickupSound.Play();

                selectCount++;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isNearAltar)
            {
                UpgradeLayout.instance.open = !UpgradeLayout.instance.open;
                UpgradeLayout.instance.OpenAndClose(UpgradeLayout.instance.open);
            }
            if (isNearStageSelect)
            {
                MapSelectSound.Play();
                SelectMapUI.instance.Open();
            }
            if (isNearChest)
            {
                Chest_UI.instance.OpenAndClose();
            }
        }
    }

    void SetPlayerAnimation(int i)
    {
        myAnimation.SetInteger("PlayerCondition", i);
    }

    int GetPlayerAnimation()
    {
        return myAnimation.GetInteger("PlayerCondition");
    }


    private void Move(Vector3 direction)
    {
        MoveCode = 1;
        if (!Cursor.visible)
        {
            SetPlayerAnimation(1);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    void Movement()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        Playermovement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        if (Playermovement == Vector3.zero)
        {
            MoveCode = 0;
        }
        if (Allmovement)
        {
            if (Playermovement == Vector3.zero)
            {
                MoveCode = 0;
                if (GetPlayerAnimation() < 3)
                {
                    SetPlayerAnimation(0);
                }
            }

            if (Input.GetKey(KeyCode.W))
            {
                Vector3 camForward = mainCamTransform.forward;
                camForward.y = 0f;
                Move(camForward);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                Vector3 camBack = -mainCamTransform.forward;
                camBack.y = 0f;
                Move(camBack);
            }
            if (Input.GetKey(KeyCode.A))
            {
                Vector3 camLeft = -mainCamTransform.right;
                Move(camLeft);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Vector3 camRight = mainCamTransform.right;
                Move(camRight);
            }

            if (Input.GetKey(KeyCode.LeftShift) && GetPlayerAnimation() == 1 && CanAction)//뛰는것
            {
                SetPlayerAnimation(2);
                ReduceStamina(3);
                MoveCode = 2;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) && GetPlayerAnimation() == 2)
            {
                SetPlayerAnimation(0); //뛰고 바로 멈출때
            }

            if (Input.GetMouseButtonDown(0) && !Cursor.visible && CanAction) //때릴때
            {
                if (stamina > 5f)
                {                  
                    SetPlayerAnimation(3);
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hp < 40)
                {
                    if (Equipments.instance.item_1.transform.childCount > 0)
                    {
                        heal.Play();
                        Destroy(Equipments.instance.item_1.transform.GetChild(0).gameObject);
                        Back_hp = hp;
                        SetPlayerAnimation(5);
                        Allmovement = false;
                    }
                    else if (Equipments.instance.item_2.transform.childCount > 0)
                    {
                        heal.Play();
                        Destroy(Equipments.instance.item_2.transform.GetChild(0).gameObject);
                        Back_hp = hp;
                        SetPlayerAnimation(5);
                        Allmovement = false;
                    }
                    else if (Equipments.instance.item_3.transform.childCount > 0)
                    {
                        heal.Play();
                        Destroy(Equipments.instance.item_3.transform.GetChild(0).gameObject);
                        Back_hp = hp;
                        SetPlayerAnimation(5);
                        Allmovement = false;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                invincible = true;
            }
        }
        Dash();
        SpecialAttack(); //특수공격
    }

    public void FollowCameraVec3()
    {
        Vector3 camForward = mainCamTransform.forward;
        camForward.y = 0f;
        transform.rotation = Quaternion.LookRotation(camForward);
    }

    void visiblefalsePotion()
    {
        if (Equipments.instance.item_1.transform.childCount <= 0
            && Equipments.instance.item_2.transform.childCount <= 0
             && Equipments.instance.item_3.transform.childCount <= 0)
        {
            PotionImage.gameObject.SetActive(false);
        }
    }

    void Dash()
    {
        if (Input.GetKey(KeyCode.W))
        {
            PlayerDash(1);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            PlayerDash(2);
        }
        if (Input.GetKey(KeyCode.A))
        {
            PlayerDash(3);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            PlayerDash(4);
        }
    }

    private void PlayerDash(int Direction)
    {
        if (Input.GetKey(KeyCode.Space) && FinishDash && GetPlayerAnimation() < 4)
        {
            if (stamina > 10)
            {
                switch (Direction)
                {
                    case 2:
                        Vector3 camBack = -mainCamTransform.forward;
                        camBack.y = 0f;
                        transform.rotation = Quaternion.LookRotation(camBack);
                        break;
                    case 3:
                        transform.rotation = Quaternion.LookRotation(-mainCamTransform.right);
                        break;
                    case 4:
                        transform.rotation = Quaternion.LookRotation(mainCamTransform.right);
                        break;
                }
                SetPlayerAnimation(6);
            }
        }
    }

    private void ControlSpeed()
    {
        if (GetPlayerAnimation() == 1)
        {
            speed = data.speed;
        }
        if (GetPlayerAnimation() == 2)
        {
            speed = data.speed * 1.5f;
        }
    }

    public void ReduceStamina(float Sta)
    {
        stamina -= Time.deltaTime * Sta;
    }

    private void Stamina_Hp_Mng()
    {
        staminaSlider.value = stamina;
        hpSlider.value = hp;
        if (stamina <= 0 && GetPlayerAnimation() != 0)
        {
            CanAction = false;
            hp += stamina;
            if (hp <= 0)
            {
                PlayerDie();
            }
            stamina = 0;
            Allmovement = true;
            SetPlayerAnimation(0);
            StartCoroutine(DelayChargeStamina());
        }

        if (stamina < data.maxStamina)
        {
            if (GetPlayerAnimation() == 0 || GetPlayerAnimation() == 1 || GetPlayerAnimation() == 5 || GetPlayerAnimation() > 9)
            {
                stamina += Time.deltaTime * 8f;
            }
        }
        ///여기까지 스테미나
        if (increaseHp)
        {
            if (Back_hp + data.maxHp * 0.3f >= hp)
            {
                hp += Time.deltaTime * 20f;
            }
            else
                increaseHp = false;
        }
    }

    IEnumerator DelayChargeStamina()
    {
        yield return new WaitForSeconds(3.0f);
        CanAction = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Item>() != null && other.isTrigger)
        {
            for (int i = 0; i < filedItems.Count; i++)
            {
                if (other.GetComponent<Item>() == filedItems[i]) { return; }
            }

            filedItems.Add(other.GetComponent<Item>());
            Debug.Log(filedItems.Count);
        }
    }

    [HideInInspector] public bool SaveCheck = false;
    private void OnTriggerStay(Collider other)      //테스트안해본 코드 CheckFiled 함수를 여기다 옮긴거임 문제없으면 여기서 쓰는걸로 :)
    {
        if (itemText == null) { return; }
        TextMesh text = itemText.GetComponent<TextMesh>();
        itemText.transform.position = other.transform.position;
        if (other.GetComponent<Door>() && !other.GetComponent<Door>().isOpen)
        {
            text.text = "OPEN (F)";
            itemText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (other.CompareTag("Door"))
                {
                    other.GetComponent<Door>().PlayAnimation_2();
                }

                else if (other.CompareTag("SideDoor"))
                {
                    other.GetComponent<Door>().PlayAnimation_1();
                }
            }
        }

        if (other.CompareTag("SaveSword"))
        {
            text.text = "SAVE (F)";
            itemText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                other.GetComponentInChildren<ParticleSystem>().Play();
                //GameSystem.instance.curPlayerHP = (int)hp;
                //GameSystem.instance.curPlayerStamina = (int)stamina;
                GameSystem.instance.curPlayerRotation = transform.rotation;
                GameSystem.instance.curPlayerPos = transform.position;
            }
        }
        if (other.CompareTag("Altar"))
        {
            isNearAltar = true;
        }
        if (other.CompareTag("Stage"))
        {
            text.text = "OPEN (F)";
            itemText.SetActive(true);
            isNearStageSelect = true;
        }
        if (other.CompareTag("Chest"))
        {
            text.text = "OPEN (F)";
            itemText.SetActive(true);
            isNearChest = true;
        }
        if(other.CompareTag("Treasure"))
        {
            text.text = "OPEN (F)";
            itemText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F) && other.GetComponent<TreasureBox>().isOpen)
            {
                Chest.Play();
                other.GetComponentInChildren<Animation>().Play();
                StartCoroutine(TreasureBoxItemDelay(other));
                other.GetComponent<TreasureBox>().isOpen = false;
            }
        }

        if (filedItems.Count == 0 || !other.isTrigger) { return; }
        if (filedItems[selectCount] == null) { return; }
        text.text = "PICK UP (F)";
        Vector3 itemPosition = filedItems[selectCount].transform.position;
        itemText.SetActive(true);
        itemText.transform.position = new Vector3(itemPosition.x, itemPosition.y + 4f, itemPosition.z); //텍스트 위치 재설정
        Debug.Log(itemText.transform.position);
    }

    IEnumerator TreasureBoxItemDelay(Collider other)
    {
        yield return new WaitForSeconds(0.5f);
        List<ItemData> itemList = DataManager.BinaryDeserialize<List<ItemData>>("ItemListData" + "_" + 1);
        float kind = Random.Range(0, 1f);
        int count = Random.Range(0, itemList.Count);
        if (kind >= 0.7f)
        {
            while (itemList[count].itemType == ItemType.Potion && itemList.Count > count + 1)
            {
                count++;
            }
        }
        else if (0.3f >= kind)
        {
            while (itemList[count].itemType != ItemType.Potion && itemList.Count > count + 1)
            {
                count++;
            }
        }
        GameObject item = Resources.Load<GameObject>(Define.ModelPath + itemList[count].modelPath);
        Vector3 ItemPosition = new Vector3(other.transform.position.x - 1.5f, other.transform.position.y + 2, other.transform.position.z + 1.5f);
        GameObject go = Instantiate(item, ItemPosition, Quaternion.identity);
        Item goItem = go.AddComponent<Item>();
        goItem.data = itemList[count];
    }

    public GameObject Wall;
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Item>() != null && other.isTrigger)
        {
            filedItems.Remove(other.GetComponent<Item>());
            itemText.SetActive(false);
        }
        itemText.SetActive(false);

        if (other.CompareTag("InvisivleWall"))
        {
            Wall = other.gameObject;
            other.GetComponent<BoxCollider>().isTrigger = false;
        }
        if (other.CompareTag("Altar"))
        {
            isNearAltar = false;
        }
        if (other.CompareTag("Stage"))
        {
            isNearStageSelect = false;
        }
        if (other.CompareTag("Chest"))
        {
            isNearChest = false;
        }
    }

    public void Hideweapon()
    {
        increaseHp = true;
        Weapon[myAnimation.GetInteger("Weapon")].SetActive(false);
        PotionPre.SetActive(true);
        PlayerParticle[0].Play();
    }

    public void Showweapon()
    {
        Weapon[myAnimation.GetInteger("Weapon")].SetActive(true);
        PotionPre.SetActive(false);
        SetPlayerAnimation(MoveCode);
        Allmovement = true;
        visiblefalsePotion();
    }

    public void DashStart()
    {
        FinishDash = false;
        Allmovement = false;
        stamina -= 10;
    }

    public void DashFinish()
    {
        FinishDash = true;
        Allmovement = true;
        SetPlayerAnimation(0);
    }

    public void DefensAttackFalse()
    {
        myAnimation.SetBool("DefensAttack", false);
    }

    private void IncreasHP()
    {
        hp += 15;
        if (hp >= 50)
        {
            hp = 50;
        }
        hpSlider.value = hp;
    }

    public void DefensAttack()
    {
        myAnimation.SetBool("DefensAttack", true);
        stamina -= 20;
    }

    public void ChangeWeapon(int WeaponCode)
    {
        if (GetPlayerAnimation() == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                Weapon[i].SetActive(false);
            }
            switch (WeaponCode)
            {
                case 1:
                    {
                        Weapon[0].SetActive(true);
                        Weapon[1].SetActive(true);
                        myAnimation.SetInteger("Weapon", 1);
                        data.WeaponCode = 1;
                        Player_Damage += 0;
                    }
                    break;
                case 2:
                    {
                        Weapon[2].SetActive(true);
                        myAnimation.SetInteger("Weapon", 2);
                        data.WeaponCode = 2;
                        Player_Damage += 2;
                    }
                    break;
                case 3:
                    {
                        Weapon[3].SetActive(true);
                        Weapon[4].SetActive(true);
                        myAnimation.SetInteger("Weapon", 3);
                        data.WeaponCode = 3;
                        Player_Damage -= 5;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void SpecialAttack()
    {

        if (Input.GetMouseButton(1) && CanAction && myAnimation.GetInteger("Weapon") == 1) // 방어
        {
            SetPlayerAnimation(4);
            Allmovement = false;
            isSpecial = true;
        }
        if (!Input.GetMouseButton(1) && GetPlayerAnimation() == 4)
        {
            RePlayer();
            isSpecial = false;
        }

        if (Input.GetMouseButtonDown(1) && CanAction && GetPlayerAnimation() != 3)
        {
            if (myAnimation.GetInteger("Weapon") == 2 && FinishSting)
            {
                SetPlayerAnimation(7);
                isSpecial = true;
                Allmovement = false;
                FinishSting = false;
                myAnimation.speed = 0.5f;
                PlayerParticle[3].Play();
            }

            if (myAnimation.GetInteger("Weapon") == 3 && FinishSting)
            {
                SetPlayerAnimation(8);
                isSpecial = true;
                Allmovement = false;
                FinishSting = false;
    
            }
        }
    }
    //특수 공격 찌르기
    private Coroutine Sting_;

    IEnumerator Sting()
    {
        Vector3 Pos;
        RaycastHit hit;
        Pos = transform.position + transform.forward * 10f;
        while (true)
        {
            stamina -= Time.deltaTime * 50f;
            Debug.DrawRay(transform.GetChild(2).position, transform.forward * 1f, Color.cyan, 5f);
            Physics.Raycast(transform.GetChild(2).position, transform.GetChild(2).forward, out hit, 1f);
            if (hit.transform == null)
            {
                transform.position = Vector3.Lerp(transform.position, Pos, Time.deltaTime * 10);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void StartSting()
    {
        FollowCameraVec3();
        myAnimation.speed = 1.0f;
        Sting_ = StartCoroutine(Sting());
    }

    private void EndSting()
    {
        StopCoroutine(Sting_);
        StartCoroutine(EndSting_());
    }

    IEnumerator EndSting_()
    {
        SetPlayerAnimation(0);
        yield return new WaitForSeconds(0.5f);
        FinishSting = true;
        RePlayer();
    }

    //특수 공격 뛰어서 찌르기

    private Coroutine JumpChop_;

    private void JumpAction()
    {
        FollowCameraVec3();
        Allmovement = false;
        PlayerParticle[4].Play();
        JumpChop_ = StartCoroutine(JumpChop());
        myAnimation.speed = 0.9f;
    }
    IEnumerator JumpChop()
    {
        Vector3 Pos;
        RaycastHit hit;
        Pos = transform.position + transform.forward * 10f;
        while (true)
        {
            stamina -= Time.deltaTime * 15f;
            Debug.DrawRay(transform.GetChild(2).position, transform.forward * 1f, Color.cyan, 5f);
            Physics.Raycast(transform.GetChild(2).position, transform.GetChild(2).forward, out hit, 1f);
            if (hit.transform == null)
            {
                transform.position = Vector3.Lerp(transform.position, Pos, Time.deltaTime * 5);
                Debug.Log("asd");
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void DownAction()
    {
        Weapon[3].transform.GetChild(1).gameObject.SetActive(true);
        Weapon[4].transform.GetChild(1).gameObject.SetActive(true);
        myAnimation.speed = 2.0f;
    }

    private void FinishJumpChop()
    {
        myAnimation.speed = 1.0f;
        StopCoroutine(JumpChop_);
        StartCoroutine(EndJumpChop());
    }

    IEnumerator EndJumpChop()
    {
        yield return new WaitForSeconds(0.3f);
        FinishSting = true;
        RePlayer();
    }

    ///////////////////////////////////////
    public void RePlayer()
    {
        SetPlayerAnimation(MoveCode);
        isSpecial = false;
        Allmovement = true;
    }

    public bool AttackAble;
    private void AttackTimeSet(int check)
    {
        if (check == 1)
        {
            AttackAble = true;
            PlayerSwingSound();
        }
        else
        {
            AttackAble = false;
        }
    }

    private void ShortWeaponAttack(int num)//단검 공격 판정 제한
    {
        if (num == 1)
        {
            Weapon[3].transform.GetChild(1).gameObject.SetActive(true);
            Weapon[4].transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            Weapon[3].transform.GetChild(1).gameObject.SetActive(false);
            Weapon[4].transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void PlayerDie()
    {
        if (!isDead)
        {
            if (Wall != null)
                Wall.GetComponent<BoxCollider>().isTrigger = true;
            SetPlayerAnimation(13);
            increaseHp = false;
            Allmovement = false;
            isDead = true;
            Invoke("PlayerFadeinout", 3f);
            Debug.Log("asdsad");
        }
    }

    private void Hit()
    {
        Allmovement = false;
        PotionPre.SetActive(false);
    }

    private void FinishHit()
    {
        Weapon[myAnimation.GetInteger("Weapon")].SetActive(true);
        myAnimation.SetBool("Up", false);
        RePlayer();
        Allmovement = true;
    }

    private void Up_Player()
    {
        myAnimation.speed = 1.0f;
        StartCoroutine(UpTiming());
    }

    IEnumerator UpTiming()
    {
        yield return new WaitForSeconds(1.0f);
        myAnimation.SetBool("Up", true);
    }

    private void SlowDie()
    {
        PlayerDeathSound();
        myAnimation.speed = 0.75f;
    }

    private void FastDie()
    {
        myAnimation.speed = 2.0f;
    }

    private void PlayerWalkSound()
    {
        WalkSound[Random.Range(0, WalkSound.Count)].Play();
    }

    public void PlayerHitSound()
    {
        HitSound[Random.Range(0, HitSound.Count)].Play();
    }

    private void PlayerSwingSound()
    {
        SwingSound[Random.Range(0, SwingSound.Count)].Play();
    }

    private void PlayerDeathSound()
    {
        DeathSound[Random.Range(0, DeathSound.Count)].Play();
    }

    public void SetActTrail(bool T_F)
    {
        switch (myAnimation.GetInteger("Weapon"))
        {
            case 1:
                Weapon[1].transform.GetChild(0).gameObject.SetActive(T_F);
                break;
            case 2:
                Weapon[2].transform.GetChild(0).gameObject.SetActive(T_F);
                break;
            case 3:
                Weapon[3].transform.GetChild(0).gameObject.SetActive(T_F);
                Weapon[4].transform.GetChild(0).gameObject.SetActive(T_F);
                break;
        }
    }

    private void EndofAttack()
    {
        if (myAnimation.GetInteger("AttackCombo") == AttackNum)
        {
            myAnimation.SetInteger("PlayerCondition", 0);
            myAnimation.SetInteger("AttackCombo", 0);
        }
    }
    private Coroutine Fade1;
    private void PlayerFadeinout()
    {
        for (int i = 0; i < 2; i++)
        {
            PlayerDieImage[i].gameObject.SetActive(true);
        }
        Fade1 = StartCoroutine(FadeIn());
        DeadBGM.Play();
    }

    float Alpha1 = 0;
    IEnumerator FadeIn()
    {
        while (true)
        {
            if (Alpha1 <= 1f)
            {
                Alpha1 += 0.005f;
            }
            else
            {
                yield return new WaitForSeconds(5f);
                PlayerDieImage[0].color = new Color(0, 0, 0, 0);
                PlayerDieImage[1].color = new Color(1, 1, 1, 0);
                for (int i = 0; i < 2; i++)
                {
                    PlayerDieImage[i].gameObject.SetActive(false);
                }
                GameSystem.instance.Load("Map_2", "UserData_2", "InventoryData", "EquipmentsData");
                StopCoroutine(FadeIn());
            }
            PlayerDieImage[0].color = new Color(0, 0, 0, Alpha1);
            PlayerDieImage[1].color = new Color(1, 1, 1, Alpha1);
            yield return new WaitForEndOfFrame();
        }

    }
}
