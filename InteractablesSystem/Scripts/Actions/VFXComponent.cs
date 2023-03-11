using NaughtyAttributes;
using UnityEngine;

namespace Wonderland
{
    [RequireComponent(typeof(ParticleSystem))]
    public class VFXComponent : MonoBehaviour
    {
        [field: SerializeField] public ParticleSystem particleSystem { get; private set; }
        [SerializeField] private bool hasDuration = true;

        [ShowIf(nameof(hasDuration))] [SerializeField]
        private float duration = 5f;

        private void Start()
        {
            Invoke(nameof(Unspawn), duration);
        }

        private void Unspawn()
        {
            particleSystem.Stop();
            Destroy(particleSystem.gameObject);
        }


        private void OnValidate()
        {
            if (!TryGetComponent(out ParticleSystem foundParticleSystem)) return;
            particleSystem = foundParticleSystem;
        }
    }
}