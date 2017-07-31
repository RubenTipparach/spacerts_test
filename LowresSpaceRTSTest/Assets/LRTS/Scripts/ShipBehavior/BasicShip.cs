using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

[RequireComponent(typeof(TSBoxCollider2D))]
public class BasicShip : TrueSyncBehaviour
{

    TSVector2 target;

    private bool movingToTarget = false;

    private bool attackingPhase = false;

    private BasicShip attackingTarget;

    [SerializeField]
    FP CoolDownFireRate;

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    FP weaponDamage;

    [SerializeField]
    FP moveSpeed = 1f;

    [SerializeField]
    FP rotationSpeed = 1f;

    [SerializeField]
    FP startingHealth = 100;

    [SerializeField]
    FP currentHealth = 100;

    [SerializeField]
    int groupId;

    [SerializeField]
    FP deltaThresholdToStop = .05f;

    [SerializeField]
    Shipclass shipClass;

    FP delta = 0;

    TSVector2 lastPosition;

    [SerializeField]
    FP smooth;

    [SerializeField]
    GameObject explosionEffect;

    FP velocity;

    FP currentSpeed;

    public string ShipId;

    public byte ShipIndex;

    CoolDownSystem attackBehavior;

    public void TakeDamage(FP val)
    {
        currentHealth -= val;
        HealthBar.value = (currentHealth/startingHealth).AsFloat();

        if (currentHealth <= 0)
        {
            TrueSyncManager.SyncedDestroy(gameObject);
            GameObject.Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
    }

    public FP GetCurrentHealth
    {
        get
        {
            return currentHealth;
        }
    }

    public TSVector2 CurrentGridPositionInt
    {
        get;
        set;
    }
    public HealthBar HealthBar
    {
        get;
        set;
    }

    public int GroupId
    {
        get
        {
            return groupId;
        }
    }

    //public TeamStance TeamDatabase
    //{
    //    get;
    //    set;
    //}

    public Shipclass ShipClass
    {
        get
        {
            return shipClass;
        }
    }

    //private GridManager _gridManagerInstance;

    public bool Initialized
    {
        get;
        private set;
    }

    private bool onFocus = false;

    void OnApplicationFocus(bool focusStatus)
    {
        onFocus = focusStatus;
    }

    /// <summary>
    /// Quick operation to help with rotational stuff, and also to help with formations.
    /// </summary>
    public FP LookAtTarget
    {
        get
        {
            var diff = (target - tsTransform2D.position).normalized;
            return FP.Atan2(diff.y, diff.x) * FP.Rad2Deg - new FP(90);
            //return TSVector2.Angle(tsTransform2D.rotation * TSVector2.right, target);
        }
    }

    GameShipManager shipManager;

    private void Start()
    {
        shipManager = GameObject.FindGameObjectWithTag("CanvasRaycastTarget").GetComponent<GameShipManager>();
        shipManager.RegisterShip(this);
    }

    // Use this for initialization
    public override void OnSyncedStartLocalPlayer()
    {
        currentHealth = startingHealth;

        Initialized = true;
    }

    // Update is called once per frame
    public override void OnSyncedUpdate()
    {

        // This will be implementing basic movement mechanics.

        if (movingToTarget)
        {
            //Debug.Log(tsTransform2D);

            lastPosition = tsTransform2D.position;

            FP targetAngle = this.LookAtTarget;
            // TODO: this might not be correct, we'll see.
            FP angularDist = TSVector2.Angle(tsTransform2D.rotation * TSVector2.right, targetAngle * TSVector2.right);// (Quaternion.Angle(transform.rotation, targetAngle) - 90 )/180;

            FP arrivalDist = TSVector2.Distance(tsTransform2D.position, target);

            //Debug.Log("angular dist"+angularDist);

            //Debug.Log("arrivalDist " + arrivalDist);
            //Debug.Log("size y " + GetComponent<TSBoxCollider2D>().size.y);

            if (/*angularDist < 10 ||*/ arrivalDist < GetComponent<TSBoxCollider2D>().size.y * 2)
            {

                tsTransform2D.position = TrueSyncExt.MoveTowardsV2(tsTransform2D.position, target, moveSpeed * TrueSyncManager.DeltaTime);
                currentSpeed = 0;

                // changing the distance to destination
                delta = TSVector2.Distance(target, tsTransform2D.position);

                // Slick movement stop criteria. Works like a charm!
                if (deltaThresholdToStop > delta)
                {
                    //Debug.Log("delta thresh " + deltaThresholdToStop);
                    //Debug.Log("delta to position " + delta);
                    movingToTarget = false;
                    tsTransform2D.position = target;
                }
            }
            else
            {
                //Debug.Log("cur speed " + currentSpeed);
                currentSpeed = TSMath.SmoothStep(currentSpeed, moveSpeed, smooth);//unitys smooth has velocity too
                var diff = (target - tsTransform2D.position).normalized;
                // TODO: will include this later
                tsTransform2D.position += (diff * currentSpeed * TrueSyncManager.DeltaTime);
            }
            
            tsTransform2D.rotation = 
                TSQuaternion.RotateTowards(
                    TSQuaternion.Euler(0, 0, tsTransform2D.rotation),
                    TSQuaternion.Euler(0,0,targetAngle), 
                    rotationSpeed /* TrueSyncManager.DeltaTime*/).eulerAngles.z;

                /*TrueSyncExt.LerpFP(
               TSQuaternion.Lerp
                targetAngle,
                rotationSpeed * TrueSyncManager.DeltaTime);*/

            Debug.DrawLine(transform.position, target.ToVector(), Color.green);
        }

        if(attackingPhase)
        {
            if(attackingTarget == null)
            {
                attackingPhase = false;
            }

            attackBehavior.UpdateAction(() =>
            {
                // TODO: deterministic random seed in the future.
                attackingTarget.TakeDamage(weaponDamage);
                Debug.Log("is attacking!");

                var diff = (attackingTarget.tsTransform2D.position - tsTransform2D.position).normalized;
                FP targetAngle =  FP.Atan2(diff.y, diff.x) * FP.Rad2Deg - new FP(90);

                var gb = GameObject.Instantiate(bulletPrefab, this.transform.position, Quaternion.Euler(0, 0, targetAngle.AsFloat()));
                var enemyCollider = attackingTarget.GetComponent<TSBoxCollider2D>().size;

                gb.GetComponent<BulletMotion>().SetTarget(Mathf.Max(enemyCollider.x.AsFloat(), enemyCollider.y.AsFloat()), attackingTarget.gameObject);
            });
        }
    }

    public void MoveShip(TSVector2 destination, int orderInList)
    {
        //target = _gridManagerInstance.PlaceOnGrid(this, destination);
        target = destination + TSVector2.right*GetComponent<TSBoxCollider2D>().size.x*orderInList;
        lastPosition = TSVector2.zero;
        movingToTarget = true;
    }

    public void SetAttacking(BasicShip ship)
    {
        attackingPhase = true;
        attackingTarget = ship;
        attackBehavior = new CoolDownSystem(CoolDownFireRate);
    }

    public enum Shipclass
    {
        Fighter,
        BattleFrigate,
        TroopTransport,
        Barge,
        ScienceVessel,
        UtilityShip
    }


    private void OnDestroy()
    {
        shipManager.RemoveShip(this);
    }
}

//public class AttackBehavior : CoolDownSystem
//{

//    public AttackBehavior(FP cd): base(cd)
//    {

//    }
//}
    

public class CoolDownSystem
{
    protected FP timePassed = 0;

    protected FP timeCool;

    public CoolDownSystem(FP timeCoolDown)
    {
        this.timeCool = timeCoolDown;
    }

    public void UpdateAction(Action runnable)
    {
        if(timePassed < timeCool)
        {
            timePassed += TrueSyncManager.DeltaTime;
        }
        else
        {
            timePassed = 0;
            runnable();
        }
    }
}

