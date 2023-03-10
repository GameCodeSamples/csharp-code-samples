using UnityEngine;

namespace Wonderland
{
    public class LookAtTarget : MonoBehaviour
    {
        [field: SerializeField] public LookAtData LookAtData { get; private set; }

        private SkeletonComponent targetSkeleton;
        private Vector3 TargetPosition { get; set; }

        public SkeletonComponent TargetSkeleton
        {
            set
            {
                targetSkeleton = value;
                var headPos = targetSkeleton.InstantiatedSkeleton.Head.position;
                var yPos = Mathf.Max(LookAtData.LookAtMinHeight, headPos.y);
                transform.position = new Vector3(headPos.x, yPos, headPos.z);
            }
        }

        public void Init(SkeletonComponent skeletonController, LookAtData lookAtData)
        {
            targetSkeleton = skeletonController;
            LookAtData = lookAtData;
        }

        private void FixedUpdate()
        {
            if (!targetSkeleton) return;
            AdjustTargetPos();
            transform.position = Vector3.Lerp(transform.position, TargetPosition, LookAtData.LookAtSmoothSpeed * Time.deltaTime);
        }

        private void AdjustTargetPos()
        {
            var distanceFromHead = Vector3.Distance(transform.position, targetSkeleton.InstantiatedSkeleton.Head.transform.position);
            if (distanceFromHead < LookAtData.LookAtTargetThreshold) return;
            
            var headPos = targetSkeleton.InstantiatedSkeleton.Head.position;
            var minHeadPos = transform.parent.TransformPoint(0f, LookAtData.LookAtMinHeight, 0f);

            var yPos = Mathf.Max(minHeadPos.y, headPos.y);
            TargetPosition = new Vector3(headPos.x, yPos, headPos.z);
        }
    }
}