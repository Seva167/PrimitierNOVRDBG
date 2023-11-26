using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using Il2Cpp;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PrimitierNOVRDBG
{
    public class Mod : MelonMod
    {
        GameObject origin;
        GameObject head;

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            GameObject.Find("CameraBody").SetActive(false);
            GameObject.Find("TitleSpace").transform.Find("TitleMenu").gameObject.SetActive(true);
            foreach (var canvas in GameObject.FindObjectsOfType<Canvas>(true))
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        bool Init()
        {
            if (origin == null)
            {
                origin = GameObject.Find("XR Origin");
                origin.GetComponent<Rigidbody>().isKinematic = true;

                var piam = origin.GetComponent<PlayerInput>().actions.actionMaps[0];

                // Menu
                piam.actions[11].AddBinding("<Keyboard>/n");
                piam.actions[12].AddBinding("<Keyboard>/m");

                // Grab
                piam.actions[7].AddBinding("<Keyboard>/g");
                piam.actions[8].AddBinding("<Keyboard>/h");

                // Bond
                piam.actions[9].AddBinding("<Keyboard>/t");
                piam.actions[10].AddBinding("<Keyboard>/y");

                // Separate
                piam.actions[13].AddBinding("<Keyboard>/u");
                piam.actions[14].AddBinding("<Keyboard>/i");
                return false;
            }
            if (head == null)
            {
                head = GameObject.Find("Main Camera");
                head.GetComponent<Camera>().fieldOfView = 90f;
                head.GetComponent<TrackedPoseDriver>().enabled = false;
                head.transform.localPosition = new Vector3(0f, 1.8f, 0f);
                return false;
            }
            if (leftHandCtrl == null)
            {
                leftHandCtrl = GameObject.Find("LeftHand Controller");
                leftHand = GameObject.Find("LeftHand").GetComponent<Grabber>();
                
                var hand = leftHand.GetComponent<Hand>();
                hand.maximumForce = float.PositiveInfinity;
                hand.maximumTorque = float.PositiveInfinity;

                hand.positionSpring = 1000000f;
                hand.rotationSpring = 1000000f;

                return false;
            }
            if (rightHandCtrl == null)
            {
                rightHandCtrl = GameObject.Find("RightHand Controller");
                rightHand = GameObject.Find("RightHand").GetComponent<Grabber>();

                var hand = rightHand.GetComponent<Hand>();
                hand.maximumForce = float.PositiveInfinity;
                hand.maximumTorque = float.PositiveInfinity;

                hand.positionSpring = 1000000f;
                hand.rotationSpring = 1000000f;

                return false;
            }

            return true;
        }

        void UpdatePlayerMovement()
        {
            var fwd = origin.transform.forward;
            var rgt = origin.transform.right;

            fwd.y = 0f;
            rgt.y = 0f;
            fwd.Normalize();
            rgt.Normalize();

            float mlt;
            if (Input.GetKey(KeyCode.LeftShift))
                mlt = 1f;
            else
                mlt = 0.1f;

            origin.transform.localPosition += (fwd * Input.GetAxis("Vertical") + rgt * Input.GetAxis("Horizontal")) * mlt;

            if (!(Input.GetMouseButton(0) || Input.GetMouseButton(1)) && hideMouse)
                origin.transform.rotation = Quaternion.Euler(0f, Input.GetAxis("Mouse X") + origin.transform.rotation.eulerAngles.y, 0f);

            if (Input.GetKey(KeyCode.Space))
                origin.transform.localPosition += Vector3.up * mlt;
            else if (Input.GetKey(KeyCode.LeftControl))
                origin.transform.localPosition -= Vector3.up * mlt;

            if (!(Input.GetMouseButton(0) || Input.GetMouseButton(1)) && hideMouse)
                head.transform.localRotation = Quaternion.Euler(-Input.GetAxis("Mouse Y") + head.transform.localRotation.eulerAngles.x, 0f, 0f);
        }

        GameObject leftHandCtrl;
        GameObject rightHandCtrl;

        Grabber leftHand;
        Grabber rightHand;

        Vector3 leftHandMove = new(-0.1f, -0.2f, 0.15f);
        Vector3 leftHandRot = new(270f, 0f, 0f);

        Vector3 rightHandMove = new(0.1f, -0.2f, 0.15f);
        Vector3 rightHandRot = new(270f, 0f, 0f);

        void UpdateHandMovement()
        {
            leftHandCtrl.transform.position = head.transform.TransformPoint(leftHandMove);
            leftHandCtrl.transform.rotation = head.transform.rotation * Quaternion.Euler(leftHandRot);

            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButton(2))
                {
                    leftHandRot += new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"));
                    return;
                }

                leftHandMove += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.mouseScrollDelta.y) * 0.05f;
            }
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                leftHandMove = new Vector3(-0.1f, -0.2f, 0.15f);
                leftHandRot = new Vector3(270f, 0f, 0f);
            }

            rightHandCtrl.transform.position = head.transform.TransformPoint(rightHandMove);
            rightHandCtrl.transform.rotation = head.transform.rotation * Quaternion.Euler(rightHandRot);

            if (Input.GetMouseButton(1))
            {
                if (Input.GetMouseButton(2))
                {
                    rightHandRot += new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"));
                    return;
                }

                rightHandMove += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.mouseScrollDelta.y) * 0.05f;
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                rightHandMove = new Vector3(0.1f, -0.2f, 0.15f);
                rightHandRot = new Vector3(270f, 0f, 0f);
            }
        }

        bool initDone = false;
        bool hideMouse = false;

        public override void OnUpdate()
        {
            if (!initDone)
            {
                initDone = Init();
                return;
            }

            UpdatePlayerMovement();
            UpdateHandMovement();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                hideMouse = !hideMouse;
                Cursor.lockState = hideMouse ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !hideMouse;
            }
        }

        public static bool god = false;
        bool ultraGrab = false;

        public override void OnGUI()
        {
            god = GUI.Toggle(new Rect(10f, 100f, 300f, 20f), god, "Godmode");
            ultraGrab = GUI.Toggle(new Rect(10f, 120f, 300f, 20f), ultraGrab, "ULTRA Grab");
            if (ultraGrab)
            {
                if (leftHand.joint != null)
                    leftHand.joint.connectedMassScale = 1000000f;
                if (rightHand.joint != null)
                    rightHand.joint.connectedMassScale = 1000000f;

                Grabber.releaseDistance = float.PositiveInfinity;
            }
            else
            {
                Grabber.releaseDistance = 1.8f;
            }
        }
    }
}