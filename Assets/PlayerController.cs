using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

enum State
{
    enChage,    // チャージ状態
    enChargeAttack,    //チャージアタック
    enNormal,   // 通常状態
}

public class PlayerController : MonoBehaviour
{
    //ステータス処理
    State state = State.enNormal;
    //走る速さ
    public float runSpeed = 5.0f;

    //Bボタンで急加速するために必要な変数
    Vector2 previousStickValue = Vector2.zero;  //前フレームのスティック入力を保持
    public float AttackSpeed = 12.5f;                     //アタックのスピード
    public float AttackDuration = 0.5f;                //アタックの持続時間
    public float AttackTimer = 0f;                     //アタックの時間管理

    //チャージアタックに必要な変数
    public float maxChargeTime = 1.5f;        //最大の力を貯める時間
    public float maxBoostForce = 20.0f;     //最大の加速力
    private float chargeTime = 0f;          //溜めた時間

    //チャージアタックのクールタイム関連
    public float chargeCoolTime = 5f;       //何秒クールタイムを設けるか
    private float coolTimer = 0f;           //クールタイムの進行状況

    //移動方向の向きを保存するための変数
    private Vector3 direction;
    //向いている方向を示す矢印のプレハブ
    [SerializeField] private GameObject arrowPrefab;    //矢印のプレハブ
    private GameObject arrowInstance;                   //矢印のインスタンス
    //チャージ可能かどうかを示すエフェクトのプレハブ
    [SerializeField] private GameObject EffectPrefab;   //チャージ可能エフェクトのプレハブ
    private GameObject effectInstance;                  //チャージ可能エフェクトのインスタンス
    //チャージ中のエフェクト
    [SerializeField] private GameObject ChargingEffect; //チャージ中のエフェクトのプレハブ
    private GameObject chargingEffectInstance;          //チャージ中のエフェクトのインスタンス
    
    //リジットボディ
    private Rigidbody rb;
    //撃破されたSE
    public AudioSource DeleteSE;
    //撃破されたか
    //private bool isDelete = false;

    //複数のコントローラーを接続するための変数
    [SerializeField] private int padNo = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Rigitbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();

        //RigitBodyの物理特性を追加
        rb.mass = 1.0f;             //質量
        rb.drag = 0.5f;             //空気抵抗（摩擦のような効果）
        rb.angularDrag = 0.5f;      //回転の減衰
        rb.useGravity = true;       //重力の有効

        rb.constraints = RigidbodyConstraints.FreezeRotationX   //X座標の回転をなくす
            | RigidbodyConstraints.FreezeRotationY              //Y座標の回転をなくす
            | RigidbodyConstraints.FreezeRotationZ;             //Z座標の回転をなくす

        //矢印のインスタンスを生成
        if (arrowPrefab != null)
        {
            arrowInstance = Instantiate(
                arrowPrefab, 
                transform.position + Vector3.up * 1.5f, 
                Quaternion.identity,
                transform);

            arrowInstance.SetActive(false);
        }

        //エフェクトのインスタンスを生成し、非表示。
        if (EffectPrefab != null)
        {
            effectInstance = Instantiate(
                EffectPrefab,
                transform.position,
                Quaternion.Euler(-90.0f,0.0f,0.0f),
                transform);

            effectInstance.SetActive(false);
        }

        //チャージ中のエフェクトのインスタンスを生成し、非表示
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

    //チャージ
    public void Charge()
    {
        chargeTime += Time.deltaTime;
        chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime); // 溜め時間を制限

        //チャージ中のエフェクトの処理
        if (chargingEffectInstance != null && !chargingEffectInstance.activeSelf)
        {
            chargingEffectInstance.SetActive(true);     //チャージ中のエフェクトを表示する
        }

        if (chargingEffectInstance != null && chargeTime == maxChargeTime)
        {
            chargingEffectInstance.SetActive(false);    //最大まで貯まったら、エフェクトを非表示にする
        }

        // チャージ中はチャージ可能エフェクトを非表示にする
        if (effectInstance != null && effectInstance.activeSelf)
        {
            effectInstance.SetActive(false);
        }

        state = State.enChargeAttack;
    }

    //チャージアタック
    private void ChargeAttack()
    {
        //チャージ中のエフェクトを非表示にする
        chargingEffectInstance.SetActive(false);
        //溜めた時間に応じて加速力を計算
        float chargeRatio = Mathf.Clamp01(chargeTime / maxChargeTime);  //0～1に正規化
        float boostForce = chargeRatio * maxBoostForce;

        //プレイヤーに加速力を適応（前方向に加速する）
        Vector3 boostDirection = transform.forward; //プレイヤーの前方向
        rb.AddForce(boostDirection * boostForce, ForceMode.Impulse);

        //クールタイムを開始
        coolTimer = chargeCoolTime;

        //チャージ関連のリセット
        chargeTime = 0f;
        //チャージアタック後に速度をリセットする
        rb.velocity = Vector3.zero; 

        state = State.enNormal;
    }

    //矢印の向きを更新させる
    private void UpdateArrowDirection()
    {
        if (arrowInstance != null)
        {
            arrowInstance.SetActive(true);
            arrowInstance.transform.position = transform.position + Vector3.up * 0.5f;      //矢印の位置を更新
            arrowInstance.transform.rotation = Quaternion.LookRotation(transform.forward);  //矢印の向きをプレイヤーに合わせる
        }
    }

    //撃破音処理
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

        //クールタイマーの更新
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
            case State.enNormal:    //通常状態

                //現在の方向を更新
                direction = new Vector3(horiz, 0, vert);

                if (lstickValue.magnitude > 0.1f)
                {
                    // オブジェクトの前方を移動方向に向ける
                    transform.rotation = Quaternion.LookRotation(direction);
                    //物理ベースの移動
                    Vector3 Force = direction.normalized * runSpeed;
                    rb.AddForce(Force, ForceMode.Force);

                    //lstickValue = transform.TransformDirection(lstickValue);

                    Vector3 movement = new Vector3(horiz, 0.0f, vert);

                    //矢印の向きを更新
                    UpdateArrowDirection();
                }
                else
                {
                    //スティック入力がない場合でも、少しずつ減速していく
                    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 1.5f);
                }
                break;
            case State.enChage:     //チャージ
                

                //現在の方向を更新
                direction = new Vector3(horiz, 0, vert);

                if (lstickValue.magnitude > 0.1f)
                {
                    // オブジェクトの前方を移動方向に向ける
                    transform.rotation = Quaternion.LookRotation(direction);
                    //transform.position += direction.normalized * runSpeed * Time.deltaTime;
                    //lstickValue = transform.TransformDirection(lstickValue);

                    Vector3 movement = new Vector3(horiz, 0.0f, vert);

                    //矢印の向きを更新
                    UpdateArrowDirection();
                }
                Charge();
                break;
            case State.enChargeAttack:     //チャージアタック
                ChargeAttack();
                break;
            default:
                break;
        }

        //if (pad.buttonEast.isPressed)   //ButtonEastが押されたらアタックステートに移行
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
            //クールタイムがない場合、エフェクトを再生する
            if (effectInstance != null && !effectInstance.activeSelf)
            {
                effectInstance.SetActive(true);
            }
        }
        else
        {
            // クールタイム中はエフェクトを非表示にする
            if (effectInstance != null && effectInstance.activeSelf)
            {
                effectInstance.SetActive(false);
            }
        }
    }

    //衝突処理
    private void OnCollisionEnter(Collision collision)
    {
        //衝突相手のタグを取得
        string otherTag = collision.gameObject.tag;

        //DeathPlaneタグに衝突したら非表示（脱落）にする
        if (otherTag == "DeathPlane")
        {
            gameObject.SetActive(false);
            return;
        }

        //Groundタグにぶつかったら、吹き飛ばし処理を行わない
        if (otherTag == "Ground" || otherTag == "Wall")
        {
            return;
        }
        var otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (rb.velocity.magnitude < otherRigidbody.velocity.magnitude)
        {
            //自分の後方方向を計算
            Vector3 knockbackDirection = -transform.forward;    //後ろ方向
            Vector3 upwardDirection = Vector3.up * 10.0f;        //上方向（少しだけ）

            //ノックバックの強さ（同じタグなら弱く、異なるタグなら飛ばす）
            float knockbackForce = (otherTag == this.gameObject.tag) ? 3.0f : 5.0f;

            //ノックバック方向を計算（後ろ方向+上方向）
            Vector3 finalknockbackDirection = (knockbackDirection + upwardDirection).normalized;

            //リジットボディにノックバックの力を加える
            rb.AddForce(finalknockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }

}
