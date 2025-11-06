using UnityEngine;

namespace Cinemachine.Examples
{

[AddComponentMenu("")] 
public class ActivateCamOnPlay : MonoBehaviour
{
    public CinemachineVirtualCameraBase vcam;

	
	void Start () 
    {
	    if (vcam)
	    {
	        vcam.MoveToTopOfPrioritySubqueue();
	    }
	}
}

}