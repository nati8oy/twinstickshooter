using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(FieldOfView))]

public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);

        Vector3 viewAngleA = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngleB = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);

        Handles.color = Color.yellow;

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.radius);


        if(fov.canSeeTarget)
        {
            Handles.color = Color.cyan;
            //Handles.DrawLine(fov.transform.position, fov.targetRefObject.transform.position);
        }
        else
        {
            Handles.color = Color.green;
        }

        //foreach (Transform visibleTarget in fov.VisibleTargets)
        //{
        //    Handles.DrawLine(fov.transform.position, visibleTarget.position);
        //}
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

}
