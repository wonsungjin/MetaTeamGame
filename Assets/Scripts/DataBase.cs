using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    MongoClient client = new MongoClient("mongodb+srv://mongo:mongo1234@cluster0.utsc38e.mongodb.net/?retryWrites=true&w=majority");
    // Start is called before the first frame update
    public IMongoDatabase database = null;
    public IMongoCollection<BsonDocument> collection;
    void Start()
    {
        database = client.GetDatabase("userlist");
        collection = database.GetCollection<BsonDocument>("user");

        /*collection.InsertOne(new BsonDocument(new Dictionary<string, object>
        {
            ["Name"] = "Name of account",
            ["Email"] = "myemail@account.com",
            ["Password"] = "don't ever do that to me. I hope this is a hash...",
            ["SomeOtherField"] = "don't need it"
        }));
        var projection = Builders<BsonDocument>.Projection.Include("Name").Include("Email").Include("Password").Exclude("_id");*/

        /*var fillter = Builders<BsonDocument>.Filter.Eq("Name", "아디");//찾을 도큐먼트의 Name이 아디인것
        var nullFillter = collection.Find(fillter).FirstOrDefault();//if null 이면 찾지 못함
        if (nullFillter != null)
        {
           Debug.Log(nullFillter.GetValue("Name"));
        }*/
        //foreach (BsonDocument doc in documents)
        //{
        //    Debug.Log($"Name: {doc.GetValue("Name")}\n" +
        //              $"Email: {doc.GetValue("Email")}\n" +
        //              $"Password: {doc.GetValue("Password")}");
        //}
        //var address = nested["address"].AsBsonDocument;
        //Console.WriteLine(address["city"].AsString);
        //// or, jump straight to the value ...
        //Console.WriteLine(nested["address"]["city"].AsString);
        //// loop through the fields array
        //var allFields = nested["fields"].AsBsonArray;
        //foreach (var fields in allFields)
        //{
        //    // grab a few of the fields:
        //    Console.WriteLine("Name: {0}, Type: {1}",
        //        fields["NAME"].AsString, fields["TYPE"].AsString);
        //}
        // var document = new BsonDocument { {"id:","아디"}, {"password:", "비번"}, {"nickname:", "닉네임"}, { "nickname:", "닉네임" } };

        //var nullFillter = collection.Find(fillter).FirstOrDefault();//if null 이면 찾지 못함

        //var update = Builders<BsonDocument>.Update.Set("merge:", "닉켈");//찾은거 바꾸기
        //collection.UpdateOne(fillter, update);//바꾸는함수
        //collection.Find(document);
        //GetScoresFromDataBase();
    }

    /*public async void SaveScoreToDataBase(string userName, int score)
    {
        var document = new BsonDocument { { userName, score } };
        await collection.InsertOneAsync(document);
    }*/
    BsonDocument nullFillter;
    public void Login()
    {
        var fillter = Builders<BsonDocument>.Filter.Eq("address", GameMGR.Instance.metaTrendAPI.res_UserProfile.userProfile.public_address);//찾을 도큐먼트의 Name이 아디인것
        nullFillter = collection.Find(fillter).FirstOrDefault();//if null 이면 찾지 못함
        if (nullFillter == null)
        {
            Debug.Log("회원가입");
            collection.InsertOne(new BsonDocument(new Dictionary<string, object>
            {
                ["address"] = GameMGR.Instance.metaTrendAPI.res_UserProfile.userProfile.public_address,
                ["username"] = GameMGR.Instance.metaTrendAPI.res_UserProfile.userProfile.username,
            }));
            var update = Builders<BsonDocument>.Update.Set("inventory", inventoryData);//찾은거 바꾸기
            collection.UpdateOne(fillter, update);
        }
        else
        {
            FindInventoryData();
            GameMGR.Instance.dataBase.InsertInventoryData();
            GameMGR.Instance.uiManager.SetParentPackAddButton();
        }

    }
    public InventoryData inventoryData = new InventoryData();
    public void FindInventoryData()
    {
        var fillter = Builders<BsonDocument>.Filter.Eq("address", GameMGR.Instance.metaTrendAPI.res_UserProfile.userProfile.public_address);//찾을 도큐먼트의 Name이 아디인것
        nullFillter = collection.Find(fillter).FirstOrDefault();//if null 이면 찾지 못함
        BsonValue value = null;
        BsonValue value2 = null;
        string number = null;
        for (int i = 0; i < 10; i++)
        {
            nullFillter.TryGetValue("inventory", out value);

            if (value.ToString() == "BsonNull") return;//인벤토리가 비어있으면 리턴
            if (i == 0) number = "one";
            else if (i == 1) number = "two";
            else if (i == 2) number = "three";
            else if (i == 3) number = "four";
            else if (i == 4) number = "five";
            else if (i == 5) number = "six";
            else if (i == 6) number = "seven";
            else if (i == 7) number = "eight";
            else if (i == 8) number = "nine";
            else if (i == 9) number = "ten";
            value.ToBsonDocument().TryGetValue(number, out value);
            if (value.ToString() == "BsonNull") continue;// 해당 칸에 덱이 없다면 다음으로 이동
            Debug.Log(number);
            CustomDeck customDeck = new CustomDeck();
            for (int num = 1; num < 7; num++)
            {
                Debug.Log(value);
                value.ToBsonDocument().TryGetValue($"tier_{num}", out value2);
                string[] valueSplit = value2.ToString().Replace("[", "").Replace("]", "").Replace(" ", "").Split(',');
                for (int j = 0; j < valueSplit.Length; j++)
                {
                    if (num == 1) customDeck.tier_1.Add(valueSplit[j]);
                    else if (num == 2) customDeck.tier_2.Add(valueSplit[j]);
                    else if (num == 3) customDeck.tier_3.Add(valueSplit[j]);
                    else if (num == 4) customDeck.tier_4.Add(valueSplit[j]);
                    else if (num == 5) customDeck.tier_5.Add(valueSplit[j]);
                    else if (num == 6) customDeck.tier_6.Add(valueSplit[j]);
                    Debug.Log(valueSplit[j]);
                }
            }
            inventoryData.AddCustomDeck(customDeck);
            Debug.Log("?");
        }
    }
    public  void InsertInventoryData()
    {
        var fillter = Builders<BsonDocument>.Filter.Eq("address", GameMGR.Instance.metaTrendAPI.res_UserProfile.userProfile.public_address);//찾을 도큐먼트의 Name이 아디인것
        var update = Builders<BsonDocument>.Update.Set("inventory", inventoryData);//찾은거 바꾸기
       collection.UpdateOne(fillter, update);
        
    }
    public void Sort()
    {
        inventoryData = new InventoryData();
        MyDeck[] myDecks = FindObjectsOfType<MyDeck>();
        for (int i = 0; i < myDecks.Length; i++)
        {
            Destroy(myDecks[i].gameObject);
        }
        FindInventoryData();
        GameMGR.Instance.dataBase.InsertInventoryData();
        GameMGR.Instance.uiManager.SetParentPackAddButton();
    }
}
public class InventoryData
{
    public CustomDeck one;
    public CustomDeck two;
    public CustomDeck three;
    public CustomDeck four;
    public CustomDeck five;
    public CustomDeck six;
    public CustomDeck seven;
    public CustomDeck eight;
    public CustomDeck nine;
    public CustomDeck ten;
    public void DeleteCustomDeck(int Num)
    {

        Debug.Log(Num);
        if (Num == 1) one = null;
        else if (Num == 2) two = null;
        else if (Num == 3) three = null;
        else if (Num == 4) four = null;
        else if (Num == 5) five = null;
        else if (Num == 6) six = null;
        else if (Num == 7) seven = null;
        else if (Num == 8) eight = null;
        else if (Num == 9) nine = null;
        else if (Num == 10) ten = null;
        GameMGR.Instance.dataBase.InsertInventoryData();
        GameMGR.Instance.dataBase.Sort();
    }
    public void AddCustomDeck(CustomDeck customDeck)
    {
        Debug.Log(customDeck.Num);
        if (one == null && customDeck != null)
        {
            customDeck.Num = 1;
            one = customDeck;
            GameMGR.Instance.uiManager.CreateMyPackButton(one);
        }
        else if (two == null && customDeck != null)
        {
            customDeck.Num = 2;
            two = customDeck;
            GameMGR.Instance.uiManager.CreateMyPackButton(two);
        }
        else if (three == null && customDeck != null)
        {
            customDeck.Num = 3;
            three = customDeck;
            GameMGR.Instance.uiManager.CreateMyPackButton(three);
        }
        else if (four == null && customDeck != null)
        {
            customDeck.Num = 4;
            four = customDeck;
            GameMGR.Instance.uiManager.CreateMyPackButton(four);
        }
        else if (five == null && customDeck != null)
        {
            customDeck.Num = 5;
            five = customDeck;
            GameMGR.Instance.uiManager.CreateMyPackButton(five);
        }
        else if (six == null && customDeck != null)
        {
            customDeck.Num = 6;
            six = customDeck;
            GameMGR.Instance.uiManager.CreateMyPackButton(six);
        }
        else if (seven == null && customDeck != null)
        {
            customDeck.Num = 7;
            seven = customDeck;
            GameMGR.Instance.uiManager.CreateMyPackButton(seven);
        }
        else if (eight == null && customDeck != null)
        {
            customDeck.Num = 8;
            eight = customDeck;
            GameMGR.Instance.uiManager.CreateMyPackButton(eight);
        }
        else if (nine == null && customDeck != null)
        {
            customDeck.Num = 9;
            nine = customDeck;
            GameMGR.Instance.uiManager.CreateMyPackButton(nine);
        }
        else if (ten == null && customDeck != null)
        {
            customDeck.Num = 10;
            ten = customDeck;
            GameMGR.Instance.uiManager.CreateMyPackButton(ten);
        }
        GameMGR.Instance.dataBase.InsertInventoryData(); 
    }
}
public class CustomDeck
{
    public int Num = 0;
    public List<string> tier_1 = new List<string>();
    public List<string> tier_2 = new List<string>();
    public List<string> tier_3 = new List<string>();
    public List<string> tier_4 = new List<string>();
    public List<string> tier_5 = new List<string>();
    public List<string> tier_6 = new List<string>();
}
