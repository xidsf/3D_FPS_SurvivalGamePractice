using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField]
    private string AnimalName; //동물의 이름
    [SerializeField]
    private int hp; //동물의 hp

    [SerializeField]
    private float walkSpeed; //걸음 속도
    [SerializeField]
    private float runSpeed; //뛰기 스피드
    private float applySpeed; //적용 스피드

    private Vector3 direction;

    //상태 스피드
    private bool isAction; //행동중인지 아닌지
    private bool isWalking; //걷는지 안걷는지
    private bool isRunning; //뛰는지 안뛰는지
    private bool isDead; //죽었는지 판별

    [SerializeField]
    private float walkTime;//걷기 시간
    [SerializeField]
    private float waitTime;//대기 시간
    [SerializeField]
    private float runTime; //뛰기 시간
    private float currentTime;


    //필요한 컴포넌트
    [SerializeField]
    private Animator anim;
    //[SerializeField]
    private Rigidbody rigid;
    //[SerializeField]
    private BoxCollider boxCol;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] sound_pig_Idle;
    [SerializeField]
    private AudioClip sound_pig_Hurt;
    [SerializeField]
    private AudioClip sound_pig_Dead;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        boxCol = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();

        currentTime = waitTime;
        isAction = true;
    }

    
    void Update()
    {
        if(!isDead)
        {
            ElapseTime();
            Rotation();
            Move();
        }
    }

    private void Move()
    {
        if(isWalking || isRunning)
        {
            rigid.MovePosition(transform.position + (transform.forward * applySpeed * Time.deltaTime)); ;
        }
    }

    private void Rotation()
    {
        if(isWalking || isRunning)
        {
            Vector3 rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0, direction.y, 0), 0.01f);
            rigid.MoveRotation(Quaternion.Euler(rotation));
        }
    }

    private void ElapseTime()
    {
        if(isAction)
        {
            currentTime -= Time.deltaTime;
            if(currentTime <= 0)
            {
                StateReset();
            }
        }
    }

    private void StateReset()
    {
        isRunning = false;
        isWalking = false;
        isAction = true;
        applySpeed = walkSpeed;
        anim.SetBool("Run", isRunning) ;
        anim.SetBool("Walk", isWalking);
        direction.Set(0, Random.Range(0, 360f), 0);
        RandomAction();
    }

    private void RandomAction()
    {
        RandSound();
        int _random = Random.Range(0, 4); //대기, 풀뜯기, 두리번, 걷기

        if(_random == 0)
        {
            Wait();
        }
        else if(_random == 1)
        {
            Eat();
        }
        else if (_random == 2)
        {
            Peek();
            
        }
        else if (_random == 3)
        {
            TryWalk();
        }
    }

    private void Wait()
    {
        currentTime = waitTime;
        Debug.Log("대기");
    }

    private void Eat()
    {
        currentTime = waitTime;
        anim.SetTrigger("Eat");
        Debug.Log("풀뜯기");
    }

    private void Peek()
    {
        currentTime = waitTime;
        anim.SetTrigger("Peek");
        Debug.Log("두리번");
    }

    private void TryWalk()
    {
        isWalking = true;
        currentTime = walkTime;
        anim.SetBool("Walk", isWalking);
        applySpeed = walkSpeed;
        Debug.Log("걷기");
    }

    private void Run(Vector3 _targetPos)
    {
        direction = Quaternion.LookRotation(transform.position - _targetPos).eulerAngles;
        currentTime = runTime;
        isWalking = false;
        isRunning = true;
        applySpeed = runSpeed;
        anim.SetBool("Run", isRunning);

    }

    public void Damage(int _dmg, Vector3 _targetPos)
    {
        if(isDead)
        {
            return;
        }
        hp -= _dmg;

        if(hp <= 0)
        {
            Dead();
            return;
        }
        PlaySE(sound_pig_Hurt);
        anim.SetTrigger("Hurt");
        Run(_targetPos);
    }

    private void Dead()
    {
        PlaySE(sound_pig_Dead);
        isWalking = false;
        isRunning = false;
        isDead = true;
        anim.SetTrigger("Dead");
        Destroy(gameObject, 10f);
    }

    private void RandSound()
    {
        int _random = Random.Range(0, 3); //일상 사운드3개

        PlaySE(sound_pig_Idle[_random]);
    }

    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

}
