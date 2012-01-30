namespace Canon.Eos.Framework
{
    public struct EosImageQuality
    {
        private EosImageSize _secondaryImageSize;

        public EosImageSize PrimaryImageSize { get; set; }

        public EosImageFormat PrimaryImageFormat { get; set; }

        public EosCompressLevel PrimaryCompressLevel { get; set; }

        public EosImageSize SecondaryImageSize 
        {
            get { return (byte)_secondaryImageSize >= 0xF ? EosImageSize.Unknown : _secondaryImageSize; }
            set { _secondaryImageSize = value; }
        }

        public EosImageFormat SecondaryImageFormat { get; set; }

        public EosCompressLevel SecondaryCompressLevel { get; set; }

        public override string ToString()
        {
            return string.Format("Primary Image: Size <{0}>, Format <{1}>, CompressLevel <{2}>\n"
                + "Secondary Image: Size <{3}>, Format <{4}>, CompressLevel <{5}>",
                this.PrimaryImageSize, this.PrimaryImageFormat, this.PrimaryCompressLevel,
                this.SecondaryImageSize, this.SecondaryImageFormat, this.SecondaryCompressLevel);
        }

        internal static EosImageQuality Create(long bitMask)
        {
            var quality = new EosImageQuality { 
                PrimaryImageSize = (EosImageSize)((bitMask >> 24) & 0xFF),
                PrimaryImageFormat = (EosImageFormat)((bitMask >> 20) & 0xF),
                PrimaryCompressLevel = (EosCompressLevel)((bitMask >> 16) & 0xF),
                SecondaryImageSize = (EosImageSize)((bitMask >> 8) & 0xF),
                SecondaryImageFormat = (EosImageFormat)((bitMask >> 4) & 0xF),
                SecondaryCompressLevel = (EosCompressLevel)(bitMask & 0xF),
            };
            return quality;
        }

        internal long ToBitMask()
        {
            return (uint)this.PrimaryImageSize << 24 |
                   (uint)this.PrimaryImageFormat << 20 |
                   (uint)this.PrimaryCompressLevel << 16 |
                   (uint)this.SecondaryImageSize << 8 |
                   (uint)this.SecondaryImageFormat << 4 |
                   (uint)this.SecondaryCompressLevel;            
        }
    }
}
