import { Router } from '@angular/router';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { Notification } from 'src/app/models/notification.model';
import { LoggingService } from 'src/app/services/logging.service';
import { NotificationService } from 'src/app/services/notification.service';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent implements OnDestroy {
  notifications: Notification[] = [];
  subscriptions: Subscription[] = [];
  newNotification:boolean = false;
  open:boolean = false;
  
  constructor(private service: NotificationService, 
    private lService: LoggingService,
    private toaster: ToastrService,
    private router: Router) {
      this.subscribeToNotificationReciever();
      this.loadNotifications();
  }
  
  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  subscribeToNotificationReciever(){
    let sub = this.service.notificationReceived.subscribe((res: Notification) => {
      this.notifications.unshift(res);
      this.newNotification = true;
    });
    this.subscriptions.push(sub);
  }

  clearNotification(index: number){
    this.notifications.splice(index, 1);
  }

  loadNotifications(){
    this.lService.loadLogs().subscribe(res => {
      this.notifications = res;
      if(this.notifications.find(f => f.read == false))
        this.newNotification = true;
    },
    err => {
      this.toaster.error("Could not load notifications");
    });
  }

  markAsRead(){
    this.open = !this.open;
    if(!this.open){
      this.notifications.filter(f => !f.read).forEach(f => {
        f.read = true;
      });
      this.newNotification = false;
    }
    this.lService.markRead();
  }

  logout(){
    localStorage.removeItem("ASPNetAuthToken");
    this.router.navigate(['auth']);
  }

}
