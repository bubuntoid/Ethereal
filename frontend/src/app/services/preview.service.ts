import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PreviewService {
  url: string = `${environment.apiUrl}/api/preview`;

  constructor(private http: HttpClient) { }

  getDescriptionPreview(youtubeVideoUrl:string){
    return this.http.post(`${this.url}/description`, {'url': youtubeVideoUrl}, {responseType: 'text'});
  }

  getChaptersPreview(youtubeVideoUrl:string, description:string){
    return this.http.post<any[]>(`${this.url}/chapters`, {'url': youtubeVideoUrl, 'description': description}, {responseType: 'json'});
  }
}
