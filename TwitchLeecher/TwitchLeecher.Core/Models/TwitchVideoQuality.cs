using System;

namespace TwitchLeecher.Core.Models
{
    public class TwitchVideoQuality : IComparable<TwitchVideoQuality>
    {
        #region Constants

        private const string SOURCE_ID = "chunked";
        private const string AUDIO_ONLY_ID = "audio_only";

        #endregion Constants

        #region Constructors

        public TwitchVideoQuality(string id, string name, string resolution = null, string codecs = null, long bandwidth = 0)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Bandwidth = bandwidth;

            Initialize(id, name, resolution, codecs);
        }

        #endregion Constructors

        #region Properties

        public string Id { get; private set; }

        // bits per second as advertised in the master playlist (BANDWIDTH), 0 if unknown
        public long Bandwidth { get; private set; }

        public string Name { get; private set; }

        public string Resolution { get; private set; }

        public int VerticalResolution { get; private set; }

        public string DisplayString { get; private set; }

        public bool IsSource { get; private set; }

        public bool IsAudioOnly { get; private set; }

        #endregion Properties

        #region Methods

        private void Initialize(string id, string name, string resolution, string codecs)
        {
            Id = id;
            Name = name;
            Resolution = resolution;

            string displayString = Name;

            if (!string.IsNullOrWhiteSpace(resolution))
            {
                displayString += $" ({resolution})";
            }

            // H.264 is the default and stays unlabelled; HEVC/AV1 need a capable player, so say so
            if (codecs != null && (codecs.Contains("hev1") || codecs.Contains("hvc1")))
            {
                displayString += " HEVC";
            }
            else if (codecs != null && codecs.Contains("av01"))
            {
                displayString += " AV1";
            }

            DisplayString = displayString;

            VerticalResolution = GetVerticalResolution(resolution);

            IsSource = id.Equals(SOURCE_ID, StringComparison.Ordinal);
            IsAudioOnly = id.Equals(AUDIO_ONLY_ID, StringComparison.Ordinal);
        }

        private int GetVerticalResolution(string resolution)
        {
            if (string.IsNullOrWhiteSpace(resolution))
            {
                return 0;
            }

            int start = resolution.IndexOf("x") + 1;

            return int.Parse(resolution.Substring(start, resolution.Length - start));
        }

        public int CompareTo(TwitchVideoQuality other)
        {
            if (other == null)
            {
                return -1;
            }

            if (VerticalResolution > other.VerticalResolution)
            {
                return -1;
            }
            else if (VerticalResolution < other.VerticalResolution)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TwitchVideoQuality other))
            {
                return false;
            }

            return Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return DisplayString;
        }

        #endregion Methods
    }
}