import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { take } from 'rxjs/operators';
import { IOSCode } from '../models/iosCode.model';
import { Observable } from 'rxjs';
import { GetIOSCodesResponse } from '../models/getIOSCodesReponse.model';

@Injectable({
  providedIn: 'root'
})
export class IOSCodesService {
  BaseURL: string = 'api/IOSCode/';

  constructor(private http: HttpClient) {

  }

  getIOSCodes(page: number, takeNo: number, search: string, deleted: boolean): Observable<GetIOSCodesResponse> {
    return this.http.get<GetIOSCodesResponse>(this.BaseURL + `GetIOSCodes?page=${page}&take=${takeNo}&search=${search}&deleted=${deleted}`).pipe(take(1));
  }

  saveIOSCode(iosCode: IOSCode) {
    return this.http.post(this.BaseURL + `SaveIOSCode`, iosCode).pipe(take(1));
  }

  updateIOSCode(iosCode: IOSCode) {
    return this.http.post(this.BaseURL + `UpdateIOSCode`, iosCode).pipe(take(1));
  }

  deleteIOSCodes(codeId: number) {
    return this.http.get(this.BaseURL + `DeleteIOSCode?id=${codeId}`).pipe(take(1));
  }

  recoverIOSCodes(codeId: number) {
    return this.http.get(this.BaseURL + `RecoverIOSCode?id=${codeId}`).pipe(take(1));
  }

  getIOSCodeDetailsByIOSNumber(iosNumber: string): Observable<IOSCode> {
    return this.http.get<IOSCode>(this.BaseURL + `GetIOSCodeDetailsByIOSNumber?iosnumber=${iosNumber}`).pipe(take(1));
  }
}
