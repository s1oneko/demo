using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Movement 
{
    [RequireComponent(typeof(CharacterController))]
    public abstract class CharacterMovementControlBase : MonoBehaviour
    {
        protected CharacterController _characterController;
        protected Animator _animator;

        //ground detection
        protected bool _characterIsOnGround;
        [SerializeField,Header("Ground Detection")] protected float _groundDetectionPositionOffset;
        [SerializeField] protected float _detectionRange;
        [SerializeField] protected LayerMask _whatIsGround;

        //ground detection
        private bool GroundDetection()
        {
            var detectionPosition = new Vector3(transform.position.x,transform.
                position.y-_groundDetectionPositionOffset,transform.position.z);
            return Physics.CheckSphere(detectionPosition, _detectionRange,_whatIsGround,
                QueryTriggerInteraction.Ignore);
        }

        //gravity
        protected readonly float CharacterGravity = -9.8f;
        protected float _characterVerticalVelocity;//update the velocity in y-axis,for gravity and jump thrust
        protected float _fallOutDeltaTime;
        protected float _fallOutTime = 0.15f;//extra time for process when going down stair
        protected readonly float _characterVerticalMaxVelocity = 54f;//only apply gravity when below max velocity
        protected Vector3 _charcterVerticalDirection;//by applying _characterVerticalVelocity for y-axis to update to implement the gravity

        //gravity
        private void SetCharacterGravity()
        {
            _characterIsOnGround = GroundDetection();
            if (_characterIsOnGround)
            {
                /*
                 reset fallOutTime to prevent falling out
                 reset verticalVelocity
                */
                _fallOutDeltaTime = _fallOutTime;
                if (_characterVerticalVelocity < 0)
                {
                    _characterVerticalVelocity = -2f;//restrict verticalVelocity
                }
            }
            else
            {
                if(_fallOutDeltaTime > 0)
                {
                    //aerial time is tiny, just wait _fallOutTime
                    _fallOutDeltaTime -= Time.deltaTime;
                }
                else
                {
                    //is falling out
                }
                if (_characterVerticalVelocity < _characterVerticalMaxVelocity)
                {
                    _characterVerticalVelocity += CharacterGravity * Time.deltaTime;
                }
            }
        }
        private void UpdateCharacterGravity()
        {
            _charcterVerticalDirection.Set(0, _characterVerticalVelocity, 0);
            _characterController.Move(_charcterVerticalDirection * Time.deltaTime);
        }

        //slop detetcion
        private Vector3 SlopResetDetection(Vector3 moveDirection)
        {
            //prevent problem when downslope 
            if(Physics.Raycast(transform.position+transform.up*0.5f,Vector3.down,out var hit,
                _characterController.height*0.85f,_whatIsGround,QueryTriggerInteraction.Ignore))
            {
                if(Vector3.Dot(Vector3.up, hit.normal)!=0) 
                {
                    return Vector3.ProjectOnPlane(transform.position, hit.normal);
                }
            }
            return moveDirection;
        }

        protected virtual void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
        }
        protected virtual void Start()
        {
            _fallOutDeltaTime = _fallOutTime;
        }
        private void Update()
        {
            SetCharacterGravity(); 
            UpdateCharacterGravity();
        }

        private void OnDrawGizmos()
        {
            var detectionPosition = new Vector3(transform.position.x, transform.
                position.y - _groundDetectionPositionOffset, transform.position.z);
            Gizmos.DrawWireSphere(detectionPosition, _detectionRange);
        }
    }
}
