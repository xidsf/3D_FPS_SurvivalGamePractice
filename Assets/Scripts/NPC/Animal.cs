using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    [SerializeField]
    protected string AnimalName; //동물의 이름
    [SerializeField]
    protected int hp; //동물의 hp

    [SerializeField]
    protected float walkSpeed; //걸음 속도
    [SerializeField]
    protected float runSpeed; //뛰기 스피드

    protected Vector3 destination;

    //상태 스피드
    protected bool isAction; //행동중인지 아닌지
    protected bool isWalking; //걷는지 안걷는지
    protected bool isRunning; //뛰는지 안뛰는지
    protected bool isDead; //죽었는지 판별

    [SerializeField]
    protected float walkTime;//걷기 시간
    [SerializeField]
    protected float waitTime;//대기 시간
    [SerializeField]
    protected float runTime; //뛰기 시간
    protected float currentTime;


    //필요한 컴포넌트
    [SerializeField]
    protected Animator anim;
    //[SerializeField]
    protected Rigidbody rigid;
    //[SerializeField]
    protected BoxCollider boxCol;
    protected AudioSource audioSource;
    protected NavMeshAgent nav;


    [SerializeField]
    protected AudioClip[] sound_animal_Idle;
    [SerializeField]
    protected AudioClip sound_animal_Hurt;
    [SerializeField]
    protected AudioClip sound_animal_Dead;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        boxCol = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();
        nav = GetComponent<NavMeshAgent>(); //navMeshAgent는 rigidbody를 Lock걸어버려서 rigid관련 함수를 무용지물로 만들어버린다.

        currentTime = waitTime;
        isAction = true;
    }


    void Update()
    {
        if (!isDead)
        {
            ElapseTime();
            Move();
        }
    }

    private void Move()
    {
        if (isWalking || isRunning)
        {
            //rigid.MovePosition(transform.position + (transform.forward * applySpeed * Time.deltaTime)); navmesh때문에 rigid가 작동하지 않는다.
            nav.SetDestination(transform.position + destination * 5f);
        }
    }


    private void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                StateReset();
            }
        }
    }

    protected virtual void StateReset()
    {
        isRunning = false;
        isWalking = false;
        isAction = true;
        nav.speed = walkSpeed;
        nav.ResetPath();
        anim.SetBool("Run", isRunning);
        anim.SetBool("Walk", isWalking);
        destination.Set(Random.Range(-0.2f, 0.2f), 0, Random.Range(0.5f, 1f));
    }

    protected void TryWalk()
    {
        isWalking = true;
        currentTime = walkTime;
        anim.SetBool("Walk", isWalking);
        nav.speed = walkSpeed;
        Debug.Log("걷기");
    }

    public virtual void Damage(int _dmg, Vector3 _targetPos)
    {
        if (isDead)
        {
            return;
        }
        hp -= _dmg;

        if (hp <= 0)
        {
            Dead();
            return;
        }
        PlaySE(sound_animal_Hurt);
        anim.SetTrigger("Hurt");
    }

    protected void Dead()
    {
        PlaySE(sound_animal_Dead);
        isWalking = false;
        isRunning = false;
        isDead = true;
        anim.SetTrigger("Dead");
        Destroy(gameObject, 10f);
    }

    protected void RandSound()
    {
        int _random = Random.Range(0, 3); //일상 사운드3개

        PlaySE(sound_animal_Idle[_random]);
    }

    protected void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }
}
