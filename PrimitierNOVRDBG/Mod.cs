using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using Il2Cpp;
using UnityEngine.InputSystem;

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
            if (leftHand == null)
            {
                leftHand = GameObject.Find("LeftHand Controller");
                return false;
            }
            if (rightHand == null)
            {
                rightHand = GameObject.Find("RightHand Controller");
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
            origin.transform.rotation = Quaternion.Euler(0f, Input.GetAxis("Mouse X") + origin.transform.rotation.eulerAngles.y, 0f);

            if (Input.GetKey(KeyCode.Space))
                origin.transform.localPosition += Vector3.up * mlt;
            else if (Input.GetKey(KeyCode.LeftControl))
                origin.transform.localPosition -= Vector3.up * mlt;

            head.transform.localRotation = Quaternion.Euler(-Input.GetAxis("Mouse Y") + head.transform.localRotation.eulerAngles.x, 0f, 0f);
        }

        GameObject leftHand;
        GameObject rightHand;

        void UpdateHandMovement()
        {
            leftHand.transform.position = head.transform.TransformPoint(new Vector3(-0.1f, -0.2f, 0.15f));
            leftHand.transform.rotation = head.transform.rotation * Quaternion.Euler(270f, 0f, 0f);

            rightHand.transform.position = head.transform.TransformPoint(new Vector3(0.1f, -0.2f, 0.15f));
            rightHand.transform.rotation = head.transform.rotation * Quaternion.Euler(270f, 0f, 0f);
        }

        bool initDone = false;
        public override void OnUpdate()
        {
            if (!initDone)
            {
                initDone = Init();
                return;
            }
            UpdatePlayerMovement();
            UpdateHandMovement();
        }

        bool loaded = false;
        public static bool god = false;
        public override void OnGUI()
        {
            god = GUI.Toggle(new Rect(10f, 100f, 300f, 50f), god, "Godmode");
        }
    }
}