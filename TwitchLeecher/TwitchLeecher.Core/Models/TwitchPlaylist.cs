using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TwitchLeecher.Core.Models
{
    public class TwitchPlaylist : List<TwitchPlaylistPart>
    {
        #region Constants

        private static readonly char[] extinfSeparators = { ':', ',' };        

        #endregion Constants
        
        #region Static Methods

        public static TwitchPlaylist Parse(string tempDir, string playlistStr, string urlPrefix) {
            List<string> lines = playlistStr.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            string playlistVersionStr = lines.Find(line => line.Trim().StartsWith("#EXT-X-VERSION:")) ?? ":";
            int playlistVersion = int.Parse(playlistVersionStr.Split(':')[1], NumberStyles.Integer, CultureInfo.InvariantCulture);
            switch (playlistVersion) {
                case 3:
                    return ParseV3(tempDir, lines, urlPrefix);                            
                case 4:
                    return ParseV4(tempDir, lines, urlPrefix);
                case 6:
                    return ParseV6(tempDir, lines, urlPrefix);
                default:
                    // V3 parsing was the old default too
                    return ParseV3(tempDir, lines, urlPrefix);
            }
        }
        
        // In use even at 2026.08.04 (https://www.twitch.tv/videos/2836855458, 1080p60)
        private static TwitchPlaylist ParseV3(string tempDir, List<string> lines, string urlPrefix)
        {
            TwitchPlaylist playlist = new TwitchPlaylist();

            int partCounter = 0;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                if (line.StartsWith("#EXTINF", StringComparison.OrdinalIgnoreCase))
                {
                    double length = Math.Max(double.Parse(line.Substring(line.LastIndexOf(":") + 1).TrimEnd(','), NumberStyles.Any, CultureInfo.InvariantCulture), 0);

                    playlist.Add(new TwitchPlaylistPart(length, urlPrefix + lines[i + 1], Path.Combine(tempDir, partCounter.ToString("D8") + ".ts")));
                    partCounter++;
                    i++;
                }
            }

            return playlist;
        }

        private static TwitchPlaylist ParseV4(string tempDir, List<string> lines, string urlPrefix)
        {
            TwitchPlaylist playlist = new TwitchPlaylist();

            int partCounter = 0;
            double lengthBuffer = 0;
            string currentPartStr = null;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                if (line.StartsWith("#EXTINF", StringComparison.OrdinalIgnoreCase))
                {
                    string partStr = lines[i + 2];

                    if (string.IsNullOrWhiteSpace(currentPartStr))
                    {
                        currentPartStr = partStr;
                    }

                    if (!currentPartStr.Equals(partStr))
                    {
                        playlist.Add(new TwitchPlaylistPart(lengthBuffer, urlPrefix + currentPartStr, Path.Combine(tempDir, partCounter.ToString("D8") + ".ts")));
                        currentPartStr = partStr;
                        lengthBuffer = 0;
                        partCounter++;
                    }

                    lengthBuffer += Math.Max(double.Parse(line.Substring(line.LastIndexOf(":") + 1).TrimEnd(','), NumberStyles.Any, CultureInfo.InvariantCulture), 0);

                    i++;
                }
            }

            if (!string.IsNullOrWhiteSpace(currentPartStr) && lengthBuffer > 0)
            {
                playlist.Add(new TwitchPlaylistPart(lengthBuffer, urlPrefix + currentPartStr, Path.Combine(tempDir, partCounter.ToString("D8") + ".ts")));
            }

            return playlist;
        }
        
        // Format for DASH streams (in use roughly from 2026.02.)
        private static TwitchPlaylist ParseV6(string tempDir, List<string> lines, string urlPrefix) {
            TwitchPlaylist playlist = new TwitchPlaylist();

            for (int i = 0; i < lines.Count; i++) {
                string line = lines[i];

                // process init segment
                if (line.StartsWith("#EXT-X-MAP:URI=")) {
                    string segmentName = line.Split("=")[1].Trim('"');
                    
                    playlist.Add(new TwitchPlaylistPart(
                        0, // review: not used anywhere?
                        urlPrefix + segmentName, 
                        Path.Combine(tempDir, segmentName))
                    );
                    
                // content segments
                } else if (line.StartsWith("#EXTINF", StringComparison.OrdinalIgnoreCase)) {
                    string segmentName = lines[i + 1];
                    double segmentLength = Math.Max(
                        double.Parse(
                            line.Split(extinfSeparators)[1],
                            NumberStyles.Any, CultureInfo.InvariantCulture), 
                        0);

                    if (!segmentName.All(c => char.IsDigit(c) || char.IsLetter(c) || c == '.') || !segmentName.EndsWith(".mp4")) {
                        // filter segment name to safe chars, and the known good extensions
                        throw new ApplicationException("VOD playlist is V6, but contains unknown entries");
                    }

                    playlist.Add(new TwitchPlaylistPart(
                        segmentLength, 
                        urlPrefix + segmentName, 
                        Path.Combine(tempDir, segmentName))
                    );
                    
                    i++;
                }
            }

            return playlist;
        }

        #endregion Static Methods
    }
}