using UnityEngine;

namespace Cinemachine.Examples
{

[AddComponentMenu("")] 
public class ActivateCameraWithDistance : MonoBehaviour
{
    public GameObject objectToCheck;
    public float distanceToObject = 15f;
    public CinemachineVirtualCameraBase initialActiveCam;
    public CinemachineVirtualCameraBase switchCameraTo;
    
    CinemachineBrain brain;

    void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        SwitchCam(initialActiveCam);
    }

    
    void Update()
    {
        if (objectToCheck && switchCameraTo)
        {
            if (Vector3.Distance(transform.position, objectToCheck.transform.position) < distanceToObject)
            {
                SwitchCam(switchCameraTo);
            }
            else
            {
                SwitchCam(initialActiveCam);
            }
        }
    }

    public void SwitchCam(CinemachineVirtualCameraBase vcam)
    {
        if (brain == null || vcam == null)
            return;
        if (brain.ActiveVirtualCamera != (ICinemachineCamera)vcam)
            vcam.MoveToTopOfPrioritySubqueue();      
    }
}

}