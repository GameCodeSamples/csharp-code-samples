using UnityEngine;
using UnityEngine.AI;

namespace Core.Runtime.Interactables
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private bool showEntryPointForward = true;

        public Vector3 GetForwardPoint()
        {
            return transform.position + transform.forward * 5;
        }

        public bool GetPointOnNavMesh(out Vector3 navMeshPoint)
        {
            navMeshPoint = default;
            if (!NavMesh.SamplePosition(transform.position, out var meshHit, 5f, NavMesh.AllAreas)) return false;

            navMeshPoint = meshHit.position;
            return true;
        }

        public bool GetPointFloorLevel(out Vector3 navMeshPoint)
        {
            var foundSample = NavMesh.SamplePosition(transform.position, out var meshHit, 5f, NavMesh.AllAreas);
            if (!foundSample)
            {
                navMeshPoint = default;
                return false;
            }
            
            navMeshPoint = new Vector3()
            {
                x = transform.position.x,
                y = meshHit.position.y,
                z = transform.position.z
            };
            return true;
        }

        private void OnDrawGizmos()
        {
            if (!showEntryPointForward) return;

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, GetForwardPoint());
        }
    }
}