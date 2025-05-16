using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FxShoot : MonoBehaviour
{
    // ShootControllerの再構成
    [SerializeField]
    private float generateHeight = 3f; //メンコ生成の高さ

    public GameObject[] stagemenkoHolder; // メンコのプレハブ
    public GameObject[] shootmenkoHolder;
    public static float stageWidth = 8f; 
    public static float stageDepth = 13f; // 台の幅と奥行き

    private GameObject generatedMenko;

    private Vector3 mkForce = new Vector3(0.0f, -2.0f, 0.0f);
    public float mkpow = 0; // 発射力
    public int totalMenkoCount = 0; //メンコの数

    private GaugeController gaugeController;
    // メンコが配置された位置を管理するリスト
    private List<Vector3> occupiedPositions = new List<Vector3>();
    // すでに配置されたメンコを管理するリスト
    private List<GameObject> currentMenkoList = new List<GameObject>();
    [SerializeField]
    private Image nextImage; //次に投げるメンコを表示する画像
    [SerializeField]
    private Sprite[] MenkoImages; //各メンコタイプに対応する画像リスト

    private GameObject nextMenko; //次に投げるメンコのプレハブ

    private int currentShootMenkoNumber = 0; // 現在射出するメンコの番号

    private List<int> availableMenkoTypes = new List<int>();

    private float minDistance = 2.0f; // メンコ間の最小距離
    private bool isDecreasingMenko = false;
    private int serialMenkoNumber = 0;

    void Start()
    {
        gaugeController = GameObject.Find("GaugeManager").GetComponent<GaugeController>();
        NextMenkoDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            for (int i = 0; i < currentMenkoList.Count; i++)
            {
                Debug.Log("リスト" + i + currentMenkoList[i].name);
            }
        }
        
        if (currentMenkoList.Count <= 2) { StageMenkoLoader(); }
        
    }

    public void mkShoot(Vector3 pos, float pow)
    {
        mkpow = pow * 10f;
        totalMenkoCount++;
        //Debug.Log(mkpow);
        if (nextMenko != null)
        {
            generatedMenko = Instantiate(nextMenko, pos + Vector3.up * generateHeight, Quaternion.identity);
            Rigidbody gMrb = generatedMenko.GetComponent<Rigidbody>();
            ShootMenko shootMenko = generatedMenko.GetComponent<ShootMenko>();
            currentShootMenkoNumber = shootMenko.menkoType;
            //発射方向の計算
            Vector3 shootDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - generatedMenko.transform.position).normalized;
            gMrb.AddForce(mkForce * mkpow, ForceMode.Impulse);
        }
    }
    public void StageMenkoLoader()
    {
        int menkoCount = 5;
        totalMenkoCount += 5;
        occupiedPositions.Clear();

        // availableMenkoTypesをリセットして、重複しないメンコタイプを選択
        availableMenkoTypes.Clear();

        // 新しいメンコを配置する際のループ
        for (int i = 0; i < menkoCount; i++)
        {
            Vector3 randomPosition = GenerateUniquePosition();

            int randomMenkoIndex;

            // ランダムにメンコを選ぶが、すでに選ばれたタイプは除外
            do
            {
                randomMenkoIndex = Random.Range(0, stagemenkoHolder.Length);
            } while (availableMenkoTypes.Contains(randomMenkoIndex));  // 既に選ばれたメンコタイプを再選しない

            // メンコをインスタンス化
            generatedMenko = Instantiate(stagemenkoHolder[randomMenkoIndex], randomPosition, Quaternion.identity);
            StageMenko stageMenko = generatedMenko.GetComponent<StageMenko>();
            stageMenko.menkoType = randomMenkoIndex;
            stageMenko.SerialNum = currentMenkoList.Count;

            // メンコタイプをリストに追加
            availableMenkoTypes.Add(randomMenkoIndex);
            currentMenkoList.Add(generatedMenko);
            serialMenkoNumber++;
        }

        // ログ出力（デバッグ用）
        foreach (int menkoType in availableMenkoTypes)
        {
            //Debug.Log(menkoType);
        }
    }


    private Vector3 GenerateUniquePosition()
    {
        Vector3 randomPosition;
        bool isUnique = false;

        // 重複しない位置が見つかるまでループ
        do
        {
            // ランダムな位置を生成
            float randomX = Random.Range(-stageWidth / 2f, stageWidth / 2f);
            float randomZ = Random.Range(-stageDepth / 2f, stageDepth / 2f);
            randomPosition = new Vector3(randomX, generateHeight, randomZ);

            // 他のメンコとの距離が十分に離れているかチェック
            isUnique = true;
            foreach (Vector3 occupiedPosition in occupiedPositions)
            {
                if (Vector3.Distance(randomPosition, occupiedPosition) < minDistance)
                {
                    isUnique = false; // 他のメンコと距離が近すぎる
                    break;
                }
            }

            // 生成された位置が重複していないかチェック
            if (isUnique)
            {
                occupiedPositions.Add(randomPosition); // 一意の位置としてリストに追加
            }

        } while (!isUnique); // 重複しない位置が見つかるまでループ

        return randomPosition;
    }

    private void NextMenkoDisplay()
    {
        // 破棄されたGameObjectをリストから削除
        currentMenkoList.RemoveAll(menko => menko == null);

        List<int> menkoTypesOnBoard = new List<int>();

        foreach (var menko in currentMenkoList)
        {
            if (menko != null)
            {
                StageMenko shootMenko = menko.GetComponent<StageMenko>();
                int menkoType = shootMenko.menkoType;
                if (!menkoTypesOnBoard.Contains(menkoType))
                {
                    menkoTypesOnBoard.Add(menkoType);
                }
            }
        }

        if (menkoTypesOnBoard.Count > 0)
        {
            // 盤面にあるメンコの種類の中からランダムに選択
            int randomIndex = Random.Range(0, menkoTypesOnBoard.Count);
            int nextMenkoType = menkoTypesOnBoard[randomIndex];

            nextMenko = shootmenkoHolder[nextMenkoType];

            // 次に射出するメンコの画像をUIに設定
            nextImage.sprite = MenkoImages[nextMenkoType];
        }
        else
        {
            Debug.LogWarning("めんこ不在");
        }
    }

    public void DecreaseMenkoCount(int menkoType, int serialNum)
    {
        for (int i = 0; i < currentMenkoList.Count; i++)
        {
            StageMenko stageMenko = currentMenkoList[i].GetComponent<StageMenko>();
            if (stageMenko != null && stageMenko.menkoType == menkoType && stageMenko.SerialNum == serialNum)
            {
                Debug.Log("タイプ" + menkoType );
                Debug.Log("リスト"+serialNum);

                Debug.Log("Stageタイプ" + stageMenko.menkoType);
                Debug.Log("Stageリスト" + stageMenko.SerialNum);
                Destroy(currentMenkoList[i]);
                currentMenkoList.RemoveAt(i);
                break; // 一致したメンコを消去後にループを終了
            }
        }

        totalMenkoCount--;
        NextMenkoDisplay();
    }

}
