pkgname=twitchleecher-dx
_pkgname=TwitchLeecher-Dx
pkgdesc="A simple download tool for Twitch streams"
pkgver=3.4.0
_pkgver=3.4.0-beta3
pkgrel=1
arch=('x86_64')
url='https://github.com/schneidermanuel/TwitchLeecher-Dx'
license=("MIT")
depends=("icu" "ffmpeg")
makedepends=("dotnet-sdk")
options=("staticlibs")
source=("$url/archive/refs/tags/v$_pkgver.tar.gz")
sha256sums=('af426f5d57720aaa8bdb6c0b2f69828bdb5b861f01ddf92f4cbe340d3dde1bfb')

build() {
  cd "$_pkgname-$_pkgver/TwitchLeecher/TwitchLeecher"
  MSBUILDDISABLENODEREUSE=1 dotnet publish \
    --configuration Release \
    --self-contained true \
    --runtime linux-x64 \
    -p:PublishTrimmed=true \
    --output ../../$pkgname \
    ./TwitchLeecher.csproj
}

package() {
  cd "$_pkgname-$_pkgver"

  install -d $pkgdir/usr/{bin,lib}
  install -d $pkgdir/usr/share/{pixmaps,applications}
  cp -r $pkgname "$pkgdir/usr/lib/"
  ln -s "$pkgdir/usr/lib/$pkgname/TwitchLeecher" "$pkgdir/usr/bin/$pkgname"
  cp TwitchLeecher/Resources/Images/TL_Icon.png "$pkgdir/usr/share/pixmaps/twitchleecher-dx.png"
  cp twitchleecher-dx.desktop "$pkgdir/usr/share/applications/twitchleecher-dx.desktop"
}
