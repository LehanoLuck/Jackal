using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("RTS Camera")]
    public class CameraMovement : MonoBehaviour
    {
        private Transform m_Transform; //camera tranform
        public bool useFixedUpdate = false; //use FixedUpdate() or Update()

        #region Movement

        public float keyboardMovementSpeed = 5f; //speed with keyboard movement
        public float screenEdgeMovementSpeed = 3f; //speed with screen edge movement
        public float followingSpeed = 5f; //speed when following a target
        public float rotationSped = 3f;
        public float panningSpeed = 10f;
        public float mouseRotationSpeed = 10f;

        #endregion

        #region Angles

        public float viewingAngle; // Угол обзора камеры
        public float tiltAngle = 60f; // Угол наклона камеры к плоскости
        public float shortPlaneAngle => tiltAngle + viewingAngle / 2; // Угол между нижней границей обзора камеры и плоскостью
        public float longPlaneAngle => shortPlaneAngle - viewingAngle; // Угол между верхней границей обзора камеры и плоскостью
        #endregion

        #region StartTransform

        public Vector3 startRotation;
        public Vector3 startPosition = new Vector3(0, 0, 0);
        public Vector3 centerMapPoint = new Vector3(0, 0, 0);

        #endregion

        #region Height

        public float maxHeight = 25f; //maximal height
        public float minHeight = 5f; //minimnal height
        public float heightDampening = 5f;
        public float keyboardZoomingSensitivity = 2f;
        public float scrollWheelZoomingSensitivity = 250f;

        private float zoomPos = 0; //value in range (0, 1) used as t in Matf.Lerp

        #endregion


        #region MapLimits

        public bool limitMap = true;
        public float fixedLimitX = 0f;
        public float fixedLimitY = 0f;
        public float shortLimitX = 0f; //x limit of map
        public float longLimitX = 0f; //x limit of map
        public float limitY = 0f; //x limit of map

        #endregion

        #region Targeting

        public Transform targetFollow; //target to follow
        public Vector3 targetOffset;

        /// <summary>
        /// are we following target
        /// </summary>
        public bool FollowingTarget
        {
            get
            {
                return targetFollow != null;
            }
        }

        #endregion

        #region Input

        public bool useScreenEdgeInput = true;
        public float screenEdgeBorder = 50f;
        public float outScreenEdgeBorder = 200f; //Диапазон за пределами экрана, входящая в диапазон 'видения' курсора мыши

        public bool useKeyboardInput = true;
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

        public bool usePanning = true;
        public KeyCode panningKey = KeyCode.Mouse2;

        public bool useKeyboardZooming = true;
        public KeyCode zoomInKey = KeyCode.E;
        public KeyCode zoomOutKey = KeyCode.Q;

        public bool useScrollwheelZooming = true;
        public string zoomingAxis = "Mouse ScrollWheel";

        public bool useKeyboardRotation = true;
        public KeyCode rotateRightKey = KeyCode.X;
        public KeyCode rotateLeftKey = KeyCode.Z;

        public bool useMouseRotation = true;
        public KeyCode mouseRotationKey = KeyCode.Mouse1;

        private Vector2 KeyboardInput
        {
            get { return useKeyboardInput ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis)) : Vector2.zero; }
        }

        private Vector2 MouseInput
        {
            get { return Input.mousePosition; }
        }

        private float ScrollWheel
        {
            get { return Input.GetAxis(zoomingAxis); }
        }

        private Vector2 MouseAxis
        {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

        private int ZoomDirection
        {
            get
            {
                bool zoomIn = Input.GetKey(zoomInKey);
                bool zoomOut = Input.GetKey(zoomOutKey);
                if (zoomIn && zoomOut)
                    return 0;
                else if (!zoomIn && zoomOut)
                    return 1;
                else if (zoomIn && !zoomOut)
                    return -1;
                else
                    return 0;
            }
        }

        private int RotationDirection
        {
            get
            {
                bool rotateRight = Input.GetKey(rotateRightKey);
                bool rotateLeft = Input.GetKey(rotateLeftKey);
                if (rotateLeft && rotateRight)
                    return 0;
                else if (rotateLeft && !rotateRight)
                    return -1;
                else if (!rotateLeft && rotateRight)
                    return 1;
                else
                    return 0;
            }
        }

        #endregion

        #region Unity_Methods

        private void Start()
        {
            m_Transform = transform;
            startRotation = new Vector3(tiltAngle, 0, 0);
            transform.rotation = Quaternion.Euler(startRotation);
            viewingAngle = gameObject.GetComponent<Camera>().fieldOfView;

            shortLimitX = fixedLimitX;
            longLimitX = fixedLimitX;
            limitY = fixedLimitY;
        }

        private void Update()
        {
            if (!useFixedUpdate)
                CameraUpdate();
        }

        private void FixedUpdate()
        {
            if (useFixedUpdate)
                CameraUpdate();
        }

        #endregion

        #region RTSCamera_Methods

        /// <summary>
        /// update camera movement and rotation
        /// </summary>
        private void CameraUpdate()
        {
            if (FollowingTarget)
                FollowTarget();
            else
                TryMove();

            Rotation();
            LimitPosition();
            HeightCalculation();
        }

        private void TryMove()
        {
            if(!Input.GetKey(mouseRotationKey) && !Input.GetKey(KeyCode.Mouse0))
            {
                Move();
            }
        }

        /// <summary>
        /// move camera with keyboard or with screen edge
        /// </summary>
        private void Move()
        {
            if (useKeyboardInput)
            {
                Vector3 desiredMove = new Vector3(KeyboardInput.x, 0, KeyboardInput.y);

                desiredMove *= keyboardMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }

            if (useScreenEdgeInput)
            {
                Vector3 desiredMove = new Vector3();

                //TODO Осторожно, говнокод, 200f - взято просто так 
                Rect leftRect = new Rect(-200f, 0, screenEdgeBorder + 200f, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder + 200f, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder + 200f);
                Rect downRect = new Rect(0, -200f, Screen.width, screenEdgeBorder + 200f);

                desiredMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.z = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

                desiredMove *= screenEdgeMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }

            if (usePanning && Input.GetKey(panningKey) && MouseAxis != Vector2.zero)
            {
                Vector3 desiredMove = new Vector3(-MouseAxis.x, 0, -MouseAxis.y);

                desiredMove *= panningSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }
        }

        /// <summary>
        /// calcualte height
        /// </summary>
        private void HeightCalculation()
        {
            if (useScrollwheelZooming)
                zoomPos += ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
            if (useKeyboardZooming)
                zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;

            zoomPos = Mathf.Clamp01(zoomPos);

            float targetHeight = Mathf.Lerp(maxHeight, minHeight, zoomPos);

            var longPlaneNormalAngle = Mathf.Deg2Rad * (90f - (tiltAngle - viewingAngle / 2)); // Угол между верхней границей камеры и перпендикуляром опущенным к плоскости

            float longLimitDistance = ((maxHeight - targetHeight) * Mathf.Tan(longPlaneNormalAngle));
            float shortLimitDistance = (maxHeight - targetHeight) / Mathf.Tan(Mathf.Deg2Rad * shortPlaneAngle);

            shortLimitX = fixedLimitX + shortLimitDistance;
            longLimitX = fixedLimitX + longLimitDistance;
            limitY = fixedLimitY + longLimitDistance;

            
            m_Transform.position = Vector3.Lerp(m_Transform.position,
                new Vector3(m_Transform.position.x, targetHeight, m_Transform.position.z), Time.deltaTime * heightDampening);
        }

        /// <summary>
        /// rotate camera
        /// </summary>
        private void Rotation()
        {
            if (useKeyboardRotation)
                transform.RotateAround(centerMapPoint, Vector3.up, RotationDirection * Time.deltaTime * rotationSped);

            if (useMouseRotation && Input.GetKey(mouseRotationKey))
                m_Transform.RotateAround(centerMapPoint, Vector3.up, -MouseAxis.x * Time.deltaTime * mouseRotationSpeed);
        }

        /// <summary>
        /// follow targetif target != null
        /// </summary>
        private void FollowTarget()
        {
            Vector3 targetPos = new Vector3(targetFollow.position.x, m_Transform.position.y, targetFollow.position.z) + targetOffset;
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, Time.deltaTime * followingSpeed);
        }

        /// <summary>
        /// limit camera position
        /// </summary>
        private void LimitPosition()
        {
            if (!limitMap)
                return;

            m_Transform.position = new Vector3(Mathf.Clamp(m_Transform.position.x, startPosition.x - limitY, startPosition.x + limitY),
                m_Transform.position.y,
                Mathf.Clamp(m_Transform.position.z, startPosition.z+ shortLimitX, startPosition.z + longLimitX));
        }

        /// <summary>
        /// set the target
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            targetFollow = target;
        }

        /// <summary>
        /// reset the target (target is set to null)
        /// </summary>
        public void ResetTarget()
        {
            targetFollow = null;
        }

        #endregion

        public void SetPositionToStart()
        {
            m_Transform.position = startPosition;
        }
    }
}