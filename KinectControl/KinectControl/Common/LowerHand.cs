﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectControl.Common
{
    class LowerHandSegment1 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y)
                if (skeleton.Joints[JointType.HandRight].Position.X >= skeleton.Joints[JointType.HipLeft].Position.X)
                    return GesturePartResult.Suceed;
                else return GesturePartResult.Pausing;
            else return GesturePartResult.Fail;
        }
    }
    class LowerHandSegment2 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y)
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ShoulderCenter].Position.Y)
                    return GesturePartResult.Suceed;
                else return GesturePartResult.Pausing;
            else return GesturePartResult.Fail;
        }
    }
}
