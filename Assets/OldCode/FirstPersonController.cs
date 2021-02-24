using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityEngine.Networking;
using Assets;
using System.Linq;
using System.Collections;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : NetworkBehaviour
    {
        [SerializeField] private bool IsWalking;
        [SerializeField] private float WalkSpeed;
        [SerializeField] private float RunSpeed;
        [SerializeField] private float JumpSpeed;
        [SerializeField] private float StickToGroundForce;
        [SerializeField] private float GravityMultiplier;
        [SerializeField] private MouseLook MouseLook;
        [SerializeField] private bool UseFovKick;
        [SerializeField] private FOVKick FovKick = new FOVKick();
        [SerializeField] private AudioClip[] FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip LandSound;           // the sound played when character touches back on ground.
        [SyncVar] public Vector3 LastFixedJoinPosition;
        public bool Driving = false;
        public GameObject BulletPrefab;
        public GameObject BulletSpawn;
        public bool Shooting = false;
        public ShootingPosition shootingPos = null;
        private Camera m_Camera;
        private Camera characterCamera;
        private bool Jump;
        private float YRotation;
        private Vector2 KeyInput;
        private Vector3 MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        private Rigidbody rb;
        [SyncVar] public GameObject Parent;
        [SyncVar] public GameObject HomeShip;
        public Gun CurrentGun;
        public Health health;
        public Vector3 velocity;
        public float yVal;
        public float xVal;
        bool isGrounded = true;

        // Use this for initialization

        public override void OnStartAuthority()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_Camera.enabled = false;
            characterCamera.enabled = true;
            m_OriginalCameraPosition = characterCamera.transform.localPosition;
            FovKick.Setup(characterCamera);
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            MouseLook.Init(transform, characterCamera.transform);
        }
        public override void OnStartClient()
        {
            rb = GetComponent<Rigidbody>();

            characterCamera = GetComponentInChildren<Camera>();
            characterCamera.enabled = false;
        }
        public void Start()
        {
            CurrentGun = transform.Find("FirstPersonCamera").transform.Find("Weapons").transform.GetChild(0).GetComponent<Gun>();
            health = transform.GetComponent<Health>();
        }
        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }
        // Update is called once per frame
        public void Died()
        {
            //respawn at ur ship
            health.currentHealth = 100;
            transform.position = HomeShip.transform.position + new Vector3(0, 2, 0);
            Parent = HomeShip;
        }
        private void Update()
        {
            if (!hasAuthority)
            {
                return;
            }
            if (health.currentHealth <= 0)
                Died();
            if (transform.position.y < -10)
                Died();
            if (Driving)
            {
                if (Input.GetKey(KeyCode.F))
                {
                    Driving = false;
                }
                if (Parent != null)
                {
                    if (Input.GetKey(KeyCode.D))
                    {
                        CmdTurn(1);
                    }
                    if (Input.GetKey(KeyCode.A))
                    {
                        CmdTurn(-1);
                    }
                    Debug.Log(Parent.GetComponent<Boat>().WheelDirection);
                }
                Vector3 BehindTheWheel = new Vector3(0, 3.348f, -3.373f);
                Vector3 rot = RotatePointAroundPivot(BehindTheWheel, Vector3.zero, Parent.transform.rotation.eulerAngles);
                Vector3 newPos = new Vector3(Parent.transform.position.x + rot.x, Parent.transform.position.y + rot.y, Parent.transform.position.z + rot.z);
                transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
            }
            else if (Shooting)
            {
                if (Input.GetKey(KeyCode.F))
                {
                    Shooting = false;
                    shootingPos = null;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    if (Parent.GetComponent<Boat>().ShootingPositionsList.First(p => p.Name == shootingPos.Name).yRotation > -30)
                        CmdMoveCannonY(-1f, shootingPos.Name);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    if (Parent.GetComponent<Boat>().ShootingPositionsList.First(p => p.Name == shootingPos.Name).yRotation < 30)
                        CmdMoveCannonY(1f, shootingPos.Name);
                }
                if (Input.GetKey(KeyCode.W))
                {
                    if (Parent.GetComponent<Boat>().ShootingPositionsList.First(p => p.Name == shootingPos.Name).zRotation < 30)
                        CmdMoveCannonZ(1f, shootingPos.Name);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    if (Parent.GetComponent<Boat>().ShootingPositionsList.First(p => p.Name == shootingPos.Name).zRotation > -30)
                        CmdMoveCannonZ(-1f, shootingPos.Name);
                }
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    //CmdFireCannon(shootingPos.Name);
                    Debug.Log("Pow!");
                }
                Vector3 rot = RotatePointAroundPivot(shootingPos.StandingPosition, Vector3.zero, Parent.transform.rotation.eulerAngles);
                Vector3 newPos = new Vector3(Parent.transform.position.x + rot.x, Parent.transform.position.y + rot.y, Parent.transform.position.z + rot.z);
                transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
            }
            else if (Parent != null)
            {
                Vector3 rot = RotatePointAroundPivot(transform.position, Parent.transform.position, Parent.GetComponent<Boat>().LastFrameQuaternionChange) - transform.position;
                Vector3 newPos = transform.position + Parent.GetComponent<Boat>().LastFrameTransformChange + rot;
                rb.position = new Vector3(newPos.x, transform.position.y, newPos.z);
                transform.position = rb.position;
            }

            RotateView();
            if (!Driving && !Shooting)
            {
                Vector3 desiredMove = transform.forward * yVal + transform.right * xVal;
                xVal = KeyInput.x;
                yVal = KeyInput.y;
                RaycastHit hitInfo;
                Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                                   m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                float desX = desiredMove.x * 0.2f;
                float desZ = desiredMove.z * 0.2f;
                float desY = desiredMove.y * 0.2f;
                desiredMove = desiredMove * 0.2f;
                desiredMove = new Vector3(desX, desY, desZ);
                if (desX != 0 || desZ != 0)
                    transform.position += desiredMove;
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (Time.time >= CurrentGun.nextTimeToFire)
                        Fire();
                }
                if (Input.GetKey(KeyCode.R))
                {
                    Reload();
                }
            }
        }
        void OnCollisionEnter(Collision theCollision)
        {
            if (theCollision.gameObject.tag.ToLower() == "boat")
            {
                isGrounded = true;
            }
        }
        void OnCollisionExit(Collision theCollision)
        {
            if (theCollision.gameObject.tag.ToLower() == "boat")
            {
                isGrounded = false;
            }
        }
        private void FixedUpdate()
        {
            if (!hasAuthority)
                return;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space) && isGrounded && !Driving)
            {
                rb.AddForce(0, 3000, 0);
            }
            float speed;
            GetInput(out speed);
            UpdateCameraPosition(speed);

            MouseLook.UpdateCursorLock();

        }
        public void Fire()
        {
            if (CurrentGun.currentAmmo > 0)
            {
                CmdFire();
            }
        }
        public void Reload()
        {
            if (!CurrentGun.reloading && CurrentGun.currentAmmo < CurrentGun.maxAmmo)
                CmdReload();
        }
        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
        }
        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running

#endif
            // set the desired speed to be walking or running
            speed = IsWalking ? WalkSpeed : RunSpeed;
            KeyInput = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (KeyInput.sqrMagnitude > 1)
            {
                KeyInput.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (IsWalking != waswalking && UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!IsWalking ? FovKick.FOVKickUp() : FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            MouseLook.LookRotation(transform, characterCamera.transform);
        }

        IEnumerator ReloadCoroutine()
        {
            CurrentGun.animator.SetBool("reloading", true);
            CurrentGun.reloading = true;
            yield return new WaitForSeconds(CurrentGun.reloadSeconds - .25f);
            CurrentGun.animator.SetBool("reloading", false);
            yield return new WaitForSeconds(.25f);
            CurrentGun.currentAmmo = CurrentGun.maxAmmo;
            CurrentGun.reloading = false;
        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
        [Command]
        public void CmdTurn(double value)
        {
            Parent.GetComponent<Boat>().WheelDirection += value;
        }


        [ClientRpc]
        void RpcNewLastFixedPos(Vector3 newPos)
        {
            LastFixedJoinPosition = newPos;
        }
       /* [Command]
        public void CmdFireCannon(string name)
        {
            Debug.Log("fire!");
            Boat bp = Parent.GetComponent<Boat>();
            ShootingPosition sp = bp.ShootingPositionsList.First(p => p.Name == name);
            foreach (GameObject cannon in sp.CannonBallSpawnPoints)
            {
                GameObject bullet = Instantiate(bp.CannonBallPrefab, cannon.transform.position, cannon.transform.rotation);
                bullet.GetComponent<CannonBall>().ProjectionForce = cannon.transform.forward * 50;
                NetworkServer.Spawn(bullet);
                Destroy(bullet, 10);
            }
        }*/
        [Command]
        public void CmdMoveCannonY(float moveValue, string name)
        {
            Boat sp = Parent.GetComponent<Boat>();
            ShootingPosition pos = sp.ShootingPositionsList.First(p => p.Name == name);

            pos.yRotation += moveValue;
            if (pos.yRotation > 30)
                pos.yRotation = 30;
            if (pos.yRotation < -30)
                pos.yRotation = -30;
            RpcAdjustZY(pos.zRotation, pos.yRotation, pos.Name);
        }
        [Command]
        public void CmdMoveCannonZ(float moveValue, string name)
        {
            Boat sp = Parent.GetComponent<Boat>();
            ShootingPosition pos = sp.ShootingPositionsList.First(p => p.Name == name);
            pos.zRotation += moveValue;
            if (pos.zRotation > 30)
                pos.zRotation = 30;
            if (pos.zRotation < -30)
                pos.zRotation = -30;
            RpcAdjustZY(pos.zRotation, pos.yRotation, pos.Name);
        }
        [ClientRpc]
        public void RpcAdjustZY(float z, float y, string name)
        {
            Boat sp = Parent.GetComponent<Boat>();
            ShootingPosition pos = sp.ShootingPositionsList.First(p => p.Name == name);
            pos.zRotation = z;
            pos.yRotation = y;
        }
        [Command]
        public void CmdFire()
        {
            if (CurrentGun.currentAmmo > 0 && !CurrentGun.reloading)
            {
                CurrentGun.nextTimeToFire = Time.time + 1f / CurrentGun.fireRate;
                GameObject bullet = Instantiate(BulletPrefab, CurrentGun.Muzzle.transform.position, CurrentGun.Muzzle.transform.rotation);
                NetworkServer.Spawn(bullet);
                Destroy(bullet, 10);
                CurrentGun.currentAmmo--;
            }
        }
        [Command]
        public void CmdReload()
        {
            StartCoroutine(ReloadCoroutine());
        }
    }
}
