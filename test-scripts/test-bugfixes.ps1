# Test script for the "bugfixes" branch.
# Builds the app, measures idle CPU automatically, then walks you through each manual check.
# Windows:  powershell -ExecutionPolicy Bypass -File test-scripts\test-bugfixes.ps1
# Linux:    pwsh test-scripts/test-bugfixes.ps1   (needs ffmpeg on PATH)
param([switch]$SkipBuild)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$proj = Join-Path $root (Join-Path 'TwitchLeecher' (Join-Path 'TwitchLeecher' 'TwitchLeecher.csproj'))
$out = Join-Path (Split-Path -Parent $proj) (Join-Path 'bin' (Join-Path 'Release' 'net10.0'))
$dll = Join-Path $out 'TwitchLeecher.dll'
$results = [ordered]@{}

function Start-App { Start-Process dotnet -ArgumentList "`"$dll`"" -PassThru }

function Wait-AppClosed($p) {
    Write-Host 'Close Twitch Leecher to continue...' -ForegroundColor Yellow
    $p.WaitForExit()
}

function Ask($id, $text) {
    Write-Host ''
    Write-Host "[$id]" -ForegroundColor Cyan
    Write-Host $text
    do { $a = Read-Host 'Result? (y = pass, n = fail, s = skip)' } until ($a -match '^[yns]$')
    $results[$id] = @{ y = 'PASS'; n = 'FAIL'; s = 'SKIP' }[$a]
}

function Set-ClipboardSafe($text) {
    try { Set-Clipboard -Value $text; return $true } catch { return $false }
}

# --- Build ---------------------------------------------------------------
if (-not $SkipBuild) {
    dotnet build $proj -c Release
    if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
}
if (-not (Test-Path $dll)) { throw "App not found at $dll" }

# --- 1. Idle CPU (automatic) ----------------------------------------------
Write-Host ''
Write-Host '[Idle CPU] Starting the app and measuring CPU for 30s. Leave it alone on the start screen.' -ForegroundColor Cyan
$p = Start-App
Start-Sleep -Seconds 15
$p.Refresh(); $cpu0 = $p.TotalProcessorTime
Start-Sleep -Seconds 30
$p.Refresh(); $cpu1 = $p.TotalProcessorTime
$pct = [math]::Round(($cpu1 - $cpu0).TotalSeconds / 30 * 100, 2)
Write-Host "Idle CPU: $pct% of one core"
# shortcut: fixed 1% threshold; compare against a master build if the number looks borderline
$results['Idle CPU < 1% of a core'] = $(if ($pct -lt 1) { 'PASS' } else { "FAIL ($pct%)" })

# --- 2. Window position (#87) ---------------------------------------------
Write-Host ''
Write-Host '[#87] Move the window somewhere obvious and resize it, then close it.' -ForegroundColor Cyan
Wait-AppClosed $p
$p = Start-App
Ask '#87 window position remembered' 'The app has restarted. Is it at the same position and size you left it?'

# --- 3. Settings text (#103) ----------------------------------------------
Ask '#103 tooltips wrap' 'Open Settings and hover several (?) help icons. Is every tooltip text shown in full (wrapped, nothing cut off)?'
Ask '#103 checkbox label' 'In Settings > Download, make the window narrow. Does "Disable conversion to mp4/mkv (also disables cropping)" wrap instead of being cut off?'
Ask '#103 crop time boxes' 'Search any channel, click Download on a VOD. In "Crop Video", tick Start/End. Do the hour/min/sec boxes show two digits (e.g. 01:23:45)? Then cancel the dialog.'

# --- 4. Pasted channel name (#56) -----------------------------------------
$channel = Read-Host 'Channel name to test search with [shroud]'
if (-not $channel) { $channel = 'shroud' }
if (Set-ClipboardSafe "  $channel`r`n") {
    Write-Host "Copied '  $channel<newline>' (with stray whitespace) to the clipboard."
} else {
    Write-Host "Could not set the clipboard. Copy the channel name from a web page by hand, including a trailing space."
}
Ask '#56 pasted channel search' 'Go to Search, clear the channel box, PASTE (Ctrl+V) and press Enter. Does the search run (no red border)?'

# --- 5. Normal download + ffmpeg -nostdin (#92) ---------------------------
Ask '#92 download + conversion' @'
Download a VOD with Crop: Start 00:00:00, End 00:02:00 (keeps it short), conversion enabled.
Expected: status reaches Done, the .mp4 plays, and the log (Downloads > log icon) has "-nostdin" in the ffmpeg command line.
'@

# --- 6. Max connections = 1 + open folder with spaces ---------------------
$spaced = Join-Path ([IO.Path]::GetTempPath()) 'TL test folder with spaces'
New-Item -ItemType Directory -Force -Path $spaced | Out-Null
[void](Set-ClipboardSafe $spaced)
Write-Host ''
Write-Host "Test folder (copied to clipboard): $spaced"
Ask 'Max connections = 1 / open folder' @"
In Settings set Max connections to 1 and the Download folder to:
  $spaced
Download the same short cropped VOD again.
Expected: the download runs (before, 1 connection crashed it immediately), and the folder icon on the finished download opens exactly that folder.
Afterwards set Max connections back to 10 and restore your download folder.
"@

# --- 7. Retries give up instead of looping forever ------------------------
Ask 'Retries stop when offline (optional)' @'
Start a long (uncropped) download, then disconnect your network once it is downloading.
Expected: after about 15-20s of retries the download shows Error, and Task Manager CPU stays low. Reconnect, then use Retry.
(Before: it retried forever with no delay.) Answer s to skip.
'@

# --- 8. Update dialog download button -------------------------------------
Ask 'Update dialog Download button (optional)' 'Only testable when an update notice appears: clicking Download opens the release page in your browser. Answer s if no update notice shows.'

# --- Summary ---------------------------------------------------------------
if (-not $p.HasExited) { Wait-AppClosed $p }
$report = ($results.GetEnumerator() | ForEach-Object { '{0,-40} {1}' -f $_.Key, $_.Value }) -join [Environment]::NewLine
$file = Join-Path ([IO.Path]::GetTempPath()) "twitchleecher-bugfixes-results-$(Get-Date -Format yyyyMMdd-HHmm).txt"
Set-Content -Path $file -Value $report
Write-Host ''
Write-Host '===== Results =====' -ForegroundColor Green
Write-Host $report
Write-Host ''
Write-Host "Saved to $file"
