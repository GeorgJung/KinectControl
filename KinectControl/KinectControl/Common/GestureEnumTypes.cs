
namespace KinectControl.Common
{
    /// <summary>
    /// the gesture part result
    /// </summary>
    /// AUTHOR: Khaled
    public enum GesturePartResult
    {
        /// <summary>
        /// Gesture part fail
        /// </summary>
        Fail,

        /// <summary>
        /// Gesture part suceed
        /// </summary>
        Suceed,

        /// <summary>
        /// Gesture part result undetermined
        /// </summary>
        Pausing
    }

    /// <summary>
    /// The gesture type
    /// </summary>
    public enum GestureType
    {
        None,
        WaveLeft,
        PunchGesture,
    }
}
