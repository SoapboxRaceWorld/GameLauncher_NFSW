namespace GameLauncher
{
    public delegate void ProgressUpdated(long dowloadLength, long downloadCurrent, long compressedLength, string fileName, int skipdownload = 0);
}