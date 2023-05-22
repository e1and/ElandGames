using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Move : MonoBehaviour
{
    Vector3 Velocity;
    Vector3 TargetVelocity;
    Vector3 WanderCenter;
    Vector3 Displacement;
    Vector3 SpawnPoint;
    Vector3 Steering;

    [SerializeField] bool _isDrawLines;
    public bool _isFearOfFire;
    [SerializeField] Item itemTorch;
    [SerializeField] bool _isSeek = true;
    [SerializeField] bool _isFlee = false;
    [SerializeField] bool _isWalk = false;
    public bool _isEscape = false;
    public bool isFire;
    [SerializeField] bool _isWander = false;
    [SerializeField] bool _isStay = false;
    [SerializeField] bool _isFollowing = false;
    [SerializeField] int activeNavMeshCoroutines;
    [SerializeField] int activeFollowCoroutines;
    [SerializeField] int activeEscapeCoroutines;
    [SerializeField] int activeWanderCoroutines;
    [SerializeField] int activeStayCoroutines;
    public bool _isAttack = false;
    [SerializeField] bool _isPursuit;
    [SerializeField] bool _isKeepDistance;
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float _distanceToTarget;
    [SerializeField] float _heightToTarget;
    [SerializeField] float _distanceToWanderPoint;
    [SerializeField] bool _isTargeting;
    [SerializeField] float _fearDistance = 3f;
    [SerializeField] float _minFollowDistance = 2f;
    [SerializeField] float _attackDistance;
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
    public NavMeshAgent agent;
    public Player player;

    bool isStopWalk;

    Coroutine WanderCoroutine;
    Coroutine WalkCoroutine;
    Coroutine EscapeCoroutine;
    Coroutine StayCoroutine;
    Coroutine FollowCoroutine;

    public Vector3 newTargetPoint = Vector3.zero;
    InventoryWindow inventoryWindow;

    [SerializeField] GameObject[] Waypoints;
    [SerializeField] GameObject[] EscapePoints;

    public Vector3 WanderPoint = Vector3.zero;
    [SerializeField] Vector3 EscapePoint = Vector3.zero;
    [SerializeField] GameObject escapePointNew;



    private void Start()
    {
        Animator = GetComponent<Animator>();
        SpawnPoint = transform.position;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        inventoryWindow = FindObjectOfType<InventoryWindow>();
        player.GetComponent<Player>();

        Animator.ResetTrigger("GetUp");

    }

    public void MoveTrigger(int b)
    {
        if (b == 1) { _isAttack = true; }
        else _isAttack = false;
    }

    //public void StopMove()
    //{
    //    if (FollowCoroutine != null) StopCoroutine(FollowCoroutine);
    //    if (WalkCoroutine != null) StopCoroutine(WalkCoroutine);
    //    if (WanderCoroutine != null) StopCoroutine(WanderCoroutine);
    //    isStopWalk = true;
    //}

    void ResetCoroutineCounter()
    {
        activeEscapeCoroutines = 0;
        activeFollowCoroutines = 0;
        activeNavMeshCoroutines = 0;
        activeStayCoroutines = 0;
        activeWanderCoroutines = 0;
    }
    void FixedUpdate()
    {
        Animator.SetFloat("Velocity", agent.velocity.magnitude);

        if (Input.GetKey(KeyCode.O))
        { 
            StopCoroutine(FollowCoroutine);
            StopCoroutine(WalkCoroutine);
            StopCoroutine(EscapeCoroutine);
            StopCoroutine(StayCoroutine);
        }
        
        if (_isKeepDistance)
        {
            if (_distanceToTarget < _minFleeDistance) _isSeek = false;
            else if (!_isFlee) _isSeek = true;
        }

        //if (obstacleHits > 100)
        //{
        //    isNavMesh = false;
        //    obstacleHits = 0;
        //}

        _distanceToTarget = Vector3.Distance(transform.position, Player.transform.position);
        _heightToTarget = Mathf.Abs(transform.position.y - Player.transform.position.y);
        _distanceToWanderPoint = Vector3.Distance(transform.position, WanderPoint);

        // Страх огня
        if (_isFearOfFire && !_isEscape && !isFire)
        {
            if (_distanceToTarget < _fearDistance)
            {
                if (inventoryWindow.LeftHandItem != null && inventoryWindow.LeftHandItem == itemTorch && inventoryWindow.LeftHandObject != null
                    && inventoryWindow.LeftHandObject.GetComponent<Torchlight>().isBurn
                    || inventoryWindow.RightHandObject != null && inventoryWindow.RightHandItem == itemTorch && inventoryWindow.RightHandObject != null
                    && inventoryWindow.RightHandObject.GetComponent<Torchlight>().isBurn)
                {
                    _isSeek = false;
                    _isWander = false;
                    _isEscape = true;
                    agent.enabled = false;

                    NewEscapePoint();

                    agent.enabled = true;
                    isNavMesh = true;
                    Debug.Log("Escape");

                    agent.SetDestination(transform.position);

                    StopAllCoroutines();
                    ResetCoroutineCounter();

                    Escape(EscapePoint);
                }
            }
        }

        // Переход от убегания к блужданию
        //if (_distanceToTarget > _minFleeDistance - 0.5f && _isWander)   
        //{ _isFlee = false; _isWander = true; }

        // Преследование
        if (!_isSeek)
        {
            if ((_distanceToTarget < _maxFollowDistance && _distanceToTarget > _minFollowDistance && _heightToTarget < 1)
                && (!_isFlee || !_isEscape || !isFire || !player.isCampfire))
            {
                //if (StayCoroutine != null) StopCoroutine(StayCoroutine);
                //if (WalkCoroutine != null) StopCoroutine(WalkCoroutine);

                _isStay = false;

                if (!_isFollowing && _distanceToTarget > _attackDistance && agent.CalculatePath(player.transform.position, agent.path))
                {
                    Debug.Log("START FOLLOWING");
                    agent.SetDestination(Player.transform.position);;
                    _isSeek = true;
                    FollowCoroutine = StartCoroutine(NavMeshFollow(Player));
                    _isFollowing = true;
                }

                //if (FollowCoroutine == null) _isFollowing = false;
            }
            //else
            //{
            //    if (FollowCoroutine != null)
            //    {
            //        _isFollowing = false;
            //        agent.SetDestination(transform.position);
            //    }
            //}
        }
        
        // Переход к блужданию
        if (_isWander == false && !_isWalk && !_isStay && !_isEscape && !_isFollowing && !isFire && !_isSeek) 
        {
            if (_distanceToTarget > _maxFollowDistance || _heightToTarget > 1 || player.isCampfire)
            {
                _isWander = true;
                _isAttack = false;
                //StopAllCoroutines();
                //ResetCoroutineCounter();
                WanderCoroutine = StartCoroutine(Wander());
            }
        }
        
        // Атака
        if (_distanceToTarget < _attackDistance && !_isAttack && _heightToTarget < 1 && !isFire && !player.isDead)
        {
            if (player.Health <= 0)
            {
                _isAttack = false;
                return;
            }
            _isAttack = true;
            Animator.SetTrigger("Attack");
        }
        
        // Ускоренный поворот к цели на ближней дистанции
        if (_distanceToTarget < _minFollowDistance + 0.5f && _heightToTarget < 1 && !isFire)
        {
            RotateToTarget(Player);
        }
    }

    void RotateToTarget(GameObject target)
    {
        if (Quaternion.LookRotation(target.transform.position - transform.position) != transform.rotation) // Проверка нужен ли поворот
        {
            Vector3 newDirection = target.transform.position - transform.position; // Направление к новой цели
            Quaternion rotation = Quaternion.LookRotation(newDirection); // Угол поворота к новой цели
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 10 * Time.deltaTime); // Плавный поворот
        }
    }

    void NewWayPoint()
    {
        WanderPoint = Waypoints[Random.Range(0, Waypoints.Length - 1)].transform.position;
    }

    void NewEscapePoint()
    {
        EscapePoint = EscapePoints[Random.Range(0, EscapePoints.Length)].transform.position;
    }

    IEnumerator Wander()
    {
        Debug.Log("Wander");
        if (!_isEscape)
        {
            activeWanderCoroutines++;
            _isWander = false;

            if (Random.Range(0, 5) > 1)
            {
                NewWayPoint();

                Debug.Log("Walk");

                agent.SetDestination(WanderPoint);
                _isWalk = true;
                StartCoroutine(NavMeshWalk(WanderPoint));
            }
            else
            {
                _isStay = true;
                StayCoroutine = StartCoroutine(Stay());
            }
        }
        activeWanderCoroutines--;
        yield return null;
        
    }

    public float loseTimer = 0;
    public float loseHeightTimer = 0;

    IEnumerator NavMeshFollow(GameObject target)
    {
        _isFollowing = true;
        Debug.Log("NavMeshFollow");
        activeFollowCoroutines++;
        _isWalk = false;

        _isSeek = true;
        agent.speed = runSpeed;


        while ((loseTimer < 2 && loseHeightTimer < 5) && !player.isSwim)
        {
            if (player.isCampfire) { _isAttack = false; break; }
            if (_isEscape) break;
            if (player.isDead) { _isAttack = false;  break; }
            agent.SetDestination(Player.transform.position);
            Debug.Log("Follow Coroutine");
            if (_distanceToTarget < _minFollowDistance) agent.speed = 0; else agent.speed = runSpeed;
            yield return null;

            if (_distanceToTarget > _maxFollowDistance && _heightToTarget > 1)
            { loseTimer += Time.deltaTime; }
            else loseTimer = 0;

            if (_heightToTarget > 2)
                loseHeightTimer += Time.deltaTime;
            else loseHeightTimer = 0;
        }
        loseHeightTimer = 0;
        loseTimer = 0;
        agent.speed = walkSpeed;
        _isSeek = false;

        _isFollowing = false;
        activeFollowCoroutines--;
        Debug.Log("End Follow");
    }

    public IEnumerator NavMeshWalk(Vector3 target)
    {
        activeNavMeshCoroutines++;
        
        //StopCoroutine("EscapeCoroutine");

        float timer = 0;
        agent.speed = walkSpeed;

        while (Vector3.Distance(transform.position, target) > 1)
        {
            Debug.Log("Walk111 " + agent.isOnNavMesh);
            yield return null;

            if (_isSeek)
            {
                break;
            }

            //if (agent.velocity.magnitude < 0.1f)
            //{
            //    timer += Time.deltaTime;
            //    if (timer > 2)
            //    {
            //        Debug.Log("Obstacle break NavMeshWalk");
            //        NewWayPoint();
            //        break;
            //    }
            //}

            if (_isEscape) break;
        }
        _isWalk = false;
        _isEscape = false;
        isFire = false;
        activeNavMeshCoroutines--;
        isStopWalk = false;
        Debug.Log("End Walk");
        yield return null;
    }

    public IEnumerator NavMeshEscape(Vector3 target)
    {
        //NavMeshHit hit;
        //if (NavMesh.Raycast(transform.position, target, out hit, 0))
        //{ target = hit.position; }
        
        Debug.Log("NavMeshEscape");
        _isWander = false;
        //StopCoroutine("WalkCoroutine");
        _isEscape = true;
        activeEscapeCoroutines++;

        while (Vector3.Distance(transform.position, target) > 1.2f)
        {
            float timer = 0;
            Debug.Log("Escape Coroutine");
            //if (agent.velocity.magnitude <= 0.1f)
            //{
            //    timer += Time.deltaTime;
            //    if (timer > 1)
            //    {
            //        Debug.Log("Obstacle break NavMeshEscape");
            //        break;
            //    }
            //}
            yield return null;
        }

        _isWalk = false;
        _isEscape = false;
        isFire = false;
        activeEscapeCoroutines--;
        isStopWalk = false;
        yield return null;
    }

    public void Escape(Vector3 target)
    {
        Debug.Log("Campfire Escape");
        //StopMove();
        Animator.SetInteger("Move", 1);
        _isWalk = false;
        _isWander = false;
        _isEscape = true;
        agent.enabled = false;
        agent.enabled = true;
        escapePointNew.transform.position = target;
        
        EscapePoint = target;
        //NewEscapePoint();
        
        StopAllCoroutines();
        ResetCoroutineCounter();
        agent.SetDestination(escapePointNew.transform.position);
        EscapeCoroutine = StartCoroutine(NavMeshEscape(escapePointNew.transform.position));
    }

    public IEnumerator Stay()
    {
        float time = 0;
        activeStayCoroutines++;
        //agent.speed = 0;
        agent.SetDestination(transform.position);
        if (!_isStay)
        {
            _isStay = true;
        }
        while (time < Random.Range(1, 3))
        {
            Debug.Log("Stay");
            if (_isAttack || _isEscape || _isWalk) 
            { 
                break; 
            }
            time += Time.deltaTime;
            yield return null;
        }
       _isStay = false;

        activeStayCoroutines--;
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
