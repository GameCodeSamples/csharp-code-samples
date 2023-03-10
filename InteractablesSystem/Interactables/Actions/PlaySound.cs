using UnityEngine;

namespace Core.Runtime.Interactables
{
    public class PlaySound : Action
    {
        [SerializeField] private AudioSource audioSource;


        public override void Execute(GameObject instigator)
        {
            audioSource.Play();
        }

        public override void CancelExecution(GameObject instigator)
        {
            audioSource.Stop();
        }

        private void OnValidate()
        {
            if (audioSource && audioSource.clip)
            {
                gameObject.name = audioSource.clip.name;
            }
        }
    }
}