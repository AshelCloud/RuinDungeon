using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager
{
    public static void BinarySerialize<T>(T t, string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Define.DataFilePath + filePath, FileMode.Create);
        formatter.Serialize(stream, t);
        stream.Close();
    }

    public static T BinaryDeserialize<T>(string filePath)
    {
        TextAsset asset = Resources.Load("Data/" + filePath) as TextAsset;
        Stream s = new MemoryStream(asset.bytes);

        BinaryFormatter formatter = new BinaryFormatter();

        T t = (T)formatter.Deserialize(s);

        s.Close();
        
        //GameSystem.instance.log.text = "DataManager : 바이너리 포맷터 생성";
        //BinaryFormatter formatter = new BinaryFormatter();
        //GameSystem.instance.log.text = "DataManager : 파일스트림 생성";
        //FileStream stream = new FileStream(Define.DataFilePath + filePath, FileMode.Open);
        //GameSystem.instance.log.text = "DataManager : 직렬화 해제";
        //T t = (T)formatter.Deserialize(stream);
        //stream.Close();
        //GameSystem.instance.log.text = "DataManager : 리턴";

        return t;
    }
}