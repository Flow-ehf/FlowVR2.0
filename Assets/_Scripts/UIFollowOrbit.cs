using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowOrbit : MonoBehaviour
{
	[SerializeField] float speed = 90;
	[SerializeField] bool followVertical = false;
	[SerializeField] bool useUnscaledTime = false;

	Transform camTrans;
	float dist;
	float height;
	Quaternion actualRot;

    // Start is called before the first frame update
    void Awake()
    {
		camTrans = Camera.main?.transform;
    }


	void Start()
	{
		if (camTrans != null)
		{
			actualRot = GetTargetRot();
			dist = (camTrans.position - transform.position).magnitude;
			height = transform.position.y;
		}
	}


    // Update is called once per frame
    void Update()
    {
		if(camTrans != null)
		{
			Quaternion targetRot = GetTargetRot();

			actualRot = Quaternion.Lerp(actualRot, targetRot, speed * (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime));

			transform.rotation = actualRot;

			Vector3 pos = camTrans.position + actualRot * Vector3.forward  * dist;
			if (!followVertical)
				pos.y = height;

			transform.position = pos;
		}
    }


	Quaternion GetTargetRot()
	{
		Vector3 targetDir = camTrans.forward;
		if (!followVertical)
		{
			targetDir.y = 0;
			targetDir.Normalize();
		}

		//actualDir = Vector3.RotateTowards(actualDir, targetDir, speed * Time.deltaTime * Mathf.Deg2Rad, 0);

		return Quaternion.LookRotation(targetDir);
	}
}
