using Cinemachine;
using UnityEngine;

namespace Wonderland
{
    public class CameraInitialPosInfo : ScriptableObject
    {
        public float initialCameraXValue;
        public float initialCameraYValue;

        public CinemachineFreeLook.Orbit[] orbits;

        public void Init(CinemachineFreeLook.Orbit[] newOrbits, float initialX, float initialY)
        {
            orbits = new CinemachineFreeLook.Orbit[3];

            for (int i = 0; i < newOrbits.Length; i++)
            {
                orbits[i].m_Height = newOrbits[i].m_Height;
                orbits[i].m_Radius = newOrbits[i].m_Radius;
            }

            initialCameraXValue = initialX;
            initialCameraXValue = initialY;
        }
    }
}