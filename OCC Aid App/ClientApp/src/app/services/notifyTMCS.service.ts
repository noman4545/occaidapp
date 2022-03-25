import { EventEmitter, Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { TMCSEmergency } from '../models/tmcsEmergency.model';

@Injectable({
  providedIn: 'root'
})
export class NotifyTMCSService {
  tmcsReceived = new EventEmitter<TMCSEmergency>();
  markCompleteTMCSReceived = new EventEmitter<number>();
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
    .withUrl('TMCSHub')
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
    this.hubConnection.on('TMCSReceived', (data: TMCSEmergency) => {
        this.tmcsReceived.emit(data);
    });
    this.hubConnection.on('MarkCompleteTMCSReceived', (data: number) => {
      this.markCompleteTMCSReceived.emit(data);
    });
    this.hubConnection.onclose(()=>{
        this.connectionIsEstablished = false;
        this.connectionEstablished.emit(false);
        this.startConnection();
    });
  }

  sendTMCSToHub(tmcs: any) {
    if(this.hubConnection == undefined) return; 
    if(this.hubConnection.state == signalR.HubConnectionState.Connected)
        this.hubConnection.invoke('SendTMCS', tmcs);
  }

  markCompleteTMCSToHub(id: any) {
    if(this.hubConnection == undefined) return; 
    if(this.hubConnection.state == signalR.HubConnectionState.Connected)
        this.hubConnection.invoke('MarkCompleteTMCS', id);
  }
}
