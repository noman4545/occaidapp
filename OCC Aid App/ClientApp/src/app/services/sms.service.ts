import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { take } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { SMS } from '../models/sms.model';
import { GetSMSResponse } from '../models/getSMSResponse.mode';

@Injectable({
  providedIn: 'root'
})
export class SMSService {
  BaseURL: string = 'api/SMS/';

  constructor(private http: HttpClient) {

  }

  getSMSs(page: number, takeNo: number, search: string, deleted: boolean): Observable<GetSMSResponse> {
    return this.http.get<GetSMSResponse>(this.BaseURL + `GetSMSs?page=${page}&take=${takeNo}&search=${search}&deleted=${deleted}`).pipe(take(1));
  }

  saveSMS(sms: SMS) {
    return this.http.post(this.BaseURL + `SaveSMS`, sms).pipe(take(1));
  }

  updateSMS(sms: SMS) {
    return this.http.post(this.BaseURL + `UpdateSMS`, sms).pipe(take(1));
  }

  deleteSMS(id: number) {
    return this.http.get(this.BaseURL + `DeleteSMS?id=${id}`).pipe(take(1));
  }

  recoverSMS(id: number) {
    return this.http.get(this.BaseURL + `RecoverSMS?id=${id}`).pipe(take(1));
  }

  getAllSMS(): Observable<SMS[]>{
    return this.http.get<SMS[]>(this.BaseURL + `GetAllSMS`).pipe(take(1));
  }

  getAllArchievedSMS(): Observable<SMS[]>{
    return this.http.get<SMS[]>(this.BaseURL + `GetArchieveSMS`).pipe(take(1));
  }

  saveArchievedSMS(sms: SMS) {
    return this.http.post(this.BaseURL + `SaveArchieveSMS`, sms).pipe(take(1));
  }

  markCompleteArchieveSMS(id: number){
    return this.http.get(this.BaseURL + `MarkArchieveSMSComplete?id=${id}`).pipe(take(1));
  }
}
