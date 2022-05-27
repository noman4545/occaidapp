import { GetTMCSResponse } from './../models/getTMCSResponse.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Block } from '../models/block.model';
import { GetZoneResponse } from '../models/getZoneResponse.model';
import { TMCSEmergency } from '../models/tmcsEmergency.model';
import { Zone } from '../models/zone.model';
import { ZoneLatest } from '../models/zone-latest.model';
import { GetZoneResponseLatest } from '../models/getZoneResponseLatest.model';
import { BlockLatest } from '../models/block-latest.model';
import { ZoneResponseLatest } from '../models/zone-response-latest.model';
import { TMCSEmergencyLatest } from '../models/tmcsEmergencyLatest.model';

@Injectable({
  providedIn: 'root'
})
export class TMCSService {
  BaseURL: string = 'api/TMCS/';

  constructor(private http: HttpClient) { }

  getZones(page: number, takeNo: number, search: string, deleted: boolean): Observable<GetZoneResponse> {
    return this.http.get<GetZoneResponse>(this.BaseURL + `GetZones?page=${page}&take=${takeNo}&search=${search}&deleted=${deleted}`).pipe(take(1));
  }

  getZonesV1(page: number, takeNo: number, search: string, deleted: boolean): Observable<GetZoneResponseLatest> {
    return this.http.get<GetZoneResponseLatest>(this.BaseURL + `GetZonesV1?page=${page}&take=${takeNo}&search=${search}&deleted=${deleted}`).pipe(take(1));
  }

  saveZone(zone: Zone) {
    return this.http.post(this.BaseURL + `SaveZone`, zone).pipe(take(1));
  }

  updateZone(zone: Zone) {
    return this.http.post(this.BaseURL + `UpdateZone`, zone).pipe(take(1));
  }

  saveZoneV1(zone: ZoneLatest) {
    return this.http.post(this.BaseURL + `SaveZoneV1`, zone).pipe(take(1));
  }

  updateZoneV1(zone: ZoneLatest) {
    return this.http.post(this.BaseURL + `UpdateZoneV1`, zone).pipe(take(1));
  }

  deleteZone(zoneId: number) {
    return this.http.get(this.BaseURL + `DeleteZone?id=${zoneId}`).pipe(take(1));
  }

  deleteZoneV1(zoneId: number) {
    return this.http.get(this.BaseURL + `DeleteZoneV1?id=${zoneId}`).pipe(take(1));
  }

  recoverZoneV1(zoneId: number) {
    return this.http.get(this.BaseURL + `RecoverZoneV1?id=${zoneId}`).pipe(take(1));
  }

  recoverZone(zoneId: number) {
    return this.http.get(this.BaseURL + `RecoverZone?id=${zoneId}`).pipe(take(1));
  }

  searchZoneByBlockId(blockId: number): Observable<Zone> {
    return this.http.get<Zone>(this.BaseURL + `SearchZoneByBlockId?id=${blockId}`).pipe(take(1));
  }

  activateZone(zoneId: number, blockId: number): Observable<TMCSEmergency> {
    return this.http.get<TMCSEmergency>(this.BaseURL + `ActivateZone?zoneId=${zoneId}&blockId=${blockId}`).pipe(take(1));
  }

  activateZoneV1(zoneId: number, blockId: number): Observable<TMCSEmergencyLatest> {
    return this.http.get<TMCSEmergencyLatest>(this.BaseURL + `ActivateZoneV1?zoneId=${zoneId}&blockId=${blockId}`).pipe(take(1));
  }

  getEmergencyZonesV1(): Observable<TMCSEmergencyLatest[]> {
    return this.http.get<TMCSEmergencyLatest[]>(this.BaseURL + `GetEmergencyZonesV1`).pipe(take(1));
  }

  getZonesImageV1(zoneFileName: string): Observable<Blob> {
    return this.http.get(this.BaseURL + `GetZonesImageV1?zoneFileName=${zoneFileName}`, { responseType: 'blob' });
  }

  getCctvImageV1(cctvFileName: string): Observable<Blob> {
    return this.http.get(this.BaseURL + `GetCCTVImageV1?cctvFileName=${cctvFileName}`, { responseType: 'blob' });
  }


  getEmergencyZones(): Observable<TMCSEmergency[]> {
    return this.http.get<TMCSEmergency[]>(this.BaseURL + `GetEmergencyZones`).pipe(take(1));
  }

  markAsComplete(id: number) {
    return this.http.get(this.BaseURL + `MarkAsComplete?id=${id}`).pipe(take(1));
  }

  markAsCompleteV1(id: number) {
    return this.http.get(this.BaseURL + `MarkAsCompleteV1?id=${id}`).pipe(take(1));
  }

  markEfcAsCompleteV1(id: number) {
    return this.http.get(this.BaseURL + `MarkEfcAsCompleteV1?id=${id}`).pipe(take(1));
  }

  reviewEfcFromDmV1(id: number) {
    return this.http.get(this.BaseURL + `EfcRequireDMReviewV1?id=${id}`).pipe(take(1));
  }

  selectFanDirection(id: number, direction: string) {
    return this.http.get(this.BaseURL + `SelectFanDirection?id=${id}&direction=${direction}`).pipe(take(1));
  }

  selectFanDirectionV1(id: number, direction: string) {
    return this.http.get(this.BaseURL + `SelectFanDirectionV1?id=${id}&direction=${direction}`).pipe(take(1));
  }

  getPossibleExt1Blocks(ext1: number): Observable<GetTMCSResponse> {
    return this.http.get<GetTMCSResponse>(this.BaseURL + `GetPossibleExt1Blocks?ext1=${ext1}`).pipe(take(1));
  }

  loadAllBlocksV1(): Observable<BlockLatest[]> {
    return this.http.get<BlockLatest[]>(this.BaseURL + 'GetBlocksV1').pipe(take(1));
  }

  loadBlockZonesV1(blockId: number): Observable<ZoneResponseLatest[]> {
    return this.http.get<ZoneResponseLatest[]>(this.BaseURL + `GetBlockZonesV1?blockId=${blockId}`).pipe(take(1));
  }
}
