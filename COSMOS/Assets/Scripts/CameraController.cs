using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    public float Radius;
    public float Height = 2f;
    public float Distance = 1;
    public bool DrawDebug = true;

    void Update()
    {
        SetCameraPos();
    }
    public void SetCameraPos()
    {
        Vector3 NewPos = Vector3.zero;
        Vector3 Rot = transform.eulerAngles;

        float radius = Radius * Distance;
        float height = Height * Distance;

        NewPos.z = Mathf.Cos(Rot.y * Mathf.Deg2Rad) * -radius;
        NewPos.y = height;
        NewPos.x = Mathf.Sin(Rot.y * Mathf.Deg2Rad) * -radius;
        Rot.x = Mathf.Atan(height / radius) * Mathf.Rad2Deg;
        if (float.IsNaN(Rot.x)) Rot.x = 0;
        if (float.IsNaN(Rot.z)) Rot.z = 0;
        transform.rotation = Quaternion.Euler(Rot.x, Rot.y, Rot.z);
        if (Target != null)
        {
            Vector3 newPos = new Vector3(0, NewPos.y + Target.position.y, 0);
            newPos.z = -NewPos.z + Target.position.z;
            newPos.x = NewPos.x + Target.position.x;

            transform.position = NewPos + Target.position;
        }
        else
        {
            transform.position = NewPos + Target.position;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (this.enabled)
        {
            SetCameraPos();
            if (DrawDebug)
            {
                Gizmos.color = Color.red;

                Vector3 targetPos = Vector3.zero;
                if (Target != null)
                {
                    targetPos = Target.position;
                }

                Gizmos.DrawLine(new Vector3(targetPos.x + Radius, targetPos.y, transform.position.z), new Vector3(targetPos.x + Radius, targetPos.y, targetPos.z));
                Gizmos.DrawLine(new Vector3(targetPos.x + Radius, transform.position.y, transform.position.z), new Vector3(targetPos.x + Radius, targetPos.y, targetPos.z));
                Gizmos.DrawLine(new Vector3(targetPos.x + Radius, transform.position.y, transform.position.z), new Vector3(targetPos.x + Radius, targetPos.y, transform.position.z));

                Gizmos.DrawLine(new Vector3(targetPos.x + Radius, transform.position.y, transform.position.z), new Vector3(transform.position.x, transform.position.y, transform.position.z));
                Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(transform.position.x, targetPos.y, transform.position.z));

                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, targetPos);

                Gizmos.color = Color.yellow;
                Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
                Gizmos.DrawWireSphere(targetPos, Radius * Distance);
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }
}
