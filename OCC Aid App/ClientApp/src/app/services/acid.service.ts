import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { ACID } from '../models/acid.model';
import { GetACIDResponse } from '../models/getACIDResponse.model';

@Injectable({
  providedIn: 'root'
})
export class ACIDService {
  BaseURL: string = 'api/Acid/';

  constructor(private http: HttpClient) {

  }

  getAcids(page: number, takeNo: number, search: string, deleted: boolean): Observable<GetACIDResponse> {
    return this.http.get<GetACIDResponse>(this.BaseURL + `GetAcids?page=${page}&take=${takeNo}&search=${search}&deleted=${deleted}`).pipe(take(1));
  }

  saveAcid(acidCode: ACID) {
    return this.http.post(this.BaseURL + `SaveAcid`, acidCode).pipe(take(1));
  }

  updateAcid(acidCode: ACID) {
    return this.http.post(this.BaseURL + `UpdateAcid`, acidCode).pipe(take(1));
  }

  deleteAcid(acidId: number) {
    return this.http.get(this.BaseURL + `DeleteAcid?id=${acidId}`).pipe(take(1));
  }

  recoverAcid(acidId: number) {
    return this.http.get(this.BaseURL + `RecoverAcid?id=${acidId}`).pipe(take(1));
  }

  getACIDDetailsByATSNumber(atsName: string): Observable<ACID> {
    return this.http.get<ACID>(this.BaseURL + `GetACIDDetailsByATSNumber?atsname=${atsName}`).pipe(take(1));
  }

  getTerritories(): Observable<string[]> {
    return this.http.get<string[]>(this.BaseURL + `GetTerritories`).pipe(take(1));
  }

  getAcidsByTerritory(territory: string): Observable<string[]> {
    return this.http.get<string[]>(this.BaseURL + `GetAcidsByTerritory?territory=${territory}`).pipe(take(1));
  }
}
