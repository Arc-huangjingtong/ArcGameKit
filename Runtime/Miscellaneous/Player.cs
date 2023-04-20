using UnityEngine;
using UnityEngine.InputSystem;

namespace HaiPackage
{
    /// <summary>
    /// 此类基于 CharacterController组件让角色的正常移动以及摆动视角
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public  class Player : MonoBehaviour
    {
        private CharacterController _controller
        {
            get
            {
                CharacterController obj;
                if (controller != null)
                    obj = controller;
                else
                    obj = controller = gameObject.GetComponent<CharacterController>();
                return obj;
            }
        }
        /// <summary>
        /// 角色控制器 此脚本需要基于此组件去使用
        /// </summary>
        private CharacterController controller;
        /// <summary>
        /// 视角、用于旋转摄像头视野
        /// </summary>
        [Tooltip("物体旋转X方向，若无,会使用主相机Transform")]
        public Transform viewX;
        private Transform _viewX
        {
            get
            {
                if (viewX == null)
                {
                    viewX = Camera.main.transform;
                }

                return viewX;
            }
        }
        private Transform view_x
        {
            get
            {
                Transform t;
                if (_viewX == null)
                {
                    var main = Camera.main;
                    t = main != null ? main.transform : this.transform;
                }
                else
                {
                    t = _viewX;
                }

                return t;
            }
        }
        [Tooltip("物体旋转Y方向，同时也控制着移动方向，若无则会使用当前脚本下Transform")]
        public Transform viewY;
        private Transform view_y => viewY == null ? transform : viewY;
        /// <summary>
        /// 主要移动物体
        /// </summary>
        /// <summary>
        /// 新输入系统，若无，会使用旧版输入系统
        /// </summary>
        public InputActionAsset inputActionAsset;
        #region 行为
        /// <summary>
        /// 移动
        /// </summary>
        protected virtual void Move()
        {
            //移动
            var inputMove = InputMove();
            _velocity = Vector3.SmoothDamp(_velocity,
                transform.TransformDirection(new Vector3(inputMove.x, 0, inputMove.y).normalized *
                                             (InputRush() ? runSpeed : walkSpeed)), ref _smoothV, .1f);
        }

        /// <summary>
        /// 重力
        /// </summary>
        protected virtual void Gravity()
        {
            //下落
            _verticalVelocity -= gravity * Time.deltaTime;
            _velocity = new Vector3(_velocity.x, _verticalVelocity, _velocity.z);
            var flags = _controller.Move(_velocity * Time.deltaTime);
            if (flags == CollisionFlags.Below)
            {
                _jumping = false;
                _lastGroundedTime = Time.time;
                _verticalVelocity = 0;
            }
        }

        /// <summary>
        /// 跳跃
        /// </summary>
        protected virtual void Jump()
        {
            //跳跃
            if (InputJump())
            {
                var timeSinceLastTouchedGround = Time.time - _lastGroundedTime;
                if (_controller.isGrounded || (!_jumping && timeSinceLastTouchedGround < 0.15f)) //判断是否在连续按空格
                {
                    _jumping = true;
                    _verticalVelocity = jumpForce;
                }
            }
        }

        /// <summary>
        /// 旋转相机和角色
        /// </summary>
        protected virtual void RotateCameraAndCharacter()
        {
            var rotationX = _rotationX.Update(rotationRaw.x, 0.05f);
            var rotationY = _rotationY.Update(rotationRaw.y, 0.05f);
            var clampedY = RestrictVerticalRotation(rotationY);
            _rotationY.current = clampedY;
            var worldUp = view_x.InverseTransformDirection(Vector3.up);
            var rotation = view_x.rotation *
                           Quaternion.AngleAxis(rotationX, worldUp) *
                           Quaternion.AngleAxis(clampedY, Vector3.left);
            view_y.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
            view_x.rotation = rotation;
        }
        #endregion

        /// <summary>
        /// 简单的行为
        /// </summary>
        protected virtual void Behavior()
        {
            Move();
            Gravity();
            Jump();
            RotateCameraAndCharacter();
        }

        /// <summary>
        /// 相机围绕Y轴旋转
        /// </summary>
        /// <param name="mouseY"></param>
        /// <returns></returns>
        protected virtual float RestrictVerticalRotation(float mouseY)
        {
            var currentAngle = NormalizeAngle(view_x.eulerAngles.x);
            var minY = -90f + currentAngle;
            var maxY = 90f + currentAngle;
            return Mathf.Clamp(mouseY, minY + 0.01f, maxY - 0.01f);
        }

        /// <summary>
        /// 限制角度在 -180° ~ 180° 之间
        /// </summary>
        /// <param name="angleDegrees"></param>
        /// <returns></returns>
        public static float NormalizeAngle(float angleDegrees)
        {
            while (angleDegrees > 180f)
            {
                angleDegrees -= 360f;
            }

            while (angleDegrees <= -180f)
            {
                angleDegrees += 360f;
            }

            return angleDegrees;
        }

        public virtual void Init()
        {
            _viewX.rotation = Quaternion.Euler(Vector3.zero);
            _rotationX = new SmoothRotation(rotationRaw.x);
            _rotationY = new SmoothRotation(rotationRaw.y);
        }

        #region 参数
        //视角
        [Tooltip("视角移动速度")] public float dpi = .5f;
        private Vector2 rotationRaw => InputLook() * dpi;
        private SmoothRotation _rotationX, _rotationY;
        // 重力
        [Tooltip("重力")] public float gravity = 18;
        [Tooltip("跳跃力")] public float jumpForce = 8;

        //行动速度
        [Tooltip("行走速度")] public float walkSpeed = 3f;
        [Tooltip("跑步速度")] public float runSpeed = 10f;
        [Tooltip("冲刺倍率")] public float rushBuff = 1f;

        /// <summary>
        /// 最后落地时间
        /// </summary>
        private float _lastGroundedTime;
        private bool _jumping;
        private float _verticalVelocity;
        private Vector3 _velocity;
        private Vector3 _smoothV;
        #endregion
        #region 输入源
        /// <summary>
        /// 移动
        /// 获取输入源 , 优先使用 New InputSystem 
        /// </summary>
        /// <returns></returns>
        private Vector2 InputMove()
        {
            Vector2 move;
            if (inputActionAsset != null)
            {
                move = inputActionAsset.FindActionMap("Player")["Move"].ReadValue<Vector2>();
            }
            else
            {
                var x = Input.GetAxisRaw("Horizontal");
                var y = Input.GetAxisRaw("Vertical");
                move = new Vector2(x, y);
                if (x == 0 || y == 0) return move;
                if (x > 0)
                    x -= .3f;
                else
                    x += .3f;
                if (y > 0)
                    y -= .3f;
                else
                    y += .3f;
                move = new Vector2(x, y);
            }

            return move;
        }

        /// <summary>
        /// 奔跑
        /// 获取输入源 , 优先使用 New InputSystem 
        /// </summary>
        /// <returns></returns>
        private bool InputRush()
        {
            return inputActionAsset != null
                ? inputActionAsset.FindActionMap("Player")["Rush"].ReadValue<float>() > 0
                : Input.GetKey(KeyCode.LeftShift);
        }

        /// <summary>
        /// 鼠标移动、视野
        /// 获取输入源 , 优先使用 New InputSystem 
        /// </summary>
        /// <returns></returns>
        private Vector2 InputLook()
        {
            //旧输入系统和新输入系统偏差大概是20倍
            return inputActionAsset != null
                ? inputActionAsset.FindActionMap("Player")["Look"].ReadValue<Vector2>()
                : new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * 20;
        }

        /// <summary>
        /// 跳跃
        /// 获取输入源 , 优先使用 New InputSystem 
        /// </summary>
        /// <returns></returns>
        private bool InputJump()
        {
            return inputActionAsset != null
                ? inputActionAsset.FindActionMap("Player")["Jump"].ReadValue<float>() > 0
                : Input.GetKeyDown(KeyCode.Space);
        }
        #endregion
        #region Unity
        /// <summary>
        /// 隐藏鼠标
        /// </summary>
        public void SetMouse()
        {
            //设置鼠标
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public virtual void Awake()
        {
            Init();
        }

        public virtual void Update()
        {
            Behavior();
        }
        #endregion
    }

    /// <summary>
    /// 辅助角色平滑移动
    /// </summary>
    public class SmoothVelocity
    {
        /// <summary>
        /// 当前位置
        /// </summary>
        private float _current;
        private float _speed;

        /// Returns the smoothed velocity.
        public float Update(float target, float smoothTime)
        {
            return _current = Mathf.SmoothDamp(_current, target, ref _speed, smoothTime);
        }

        public float current
        {
            set => _current = value;
        }
    }
    /// <summary>
    /// 辅助相机平滑旋转
    /// </summary>
    public class SmoothRotation
    {
        private float _current;
        private float _speed;

        /// <summary>
        /// 当前位置
        /// </summary>
        public SmoothRotation(float startAngle)
        {
            _current = startAngle;
        }

        public float Update(float target, float smoothTime)
        {
            return _current = Mathf.SmoothDampAngle(_current, target, ref _speed, smoothTime);
        }

        public float current
        {
            set => _current = value;
        }
    }
}