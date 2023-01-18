using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;

public class DatabaseAccess : MonoBehaviour
{
    TestInputStr testInputStr = null;

    // DataBase : Collection의 물리적 컨테이너
    // Collection : Document의 그룹, Document의 내부에 위치해 있음
    // Document : 한개 이상의 Key-value 쌍으로 이루어진 구조
    // Key / Field : 컬럼 명과 저장 값
    // projection : Document를 조회할 때 보여질 Field를 정함

    #region Client 주소 설정
    MongoClient client = new MongoClient("mongodb+srv://MetaTrand:dkagh@cluster0.yytmq3i.mongodb.net/?retryWrites=true&w=majority");
    #endregion

    IMongoDatabase database = null;
    IMongoCollection<BsonDocument> collection;

    private void Awake()
    {

    }

    void Start()
    {
        testInputStr = FindObjectOfType<TestInputStr>();
        BsonComunication();
    }

    public void BsonComunication()
    {
        #region Database 연동
        // MongoDB database name
        database = client.GetDatabase("Test");
        #endregion

        #region Collection 연동
        // 해당 Database에 있는 Collection 가져오기
        // MongoDB collection name
        collection = database.GetCollection<BsonDocument>("TestCollection");
        #endregion

        #region 데이터 추가
        ////데이터베이스 추가
        BsonDocument document = new BsonDocument { { "name", "혜원" }, { "age", 26 }, { "tall", 123} };
        //collection.InsertOne(document);
        #endregion

        #region 데이터에서 해당 조건 필터
        var fillter = Builders<BsonDocument>.Filter.Eq("ID", "집가고싶다");
        #endregion

        #region 조건에 해당하는 데이터 불러오기
        var callName = collection.Find(fillter).FirstOrDefault();
        #endregion

        // 조건에 부합하는 자료가 없을 때 = null; 
        if (callName == null) Debug.Log("널");

        #region 데이터베이스 내용 변경
        // 하나의 데이터 베이스에 접근해서 다른 정보 변경 가능
        // ex) name으로 접근 후 age 변경 가능
        var update = Builders<BsonDocument>.Update.Set("PW", "ABC");
        collection.UpdateOne(fillter, update);
        #endregion

        #region 데이터베이스 내용 추가
        // fillter기준으로 검색된 데이터에 nickname이라는 key에 개발자라는 value 추가
        var updateSet = Builders<BsonDocument>.Update.Set("ID", "집가고싶다");
        collection.UpdateOne(fillter, updateSet);
        #endregion

        Debug.Log(callName.ToString());

        #region Document로 접근하여 원하는 Value 출력
        var nullFillter = collection.Find(fillter).FirstOrDefault();//if null 이면 찾지 못함
        if (nullFillter != null)
        {
            Debug.Log(nullFillter.GetValue("PW"));
        }
        #endregion

        string IDInput = nullFillter.GetValue("ID").ToString();
        string PWInput = nullFillter.GetValue("PW").ToString();

        testInputStr.InputText(IDInput, PWInput);
    }
}
