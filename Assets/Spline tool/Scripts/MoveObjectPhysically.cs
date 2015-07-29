// Copyright (c) 2014 Raed Abdullah
// moves the object along the spline using rigidbody physics

using UnityEngine;
using SplineTool;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FollowSpline))]
public class MoveObjectPhysically : MonoBehaviour
{
    #region variables
    // movement speed
    public float speed = 5;

    [Range(0.01f, 100)]
    public float rotationSpeed = 20;
    #endregion

    /// <summary>
    /// Moving along the spline 
    /// </summary>
    /// <param name="pos">target position this object will move to</param>
    public void Move(Vector3 pos)
    {
        // converting the position to a direction
        pos = (pos - transform.position).normalized;
        // freezing the y axis to let the gravity handle it 
        // if you want moving without gravity try MoveObject script
        pos.y = transform.position.y;

        // zeroing the velocity but the y-axis
        Vector3 vel = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);

        // looking at the target
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(pos, transform.up), rotationSpeed / 100);

        // freezing all but Y-Axis in rotation
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        // adding velocity to the object forward 
        vel += transform.forward * speed;

        // assigning velocity to the rigidbody
        GetComponent<Rigidbody>().velocity = vel;
    }
}
