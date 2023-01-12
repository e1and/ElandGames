using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Move : MonoBehaviour
{
    [SerializeField] Vector3 Velocity;
    [SerializeField] Vector3 TargetVelocity;
    [SerializeField] Vector3 WanderCenter;
    [SerializeField] Vector3 Displacement;
    [SerializeField] Vector3 SpawnPoint;
    Vector3 Steering;
    Coroutine Targeting;
    Coroutine Staying;
    [SerializeField] bool _isDrawLines;
    [SerializeField] bool _isSeek = true;
    [SerializeField] bool _isWander = false;
    [SerializeField] bool _isStay = false;
    public bool _isAttack = false;
    [SerializeField] bool _isPursuit;
    [SerializeField] bool _isKeepDistance;
    [SerializeField] float _speed = 4f;
    [SerializeField] float _followSpeed = 15f;
    [SerializeField] float _distanceToTarget;
    [SerializeField] bool _isTargeting;
    [SerializeField] float _minFollowDistance = 2f;
    [SerializeField] float _maxFollowDistance = 5f;
    [SerializeField] float _minFleeDistance = 2f;
    [SerializeField] float _maxFleeDistance = 5f;
    [SerializeField] float _slowingRadius = 3f;
    [SerializeField] float _mass = 70f;
    [SerializeField] float _wanderRadius = 2;
    [SerializeField] float _wanderCenter = 5;
    [SerializeField] float _wanderAngle;
    [SerializeField] float _wanderAngleRange = 30;
    [SerializeField] float _wanderMaxRadius = 20;
    public GameObject Player;
    private Animator Animator;
    public LineRenderer moveLine;
    public LineRenderer targetLine;
    Rigidbody rb;
    public bool isObstacle;
    public int obstacleHits = 0;
    public bool isNavMesh;
    private NavMeshAgent agent;

    [SerializeField] GameObject[] Waypoints;    

    [SerializeField] Vector3 WanderPoint = Vector3.zero;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        SpawnPoint = transform.position;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }
    void FixedUpdate()
    {
        if (_isKeepDistance)
        if (_distanceToTarget < _minFleeDistance) _isSeek = false;
        else _isSeek = true;

        if (!isNavMesh)
        {
            if (_isPursuit && _isSeek && !_isWander) { Pursuit(); _speed = _speed * 1.5f; }
            else if (_isPursuit && !_isSeek && !_isWander) { Evade(); _speed = _speed * 1.5f; }
            else if (_isWander == false)
            {
                _speed = _followSpeed;
                if (_isSeek) Seek(Player.transform.position);
                else Flee(Player.transform.position);
            }
        }

        if (obstacleHits > 100)
        {
            isNavMesh = false;
            obstacleHits = 0;
        }
        
        if (_isDrawLines) DrawLines();

        _distanceToTarget = Vector3.Distance(transform.position, Player.transform.position);

        if (_distanceToTarget < _maxFollowDistance)
        {
            _isSeek = true;
            isNavMesh = false;
            _isStay = false;
            if (Staying != null)
            {
                StopCoroutine(Staying);   
            }
        }

        if (_isWander == false && _distanceToTarget > _maxFollowDistance && !isNavMesh && !_isStay) 
        { 
            _isWander = true;  
            StartCoroutine(Wander()); 
        }

        if (_distanceToTarget < _minFollowDistance + 0.5f && !_isAttack)
        {
            Animator.SetTrigger("Attack");
            //TargetingInBattle();
        }

        if (_distanceToTarget < _minFollowDistance + 0.5f && Targeting == null)
        {
            //Targeting = StartCoroutine(RotateToTarget());
        }
    }

    void Move(Vector3 velocity, Vector3 targetVelocity)
    {
        if (isObstacle) Animator.SetInteger("Move", 0);

        velocity.y = 0;
        Steering = Vector3.ClampMagnitude(targetVelocity - Velocity, _speed) / _mass;
        Velocity = Vector3.ClampMagnitude(Velocity + Steering, _speed);
        if (_distanceToTarget > _minFollowDistance + 0.2 && !isObstacle || !_isSeek && _distanceToTarget < _minFleeDistance)
        {
            if (!_isAttack)
            {
                var rbY = rb.velocity.y;
                rb.velocity = new Vector3(Velocity.x * Time.fixedDeltaTime * 20, rbY, Velocity.z * Time.fixedDeltaTime * 20);  /////////////////////////////////
                Animator.SetInteger("Move", 1);
            }
        }
        else Animator.SetInteger("Move", 0);

        float singleStep = _followSpeed * Time.deltaTime * 10f;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, velocity, singleStep, 0.0f);

        if (velocity != Vector3.zero)
        {
            if (_distanceToTarget > _minFollowDistance + 2)
            {
                transform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Player.transform.position - transform.position), 10 * Time.deltaTime);
            }
        }
    }
    public void MoveTrigger(int b)
    {
        if (b == 1) { _isAttack = true; }
        else _isAttack = false;
    }


    IEnumerator RotateToTarget()
    {
        _isTargeting = true;
        while (_distanceToTarget < 4)
        {
            float singleStep = _followSpeed * Time.deltaTime * 10000f;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, Velocity, singleStep, 0.0f);

            if (Velocity != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);

            yield return null;
        }
        _isTargeting = false;
        Targeting = null;
    }
    void TargetingInBattle()
    {
        if (Quaternion.LookRotation(Player.transform.position - transform.position) != transform.rotation) // Проверка нужен ли поворот
        {
            Vector3 newDirection = Player.transform.position - transform.position; // Направление к новой цели
            Quaternion rotation = Quaternion.LookRotation(newDirection); // Угол поворота к новой цели
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 10 * Time.deltaTime); // Плавный поворот
        }
    }

    void Seek(Vector3 target)
    {
        if (_distanceToTarget > _minFollowDistance && _distanceToTarget < _maxFollowDistance)
        {
            isNavMesh = false;
            agent.enabled = false;
            TargetVelocity = (target - transform.position).normalized * _speed;
            if (_distanceToTarget < _slowingRadius && _slowingRadius > _minFollowDistance)
            {
                TargetVelocity = TargetVelocity.normalized * _distanceToTarget / _slowingRadius;
            }

                Move(Velocity, TargetVelocity);

        }
    }

    void Pursuit()
    {
        float distance = Vector3.Distance(Player.transform.position, transform.position);
        float predictFrames = distance / Player.GetComponent<Player>().Speed;
        Vector3 futurePosition = Player.transform.position + Player.GetComponent<Player>().moveVector * predictFrames;
        Seek(futurePosition);
    }

    void Evade()
    {
        float distance = Vector3.Distance(Player.transform.position, transform.position);
        float predictFrames = distance / Player.GetComponent<Player>().Speed;
        Vector3 futurePosition = Player.transform.position + Player.GetComponent<Player>().moveVector * predictFrames;
        Flee(futurePosition);
    }

    void Flee(Vector3 target)
    {
        if (_distanceToTarget < _minFleeDistance)
        {
            isNavMesh = false;
            TargetVelocity = (transform.position - target).normalized * _speed;
            Move(Velocity, TargetVelocity);
        }
    }

    void NewWanderPoint()
    {
        _wanderAngle += Random.Range(-_wanderAngleRange, _wanderAngleRange);

        Displacement = Vector3.zero;

        Displacement.x = Mathf.Cos(_wanderAngle * Mathf.Deg2Rad);
        Displacement.z = Mathf.Sin(_wanderAngle * Mathf.Deg2Rad);
        Displacement = Displacement.normalized * _wanderRadius;

        WanderCenter = Velocity.normalized * _wanderCenter;
        WanderCenter.y = 0;

        WanderPoint = transform.position + WanderCenter + Displacement;
    }

    void NewWayPoint()
    {
        WanderPoint = Waypoints[Random.Range(0, Waypoints.Length - 1)].transform.position;
    }
    
    IEnumerator Wander()
    {
        _isWander = false;
        _isSeek = false;
        _speed = 3;

        if (Random.Range(0, 5) > 2) 
        { 
            NewWayPoint();
            agent.enabled = true;
            isNavMesh = true;
            Debug.Log("Go");

            agent.SetDestination(WanderPoint);
            
            StartCoroutine(NavMeshWay());
        }
        else
        {
            StartCoroutine(Stay());
        }

        yield return null;
    }

    IEnumerator NavMeshWay()
    {
        Animator.SetInteger("Move", 1);
        while (Vector3.Distance(transform.position, WanderPoint) > 1)
        {
            //Debug.Log(Vector3.Distance(transform.position, WanderPoint));
            yield return null;

            if (_distanceToTarget < _maxFollowDistance)
            {
                _isSeek = true;
                break;
            }

            if (isObstacle)
            {
                Debug.Log("Obstacle");
                Stay();
                break;
            }
        }
        isNavMesh = false;
        agent.enabled = false;
        _isWander = false;
    }

    IEnumerator Stay()
    {
        Debug.Log("Stay");
        _isStay = true;
        agent.enabled = false;
        Animator.SetInteger("Move", 0);
        yield return new WaitForSeconds(Random.Range(1, 3));
        _isStay = false;
    }

    void DrawLines()
    {
        Vector3 startPoint = transform.position;
        Vector3 movePoint = transform.position + Velocity + Steering;
        Vector3 targetPoint = transform.position + TargetVelocity;
        startPoint.y = 0;
        movePoint.y = 0;
        targetPoint.y = 0;

        moveLine.SetPosition(0, startPoint);
        moveLine.SetPosition(1, movePoint);
        targetLine.SetPosition(0, startPoint);
        targetLine.SetPosition(1, targetPoint);
    }

    public enum AiMoveType : byte
    { 
        None = 0,
        Wait = 1,
        Seek = 2,
        Flee = 3,
        Pursuit = 4,
        Evade = 5,
        Wander = 6
    }
}
