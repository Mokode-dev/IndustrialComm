using System;

namespace IndustrialComm.Framing
{
    /// <summary>
    /// Strategy that inspects a receive buffer and decides whether a complete frame is present.
    /// </summary>
    public interface IFrameDetector
    {
        /// <summary>
        /// Attempts to detect a complete frame starting at the beginning of <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Current receive buffer contents (may contain partial or multiple frames).</param>
        /// <param name="frameLength">When successful, total byte length of the first complete frame.</param>
        /// <param name="bytesToSkip">When no frame can start at offset 0, number of bytes to discard (resync).</param>
        /// <returns>Detection status.</returns>
        FrameDetectStatus TryDetect(ReadOnlySpan<byte> buffer, out int frameLength, out int bytesToSkip);
    }

    /// <summary>Result of a frame detection attempt.</summary>
    public enum FrameDetectStatus
    {
        /// <summary>Not enough data yet; keep buffering.</summary>
        NeedMoreData = 0,

        /// <summary>A complete frame of <c>frameLength</c> bytes is available at the start of the buffer.</summary>
        FrameReady = 1,

        /// <summary>Leading garbage detected; skip <c>bytesToSkip</c> bytes and try again.</summary>
        Skip = 2,

        /// <summary>Buffer is invalid and should be cleared (e.g. declared length exceeds max).</summary>
        Invalid = 3,
    }
}
