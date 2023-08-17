using System.Collections;
using UnityEngine;
namespace FrontierIsland
{
    public class CameraManager : MonoBehaviour
    {
        private static CameraManager instance;
        public static CameraManager Instance
        {
            get { return instance; }
        }

        #region public_field
        public float maxCameraSize = 10, maxFocusSpeed = 30, smoothTime = 0.05f;
        public int dragFlag, scrollFlag;
        #endregion

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Vector3 dragStartPosition;

        private IEnumerator actionLoop;
        protected virtual IEnumerator ActionLoop
        {
            get { return actionLoop; }
            set
            {
                if (actionLoop != null)
                {
                    StopCoroutine(actionLoop);
                }
                actionLoop = value;
                if (actionLoop != null)
                    StartCoroutine(actionLoop);
            }
        }

        public bool IsMouseOverGameWindow 
        { 
            get 
            { 
                return !(0 > Input.mousePosition.x || 
                        0 > Input.mousePosition.y || 
                        Screen.width < Input.mousePosition.x || 
                        Screen.height < Input.mousePosition.y); 
            } 
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }


        void Update()
        {
            HandleMovement();
            HandleScroll();
        }

        private void HandleMovement()
        {
            // Drag code from https://youtu.be/rnqF6S7PfFA?t=756

            if (dragFlag != 0) return;
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float entry;
                if (plane.Raycast(ray, out entry))
                {
                    dragStartPosition = ray.GetPoint(entry);
                }
                ActionLoop = null;
            }

            if (Input.GetMouseButton(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float entry;
                if (plane.Raycast(ray, out entry))
                {
                    transform.position += dragStartPosition - ray.GetPoint(entry);
                }
            }
        }

        private void HandleScroll()
        {
            if (scrollFlag != 0) return;
            if (Input.mouseScrollDelta.y != 0)
            {
                // subtract because controls are inverted
                float size = Camera.main.orthographicSize - Input.mouseScrollDelta.y;
                Camera.main.orthographicSize = Mathf.Clamp(size, 1, maxCameraSize);
            }
        }

        public void StartFocus(Vector3 pos)
        {
            ActionLoop = Focus(pos);
        }

        private IEnumerator Focus(Vector3 pos)
        {
            // TODO: allow focus in y axis
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2, Screen.height/2));

            Vector3 initalCamPos    = transform.position;
            Vector3 targetDelta     = Vector3.zero;
            Vector3 currentDelta    = Vector3.zero;
            Vector3 velocity = Vector3.zero;
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                targetDelta = pos - ray.GetPoint(entry);
                targetDelta.y = 0;
            }

            while ((targetDelta - currentDelta).magnitude > 0.1f)
            {
                currentDelta = Vector3.SmoothDamp(currentDelta, targetDelta, ref velocity, smoothTime, maxFocusSpeed);
                transform.position = initalCamPos + currentDelta;
                yield return null;
            }
            transform.position = initalCamPos + targetDelta;
        }
    }
}
