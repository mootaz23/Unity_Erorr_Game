using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
 
public class Movment : NetworkBehaviour
    { 
        [SerializeField]
        private float walkSpeed = 3.5f;
        [SerializeField]
        private float runSpeedOffset = 2.0f;
        [SerializeField]
        private float rotationSpeed = 3.5f;
        [SerializeField]
        private FixedJoystick fixedJoystick;
        private Animator anim;
        private Rigidbody2D rb;
        private float inputX;
        private float inputY;
        [SerializeField]
        private Vector2 defaultInitialPositionOnPlane = new Vector2(-4, 4);
        [SerializeField]
        private NetworkVariable<Vector2> networkPositionDirection = new NetworkVariable<Vector2>();

        [SerializeField]
        private NetworkVariable<Vector2> networkRotationDirection = new NetworkVariable<Vector2>();


        [SerializeField]
        private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();
        // client caches positions
        private Vector2 oldInputPosition = Vector2.zero;
        private Vector2 oldInputRotation = Vector2.zero;
        private PlayerState oldPlayerState = PlayerState.Idle;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponentInChildren<Animator>();
        }
        private void Start() {
            if(IsClient && IsOwner)
            {
                transform.position = new Vector3(Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y), 0,
                   Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y));
            }
        }
        public override void OnNetworkSpawn()
        {
            if(IsClient && IsOwner)
            {
                fixedJoystick = FindObjectOfType<FixedJoystick>();
                CameraControls cam = FindObjectOfType<CameraControls>();
                cam.player = transform;
            }
        }
        private void ClientMoveAndRotate()
        {
            if (networkPositionDirection.Value != Vector2.zero)
            {
                rb.velocity = networkPositionDirection.Value;
                Vector3 characterScale = transform.localScale;
                if(networkRotationDirection.Value.x < 0)
                {
                    if( characterScale.x > 0 )
                    {
                        characterScale.x *= -1;
                    }
                    
                }
                if(networkRotationDirection.Value.x > 0)
                {
                    if(characterScale.x < 0)
                    {
                        characterScale.x *= -1;
                    }
    
                }
                transform.localScale = characterScale;
            }
           
        }

        private void ClientVisuals()
        {
            if (oldPlayerState != networkPlayerState.Value)
            {
                oldPlayerState = networkPlayerState.Value;
                anim.SetTrigger($"{networkPlayerState.Value}");
            }
        }
        private void ClientInput()
        {
            inputX = Input.GetAxis("Horizontal")+ fixedJoystick.Horizontal;
            inputY = Input.GetAxis("Vertical") + fixedJoystick.Vertical;

            // left & right rotation
            Vector2 inputRotation = new Vector3(inputX, inputY, 0);
             // forward & backward direction
            float forwardInput = inputY + inputX;
            Vector2 inputPosition =  new Vector2(inputX, inputY);   
            // change animation states
            if (forwardInput == 0) UpdatePlayerStateServerRpc(PlayerState.Idle);
            else UpdatePlayerStateServerRpc(PlayerState.Walk);
            // else if (ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
            // {
            //     UpdatePlayerStateServerRpc(PlayerState.Run);
            // }

            // let server know about position and rotation client changes
            if (oldInputPosition != inputPosition ||
                oldInputRotation != inputRotation)
            {
                oldInputPosition = inputPosition;
                UpdateClientPositionAndRotationServerRpc(inputPosition * walkSpeed, inputRotation);
            }
        }
        [ServerRpc]
        public void UpdateClientPositionAndRotationServerRpc(Vector2 newPosition, Vector2 newRotation)
        {
            networkPositionDirection.Value = newPosition;
            networkRotationDirection.Value = newRotation;
        }
        [ServerRpc]
        public void UpdatePlayerStateServerRpc(PlayerState state)
        {
            networkPlayerState.Value = state;
        }
        private static bool ActiveRunningActionKey()
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }
        private void Update() {
            if(IsClient && IsOwner && IsSpawned)
            {
                ClientInput();
            }
            ClientMoveAndRotate();
            ClientVisuals();
            
            //transform.Translate(inputX * Speed * Time.deltaTime, inputY* Speed  * Time.deltaTime, 0f);
            
        }
    }