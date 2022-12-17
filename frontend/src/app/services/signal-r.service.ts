import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr'
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection!: signalR.HubConnection;

  public startConnection = (jobId: string) => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/logger?jobId=${jobId}`)
      .build()

    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err))
  }

  public addReceiveLogListener = (onReceiveLog: (data: any) => void) => {
    this.hubConnection.on('onReceiveLog', (data) =>{
      onReceiveLog(data);
    })
  }
}