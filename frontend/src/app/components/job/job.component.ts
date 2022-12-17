import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { MainService } from 'src/app/services/main.service';
import { SignalRService } from 'src/app/services/signal-r.service';

@Component({
  selector: 'app-job',
  templateUrl: './job.component.html',
  styleUrls: ['./job.component.css']
})
export class JobComponent implements OnInit {

  private routeSub: Subscription | undefined;
  constructor(private route: ActivatedRoute, private mainService : MainService,
    private signalR: SignalRService, private title : Title) { }

  error: string | undefined;

  job: any;
  currentStatus: any;
  currentLog: any;

  ngOnInit(): void {
    this.routeSub = this.route.params.subscribe(params => {
      var id = params['id'];
      this.mainService.getJob(id).subscribe(jobResponse =>{
        this.job = jobResponse;
        this.currentStatus = this.job.status;
        this.currentLog = this.job.lastLogMessage;
        this.title.setTitle(this.currentLog);

        if (this.isFailed()){
          this.error = this.job.lastLogMessage;
          this.title.setTitle(this.job.lastLogMessage);
        }
        
        if (this.isProcessing()){
          this.signalR.startConnection(id);
          this.signalR.addReceiveLogListener(data =>{
            console.log(data);
            this.currentStatus = data.status;
            this.currentLog = data.log;
            if (this.isFailed()){
              this.error = this.currentLog;
            }
            this.title.setTitle(this.currentLog);
          })
        }

        console.log(this.job);
      })
    });
  }
  
  isLoadingChapters: boolean = false;
  onIsLoadingChaptersChanged(event: any){
    this.isLoadingChapters = event;
  }

  openUrl(event:any){
    window.open(event);
  }

  isProcessing(){
    return this.currentStatus == 'Created' || this.currentStatus == 'Processing'
  }

  isSucceed(){
    return this.currentStatus == 'Succeed';
  }

  isFailed(){
    return this.currentStatus == 'Failed' || this.currentStatus == 'Expired';
  }
}
