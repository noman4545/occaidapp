import { EventEmitter, Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { SMS } from '../models/sms.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  notificationReceived = new EventEmitter<Notification>();
  smsReceived = new EventEmitter<SMS>();
  connectionEstablished = new EventEmitter<Boolean>(); 
  connectionIsEstablished = false;  
  private hubConnection: HubConnection | undefined; 

  constructor(){
    this.createConnection();
    this.registerOnServerEvents();
    this.startConnection();
  }

  private createConnection() {
    this.hubConnection = new HubConnectionBuilder()
    .withUrl('NotificationHub')
    .configureLogging(signalR.LogLevel.Information)
    .build(); 
  }  
    
  private startConnection(): void {
    if(this.hubConnection == undefined) return;  
    this.hubConnection
    .start()
    .then(() => { 
        this.connectionIsEstablished = true;  
        this.connectionEstablished.emit(true);
    })
    .catch(err => {  
        this.restartConnection();  
    });  
  }

  private restartConnection(): void {
    let interval = setInterval(() => {
        if(this.hubConnection == undefined) return; 
        this.hubConnection
        .start()
        .then(() => { 
            this.connectionIsEstablished = true;  
            this.connectionEstablished.emit(true);
            clearInterval(interval); 
        }).catch(err => {  
        });
    }, 5000);
  }

  private registerOnServerEvents(): void {
    if(this.hubConnection == undefined) return; 
    this.hubConnection.on('NotificationReceived', (data: Notification) => {
        this.notificationReceived.emit(data);
    });
    this.hubConnection.on('SMSReceived', (data: SMS) => {
      this.smsReceived.emit(data);
    });
    
    this.hubConnection.onclose(()=>{
        this.connectionIsEstablished = false;
        this.connectionEstablished.emit(false);
        this.startConnection();
    });
  }

  sendNotificationToHub(notification: any) {
    if(this.hubConnection == undefined) return; 
    if(this.hubConnection.state == signalR.HubConnectionState.Connected)
        this.hubConnection.invoke('SendNotification', notification);
  }

  sendSMSToHub(sms: SMS) {
    if(this.hubConnection == undefined) return; 
    if(this.hubConnection.state == signalR.HubConnectionState.Connected)
        this.hubConnection.invoke('SendSMS', sms);
  }
}
