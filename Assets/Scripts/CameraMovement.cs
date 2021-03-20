using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        public float rotationSped = 3f;
        public float panningSpeed = 10f;
        public float mouseRotationSpeed = 10f;

        #endregion

        #region Angles

        public float viewingAngle; //camera angle
        public float tiltAngle = 60f; //tilt angle of the camera to the plane
        public float shortPlaneAngle => tiltAngle + viewingAngle / 2; //angle between the lower boundary of the camera's view and the plane
        public float longPlaneAngle => shortPlaneAngle - viewingAngle; //angle between the upper boundary of the camera's view and the plane
        #endregion

        #region StartTransform

        public Vector3 startPosition = new Vector3(0, 0, 0);
        public Vector3 centerMapPoint;
        public Vector3 focusPoint = new Vector3(0, 0, 0);

        #endregion

        #region Height

        public float maxHeight = 25f;
        public float minHeight = 5f;
        public float heightDampening = 5f;
        public float keyboardZoomingSensitivity = 2f;
        public float scrollWheelZoomingSensitivity = 250f;

        private float zoomPos = 0; //value in range (0, 1) used as t in Matf.Lerp

        #endregion


        #region MapLimits

        public bool limitMap = true;
        public float mapSize = 20f;
        public float limitValue;
        public float limitShift;

        #endregion

        #region Input

        public bool useScreenEdgeInput = false;
        public float screenEdgeBorder = 50f;
        public float outScreenEdgeBorder = 200f; //Диапазон за пределами экрана, входящая в диапазон 'видения' курсора мыши

        public bool useKeyboardInput = true;
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

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

        private IDictionary savePoints;

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
            m_Transform.rotation = Quaternion.Euler(new Vector3(tiltAngle, 0, 0));
            viewingAngle = gameObject.GetComponent<Camera>().fieldOfView;
            DefineCenterMapPoint();
            SetStartPoints();
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
            TryMove();

            Rotation();
            LimitPosition();
            HeightCalculation();
            UseSavePoints();
        }

        /// <summary>
        /// move camera if the key isn't pressed to rotate the camera
        /// </summary>
        private void TryMove()
        {
            if(!Input.GetKey(KeyCode.Mouse0))
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
            if(useScreenEdgeInput && KeyboardInput.x==0 && KeyboardInput.y==0)
            {
                Vector3 desiredMove = new Vector3();

                Rect leftRect = new Rect(-outScreenEdgeBorder, 0, screenEdgeBorder + outScreenEdgeBorder, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder + outScreenEdgeBorder, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder + outScreenEdgeBorder);
                Rect downRect = new Rect(0, -outScreenEdgeBorder, Screen.width, screenEdgeBorder + outScreenEdgeBorder);

                desiredMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.z = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

                desiredMove *= screenEdgeMovementSpeed;
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
            limitShift = (maxHeight - targetHeight) / Mathf.Tan(Mathf.Deg2Rad * shortPlaneAngle); //Передняя граница камеры
            limitValue = (mapSize - targetHeight / maxHeight * mapSize) - limitShift; //Величина границы передвижения камеры

            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));
            focusPoint = ray.GetPoint(Mathf.Sqrt(Mathf.Pow(targetHeight, 2f) + Mathf.Pow(mapSize - limitValue, 2)));

            m_Transform.position = Vector3.Lerp(m_Transform.position,
                new Vector3(m_Transform.position.x, targetHeight, m_Transform.position.z) + transform.forward * limitShift, Time.deltaTime * heightDampening);
        }

        /// <summary>
        /// rotate camera
        /// </summary>
        private void Rotation()
        {
            if (useKeyboardRotation)
                m_Transform.RotateAround(focusPoint, Vector3.up, RotationDirection * Time.deltaTime * rotationSped);

            if (useMouseRotation && Input.GetKey(mouseRotationKey))
                m_Transform.RotateAround(focusPoint, Vector3.up, -MouseAxis.x * Time.deltaTime * mouseRotationSpeed);
        }

        /// <summary>
        /// limit camera position
        /// </summary>
        private void LimitPosition()
        {
            if (!limitMap)
                return;

            var yAngle = Mathf.Deg2Rad * (transform.rotation.eulerAngles.y);
            var xPos = centerMapPoint.x + (transform.position.x - centerMapPoint.x) * Mathf.Cos(yAngle) - (transform.position.z - centerMapPoint.z) * Mathf.Sin(yAngle);
            var zPos = centerMapPoint.z + (transform.position.x - centerMapPoint.x) * Mathf.Sin(yAngle) + (transform.position.z - centerMapPoint.z) * Mathf.Cos(yAngle);

            var tempPoint = new Vector3(Mathf.Clamp(xPos, startPosition.x-limitValue, startPosition.x + limitValue),
                m_Transform.position.y,
                Mathf.Clamp(zPos, startPosition.z, startPosition.z + limitValue));

            var _xPos = centerMapPoint.x + (tempPoint.x - centerMapPoint.x) * Mathf.Cos(-yAngle) - (tempPoint.z - centerMapPoint.z) * Mathf.Sin(-yAngle);
            var _zPos = centerMapPoint.z + (tempPoint.x - centerMapPoint.x) * Mathf.Sin(-yAngle) + (tempPoint.z - centerMapPoint.z) * Mathf.Cos(-yAngle);

            m_Transform.position = new Vector3(_xPos, transform.position.y, _zPos);
        }

        /// <summary>
        /// Define center map point
        /// </summary>
        private void DefineCenterMapPoint()
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));
            centerMapPoint = ray.GetPoint(Mathf.Sqrt(Mathf.Pow(maxHeight, 2f) + Mathf.Pow(mapSize / 2, 2)));
        }
        #endregion

        /// <summary>
        /// Set start camera position
        /// </summary>
        public void SetPositionToStart()
        {
            m_Transform.position = startPosition;
        }

        private void UseSavePoints()
        {
            AddSavePoint();
            LoadSavePoint();
        }

        private void SetStartPoints()
        {
            savePoints = new Dictionary<KeyCode, (float, Vector3, Quaternion)>();
            savePoints.Add(KeyCode.Alpha0, (zoomPos,transform.position,transform.rotation));
            savePoints.Add(KeyCode.Alpha1, (zoomPos, transform.position, transform.rotation));
            savePoints.Add(KeyCode.Alpha2, (zoomPos, transform.position, transform.rotation));
            savePoints.Add(KeyCode.Alpha3, (zoomPos, transform.position, transform.rotation));
            savePoints.Add(KeyCode.Alpha4, (zoomPos, transform.position, transform.rotation));
            savePoints.Add(KeyCode.Alpha5, (zoomPos, transform.position, transform.rotation));
            savePoints.Add(KeyCode.Alpha6, (zoomPos, transform.position, transform.rotation));
            savePoints.Add(KeyCode.Alpha7, (zoomPos, transform.position, transform.rotation));
            savePoints.Add(KeyCode.Alpha8, (zoomPos, transform.position, transform.rotation));
            savePoints.Add(KeyCode.Alpha9, (zoomPos, transform.position, transform.rotation));
        }

        private void AddSavePoint()
        {
            if(Input.GetKey(KeyCode.LeftAlt))
            {
                foreach (KeyCode key in savePoints.Keys)
                {
                    if (Input.GetKey(key))
                    {
                        savePoints[key] = (zoomPos, transform.position, transform.rotation);
                    }
                }
            }
        }

        private void LoadSavePoint()
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                foreach (KeyCode key in savePoints.Keys)
                {
                    if (Input.GetKey(key))
                    {
                        var turple = ((float, Vector3, Quaternion))savePoints[key];
                        zoomPos = turple.Item1;
                        transform.position = turple.Item2;
                        transform.rotation = turple.Item3;
                    }
                }
            }
        }
    }
}