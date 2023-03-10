using System;
using NaughtyAttributes;
using UnityEngine;

namespace Wonderland
{
    [CreateAssetMenu(menuName = "Wonderland/Camera/CameraData")]
    public class CameraData : ScriptableObject
    {
        public event Action OnCameraValuesUpdated;

        [SerializeField] private CameraInitialPosInfo initialCameraPosition;
        [SerializeField] private LookAtData lookAtData;

        [Foldout("Drag config"), SerializeField] 
        private float xAxisMaxSpeed = 95f;

        [Foldout("Drag config")] [SerializeField]
        private bool xAxisInverted;

        [Foldout("Drag config")] [SerializeField]
        private float yAxisMaxSpeed = 0.45f;

        [Foldout("Drag config")] [SerializeField]
        private bool yAxisInverted = true;

        [Foldout("Drag config")] [SerializeField]
        private float axisAcceleration = 0.5f;

        [Foldout("Drag config")] [SerializeField]
        private float axisDeceleration = 0.5f;

        [Foldout("Zoom config")] [SerializeField]
        private float zoomSense = 1.0f;

        [Foldout("Zoom config")] [SerializeField]
        private float zoomDecelerationSpeed = 1f;

        [Foldout("Zoom config")] [MinValue(0.1f), MaxValue(1)] [SerializeField]
        private float minZoomScale = 0.5f;

        [Foldout("Zoom config")] [MinValue(1), MaxValue(10)] [SerializeField]
        private float maxZoomScale = 5f;

        [Foldout("Zoom config")] [SerializeField]
        private float zoomToStartThreshold = 0.0001f;

        [Foldout("Drag config")] [SerializeField]
        private float xRotationToStartThreshold = 0.1f;

        [Foldout("Drag config")] [SerializeField]
        private float yRotationToStartThreshold = 0.1f;

        public float XAxisMaxSpeed
        {
            get => xAxisMaxSpeed;
            set
            {
                xAxisMaxSpeed = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }

        public bool XAxisInverted
        {
            get => xAxisInverted;
            set
            {
                xAxisInverted = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }

        public bool YAxisInverted
        {
            get => yAxisInverted;
            set
            {
                yAxisInverted = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }

        public float YAxisMaxSpeed
        {
            get => yAxisMaxSpeed;
            set
            {
                yAxisMaxSpeed = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }

        public float AxisAcceleration
        {
            get => axisAcceleration;
            set
            {
                axisAcceleration = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }

        public float AxisDeceleration
        {
            get => axisDeceleration;
            set
            {
                axisDeceleration = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }

        public float ZoomSense
        {
            get => zoomSense;
            set
            {
                zoomSense = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }

        public float ZoomDecelerationSpeed
        {
            get => zoomDecelerationSpeed;
            set
            {
                zoomDecelerationSpeed = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }

        public float MinZoomScale
        {
            get => minZoomScale;
            set
            {
                minZoomScale = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }

        public float MaxZoomScale
        {
            get => maxZoomScale;
            set
            {
                maxZoomScale = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }


        public float ZoomToStartThreshold
        {
            get => zoomToStartThreshold;
            set
            {
                zoomToStartThreshold = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }

        public float XRotationToStartThreshold
        {
            get => xRotationToStartThreshold;
            set
            {
                xRotationToStartThreshold = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }

        public float YRotationToStartThreshold
        {
            get => yRotationToStartThreshold;
            set
            {
                yRotationToStartThreshold = value;
                OnCameraValuesUpdated?.Invoke();
            }
        }
    }
}