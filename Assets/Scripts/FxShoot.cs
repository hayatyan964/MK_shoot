using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FxShoot : MonoBehaviour
{
    // ShootController�̍č\��
    [SerializeField]
    private float generateHeight = 3f; //�����R�����̍���

    public GameObject[] stagemenkoHolder; // �����R�̃v���n�u
    public GameObject[] shootmenkoHolder;
    public static float stageWidth = 8f; 
    public static float stageDepth = 13f; // ��̕��Ɖ��s��

    private GameObject generatedMenko;

    private Vector3 mkForce = new Vector3(0.0f, -2.0f, 0.0f);
    public float mkpow = 0; // ���˗�
    public int totalMenkoCount = 0; //�����R�̐�

    private GaugeController gaugeController;
    // �����R���z�u���ꂽ�ʒu���Ǘ����郊�X�g
    private List<Vector3> occupiedPositions = new List<Vector3>();
    // ���łɔz�u���ꂽ�����R���Ǘ����郊�X�g
    private List<GameObject> currentMenkoList = new List<GameObject>();
    [SerializeField]
    private Image nextImage; //���ɓ����郁���R��\������摜
    [SerializeField]
    private Sprite[] MenkoImages; //�e�����R�^�C�v�ɑΉ�����摜���X�g

    private GameObject nextMenko; //���ɓ����郁���R�̃v���n�u

    private int currentShootMenkoNumber = 0; // ���ݎˏo���郁���R�̔ԍ�

    private List<int> availableMenkoTypes = new List<int>();

    private float minDistance = 2.0f; // �����R�Ԃ̍ŏ�����
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
                Debug.Log("���X�g" + i + currentMenkoList[i].name);
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
            //���˕����̌v�Z
            Vector3 shootDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - generatedMenko.transform.position).normalized;
            gMrb.AddForce(mkForce * mkpow, ForceMode.Impulse);
        }
    }
    public void StageMenkoLoader()
    {
        int menkoCount = 5;
        totalMenkoCount += 5;
        occupiedPositions.Clear();

        // availableMenkoTypes�����Z�b�g���āA�d�����Ȃ������R�^�C�v��I��
        availableMenkoTypes.Clear();

        // �V���������R��z�u����ۂ̃��[�v
        for (int i = 0; i < menkoCount; i++)
        {
            Vector3 randomPosition = GenerateUniquePosition();

            int randomMenkoIndex;

            // �����_���Ƀ����R��I�Ԃ��A���łɑI�΂ꂽ�^�C�v�͏��O
            do
            {
                randomMenkoIndex = Random.Range(0, stagemenkoHolder.Length);
            } while (availableMenkoTypes.Contains(randomMenkoIndex));  // ���ɑI�΂ꂽ�����R�^�C�v���đI���Ȃ�

            // �����R���C���X�^���X��
            generatedMenko = Instantiate(stagemenkoHolder[randomMenkoIndex], randomPosition, Quaternion.identity);
            StageMenko stageMenko = generatedMenko.GetComponent<StageMenko>();
            stageMenko.menkoType = randomMenkoIndex;
            stageMenko.SerialNum = currentMenkoList.Count;

            // �����R�^�C�v�����X�g�ɒǉ�
            availableMenkoTypes.Add(randomMenkoIndex);
            currentMenkoList.Add(generatedMenko);
            serialMenkoNumber++;
        }

        // ���O�o�́i�f�o�b�O�p�j
        foreach (int menkoType in availableMenkoTypes)
        {
            //Debug.Log(menkoType);
        }
    }


    private Vector3 GenerateUniquePosition()
    {
        Vector3 randomPosition;
        bool isUnique = false;

        // �d�����Ȃ��ʒu��������܂Ń��[�v
        do
        {
            // �����_���Ȉʒu�𐶐�
            float randomX = Random.Range(-stageWidth / 2f, stageWidth / 2f);
            float randomZ = Random.Range(-stageDepth / 2f, stageDepth / 2f);
            randomPosition = new Vector3(randomX, generateHeight, randomZ);

            // ���̃����R�Ƃ̋������\���ɗ���Ă��邩�`�F�b�N
            isUnique = true;
            foreach (Vector3 occupiedPosition in occupiedPositions)
            {
                if (Vector3.Distance(randomPosition, occupiedPosition) < minDistance)
                {
                    isUnique = false; // ���̃����R�Ƌ������߂�����
                    break;
                }
            }

            // �������ꂽ�ʒu���d�����Ă��Ȃ����`�F�b�N
            if (isUnique)
            {
                occupiedPositions.Add(randomPosition); // ��ӂ̈ʒu�Ƃ��ă��X�g�ɒǉ�
            }

        } while (!isUnique); // �d�����Ȃ��ʒu��������܂Ń��[�v

        return randomPosition;
    }

    private void NextMenkoDisplay()
    {
        // �j�����ꂽGameObject�����X�g����폜
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
            // �Ֆʂɂ��郁���R�̎�ނ̒����烉���_���ɑI��
            int randomIndex = Random.Range(0, menkoTypesOnBoard.Count);
            int nextMenkoType = menkoTypesOnBoard[randomIndex];

            nextMenko = shootmenkoHolder[nextMenkoType];

            // ���Ɏˏo���郁���R�̉摜��UI�ɐݒ�
            nextImage.sprite = MenkoImages[nextMenkoType];
        }
        else
        {
            Debug.LogWarning("�߂񂱕s��");
        }
    }

    public void DecreaseMenkoCount(int menkoType, int serialNum)
    {
        for (int i = 0; i < currentMenkoList.Count; i++)
        {
            StageMenko stageMenko = currentMenkoList[i].GetComponent<StageMenko>();
            if (stageMenko != null && stageMenko.menkoType == menkoType && stageMenko.SerialNum == serialNum)
            {
                Debug.Log("�^�C�v" + menkoType );
                Debug.Log("���X�g"+serialNum);

                Debug.Log("Stage�^�C�v" + stageMenko.menkoType);
                Debug.Log("Stage���X�g" + stageMenko.SerialNum);
                Destroy(currentMenkoList[i]);
                currentMenkoList.RemoveAt(i);
                break; // ��v���������R��������Ƀ��[�v���I��
            }
        }

        totalMenkoCount--;
        NextMenkoDisplay();
    }

}
