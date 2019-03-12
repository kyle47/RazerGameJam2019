using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class TankComponent : NetworkBehaviour
{
    public float RotationSpeed = 15.0f;


    public float TreadInputVertical;
    public float TreadInputHorizontal;

    public float GunInputHorizontal;

    public Rigidbody2D Rigidbody2D;

    public GameObject Treads;
    public SpriteRenderer[] TreadGraphics;
    public GameObject Gun;
    public SpriteRenderer[] GunGraphics;

    public PlayerComponent Driver;
    public PlayerComponent Gunner;

    public float Speed = 1.0f;

    [SyncVar]
    protected float _treadRotation;

    [SyncVar]
    protected float _gunRotation;

    private void FixedUpdate()
    {
        if (isServer)
        {
            var direction = new Vector2(TreadInputHorizontal, TreadInputVertical);

            if(direction.magnitude > 1.0f)
            {
                direction = direction.normalized;
            }

            Rigidbody2D.velocity = direction * Speed;

            if(TreadInputHorizontal != 0.0f && TreadInputVertical != 0.0f)
            {
                var lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
                Treads.transform.rotation = Quaternion.Lerp(Treads.transform.rotation, lookRotation, 0.2f);
                _treadRotation = Treads.transform.rotation.eulerAngles.z;
            }

            var gunInput = Mathf.Clamp(GunInputHorizontal, -1.0f, 1.0f);
            var gunRotation = Quaternion.Euler(0.0f, 0.0f, RotationSpeed * gunInput);
            Gun.transform.rotation = Quaternion.Lerp(Gun.transform.rotation, Gun.transform.rotation * gunRotation, .2f);

            _gunRotation = Gun.transform.rotation.eulerAngles.z;
        }

        if(isClientOnly)
        {
            var treadTargetRotation = Quaternion.Euler(
                Treads.transform.rotation.eulerAngles.x,
                Treads.transform.rotation.eulerAngles.y,
                _treadRotation
            );

            Treads.transform.rotation = Quaternion.Lerp(Treads.transform.rotation, treadTargetRotation, 0.2f);

            var gunTargetRotation = Quaternion.Euler(
                Gun.transform.rotation.eulerAngles.x,
                Gun.transform.rotation.eulerAngles.y,
                _gunRotation
            );

            Gun.transform.rotation = Quaternion.Lerp(Gun.transform.rotation, gunTargetRotation, 0.2f);
        }
    }

    [ClientRpc]
    public void Rpc_SetGunColor(Color color)
    {
        GunGraphics.ToList().ForEach(sprite => sprite.color = color);
    }

    [ClientRpc]
    public void Rpc_SetTreadColor(Color color)
    {
        TreadGraphics.ToList().ForEach(sprite => sprite.color = color);
    }
}
