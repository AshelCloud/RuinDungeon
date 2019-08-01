using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    [HideInInspector] public List<GameObject> Removelist = new List<GameObject>();

    List<MapData> mapDatas;
    List<MonsterData> monsterDatas;
    List<GameObject> objs;
    PlayerData userData;

    private static GameSystem _instance;

    public static GameSystem instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GameSystem)) as GameSystem;
            }

            return _instance;
        }
    }

    private void Awake()
    {
        //mapDatas = new List<MapData>();
        //monsterDatas = new List<MonsterData>();
        //objs = new List<GameObject>(GameObject.FindObjectsOfType<GameObject>());

        //for (int i = 0; i < objs.Count;)
        //{
        //    if (objs[i].transform.parent)
        //    {
        //        objs.RemoveAt(i);
        //    }
        //    else
        //    {
        //        i++;
        //    }
        //}

        //for (int i = 0; i < objs.Count; i++)
        //{
        //    if (objs[i].GetComponent<Monster>())
        //    {
        //        GameObject monsterGameObject = objs[i].gameObject;
        //        Monster monster = monsterGameObject.GetComponent<Monster>();
        //        monster.data.position = monsterGameObject.transform.position;
        //        monster.data.rotation = monsterGameObject.transform.rotation;
        //        monsterDatas.Add(objs[i].GetComponent<Monster>().data);
        //    }
        //    else if (objs[i].GetComponent<Player>())
        //    {
        //        userData.position = objs[i].transform.position;
        //        userData.rotation = objs[i].transform.rotation;
        //        Player p = objs[i].GetComponent<Player>();
        //        userData.hp = p.data.hp;
        //        userData.maxHp = p.data.maxHp;
        //        userData.stamina = p.data.stamina;
        //        userData.maxStamina = p.data.maxStamina;
        //        userData.speed = p.data.speed;
        //        userData.rotationSpeed = p.data.rotationSpeed;
        //        userData.stat = p.data.stat;
        //    }
        //    else
        //    {
        //        MapData data = new MapData();
        //        data.position = objs[i].transform.position;
        //        data.rotation = objs[i].transform.rotation;
        //        data.scale = objs[i].transform.localScale;

        //        int index = objs[i].name.IndexOf('(');

        //        if (index != -1)
        //        {
        //            objs[i].name = objs[i].name.Substring(0, index - 1);
        //        }

        //        data.name = objs[i].name;

        //        mapDatas.Add(data);
        //    }
        //}
    }

    private void Start()
    {
        mapDatas = DataManager.BinaryDeserialize<List<MapData>>("Map_1");
        userData = DataManager.BinaryDeserialize<PlayerData>("UserData_1");

        InitializeCanvas();
        InitializeMap();
        InitializePlayer();
        player.GetComponent<Player>().data.WeaponCode = 1;
        InitializeEquipments();
        InitializeInventory();
        InitializeUpgradeLayout();
        InitializeMapSelect();
        InitializeChestLayout();
        InitializeWeight();
    }

    Player player;
    GameObject monsterCanvas;
    private void OnGUI()
    {
        //if(GUILayout.Button("Save"))
        //{
        //    //DataManager.BinarySerialize(mapDatas, "Map_1.txt");
        //    //DataManager.BinarySerialize(userData, "UserData_2.txt");
        //    //DataManager.BinarySerialize(monsterDatas, "MonsterData.txt");
        //}

        //if(GUILayout.Button("Load"))
        //{
        //    Initialize();
        //}
    }

    // 시현아 아마 니가 이걸 볼때에 나는 존나 깊은 잠에 빠져있겠지...
    // 하지만 여기서 내가 수정하기엔 너무나도 옛날 파일이로구나
    // 미안하지만 니가 수정해야할 코드들이 있단다.
    // SelectMapUI 쪽에도 주석이 있으니 확인하길 바란다.
    // 이 주석들의 방법을 다 읽고 수정한뒤에 적용하면
    // 맵이 불러 와질것이다. 화이팅 오시현
    //                                          -김경민 올림-

    //----1번방법
    //Initialize 안에 있는 DataManager.BinaryDeserialize<>(" 이부분이 말하는거임 "); 파일들
    //string 변수들 다 하나씩 만들어서
    //밑에있는 Load 함수로 흘러들어온 인자들 변수 하나씩 만든다음에
    //DataManager.BinaryDeserialize<>(" <<<<이부분이 말하는거임>>>>> "); 여따가 집어쳐넣으세요

    //----2번 방법
    //userData = DataManager.BinaryDeserialize<PlayerData>("UserData");
    //mapDatas = DataManager.BinaryDeserialize<List<MapData>>("TestMapData");
    //monsterDatas = DataManager.BinaryDeserialize<List<MonsterData>>("MonsterData");
    //Initialize에 BinaryDeserialize 다 지우고 위에 3줄 집어쳐넣고 주석 풀면된다
    //Inventory랑 Equipments는 우째야 하냐면 어차피 이름 바뀔리 없어서 걍 냅둬도 될듯

    public void Save()
    {   
        
    }

    public void Load(string mapName = "TestMapData", string userName = "UserData", string inventoryName = "InventoryData", string equipmentsName = "EquipmentsData")
    {
        userData = DataManager.BinaryDeserialize<PlayerData>(userName);

        monsterDatas = DataManager.BinaryDeserialize<List<MonsterData>>("MonsterData");
        
        mapDatas = DataManager.BinaryDeserialize<List<MapData>>(mapName);

        Initialize();
    }

    public int curPlayerHP;
    public int curPlayerStamina;
    public Vec3 curPlayerPos;
    public quaternion curPlayerRotation;
    void Initialize()
    {
        Destroy(Camera.main.gameObject);
        Destroy(player.GetComponent<Player>().itemText);
        for (int i = 0; i < Removelist.Count; i++)
            Destroy(Removelist[i]);
        Removelist.RemoveRange(0, Removelist.Count);

        InitializeManager();
        InitializeCanvas();
        InitializePlayer();
        InitializeMap();
        InitializeMonster();
        InitializeClock();
        EquipmentsSet();

        player.GetComponent<Player>().BGM.Play();
        if (curPlayerPos != Vector3.zero)
        {
            player.transform.position = curPlayerPos;
            player.transform.rotation = curPlayerRotation;
        }
    }

    PuzzleManager p;
    void InitializeManager()
    {
        if (p == null)
            p = Instantiate(Resources.Load<PuzzleManager>("Prefab/PuzzleManager"));
        else
        {
            p.triangles.RemoveRange(0, p.triangles.Count);
            p.bossDoor.RemoveRange(0, p.bossDoor.Count);
            p.correct = false;
            Debug.Log(p.triangles.Count);

        }
    }

    Slider heroHPSlider;
    Slider heroStaminaSlider;

    Image hero_Info;
    Image Potion_Image;
    Image Fadeinout;
    Image DieText;

    void InitializeCanvas()
    {
        heroHPSlider = Instantiate(Resources.Load<Slider>(Define.PrefabFilePath + Define.HeroHpPath));
        heroStaminaSlider = Instantiate(Resources.Load<Slider>(Define.PrefabFilePath + Define.HeroStaminaPath));
        hero_Info = Instantiate(Resources.Load<Image>(Define.PrefabFilePath + "Player_Info"));
        Potion_Image = Instantiate(Resources.Load<Image>(Define.PrefabFilePath + "Potion_Image"));
        Fadeinout = Instantiate(Resources.Load<Image>(Define.PrefabFilePath + "PlayerFadeinout"));
        DieText = Instantiate(Resources.Load<Image>(Define.PrefabFilePath + "DeadText"));
        Removelist.Add(Fadeinout.gameObject);
        Removelist.Add(DieText.gameObject);

        heroHPSlider.name = "HeroHpSlider";
        heroStaminaSlider.name = "HeroStaminaSlider";
        hero_Info.name = "Hero_InfoImage";
        Potion_Image.name = "Potion_Image";
        Fadeinout.name = "Fadeinout";
        DieText.name = "DieText";

        GameObject heroCanvas = Instantiate(Resources.Load<GameObject>(Define.PrefabFilePath + Define.HeroCanvas));
        Removelist.Add(heroCanvas.gameObject);
        monsterCanvas = Instantiate(Resources.Load<GameObject>(Define.PrefabFilePath + Define.MonsterCanvas));
        Removelist.Add(monsterCanvas.gameObject);

        heroCanvas.name = "HeroCanvas";
        monsterCanvas.name = "MonsterCanvas";

        Vector3 pos = heroHPSlider.transform.localPosition;
        Vector3 pos1 = heroStaminaSlider.transform.localPosition;
        Vector3 pos2 = hero_Info.transform.localPosition;
        Vector3 pos3 = Potion_Image.transform.localPosition;
        Vector3 pos4 = Fadeinout.transform.localPosition;
        Vector3 pos5 = DieText.transform.localPosition;

        hero_Info.transform.SetParent(heroCanvas.transform);
        hero_Info.transform.localPosition = pos2;
        heroHPSlider.transform.SetParent(heroCanvas.transform);
        heroHPSlider.transform.localPosition = pos;
        heroStaminaSlider.transform.SetParent(heroCanvas.transform);
        heroStaminaSlider.transform.localPosition = pos1;
        Potion_Image.transform.SetParent(heroCanvas.transform);
        Potion_Image.transform.localPosition = pos3;
        Fadeinout.transform.SetParent(heroCanvas.transform);
        Fadeinout.transform.localPosition = pos4;
        DieText.transform.SetParent(heroCanvas.transform);
        DieText.transform.localPosition = pos5;
    }

    void InitializePlayer()
    {
        player = Instantiate(Resources.Load<Player>(Define.ModelPath + Define.HeroPath), userData.position, userData.rotation);
        player.data = userData;
        player.name = "Player";
        Removelist.Add(player.gameObject);

        PlayerCamera playerCamera = Instantiate(Resources.Load<PlayerCamera>(Define.PrefabFilePath + Define.PlayerCameraPath));
        Removelist.Add(playerCamera.gameObject);
        playerCamera.name = "PlayerCamera";

        Transform PlayerPivot = null;
        for(int i = 0; i < player.transform.childCount; i ++)
        {
            PlayerPivot = player.transform.GetChild(i);
            if (PlayerPivot.name == "Pivot" || PlayerPivot.name == "pivot") { break; }
        }

        AudioSource audio;
        for (int i = 1; i <= 5; i++)
        {
            audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "Hit" + i));
            SoundSet(audio, player.transform);
            player.GetComponent<Player>().HitSound.Add(audio);
            audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "Swing" + i));
            SoundSet(audio, player.transform);
            player.GetComponent<Player>().SwingSound.Add(audio);
        }

        for (int i = 1; i <= 6; i++)
        {
            audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "Walk" + i));
            SoundSet(audio, player.transform);
            player.GetComponent<Player>().WalkSound.Add(audio);
        }

        for (int i = 1; i <= 2; i++)
        {
            audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "PlayerDeath" + i));
            SoundSet(audio, player.transform);
            player.GetComponent<Player>().DeathSound.Add(audio);
        }
        audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "Pickup"));
        SoundSet(audio, player.transform);
        player.GetComponent<Player>().PickupSound = audio;

        audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "StatsUpgrade"));
        SoundSet(audio, player.transform);
        player.GetComponent<Player>().UpgradeSound = audio;

        audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "ShieldBlock"));
        SoundSet(audio, player.transform);
        player.GetComponent<Player>().ShieldBlockSound = audio;

        audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "Dungeon_Select"));
        SoundSet(audio, player.transform);
        player.GetComponent<Player>().MapSelectSound = audio;

        audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "You_are_Dead"));
        SoundSet(audio, player.transform);
        player.GetComponent<Player>().DeadBGM = audio;

        audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "Heal"));
        SoundSet(audio, player.transform);
        player.GetComponent<Player>().heal = audio;

        audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "Chest"));
        SoundSet(audio, player.transform);
        player.GetComponent<Player>().Chest = audio;

        audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "D1_Background"));
        Removelist.Add(audio.gameObject);
        player.GetComponent<Player>().BGM = audio;

        playerCamera.Initialize(PlayerPivot);

        player.Initialize(heroHPSlider, heroStaminaSlider, Potion_Image, Fadeinout, DieText, playerCamera);
    }


    void InitializeMap()
    {
        NavigationBuilder navBuilder = Instantiate(Resources.Load<NavigationBuilder>(Define.NavigationBuilder));
        navBuilder.m_Size = new Vector3(1000f, 1000f, 1000f);
        navBuilder.ActiveInitialize();
        Removelist.Add(navBuilder.gameObject);

        for (int i = 0; i < mapDatas.Count; i++)
        {
            GameObject go = Resources.Load<GameObject>(Define.PrefabFilePath + mapDatas[i].name);

            if (go == null || go.GetComponent<PlayerCamera>()) { continue; }
            
            GameObject gameObject = Instantiate(go, mapDatas[i].position, mapDatas[i].rotation);
            if (gameObject.GetComponent<Triangle>()) { gameObject.GetComponent<Triangle>().Initialize(); }
            if (gameObject.GetComponent<BossDoor>()) { gameObject.GetComponent<BossDoor>().Initialize(); }

            Removelist.Add(gameObject);
            gameObject.transform.localScale = mapDatas[i].scale;
            if (gameObject.layer != 8)
            {
                gameObject.AddComponent<NavMeshSourceTag>();
            }
            gameObject.isStatic = true;

            gameObject.name = mapDatas[i].name;
        }
    }

    void InitializeMonster()
    {
        Slider monsterHpSlider;
        Image clearPopUp;
        AudioSource audio;
        for (int i = 0; i < monsterDatas.Count; i++)
        {
            Monster monster = Instantiate(Resources.Load<Monster>(Define.ModelPath + monsterDatas[i].modelPath), monsterDatas[i].position, monsterDatas[i].rotation);
            monster.data = monsterDatas[i];
            Removelist.Add(monster.gameObject);

            monster.name = monster.data.name;

            monsterHpSlider = Instantiate(Resources.Load<Slider>(Define.PrefabFilePath + Define.MonsterHpPath));

            monsterHpSlider.name = "MonsterHpSlider";

            Vector3 SliderPos = monsterHpSlider.transform.localPosition;

            monsterHpSlider.transform.SetParent(monsterCanvas.transform);
            monsterHpSlider.transform.localPosition = SliderPos;

            clearPopUp = null;
            if (monster.CompareTag("Boss"))
            {
                clearPopUp = Instantiate(Resources.Load<Image>(Define.PrefabFilePath + "ClearPopUp"));
                clearPopUp.transform.SetParent(GameObject.Find("Canvas").transform);
                clearPopUp.gameObject.SetActive(false);
                clearPopUp.transform.localPosition = Vector3.zero;
                for (int n = 1; n <= 2; n++)
                {
                    audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "BossWalk" + n));
                    SoundSet(audio, monster.transform);
                    monster.GetComponent<Monster>().walk.Add(audio);
                    SoundMng.instance.SFX.Add(audio);
                }
                for (int n = 1; n <= 5; n++)
                {
                    audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "BossSwing" + n));
                    SoundSet(audio, monster.transform);
                    monster.GetComponent<Monster>().BossAttak.Add(audio);
                    SoundMng.instance.SFX.Add(audio);
                }
                for (int n = 1; n <= 3; n++)
                {
                    audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "BossSwingA" + n));
                    SoundSet(audio, monster.transform);
                    monster.GetComponent<Monster>().BossAttak.Add(audio);
                    SoundMng.instance.SFX.Add(audio);
                }
                audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "BossBreath"));
                SoundSet(audio, monster.transform);
                monster.GetComponent<Monster>().BossBreath = audio;
                SoundMng.instance.SFX.Add(audio);

                audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "BossGroundHit"));
                SoundSet(audio, monster.transform);
                monster.GetComponent<Monster>().BossJump = audio;
                SoundMng.instance.SFX.Add(audio);

                audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "ADarkenedKnight 1"));
                SoundSet(audio, monster.transform);
                monster.GetComponent<Monster>().BossBGM = audio;

                audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "DungoenClear"));
                monster.GetComponent<Monster>().ClearSound = audio;
                Removelist.Add(audio.gameObject);
                SoundMng.instance.BGM.Add(audio);
            }
            else
            {
                for (int j = 1; j <= 5; j++)
                {
                    audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "Swing" + j));
                    SoundSet(audio, monster.transform);
                    monster.GetComponent<Monster>().MonsterAttack.Add(audio);
                    Removelist.Add(audio.gameObject);
                }
                audio = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "MonsterWalk"));
                SoundSet(audio, monster.transform);
                Removelist.Add(audio.gameObject);
                monster.GetComponent<Monster>().walk.Add(audio);
                SoundMng.instance.SFX.Add(audio);
            }

            monster.Initialize(player, monsterHpSlider, clearPopUp);
        }
    }

    private void SoundSet(AudioSource a, Transform T)
    {
        a.transform.SetParent(T);
        a.transform.localPosition = Vector3.zero;
    }

    void InitializeUI()
    {
        InitializeEquipments();
        InitializeInventory();
        InitializeUpgradeLayout();
        InitializeMapSelect();
        InitializeChestLayout();
        InitializeWeight();
    }

    Inventory inventory;
    void InitializeInventory()
    {
        Inventory _inventory = Resources.Load<Inventory>(Define.PrefabFilePath + "Inventory");
        inventory = Instantiate(_inventory, Vector3.zero, Quaternion.identity);

        GameObject canvas = GameObject.Find("Canvas");
        inventory.transform.SetParent(canvas.transform);

        inventory.Initialize();
    }

    Equipments equipments;
    void InitializeEquipments()
    {
        Equipments _equipments = Resources.Load<Equipments>(Define.PrefabFilePath + "Equipments");
        equipments = Instantiate(_equipments, Vector3.zero, Quaternion.identity);

        GameObject canvas = GameObject.Find("Canvas");
        equipments.transform.SetParent(canvas.transform);

        equipments.Initialize(player);
    }

    void EquipmentsSet()
    {
        equipments.Equipmentset(player);
    }

    UpgradeLayout upgradeLayout;
    void InitializeUpgradeLayout()
    {
        UpgradeLayout _upgradeLayout = Resources.Load<UpgradeLayout>(Define.PrefabFilePath + "UpgradeLayout");
        upgradeLayout = Instantiate(_upgradeLayout, Vector3.zero, Quaternion.identity);

        GameObject canvas = GameObject.Find("Canvas");
        upgradeLayout.transform.SetParent(canvas.transform);

        upgradeLayout.Initialize(player);
    }

    Chest_UI chestLayout;
    void InitializeChestLayout()
    {
        Chest_UI _chestLayout = Resources.Load<Chest_UI>(Define.PrefabFilePath + "Chest_UI");
        chestLayout = Instantiate(_chestLayout, Vector3.zero, Quaternion.identity);

        GameObject canvas = GameObject.Find("Canvas");
        chestLayout.transform.SetParent(canvas.transform);

        chestLayout.Initialize();
    }

    SelectMapUI mapselect;
    void InitializeMapSelect()
    {
        SelectMapUI _mapselect = Resources.Load<SelectMapUI>(Define.PrefabFilePath + "SelectMapUI");
        mapselect = Instantiate(_mapselect, Vector3.zero, Quaternion.identity);
        Removelist.Add(mapselect.gameObject);

        GameObject canvas = GameObject.Find("Canvas");
        mapselect.transform.SetParent(canvas.transform);

        mapselect.Initialize();
    }

    Timer timer;
    void InitializeClock()
    {
        Timer _timer = Resources.Load<Timer>(Define.PrefabFilePath + "Timer");
        timer = Instantiate(_timer, Vector3.zero, Quaternion.identity);
        Removelist.Add(timer.gameObject);

        GameObject canvas = GameObject.Find("Canvas");
        timer.transform.SetParent(canvas.transform);

        timer.Initialize();
    }

    Weight weight;
    void InitializeWeight()
    {
        Weight _weight = Resources.Load<Weight>(Define.PrefabFilePath + "Weight");
        weight = Instantiate(_weight, Vector3.zero, Quaternion.identity);

        GameObject canvas = GameObject.Find("Canvas");
        weight.transform.SetParent(canvas.transform);

        weight.Initialize();
    }
}
