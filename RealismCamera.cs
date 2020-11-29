#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Avalix
{

    public class RealismCamera : MonoBehaviour
    {      

        // Lock cursor on start and during gameplay
        // Pressing escape will release the cursor lock,
        public bool lockCursor = true;

        // Allows the option for gamepads
        public bool useGamePad = false;

        public Transform target;

        Vector3 currentRotation;

        public float mouseSensitivityX = 10.0f;
        public float mouseSensitivityY = 10.0f;
        public float joystickSensitivityX = 80.0f;
        public float joystickSensitivityY = 80.0f;

        public bool invertX;
        public bool invertY;

        Vector3 rotationSmoothVelocity;
        public float rotationSmoothTime;

        public float positionSmoothTime;

        public float camDistance;

        public Vector2 pitchMinMax = new Vector2 (-40, 85);

        public float yaw;
        public float pitch;

        // Camera collision
        RaycastHit hit;

        void Start() {
            LockCursor();
        }

        // Update is called once per frame
        void Update()
        {   
            ManageCursor();

            if(useGamePad) {
                yaw += (invertX) ? Input.GetAxis ("JoystickLookX") * joystickSensitivityX * -1 : Input.GetAxis ("JoystickLookX") * joystickSensitivityX;
                pitch += (invertY) ? Input.GetAxis ("JoystickLookY") * joystickSensitivityY * -1 : Input.GetAxis ("JoystickLookY") * joystickSensitivityY;
            } else {
                yaw += (invertX) ? Input.GetAxis ("MouseLookX") * mouseSensitivityX * -1 : Input.GetAxis ("MouseLookX") * mouseSensitivityX;
                pitch -= (invertY) ? Input.GetAxis ("MouseLookY") * mouseSensitivityY * -1 : Input.GetAxis ("MouseLookY") * mouseSensitivityY;
            }

            if(target) {
                pitch = Mathf.Clamp (pitch, pitchMinMax.x, pitchMinMax.y); 

                transform.position = Vector3.Lerp(transform.position, target.position - transform.forward * camDistance, positionSmoothTime);

                currentRotation = Vector3.SmoothDamp (currentRotation, new Vector3 (pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
                transform.eulerAngles = currentRotation;
            }

        }

        void LateUpdate() {
            if(target) {
                pitch = Mathf.Clamp (pitch, pitchMinMax.x, pitchMinMax.y); 

                transform.position = Vector3.Lerp(transform.position, target.position - transform.forward * camDistance, positionSmoothTime);

                currentRotation = Vector3.SmoothDamp (currentRotation, new Vector3 (pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
                transform.eulerAngles = currentRotation;
            }
        }

        void ManageCursor() {
            if(Input.anyKey || Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
                LockCursor();
            }

            if(Input.GetKeyUp(KeyCode.Escape)) {
                ReleaseCursor();
            }
        }

        void LockCursor() {
            if (lockCursor) {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
            }
        }

        void ReleaseCursor() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(RealismCamera))]
    [CanEditMultipleObjects]
    class RealismCameraEditor : Editor {

        SerializedProperty target;
        SerializedProperty useGamePad;
        SerializedProperty lockCursor;
        SerializedProperty mouseSensitivityX;
        SerializedProperty mouseSensitivityY;
        SerializedProperty joystickSensitivityX;
        SerializedProperty joystickSensitivityY;
        SerializedProperty invertX;
        SerializedProperty invertY;
        SerializedProperty camDistance;
        SerializedProperty rotationSmoothTime;
        SerializedProperty positionSmoothTime;

        bool cameraSettings;
        bool controllerSettings;
        bool mouseSettings;

        void OnEnable() {
            target = serializedObject.FindProperty ("target");
            useGamePad = serializedObject.FindProperty ("useGamePad");
            lockCursor = serializedObject.FindProperty ("lockCursor");
            mouseSensitivityX = serializedObject.FindProperty ("mouseSensitivityX");
            mouseSensitivityY = serializedObject.FindProperty ("mouseSensitivityY");
            joystickSensitivityX = serializedObject.FindProperty ("joystickSensitivityX");
            joystickSensitivityY = serializedObject.FindProperty ("joystickSensitivityY");
            invertX = serializedObject.FindProperty ("invertX");
            invertY = serializedObject.FindProperty ("invertY");
            camDistance = serializedObject.FindProperty ("camDistance");
            rotationSmoothTime = serializedObject.FindProperty ("rotationSmoothTime");
            positionSmoothTime = serializedObject.FindProperty ("positionSmoothTime");
        }

        public override void OnInspectorGUI() {

            serializedObject.Update ();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Unity Realism Camera v0.9", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(lockCursor, new GUIContent("Lock Cursor"));
            EditorGUILayout.Space();

            cameraSettings = EditorGUILayout.Foldout(cameraSettings, "Camera Settings");

            if(cameraSettings) {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Player or Follow Target", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.ObjectField(target, typeof(Transform));

                EditorGUILayout.Space();
                EditorGUILayout.Slider (camDistance, 0.0f, 5.0f, new GUIContent ("Follow Distance"));
                ProgressBar (camDistance.floatValue / 5.0f, "");
                
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Invert Rotations (Mouse and Gamepads/Controllers", EditorStyles.boldLabel);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(invertX, new GUIContent("Invert X Rotation"));

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(invertY, new GUIContent("Invert Y Rotation"));
                EditorGUILayout.Space();

                EditorGUILayout.Slider (rotationSmoothTime, 0.0f, 1.0f, new GUIContent ("Rotation Smoothing"));
                ProgressBar (rotationSmoothTime.floatValue / 1.0f, "");
                EditorGUILayout.Space();

                EditorGUILayout.Slider (positionSmoothTime, 0.0f, 1.0f, new GUIContent ("Position Smoothing"));
                ProgressBar (positionSmoothTime.floatValue / 1.0f, "");
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();

            EditorGUILayout.Space();

            controllerSettings = EditorGUILayout.Foldout(controllerSettings, "Gamepads and Controllers");

            if(controllerSettings) {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(useGamePad, new GUIContent("Enable Controllers"));
                EditorGUILayout.Space();

                if(useGamePad.boolValue) {
                    EditorGUILayout.Space();
                    EditorGUILayout.Slider (joystickSensitivityX, 0.0f, 100.0f, new GUIContent ("Joystick X Sensitivity"));
                    ProgressBar (joystickSensitivityX.floatValue / 100.0f, "");
                    EditorGUILayout.Space();

                    EditorGUILayout.Slider (joystickSensitivityY, 0.0f, 100.0f, new GUIContent ("Joystick Y Sensitivity"));
                    ProgressBar (joystickSensitivityY.floatValue / 100.0f, "");
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.Space();

            mouseSettings = EditorGUILayout.Foldout(mouseSettings, "Mouse Settings");

            if(mouseSettings) {
                EditorGUILayout.Space();
                EditorGUILayout.Slider (mouseSensitivityX, 0.0f, 100.0f, new GUIContent ("Mouse X Sensitivity"));
                ProgressBar (mouseSensitivityX.floatValue / 100.0f, "");
                EditorGUILayout.Space();

                EditorGUILayout.Slider (mouseSensitivityY, 0.0f, 100.0f, new GUIContent ("Mouse Y Sensitivity"));
                ProgressBar (mouseSensitivityY.floatValue / 100.0f, "");
                EditorGUILayout.Space();
            }

            

            serializedObject.ApplyModifiedProperties ();
        
        }

        void ProgressBar (float value, string label) {
            Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
            EditorGUI.ProgressBar (rect, value, label);
            EditorGUILayout.Space ();
        }

    }
    #endif

}