using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    public float Radius;
    public float Height = 2f;
    public float Distance = 1;
    public bool DrawDebug = false;

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
        transform.rotation = Quaternion.Euler(Rot.x, Rot.y, Rot.z);
        if (Target != null)
        {
            Vector3 newPos = new Vector3(0, NewPos.y + Target.position.y, 0);
            //newPos.z = Mathf.Lerp(transform.position.z, NewPos.z + Target.transform.position.z, Time.deltaTime * Speed);
            newPos.z = -NewPos.z + Target.position.z;
            //newPos.x = Mathf.Lerp(transform.position.x, NewPos.x + Target.transform.position.x, Time.deltaTime * Speed);
            newPos.x = NewPos.x + Target.position.x;

            transform.position = NewPos + Target.position;
        }
        else
        {
            transform.position = NewPos + Target.position;
        }
    }
    private void OnDrawGizmos()
    {
        if (this.enabled)
        {
            SetCameraPos();
        }
        if (DrawDebug)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(new Vector3(Target.position.x + Radius, Target.position.y, transform.position.z), new Vector3(Target.position.x + Radius, Target.position.y, Target.position.z));
            Gizmos.DrawLine(new Vector3(Target.position.x + Radius, transform.position.y, transform.position.z), new Vector3(Target.position.x + Radius, Target.position.y, Target.position.z));
            Gizmos.DrawLine(new Vector3(Target.position.x + Radius, transform.position.y, transform.position.z), new Vector3(Target.position.x + Radius, Target.position.y, transform.position.z));

            Gizmos.DrawLine(new Vector3(Target.position.x + Radius, transform.position.y, transform.position.z), new Vector3(transform.position.x, transform.position.y, transform.position.z));
            Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(transform.position.x, Target.position.y, transform.position.z));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, Target.position);

            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
            Gizmos.DrawWireSphere(Target.position, Radius);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
