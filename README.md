# YOVPS
Youtube One Video Playlist Splitter

## Backend
test server endpoint: http://81.177.135.200:322 </br>
swagger: http://81.177.135.200:322/swagger/index.html

```
$ chmod +x scripts/run.sh
$ ./scripts/run.sh
```

#### Todo / Goals
v1 / mvp:
- DownloadMp3Async optimization 
  - Fork https://github.com/Tyrrrz/YoutubeExplode and reimplement **client.Videos.Streams.GetAsync(info)**<br>
  add param with specific stream position
- YOVPS.WebAPI auto tests
- VideoSplitterService unit tests
- StringExtensions unit tests
- TimeSpanExtensions unit tests
- Add logs

v2:
- db usage with processing status 
  - MariaDb / MySQL / PostgreSQL
- total optimization
- thumbnail for each chapter
- native ffmpeg usage / own client (?)
- choose bitrate quality
