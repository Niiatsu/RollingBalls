using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

enum State
{
    enChage,    // �`���[�W���
    enChargeAttack,    //�`���[�W�A�^�b�N
    enNormal,   // �ʏ���
}

public class PlayerController : MonoBehaviour
{
    //�X�e�[�^�X����
    State state = State.enNormal;
    //���鑬��
    public float runSpeed = 5.0f;

    //B�{�^���ŋ}�������邽�߂ɕK�v�ȕϐ�
    Vector2 previousStickValue = Vector2.zero;  //�O�t���[���̃X�e�B�b�N���͂�ێ�
    public float AttackSpeed = 12.5f;                     //�A�^�b�N�̃X�s�[�h
    public float AttackDuration = 0.5f;                //�A�^�b�N�̎�������
    public float AttackTimer = 0f;                     //�A�^�b�N�̎��ԊǗ�

    //�`���[�W�A�^�b�N�ɕK�v�ȕϐ�
    public float maxChargeTime = 1.5f;        //�ő�̗͂𒙂߂鎞��
    public float maxBoostForce = 20.0f;     //�ő�̉�����
    private float chargeTime = 0f;          //���߂�����

    //�`���[�W�A�^�b�N�̃N�[���^�C���֘A
    public float chargeCoolTime = 5f;       //���b�N�[���^�C����݂��邩
    private float coolTimer = 0f;           //�N�[���^�C���̐i�s��

    //�ړ������̌�����ۑ����邽�߂̕ϐ�
    private Vector3 direction;
    //�����Ă���������������̃v���n�u
    [SerializeField] private GameObject arrowPrefab;    //���̃v���n�u
    private GameObject arrowInstance;                   //���̃C���X�^���X
    //�`���[�W�\���ǂ����������G�t�F�N�g�̃v���n�u
    [SerializeField] private GameObject EffectPrefab;   //�`���[�W�\�G�t�F�N�g�̃v���n�u
    private GameObject effectInstance;                  //�`���[�W�\�G�t�F�N�g�̃C���X�^���X
    //�`���[�W���̃G�t�F�N�g
    [SerializeField] private GameObject ChargingEffect; //�`���[�W���̃G�t�F�N�g�̃v���n�u
    private GameObject chargingEffectInstance;          //�`���[�W���̃G�t�F�N�g�̃C���X�^���X
    
    //���W�b�g�{�f�B
    private Rigidbody rb;
    //���j���ꂽSE
    public AudioSource DeleteSE;
    //���j���ꂽ��
    //private bool isDelete = false;

    //�����̃R���g���[���[��ڑ����邽�߂̕ϐ�
    [SerializeField] private int padNo = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Rigitbody�R���|�[�l���g���擾
        rb = GetComponent<Rigidbody>();

        //RigitBody�̕���������ǉ�
        rb.mass = 1.0f;             //����
        rb.drag = 0.5f;             //��C��R�i���C�̂悤�Ȍ��ʁj
        rb.angularDrag = 0.5f;      //��]�̌���
        rb.useGravity = true;       //�d�̗͂L��

        rb.constraints = RigidbodyConstraints.FreezeRotationX   //X���W�̉�]���Ȃ���
            | RigidbodyConstraints.FreezeRotationY              //Y���W�̉�]���Ȃ���
            | RigidbodyConstraints.FreezeRotationZ;             //Z���W�̉�]���Ȃ���

        //���̃C���X�^���X�𐶐�
        if (arrowPrefab != null)
        {
            arrowInstance = Instantiate(
                arrowPrefab, 
                transform.position + Vector3.up * 1.5f, 
                Quaternion.identity,
                transform);

            arrowInstance.SetActive(false);
        }

        //�G�t�F�N�g�̃C���X�^���X�𐶐����A��\���B
        if (EffectPrefab != null)
        {
            effectInstance = Instantiate(
                EffectPrefab,
                transform.position,
                Quaternion.Euler(-90.0f,0.0f,0.0f),
                transform);

            effectInstance.SetActive(false);
        }

        //�`���[�W���̃G�t�F�N�g�̃C���X�^���X�𐶐����A��\��
        if (ChargingEffect != null)
        {
            chargingEffectInstance = Instantiate(
                ChargingEffect,
                transform.position,
                Quaternion.Euler(-90.0f, 0.0f, 0.0f),
                transform);

            chargingEffectInstance.SetActive(false);
        }
    }

    //�`���[�W
    public void Charge()
    {
        chargeTime += Time.deltaTime;
        chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime); // ���ߎ��Ԃ𐧌�

        //�`���[�W���̃G�t�F�N�g�̏���
        if (chargingEffectInstance != null && !chargingEffectInstance.activeSelf)
        {
            chargingEffectInstance.SetActive(true);     //�`���[�W���̃G�t�F�N�g��\������
        }

        if (chargingEffectInstance != null && chargeTime == maxChargeTime)
        {
            chargingEffectInstance.SetActive(false);    //�ő�܂Œ��܂�����A�G�t�F�N�g���\���ɂ���
        }

        // �`���[�W���̓`���[�W�\�G�t�F�N�g���\���ɂ���
        if (effectInstance != null && effectInstance.activeSelf)
        {
            effectInstance.SetActive(false);
        }

        state = State.enChargeAttack;
    }

    //�`���[�W�A�^�b�N
    private void ChargeAttack()
    {
        //�`���[�W���̃G�t�F�N�g���\���ɂ���
        chargingEffectInstance.SetActive(false);
        //���߂����Ԃɉ����ĉ����͂��v�Z
        float chargeRatio = Mathf.Clamp01(chargeTime / maxChargeTime);  //0�`1�ɐ��K��
        float boostForce = chargeRatio * maxBoostForce;

        //�v���C���[�ɉ����͂�K���i�O�����ɉ�������j
        Vector3 boostDirection = transform.forward; //�v���C���[�̑O����
        rb.AddForce(boostDirection * boostForce, ForceMode.Impulse);

        //�N�[���^�C�����J�n
        coolTimer = chargeCoolTime;

        //�`���[�W�֘A�̃��Z�b�g
        chargeTime = 0f;
        //�`���[�W�A�^�b�N��ɑ��x�����Z�b�g����
        rb.velocity = Vector3.zero; 

        state = State.enNormal;
    }

    //���̌������X�V������
    private void UpdateArrowDirection()
    {
        if (arrowInstance != null)
        {
            arrowInstance.SetActive(true);
            arrowInstance.transform.position = transform.position + Vector3.up * 0.5f;      //���̈ʒu���X�V
            arrowInstance.transform.rotation = Quaternion.LookRotation(transform.forward);  //���̌������v���C���[�ɍ��킹��
        }
    }

    //���j������
    public void deleteSE()
    {
        DeleteSE.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Gamepad.all.Count <= padNo)
        {
            return;
        }

        var pad = Gamepad.all[padNo];

        //�N�[���^�C�}�[�̍X�V
        if (coolTimer > 0)
        {
            coolTimer -= Time.deltaTime;
            coolTimer = Mathf.Max(coolTimer, 0f);
        }

        Vector2 lstickValue = pad.leftStick.ReadValue();
        float vert = lstickValue.y;
        float horiz = lstickValue.x;

        switch (state)
        {
            case State.enNormal:    //�ʏ���

                //���݂̕������X�V
                direction = new Vector3(horiz, 0, vert);

                if (lstickValue.magnitude > 0.1f)
                {
                    // �I�u�W�F�N�g�̑O�����ړ������Ɍ�����
                    transform.rotation = Quaternion.LookRotation(direction);
                    //�����x�[�X�̈ړ�
                    Vector3 Force = direction.normalized * runSpeed;
                    rb.AddForce(Force, ForceMode.Force);

                    //lstickValue = transform.TransformDirection(lstickValue);

                    Vector3 movement = new Vector3(horiz, 0.0f, vert);

                    //���̌������X�V
                    UpdateArrowDirection();
                }
                else
                {
                    //�X�e�B�b�N���͂��Ȃ��ꍇ�ł��A�������������Ă���
                    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 1.5f);
                }
                break;
            case State.enChage:     //�`���[�W
                

                //���݂̕������X�V
                direction = new Vector3(horiz, 0, vert);

                if (lstickValue.magnitude > 0.1f)
                {
                    // �I�u�W�F�N�g�̑O�����ړ������Ɍ�����
                    transform.rotation = Quaternion.LookRotation(direction);
                    //transform.position += direction.normalized * runSpeed * Time.deltaTime;
                    //lstickValue = transform.TransformDirection(lstickValue);

                    Vector3 movement = new Vector3(horiz, 0.0f, vert);

                    //���̌������X�V
                    UpdateArrowDirection();
                }
                Charge();
                break;
            case State.enChargeAttack:     //�`���[�W�A�^�b�N
                ChargeAttack();
                break;
            default:
                break;
        }

        //if (pad.buttonEast.isPressed)   //ButtonEast�������ꂽ��A�^�b�N�X�e�[�g�Ɉڍs
        //{
        //    state = State.enAttack;
        //}

        if (pad.buttonSouth.isPressed)
        {
            if (coolTimer <= 0)
            {
                state = State.enChage;
            }
        }

        if (coolTimer <= 0)
        {
            //�N�[���^�C�����Ȃ��ꍇ�A�G�t�F�N�g���Đ�����
            if (effectInstance != null && !effectInstance.activeSelf)
            {
                effectInstance.SetActive(true);
            }
        }
        else
        {
            // �N�[���^�C�����̓G�t�F�N�g���\���ɂ���
            if (effectInstance != null && effectInstance.activeSelf)
            {
                effectInstance.SetActive(false);
            }
        }
    }

    //�Փˏ���
    private void OnCollisionEnter(Collision collision)
    {
        //�Փˑ���̃^�O���擾
        string otherTag = collision.gameObject.tag;

        //DeathPlane�^�O�ɏՓ˂������\���i�E���j�ɂ���
        if (otherTag == "DeathPlane")
        {
            gameObject.SetActive(false);
            return;
        }

        //Ground�^�O�ɂԂ�������A������΂��������s��Ȃ�
        if (otherTag == "Ground" || otherTag == "Wall")
        {
            return;
        }
        var otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (rb.velocity.magnitude < otherRigidbody.velocity.magnitude)
        {
            //�����̌���������v�Z
            Vector3 knockbackDirection = -transform.forward;    //������
            Vector3 upwardDirection = Vector3.up * 10.0f;        //������i���������j

            //�m�b�N�o�b�N�̋����i�����^�O�Ȃ�キ�A�قȂ�^�O�Ȃ��΂��j
            float knockbackForce = (otherTag == this.gameObject.tag) ? 3.0f : 5.0f;

            //�m�b�N�o�b�N�������v�Z�i������+������j
            Vector3 finalknockbackDirection = (knockbackDirection + upwardDirection).normalized;

            //���W�b�g�{�f�B�Ƀm�b�N�o�b�N�̗͂�������
            rb.AddForce(finalknockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }

}
