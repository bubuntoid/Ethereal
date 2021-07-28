# YOVPS
Youtube One Video Playlist Splitter

## Backend
* .NET 5.0
* ASP.NET Core 5.6.3
* NLog 4.5.11
* FFMPEG 4.2.4-1ubuntu0.1
* FluentValidation 10.3.0
* Nunit 3.13.1
  * FluentAssertions 5.10.3

test server endpoint: http://81.177.135.200:322 </br>
swagger: http://81.177.135.200:322/swagger/index.html

```
$ chmod +x scripts/run.sh
$ ./scripts/run.sh
```

## Frontend
* PUG
* SCSS

#### Todo / Goals
v1 / mvp:
- [ ] DownloadMp3Async optimization 
  - Fork https://github.com/Tyrrrz/YoutubeExplode and reimplement **client.Videos.Streams.GetAsync(info)**<br>
  add param with specific stream position
- [ ] YOVPS.WebAPI auto tests
- [ ] VideoSplitterService unit tests
- [ ] StringExtensions unit tests
- [ ] TimeSpanExtensions unit tests
- [X] Add logs
- [X] dry VideoSplitterService

v2:
- [ ] db usage with processing status 
  - MariaDb / MySQL / PostgreSQL
- [X] total optimization
- [X] thumbnail for each chapter
- [X] native ffmpeg usage / own client (?)
- [X] choose bitrate quality
- [X] fluent validator
- [ ] github ci/cd
