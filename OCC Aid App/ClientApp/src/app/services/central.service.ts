import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CentralService {
  BaseURL: string = 'api/Central/';

  constructor(private http: HttpClient) {}

  getRoles(): Observable<string[]>{
    return this.http.get<string[]>(this.BaseURL + 'GetRoles').pipe(take(1));
  }
}
