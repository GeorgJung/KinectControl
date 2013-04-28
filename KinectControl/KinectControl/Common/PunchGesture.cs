using Microsoft.Kinect;

namespace KinectControl.Common
{
    /// AUTHOR: Khaled
    class PunchGesture1 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            SkeletonAnalyzer analyzer = new SkeletonAnalyzer();
            analyzer.SetBodySegments(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.HipCenter]);
            if ((skeleton.Joints[JointType.HandRight].Position.X < skeleton.Joints[JointType.ShoulderRight].Position.X + 0.4) &&
                (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.HipCenter].Position.X))
            {
                if ((skeleton.Joints[JointType.HandRight].Position.Z) < (skeleton.Joints[JointType.ElbowRight].Position.Z)
                    && skeleton.Joints[JointType.ElbowRight].Position.Z < skeleton.Joints[JointType.ShoulderRight].Position.Z)
                {
                    if (analyzer.GetBodySegmentAngle(skeleton.Joints) > 10 && analyzer.GetBodySegmentAngle(skeleton.Joints) < 60)
                        return GesturePartResult.Suceed;
                }
                return GesturePartResult.Pausing;
            }
            else
                return GesturePartResult.Fail;
        }
    }
}



