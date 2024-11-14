using MyBox;
using UnityEngine;

public class test_QuaternionAngleAxis : MonoBehaviour
{
    public Transform Target;

    public Vector3 Axis = Vector3.zero;

    public Vector3 Add2 = Vector3.zero;

    public float Angle = 30f;

    [ButtonMethod]
    public void Test1()
    {
        Target.forward += Quaternion.AngleAxis(Angle, Target.right) * Target.forward;
    }

    [ButtonMethod]
    public void Test2()
    {
        var forward = Target.forward;
        Target.forward = new Vector3(
            forward.x + Add2.x,
            forward.y + Add2.y,
            forward.z + Add2.z
            );
    }
}
