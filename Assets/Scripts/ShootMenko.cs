using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class ShootMenko : MonoBehaviour
{
    public bool isFlipped = false;
    private float minCollisionForce = 7f;

    FxShoot shootController;
    StageMenko stageMenko;
    mainGameManager mainGameManager;

    private float jumpForce = 4.5f;
    private float rotationDelay = 0.07f;
    private float flipTorque = 100f;
    private float rotationDuration = 0;

    private Collider myCollider;
    private Collider targetCollider;

    private bool isProcessing = false;

    public int menkoType = 0;

    AudioSource audioSource;
    public AudioClip lowSound;
    public AudioClip highSound;
    private bool AudioCount = false;

    private void Start()
    {
        shootController = GameObject.Find("ShootManager").GetComponent<FxShoot>();
        rotationDuration = UnityEngine.Random.Range(0.7f, 0.7f);
        mainGameManager = GameObject.Find("mainGame").GetComponent<mainGameManager>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isProcessing) return;
        isProcessing = true;

        float mkforce = shootController.mkpow;
        bool isMenkoCollision = collision.gameObject.CompareTag("Menko");
        bool isStageCollision = collision.gameObject.CompareTag("Ground");

        if (mkforce > minCollisionForce && isMenkoCollision && !isFlipped)
        {
            StageMenko stageMenko = collision.gameObject.GetComponent<StageMenko>();

            if (stageMenko != null && stageMenko.menkoType == menkoType)
            {
                StartCoroutine(FlipAndDestroy(collision.gameObject));
            }
        }else if (isMenkoCollision && isFlipped)
        {
            Debug.Log("”j‰óB");
            Destroy(this.gameObject, 0.1f);
        }
        else if (isStageCollision && !isFlipped)
        {
            Debug.Log("ƒXƒe[ƒW”j‰ó");
            Destroy(this.gameObject, 1.3f);
        }

        if (mkforce > minCollisionForce && !AudioCount)
        {
            audioSource.PlayOneShot(highSound);
            AudioCount = true;
        }
        else if(!AudioCount)
        {
            audioSource.PlayOneShot(lowSound);
            AudioCount = true;
        }

        isProcessing = false;
    }


    private IEnumerator FlipAndDestroy(GameObject targetMenko)
    {
        myCollider = this.GetComponent<BoxCollider>();
        targetCollider = targetMenko.GetComponent<BoxCollider>();
        Rigidbody myRb = this.GetComponent <Rigidbody>();
        Rigidbody targetRb = targetMenko.GetComponent<Rigidbody>();

        stageMenko = targetMenko.GetComponent<StageMenko>();

        //if (stageMenko.menkoType == menkoType && !isFlipped)
        if(!isFlipped)
        {

            myRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            targetRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            myCollider.enabled = false;
            targetCollider.enabled = false;
            
            yield return new WaitForSeconds(rotationDelay);
            targetRb.AddTorque(Vector3.forward * flipTorque, ForceMode.Impulse);
            myRb.AddTorque(Vector3.forward * flipTorque, ForceMode.Impulse);
            yield return new WaitForSeconds(rotationDuration);
            
            targetRb.angularVelocity = Vector3.zero;
            myRb.angularVelocity = Vector3.zero;

            isFlipped = true;

            myCollider.enabled = true;
            targetCollider.enabled = true;

            mainGameManager.increaseScore();
            shootController.DecreaseMenkoCount(stageMenko.menkoType,stageMenko.SerialNum);

            Destroy(targetMenko.gameObject);
            Destroy(this.gameObject);

        }
    }

}
