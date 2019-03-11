using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;


public class TankComponent : NetworkBehaviour
{
    public float InputVertical;
    public float InputHorizontal;

    public Rigidbody2D Rigidbody2D;

    public GameObject Treads;

    public float Speed = 1.0f;

    [SyncVar]
    protected float _treadRotation;

    private void FixedUpdate()
    {
        if (isServer)
        {
            var direction = new Vector2(InputHorizontal, InputVertical);

            if(direction.magnitude > 1.0f)
            {
                direction = direction.normalized;
            }

            Rigidbody2D.velocity = direction * Speed;

            if(InputHorizontal != 0.0f && InputVertical != 0.0f)
            {
                var lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
                Treads.transform.rotation = Quaternion.Lerp(Treads.transform.rotation, lookRotation, 0.2f);
                _treadRotation = Treads.transform.rotation.eulerAngles.z;
            }
        }

        if(isClientOnly)
        {
            var treadTargetRotation = Quaternion.Euler(
                Treads.transform.rotation.eulerAngles.x,
                Treads.transform.rotation.eulerAngles.y,
                _treadRotation
            );

            Treads.transform.rotation = Quaternion.Lerp(Treads.transform.rotation, treadTargetRotation, 0.2f);
        }
    }
}
