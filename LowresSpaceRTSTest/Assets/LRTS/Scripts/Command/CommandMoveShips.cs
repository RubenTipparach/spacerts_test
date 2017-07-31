using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class CommandMoveShips : TrueSyncBehaviour
{

    private byte MOUSE_X = 0;
    private byte MOUSE_Y = 1;
    private byte BOOL_CLICKED_RIGHT = 2;
    private byte SHIP_OPERATIONS_LIST = 3;
    private byte BOOL_ATTACKING_TARGET = 4;
    private byte SHIP_ATTACKING_LIST = 5;

    private bool onFocus = false;

    private TSVector2 destination;

    GameShipManager gameShipManager;

    void OnApplicationFocus(bool focusStatus)
    {
        onFocus = focusStatus;
    }

    public override void OnSyncedStartLocalPlayer()
    {
    }

    public override void OnSyncedStart()
    {
        //todo set singleton instead.
        gameShipManager = GameObject.FindGameObjectWithTag("CanvasRaycastTarget").GetComponent<GameShipManager>();
    }

    public override void OnSyncedInput()
    {
        // if not on focus, send current position
        //if (!onFocus)
        //{
        //    TrueSyncInput.SetInt(MOUSE_X, (int)transform.position.x);
        //    TrueSyncInput.SetInt(MOUSE_Y, (int)transform.position.z);
        //    Debug.Log("notfocused" );

        //    return;
        //}

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            TSVector2 tsv = new TSVector2(v.x, v.y);

            // add tick-input to local queue and send to peers
            TrueSyncInput.SetFP(MOUSE_X, tsv.x);
            TrueSyncInput.SetFP(MOUSE_Y, tsv.y);
            TrueSyncInput.SetBool(BOOL_CLICKED_RIGHT, true);
            TrueSyncInput.SetByteArray(SHIP_OPERATIONS_LIST, gameShipManager.SelectedShipsIndecies);

            // Debug.Log("hitpoint " + tsv);

            TSCollider2D c = TSPhysics2D.OverlapPoint(tsv);
            if(c != null)
            {
                // begin attack phase.
                // we'll deal with attacking sides later :P
                TrueSyncInput.SetBool(BOOL_ATTACKING_TARGET, true);
                TrueSyncInput.SetByte(SHIP_ATTACKING_LIST, c.GetComponent<BasicShip>().ShipIndex);
            }
        }
        //else
        //{
        //}

        //needs to deal with mouse click.
    }

    public override void OnSyncedUpdate()
    {
   
        if(TrueSyncInput.GetBool(BOOL_CLICKED_RIGHT))
        {
            // receive click pos from input queue
            destination.x = TrueSyncInput.GetFP(MOUSE_X);
            destination.y = TrueSyncInput.GetFP(MOUSE_Y);

            // Since this a move action, will continue with this operation. We'll add to this in the future.
            List<byte> ids = new List<byte>(TrueSyncInput.GetByteArray(SHIP_OPERATIONS_LIST));
            bool isAttacking = TrueSyncInput.GetBool(BOOL_ATTACKING_TARGET);
                
            gameShipManager.ForEachAndAllShips((shipAction) =>
            {
                // TODO: a dictionary may have faster lookup tume.
                if(ids.Contains(shipAction.ShipIndex))
                {

                    if(isAttacking)
                    {
                        byte attackId = TrueSyncInput.GetByte(SHIP_ATTACKING_LIST);

                        shipAction.SetAttacking(gameShipManager.GetShipByIndex(attackId));
                    }
                    else
                    {
                        int count = ids.Count;
                        int order = ids.FindIndex(p=> p == shipAction.ShipIndex);

                        shipAction.MoveShip(destination, order);
                    }
                }
            });

            TrueSyncInput.SetBool(BOOL_CLICKED_RIGHT, false);
        }
    }
}
