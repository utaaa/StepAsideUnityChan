using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnityChanController : MonoBehaviour {

    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;

    //unityちゃんを移動させるコンポーネントを入れる
    private Rigidbody myRigidbody;

    //前進するための力
    private float forwardForce = 800.0f;

    //左右に移動するための力
    private float turnForce = 500.0f;

    //ジャンプするための力
    private float upForce = 500.0f;

    //左右の移動できる範囲
    private float movableRange = 3.4f;

    //動きを減速させる係数
    private float coefficienet = 0.95f;

    //ゲーム終了の判定
    private bool isEnd = false;

    //ゲーム終了時に表示させるテキスト
    private GameObject stateText;

	// Use this for initialization
	void Start ()
    {
        //Animatorコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();

        //走るアニメーションを開始
        this.myAnimator.SetFloat("Speed", 1);

        //Rigidbodyコンポーネントを所得
        this.myRigidbody = GetComponent<Rigidbody>();

        //シーン内のstateTextオブジェクトを取得
        this.stateText = GameObject.Find("GameResultText");
	}
	
	// Update is called once per frame
	void Update ()
    {
        //ゲーム終了ならunityちゃんの動きを減衰する
        if (this.isEnd)
        {
            this.forwardForce *= this.coefficienet;
            this.turnForce *= this.coefficienet;
            this.upForce *= this.coefficienet;
            this.myAnimator.speed *= this.coefficienet;
        }

        //unityちゃんに前方向の力を加える
        this.myRigidbody.AddForce(this.transform.forward * this.forwardForce);

        //unityちゃんを矢印キーまたはボタンに応じて左右に移動させる
        if (Input.GetKey(KeyCode.LeftArrow) && -this.movableRange < this.transform.position.x)
        {
            //左に移動
            this.myRigidbody.AddForce(-this.turnForce, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow) && this.transform.position.x < this.movableRange)
        {
            //右に移動
            this.myRigidbody.AddForce(this.turnForce, 0, 0);
        }

        //Jampステートの場合はJampにfalseをセットする
        if (this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jamp"))
        {
            this.myAnimator.SetBool("Jamp", false);
        }


        //ジャンプしていない時にスペースが押されたらジャンプする
        if (Input.GetKeyDown(KeyCode.Space) && this.transform.position.y < 0.5f)
        {
            //ジャンプアニメを再生
            this.myAnimator.SetBool("jump", true);

            //Unityちゃんに上方向の力を加える
            this.myRigidbody.AddForce(this.transform.up * this.upForce);
        }
	}

    //トリガーモードで他のオブジェクトと接触した場合の処理
    void OnTriggerEnter(Collider other)
    {
        //障害物に衝突した場合
        if (other.gameObject.tag == "CarTag" || other.gameObject.tag == "TrafficCornTag")
        {
            this.isEnd = true;
            
            //stateTextにGAME OVERを表示
            this.stateText.GetComponent<Text>().text = "GAME OVER";

        }

        //ゴール地点に到達した場合
        if (other.gameObject.tag == "GoalTag")
        {
            this.isEnd = true;
           
            //stateTextにGAME CLEARを表示
            this.stateText.GetComponent<Text>().text = "CLEAR!!";
        }

        //コインに衝突した場合
        if (other.gameObject.tag == "CoinTag")
        {
            //パーティクルを再生
            GetComponent<ParticleSystem>().Play();

            //接触したコインのオブジェクトを廃棄
            Destroy(other.gameObject);
        }

    }

}
