# YOVPS
**Youtube One Video Playlist Splitter** - Solution that provide functionality for splitting huge youtube one-video playlists to mp3 files using just url and (optionally) time code description. For most of processing operations it uses [FFMPEG](https://www.ffmpeg.org/) library.

## Navigation
[YOVPS](https://github.com/bubuntoid/YOVPS/) - Core and WebAPI. **(we are here)**<br>
[YOVPS.Cli](https://github.com/bubuntoid/YOVPS.CLI/) - Cross platform command line interface application.<br>
[YOVPS.Web](#) - Web client written in native JS.<br>
[YOVPS.Desktop](#) - Desktop client written in electron.<br>

![](./assets/Screenshot_1.png)
![](./assets/Screenshot_2.png)
![](./assets/Screenshot_3.png)

## Syntax
There is no strict rule, if youtube recognize chapters from your video then we can parse it without any other additional actions.
How do youtube chapters work: https://www.tubics.com/blog/youtube-chapters/

Correct (https://www.youtube.com/watch?v=UZ7oOhhPEWU&t=3448s&ab_channel=kneon):
```
1. (0:00) KOPI / KOBASOLO – Swayed in Spring Reiniscence
2. (04:11) Sakanaction – Shin Takarajima
3. (09:13) Memai Siren - Nisemono no Utage
```
Correct as well (https://www.youtube.com/watch?v=CU_ruPKWJpc&t=82s&ab_channel=Asthenic):
```
0:00 Lazerhawk - So Far Away 
https://soundcloud.com/lazerhawk/so-f...
https://lazerhawk.bandcamp.com/

4:38  Zane Alexander - Elementary
https://soundcloud.com/thezanealexand...
https://zanealexander.bandcamp.com/al...

8:55 oDDling - Reverie
https://soundcloud.com/oddlingmusic/r...
https://oddling.bandcamp.com/releases
```
If your video has no timecodes in description, but you got one (often situation when people leave time codes in comments) or writted it by yourself, then you can explicit custom description with timecodes with youtube like syntax.
Ensure that you have new line after each time code.

## Dependencies
* .NET 5.0
* ASP.NET Core 5.6.3
* NLog 4.5.11
* FFMPEG 4.2.4-1ubuntu0.1
* FluentValidation 10.3.0
* Autofac 6.2.0
* AutoMapper 10.1.1
* Nunit 3.13.1
* FluentAssertions 5.10.3

## Backend
test server endpoint: http://81.177.135.200:322 </br>
swagger: http://81.177.135.200:322/swagger/index.html

```
$ chmod +x scripts/run.sh
$ ./scripts/run.sh
```

### **FFMPEG**
Download and install fmpeg (https://ffmpeg.org/)<br>
Specify path to FFMPEG executables in **appsetings.json**:

**Linux**:
```json
"FFMPEG": {
    "TempPath" : "{current}/temp",
    "ExecutablesPath" : "ffmpeg"
}
```
**Windows**:
```json
"FFMPEG": {
    "TempPath" : "{current}/temp",
    "ExecutablesPath" : "C://Path//To//ffmpeg.exe"
}
```
