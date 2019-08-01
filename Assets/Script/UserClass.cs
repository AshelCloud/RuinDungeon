using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapData
{
    public string name;
    public Vec3 position;
    public Vec3 scale;
    public quaternion rotation;
}

[System.Serializable]
public struct PlayerData
{
    public float hp;
    public int maxHp;
    public float stamina;
    public int maxStamina;
    /// <summary>
    /// This is usually a speed. If you want to run, multiply this by 1.5f
    /// </summary>
    public float speed;
    public float rotationSpeed;
    public Vec3 position;
    public quaternion rotation;
    public PlayerStat stat;
    public int WeaponCode;
}

[System.Serializable]
public struct PlayerStat
{
    /// <summary>
    /// Strong
    /// </summary>
    public int str;
    /// <summary>
    /// Luck
    /// </summary>
    public int luk;

    public int armor;

    public int damage;
}

[System.Serializable]
public struct MonsterData
{
    public int hp;
    public string name;
    public Vec3 position;
    public quaternion rotation;
    public float RotationSpeed;
    public string modelPath;
    public float detectRange;
    public float attackRange;
    public int MaxAttackCount;
    public int Damage;
    public float delay;
    public List<ItemData> itemList;
    public int classLevel;
}

[System.Serializable]
public struct ItemData
{
    public string name;
    public string discription;
    public ItemType itemType;
    public ItemClass itemClass;
    public string spritePath;
    public string modelPath;
    public Vec3 position;
    public int weight;
    public float damage;
    public float armor;
    public bool wearing;
}

public enum ItemType
{
    SwordAndShiled,
    TwoHandSword,
    Knife,
    Potion,
    Helmet,
    Top,
    Pants
}

[System.Serializable]
public struct InventoryData
{
    public float weight;
    public List<ItemData> items;
}

[System.Serializable]
public struct EquipmentsData
{
    public List<ItemData> items;
}

[System.Serializable]
public struct Vec3
{
    public float x, y, z;

    public Vec3(Vector3 other)
    {
        x = other.x;
        y = other.y;
        z = other.z;
    }

    public static implicit operator Vector3(Vec3 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    public static implicit operator Vec3(Vector3 v)
    {
        return new Vec3(v);
    }
}

[System.Serializable]
public struct quaternion
{
    public float x, y, z, w;

    public quaternion(Quaternion other)
    {
        x = other.x;
        y = other.y;
        z = other.z;
        w = other.w;
    }

    public static implicit operator Quaternion(quaternion q)
    {
        return new Quaternion(q.x, q.y, q.z, q.w);
    }

    public static implicit operator quaternion(Quaternion q)
    {
        return new quaternion(q);
    }
}

public enum ItemClass
{
    Lowly,
    Usable,
    Useful,
    Perfect
}

[System.Serializable]
public struct ChestData
{
    public List<ItemData> items;
}