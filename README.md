The is a new maintained fork of [Twitch Leecher](https://github.com/Franiac/TwitchLeecher)
Don't forget to leve a star if you like it!
<p align="center">
  <img src="https://github.com/schneidermanuel/TwitchLeecher-Dx/assets/57318033/35f55b28-9970-4c95-89fb-01fac4ad5711" />
</p>

# Twitch Leecher-DX
If you are looking for an extremely fast and easy to use Twitch VOD downloader, this is your tool!

## Linux Version
There is an [aur package](https://aur.archlinux.org/packages/twitchleecher-dx) available!

## Windows Version
Download the executable [here](https://github.com/schneidermanuel/TwitchLeecher-Dx/releases/download/v3.9.0/twitchleecher-dx-3.9.0-setup.exe)
- Requires [.net 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

# Legal Disclaimer
1. Purpose of the Software
This software (Twitch Leecher DX) is provided as a technical tool for personal use only. It is intended to facilitate the download of video content from Twitch for offline viewing, archiving, or personal backup purposes (e.g., for creators to save their own broadcasts).

2. User Responsibility
By using this software, you agree to comply with all applicable copyright laws and the Twitch Terms of Service (ToS). The developer of this software does not encourage, support, or induce any form of copyright infringement or unauthorized distribution of content. You are solely responsible for the legality of the content you download.

3. Access to Restricted Content
This tool does not bypass or circumvent any digital rights management (DRM) or payment barriers.

Public Content: The tool only accesses publicly available video fragments.

Subscription-Only Content: Access to subscriber-only VODs requires a valid user-authorized token. This software only facilitates the download if the user already possesses the legal right to access said content through their own active subscription.

4. No Affiliation
This project is an independent open-source "fork" and is not affiliated with, authorized, maintained, sponsored, or endorsed by Twitch Interactive, Inc., Amazon.com, Inc., or any of their affiliates or subsidiaries. "Twitch" is a registered trademark of Twitch Interactive, Inc.

5. Limitation of Liability
The software is provided "as is", without warranty of any kind. The developer shall not be liable for any claims, damages, or other liabilities, including account suspensions or legal actions resulting from the use of this tool.

## What is the difference compared to other VOD downloaders?
Nearly all of the well known VOD downloaders execute the download process via FFMPEG's integrated download capabilities. However, this is extremely slow. The download speed rarely exceeds 1.5Mbit even if the internet connection is 100 times faster. Twitch Leecher-DX does not use FFMPEG for download tasks at all. It downloads thousands of small video chunks in parallel while using all of the available bandwidth of your internet connection. As soon as all video chunks are downloaded, FFMPEG is only used to merge those chunks together in order to create a single video file again.

## Features
- Very easy to use, no manual needed
- Intuitive and stylish GUI
- Up to 20 times faster download speed compared to direct download with FFMPEG
- Browse your past broadcasts, uploads and highlights within the application
- Search channels, VOD urls and VOD IDs
- Sub-Only video download support
- Audio-Only download support
- Time Selection for VOD downloads
- Queue multiple downloads
- Specify default search parameters
- Specify default download quality
- Specify default download folders
- Specify a filename template with wildcards for your downloads
- Developed by an experienced Software Engineer
- Free and Open Source
- Actively maintained
- Modern .net 6.0 Framework
- Save favorite search parameter as preset
- Download VOD's that might are unavailable on twitch due to missing chunk data
  
## Support & Issues

If you have any problem or wish a feature for Twitch Leecher-DX, feel free to open an issue on this page!
> **IMPORTANT:** Help me be efficient, please! I am developing Twitch Leecher in my free time for no money. Contribute to the project by posting complete, structured and helpful issues which I can reproduce quickly without asking for missing information. When creating a new issue please follow the below checklist:

- Windows Insider Builds are NOT supported!
- Upgrade to the latest version of Twitch Leecher DX and .net 6.0 runtime
- Provide the version of Twitch Leecher-DX you are using
- Provide as much information about the VOD as possible (Url, Channel, ID)
- Provide information about your operating system (e.g. Windows 10 64 Bit)
- Try to describe the problem as detailed as possible, I cannot read your mind ;)
- Is there any additional information about the issue that might be interesting for me? Write it down!
- When you have a problem with a download, provide the download log created by Twitch Leecher (see screenshot below)
![2023-09-17 10 19 42](https://github.com/schneidermanuel/TwitchLeecher-Dx/assets/57318033/1472d989-4df9-44c6-9ccb-4519345d2234)

# Donate

If you wan't to support me and the development of this project, please consider donating! [Donate](https://www.tipeeestream.com/brainyxs/donation)

## LICENSE
[MIT License](https://github.com/schneidermanuel/TwitchLeecher-DX/blob/master/LICENSE)
