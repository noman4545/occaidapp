import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { GetLogsResponse } from '../models/getLogsResponse.model';
import { Log } from '../models/log.model';
import { Notification } from '../models/notification.model';
import { TMCSEmergency } from '../models/tmcsEmergency.model';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root'
})
export class LoggingService {
  BaseURL: string = 'api/Logs/';

  constructor(private http: HttpClient, 
    private toater: ToastrService, 
    private notify: NotificationService) {}

  log(screen: string, logMessage: string): void {
    this.http.post<Notification>(this.BaseURL + 'Log', {
      message: logMessage,
      screen: screen
    }).pipe(take(1)).subscribe(res => {
      this.notify.sendNotificationToHub(res);
    },
    err => {
      this.toater.error("Could not save log.");
    });
  }

  loadLogs(): Observable<Notification[]>{
    return this.http.get<Notification[]>(this.BaseURL + 'GetLogs').pipe(take(1));
  }

  markRead(): void{
    this.http.get(this.BaseURL + 'MarkAsRead').subscribe(res => {},
      err => {
        this.toater.error("Could not mark notifications as read.");
      });
  }

  loadSpecialLogs(page: number, takeNo: number, search: string, role: string, screen: string): Observable<GetLogsResponse>{
    return this.http.get<GetLogsResponse>(this.BaseURL + `GetSpecialLogs?page=${page}&take=${takeNo}&search=${search}&role=${role}&screen=${screen}`).pipe(take(1));
  }
}
