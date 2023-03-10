using System.IO;
using Cinemachine;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Wonderland
{
    public class CameraValuesTweaker : MonoBehaviour
    {
        [SerializeField] private CameraData cameraData;
        [SerializeField] private TouchData touchData;

        [Foldout("Camera senses & config")] [SerializeField]
        private SliderInfo xAxisMaxSpeed;

        [Foldout("Camera senses & config")] [SerializeField]
        private SliderInfo yAxisMaxSpeed;

        [Foldout("Camera senses & config")] [SerializeField]
        private SliderInfo axisAcceleration;

        [Foldout("Camera senses & config")] [SerializeField]
        private SliderInfo axisDeceleration;

        [Foldout("Camera senses & config")] [SerializeField]
        private SliderInfo zoomSpeed;

        [Foldout("Camera senses & config")] [SerializeField]
        private SliderInfo zoomDeceleration;

        [Foldout("Camera senses & config")] [SerializeField]
        private SliderInfo minZoomScale;

        [Foldout("Camera senses & config")] [SerializeField]
        private SliderInfo maxZoomScale;

        [Foldout("Camera senses & config")] [SerializeField]
        private Toggle xAxisInverted;

        [Foldout("Camera senses & config")] [SerializeField]
        private Toggle yAxisInverted;

        [Foldout("Camera thresholds")] [SerializeField]
        private SliderInfo zoomToStartThreshold; //TODO: move to a more general script

        [Foldout("Camera thresholds")] [SerializeField]
        private SliderInfo xRotationToStartThreshold;

        [Foldout("Camera thresholds")] [SerializeField]
        private SliderInfo yRotationToStartThreshold;

        [Foldout("Input thresholds")] [SerializeField]
        private SliderInfo tapThreshold;
        
        [Foldout("Input thresholds")] [SerializeField]
        private SliderInfo tapDistanceThreshold;

        [Foldout("Input thresholds")] [SerializeField]
        private SliderInfo holdThreshold;

        [Foldout("Input thresholds")] [SerializeField]
        private SliderInfo uiDragThreshold;

        [Foldout("Input thresholds")] [SerializeField]
        private SliderInfo worldDragThreshold;

        [Foldout("Generate initial position")] [SerializeField]
        private string cameraPositionName;

        [Foldout("Generate initial position")] [SerializeField]
        private float cameraInitialX;

        [Foldout("Generate initial position")] [SerializeField]
        private float cameraInitialY;

        private void Awake()
        {
            SetInitialValues();

            Subscribe();
        }

        private void ToggleValueChanged(bool b)
        {
            UpdateCameraValues();
        }

        private void SliderValueChanged(float f)
        {
            UpdateCameraValues();
        }

        private void UpdateCameraValues()
        {
            var cameraValuesData = new CameraValuesData
            {
                AxisAcceleration = axisAcceleration.CurrentValue,
                AxisDeceleration = axisDeceleration.CurrentValue,
                ZoomDeceleration = zoomDeceleration.CurrentValue,
                XAxisMaxSpeed = xAxisMaxSpeed.CurrentValue,
                YAxisMaxSpeed = yAxisMaxSpeed.CurrentValue,
                ZoomSpeed = zoomSpeed.CurrentValue,
                ZoomToStartThreshold = zoomToStartThreshold.CurrentValue,
                MaxZoomScale = maxZoomScale.CurrentValue,
                MinZoomScale = minZoomScale.CurrentValue,
                XRotationToStartThreshold = xRotationToStartThreshold.CurrentValue,
                YRotationToStartThreshold = yRotationToStartThreshold.CurrentValue,
                TapThreshold = tapThreshold.CurrentValue,
                TapDistanceThreshold = tapDistanceThreshold.CurrentValue,
                HoldThreshold = holdThreshold.CurrentValue,
                UiDragThreshold = uiDragThreshold.CurrentValue,
                WorldDragThreshold = worldDragThreshold.CurrentValue,
                XAxisInverted = xAxisInverted.isOn,
                YAxisInverted = yAxisInverted.isOn
            };

            ApplyTweakedCameraValues(cameraValuesData);
        }

        private void ApplyTweakedCameraValues(CameraValuesData cameraValues)
        {
            cameraData.XAxisMaxSpeed = cameraValues.XAxisMaxSpeed;
            cameraData.YAxisMaxSpeed = cameraValues.YAxisMaxSpeed;
            cameraData.AxisAcceleration = cameraValues.AxisAcceleration;
            cameraData.AxisDeceleration = cameraValues.AxisDeceleration;
            cameraData.XAxisInverted = cameraValues.XAxisInverted;
            cameraData.YAxisInverted = cameraValues.YAxisInverted;

            cameraData.XRotationToStartThreshold = cameraValues.XRotationToStartThreshold;
            cameraData.YRotationToStartThreshold = cameraValues.YRotationToStartThreshold;
            cameraData.XRotationToStartThreshold = cameraValues.XRotationToStartThreshold;
            cameraData.YRotationToStartThreshold = cameraValues.YRotationToStartThreshold;
            cameraData.ZoomToStartThreshold = cameraValues.ZoomToStartThreshold;

            cameraData.ZoomSense = cameraValues.ZoomSpeed;
            cameraData.ZoomDecelerationSpeed = cameraValues.ZoomDeceleration;
            cameraData.MaxZoomScale = cameraValues.MaxZoomScale;
            cameraData.MinZoomScale = cameraValues.MinZoomScale;
            touchData.TapTimeThreshold = cameraValues.TapThreshold;
            touchData.TapDistanceThreshold = cameraValues.TapDistanceThreshold;
            touchData.HoldThreshold = cameraValues.HoldThreshold;
            touchData.DragUIThreshold = cameraValues.UiDragThreshold;
            touchData.DragWorldThreshold = cameraValues.WorldDragThreshold;
        }

#if UNITY_EDITOR
        [Button("CreateInitialCameraPositionAsset")]
        public void GetInitialPositionAsset()
        {
            //it is expensive but it is just a tool for the designer
            var cinemachineFreeLook = FindObjectOfType<CinemachineFreeLook>();

            var newItem = ScriptableObject.CreateInstance<CameraInitialPosInfo>();
            newItem.Init(cinemachineFreeLook.m_Orbits, cameraInitialX, cameraInitialY);

            if (cameraPositionName != null)
            {
                AssetDatabase.CreateAsset(newItem, "Assets/Settings/Camera/" + cameraPositionName + ".asset");
                return;
            }

            var currentPath = "Assets/Settings/Camera/CameraInitialPos.asset";
            var numberOfFiles = 0;

            while (File.Exists(currentPath))
            {
                numberOfFiles++;
                currentPath = "Assets/Settings/Camera/CameraInitialPos(" + numberOfFiles + ").asset";
            }

            AssetDatabase.CreateAsset(newItem, currentPath);
        }
#endif

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private void UnSubscribe()
        {
            xAxisMaxSpeed.OnValueChange -= SliderValueChanged;
            yAxisMaxSpeed.OnValueChange -= SliderValueChanged;
            axisAcceleration.OnValueChange -= SliderValueChanged;
            axisDeceleration.OnValueChange -= SliderValueChanged;
            zoomSpeed.OnValueChange -= SliderValueChanged;
            zoomDeceleration.OnValueChange -= SliderValueChanged;
            zoomToStartThreshold.OnValueChange -= SliderValueChanged;
            xRotationToStartThreshold.OnValueChange -= SliderValueChanged;
            yRotationToStartThreshold.OnValueChange -= SliderValueChanged;
            tapThreshold.OnValueChange -= SliderValueChanged;
            tapDistanceThreshold.OnValueChange -= SliderValueChanged;
            maxZoomScale.OnValueChange -= SliderValueChanged;
            minZoomScale.OnValueChange -= SliderValueChanged;

            xAxisInverted.onValueChanged.RemoveListener(ToggleValueChanged);
            yAxisInverted.onValueChanged.RemoveListener(ToggleValueChanged);
        }

        private void Subscribe()
        {
            xAxisMaxSpeed.OnValueChange += SliderValueChanged;
            yAxisMaxSpeed.OnValueChange += SliderValueChanged;
            axisAcceleration.OnValueChange += SliderValueChanged;
            axisDeceleration.OnValueChange += SliderValueChanged;
            zoomSpeed.OnValueChange += SliderValueChanged;
            zoomDeceleration.OnValueChange += SliderValueChanged;
            zoomToStartThreshold.OnValueChange += SliderValueChanged;
            xRotationToStartThreshold.OnValueChange += SliderValueChanged;
            yRotationToStartThreshold.OnValueChange += SliderValueChanged;
            maxZoomScale.OnValueChange += SliderValueChanged;
            minZoomScale.OnValueChange += SliderValueChanged;
            tapThreshold.OnValueChange += SliderValueChanged;
            tapDistanceThreshold.OnValueChange += SliderValueChanged;
            holdThreshold.OnValueChange += SliderValueChanged;
            worldDragThreshold.OnValueChange += SliderValueChanged;
            uiDragThreshold.OnValueChange += SliderValueChanged;

            xAxisInverted.onValueChanged.AddListener(ToggleValueChanged);
            yAxisInverted.onValueChanged.AddListener(ToggleValueChanged);
        }

        private void SetInitialValues()
        {
            xAxisInverted.SetIsOnWithoutNotify(cameraData.XAxisInverted);
            yAxisInverted.SetIsOnWithoutNotify(cameraData.YAxisInverted);
            axisAcceleration.SetValuesWithoutNotify(cameraData.AxisAcceleration);
            axisDeceleration.SetValuesWithoutNotify(cameraData.AxisDeceleration);
            xAxisMaxSpeed.SetValuesWithoutNotify(cameraData.XAxisMaxSpeed);
            yAxisMaxSpeed.SetValuesWithoutNotify(cameraData.YAxisMaxSpeed);
            maxZoomScale.SetValuesWithoutNotify(cameraData.MaxZoomScale);
            minZoomScale.SetValuesWithoutNotify(cameraData.MinZoomScale);
            zoomDeceleration.SetValuesWithoutNotify(cameraData.ZoomDecelerationSpeed);
            zoomSpeed.SetValuesWithoutNotify(cameraData.ZoomSense);
            xRotationToStartThreshold.SetValuesWithoutNotify(cameraData.XRotationToStartThreshold);
            yRotationToStartThreshold.SetValuesWithoutNotify(cameraData.YRotationToStartThreshold);
            zoomToStartThreshold.SetValuesWithoutNotify(cameraData.ZoomToStartThreshold);
            tapThreshold.SetValuesWithoutNotify(touchData.TapTimeThreshold);
            tapDistanceThreshold.SetValuesWithoutNotify(touchData.TapDistanceThreshold);
            holdThreshold.SetValuesWithoutNotify(touchData.TapTimeThreshold);
            worldDragThreshold.SetValuesWithoutNotify(touchData.DragWorldThreshold);
            uiDragThreshold.SetValuesWithoutNotify(touchData.DragUIThreshold);
        }
    }

    public struct CameraValuesData
    {
        public float XAxisMaxSpeed;
        public float YAxisMaxSpeed;
        public float AxisAcceleration;
        public float AxisDeceleration;
        public float ZoomSpeed;
        public float ZoomDeceleration;
        public float MaxZoomScale;
        public float MinZoomScale;
        public bool XAxisInverted;
        public bool YAxisInverted;
        public float ZoomToStartThreshold;
        public float XRotationToStartThreshold;
        public float YRotationToStartThreshold;
        public float TapThreshold;
        public float TapDistanceThreshold;
        public float HoldThreshold;
        public float UiDragThreshold;
        public float WorldDragThreshold;
    }
}