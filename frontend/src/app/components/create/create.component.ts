import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { MainService } from 'src/app/services/main.service';
import { PreviewService } from 'src/app/services/preview.service';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {

  constructor(private previewService: PreviewService, private router: Router,
    private mainService: MainService, private title : Title) { }

  ngOnInit(): void {
  }

  error: any;
  hasError() { 
    return this.error != null || this.error != undefined;
  }

  chaptersError: any;
  hasChaptersError() {
    return this.chaptersError != null || this.chaptersError != undefined;
  }

  isCorrectUrl: boolean = false;
  url: string = '';
  isLoadingDescription: boolean = true;
  onUrlChanged(event: any){
    this.isCorrectUrl = this.isValidYoutubeUrl(event);
    if (this.isCorrectUrl == false){
      this.error = 'Invalid youtube url';
      return;
    } else{
      this.error = null;
    }

    this.url = event;
    this.isLoadingDescription = true;
    this.previewService.getDescriptionPreview(this.url).subscribe(descriptionResponse => {
      this.onDescriptionChanged(descriptionResponse);
      this.isLoadingDescription = false;
    }, error => {
      if (error.status == 0){
        console.log(error);
        this.error = 'Server not responding (CONNECTION REFUSED)';
        this.isLoadingDescription = false;
      } 
      else {
        var jsonError = JSON.parse(error.error);
        this.error = jsonError.errorMessage;
        this.isLoadingDescription = false;
      }
    });
  }

  useCustomDescription: boolean = false;
  onUseCustomDescriptionChanged(event: any){
    this.useCustomDescription = event;
  }

  description: string = "";
  onDescriptionChanged(event: any){
    this.chaptersError = null;
    this.description = event;
    this.onIsLoadingChaptersChanged(true);
    this.previewService.getChaptersPreview(this.url, this.description).subscribe(chaptersResponse => {
      this.previewChapters = chaptersResponse;
      this.onIsLoadingChaptersChanged(false);
    }, error =>{
      console.log(error);
      this.chaptersError = error.error.errorMessage;
      this.onIsLoadingChaptersChanged(false);
    })
  }

  previewChapters: any[] = [];
  isLoadingChapters: boolean = true;
  onIsLoadingChaptersChanged(event: any){
    this.isLoadingChapters = event;
  }

  processing: boolean = false;
  onParseChaptersButtonClick() {
    this.processing = true;
    this.mainService.initializeJob(this.url, this.description).subscribe(createdJobResponse => {
      this.title.setTitle('Processing...');
      this.router.navigateByUrl(`job/${createdJobResponse.id}`);
    }, error => {
      error.error.errorMessage;
      this.processing = false;
    });
  }

  onConvertVideoToMp3ButtonClick() {
    this.processing = true;
    this.mainService.convertVideoToMp3(this.url).subscribe(createdJobResponse => {
      this.title.setTitle('Processing...');
      this.router.navigateByUrl(`job/${createdJobResponse.id}`);
    }, error =>{
      this.error = error.error.errorMessage;
      this.processing = false;
    });
  }

  isValidYoutubeUrl(url : string) {
    if (url != undefined || url != '') {
      var regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=|\?v=)([^#\&\?]*).*/;
      var match = url.match(regExp);
        if (match && match[2].length == 11) {
          return true; 
        } else {
          return false; 
        }
      }
      return false;
  }
}
