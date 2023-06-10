using System.Linq;
using CockyGrabber;
using CockyGrabber.Grabbers;
using TwitchLeecher.Services.Interfaces;

namespace TwitchLeecher.Services.Services
{
    internal class CookieService : ICookieService
    {
        private readonly IRuntimeDataService _runtimeDataService;

        public CookieService(IRuntimeDataService runtimeDataService)
        {
            _runtimeDataService = runtimeDataService;
        }

        public bool GrabTwitchSessionToken()
        {
            var grabbers = new BlinkGrabber[]
            {
                new BraveGrabber(),
                new ChromeGrabber(),
                new OperaGrabber(),
                new OperaGxGrabber(),
                new EdgeGrabber()
            };
            var geckoGrabbers = new[]
            {
                new FirefoxGrabber()
            };

            foreach (var grabber in grabbers)
            {
                var token = GetCookie(grabber);
                if (!string.IsNullOrEmpty(token))
                {
                    _runtimeDataService.RuntimeData.AuthInfo.AccessTokenSubOnly = token;
                    return true;
                }
            }

            foreach (var grabber in geckoGrabbers)
            {
                var token = GetCookie(grabber);
                if (!string.IsNullOrEmpty(token))
                {
                    _runtimeDataService.RuntimeData.AuthInfo.AccessTokenSubOnly = token;
                    return true;
                }
            }


            return false;
        }

        public string GetCookie(BlinkGrabber grabber)
        {
            return grabber.GetCookiesBy(Blink.Cookie.Header.host_key, ".twitch.tv")
                .Where(cookie => cookie.Name == "auth-token").Select(cookie => cookie.DecryptedValue)
                .FirstOrDefault();
        }

        public string GetCookie(GeckoGrabber grabber)
        {
            return grabber.GetCookiesBy(Gecko.Cookie.Header.host, ".twitch.tv")
                .Where(cookie => cookie.Name == "auth-token").Select(cookie => cookie.Value)
                .FirstOrDefault();
        }
    }
}