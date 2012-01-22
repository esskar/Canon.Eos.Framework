namespace Canon.Eos.Framework
{
    public class EosVolumeInfo
    {
        internal EosVolumeInfo() { }

        public long Access { get; internal set; }

        public ulong FreeSpaceInBytes { get; internal set; }

        public ulong MaxCapacityInBytes { get; internal set; }               

        public long StorageType { get; internal set; }                              

        public string VolumeLabel { get; internal set; }
    }
}
