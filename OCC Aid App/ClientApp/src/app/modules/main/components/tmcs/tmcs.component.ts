import { GetTMCSResponse } from './../../../../models/getTMCSResponse.model';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { Block } from 'src/app/models/block.model';
import { TMCSEmergency } from 'src/app/models/tmcsEmergency.model';
import { Zone } from 'src/app/models/zone.model';
import { ZoneSearch } from 'src/app/models/zoneSearch.model';
import { LoggingService } from 'src/app/services/logging.service';
import { NotifyTMCSService } from 'src/app/services/notifyTMCS.service';
import { TMCSService } from 'src/app/services/tmcs.service';
import { Token } from 'src/app/models/token.model';

@Component({
  selector: 'app-tmcs',
  templateUrl: './tmcs.component.html',
  styleUrls: ['./tmcs.component.css']
})
export class TMCSComponent implements OnInit, OnDestroy {
  form: FormGroup | undefined;
  
  zone: Zone | undefined;
  tmcsEmergency: TMCSEmergency[] = [];
  selectedTMCSEmergency: TMCSEmergency | undefined;

  activatedMessage: string = '';
  error: boolean = false;
  loading: boolean = false;
  loadingList: boolean = false;
  token: Token | undefined;
  dropdownValue: string = "";

  tmcsResponse: GetTMCSResponse | undefined;

  subscriptions: Subscription[] = [];
  constructor(private router: Router, 
    private fb: FormBuilder,
    private service: TMCSService,
    private toaster: ToastrService,
    private log: LoggingService,
    private notify: NotifyTMCSService) {
      if(localStorage.getItem('ASPNetAuthToken')){
        this.token = JSON.parse(localStorage.getItem('ASPNetAuthToken') as string) as Token;
      }
      else
        this.router.navigate(['']);

      this.newForm();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  subscribeToTMCSReciever(){
    let sub = this.notify.tmcsReceived.subscribe((res: TMCSEmergency) => {
      if(this.tmcsEmergency.find(f => f.id == res.id)){
        this.tmcsEmergency[this.tmcsEmergency.findIndex(f => f.id == res.id)] = res;
        this.dropdownValue = this.tmcsEmergency.findIndex(f => f.id == res.id).toString();
      }
      else{
        this.tmcsEmergency.unshift(res);
        this.dropdownValue = "0";
      }
      this.selectedTMCSEmergency = res;
    });
    this.subscriptions.push(sub);

    sub = this.notify.markCompleteTMCSReceived.subscribe((res: number) => {
      let tmcs = this.tmcsEmergency.find(f => f.id == res);
      if(tmcs){
        this.toaster.success(tmcs.zone.name + " has been deactivated and marked as complete.");
        this.tmcsEmergency.splice(this.tmcsEmergency.findIndex(f => f.id == res), 1);
        if(this.selectedTMCSEmergency = tmcs){
          this.dropdownValue = "";
          this.selectedTMCSEmergency = undefined;
        }
      }
    });
    this.subscriptions.push(sub);
  }

  ngOnInit(): void {
    if(this.token?.role.toLowerCase() != "tc_l6" && this.token?.role.toLowerCase() != "tc_l4"){
      this.loadEmergencyZones();
      this.subscribeToTMCSReciever();
    }
  }

  loadEmergencyZones(){
    this.loading = true;
    this.service.getEmergencyZones().subscribe(res => {
      this.tmcsEmergency = res;
      this.loading = false;
    },
    err => {
      this.toaster.error("Unable to load activated zones.");
      this.loading = false;
    });
  }

  getZoneDetails(index: number){
    this.selectedTMCSEmergency = this.tmcsEmergency[index];
  }

  markAsComplete(){
    if(this.selectedTMCSEmergency && this.selectedTMCSEmergency.id){
      this.service.markAsComplete(this.selectedTMCSEmergency.id).subscribe(res => {
        this.notify.markCompleteTMCSToHub(this.selectedTMCSEmergency?.id)
        this.loadEmergencyZones();
        this.dropdownValue = "";
        this.toaster.success("Zone has been deactivated and marked as complete.");
        this.log.log("TMCS", `${this.token?.email} has marked the zone "${this.selectedTMCSEmergency?.zone.name}" complete.`);
        this.selectedTMCSEmergency = undefined;
      },
      err => {
        this.toaster.error("Unable to mark zone as complete.");
      });
    }
  }

  selectFanDirection(direction: string){
    if(this.selectedTMCSEmergency && this.selectedTMCSEmergency.id){
      this.service.selectFanDirection(this.selectedTMCSEmergency.id, direction).subscribe(res => {
        if(this.selectedTMCSEmergency){
          this.selectedTMCSEmergency.dmDecision = direction;
          this.tmcsEmergency[this.tmcsEmergency.findIndex(f => f.id == this.selectedTMCSEmergency?.id)] = this.selectedTMCSEmergency;
          this.dropdownValue = this.tmcsEmergency.findIndex(f => f.id == this.selectedTMCSEmergency?.id).toString();
          this.notify.sendTMCSToHub(this.selectedTMCSEmergency);
          if(this.selectedTMCSEmergency.dmDecision.toLowerCase() == 'right')
            this.log.log("TMCS", `${this.token?.email} has selected ${this.selectedTMCSEmergency.zone.rightName} fan direction for zone "${this.selectedTMCSEmergency?.zone.name}".`);
          else if(this.selectedTMCSEmergency.dmDecision.toLowerCase() == 'left')
            this.log.log("TMCS", `${this.token?.email} has selected ${this.selectedTMCSEmergency.zone.leftName} fan direction for zone "${this.selectedTMCSEmergency?.zone.name}".`);
          else
            this.log.log("TMCS", `${this.token?.email} has selected ${this.selectedTMCSEmergency.zone.upName} fan direction for zone "${this.selectedTMCSEmergency?.zone.name}".`);
        }
        this.toaster.success("EFC has been notified about your decision.");
      },
      err => {
        this.toaster.error("Unable to notify EFC.");
      });
    }
  }

  //#region For TC
  newForm() {
    this.form = this.fb.group({
      'ext1': [null, [Validators.required, Validators.min(0)]],
      'ext1Block': ['', Validators.required],
      'ext2': ['', [Validators.required]],
    });
  }

  search(value: ZoneSearch){
    this.loading = true;
    let index: number | undefined = this.tmcsResponse?.blocks.findIndex(f => f.name == value.ext1Block);
    if(index != undefined && index != null && index > -1){
      if(this.tmcsResponse)
        this.activateZone(this.tmcsResponse.zones[index as number]);
    }
  }

  activateZone(currentZone: Zone) {
    if(currentZone && currentZone.zoneId && currentZone.blocks[0].blockId){
      this.service.activateZone(currentZone.zoneId, currentZone.blocks[0].blockId).subscribe(res => {
          this.loading = false;
          this.zone = currentZone;
          this.log.log("TMCS", `${this.token?.email} has activated "${currentZone.name}", train is in block "${currentZone.blocks[0].name}" ${currentZone.blocks[0].shaftName != "" ? (' and there is a shaft "' + currentZone.blocks[0].shaftName + '".') : '.' }`);
          this.activatedMessage = `"${currentZone.name}" is activated.`;
          this.notify.sendTMCSToHub(res);
      },
      err => {
        this.zone = currentZone;
        this.activatedMessage = `"${currentZone.name}" is already activated.`;
        this.loading = false;
      });
    }
  }
  //#endregion

  close(){
    this.zone = undefined;
    this.selectedTMCSEmergency = undefined;
  }

  getPossibleExt1Blocks(){
    this.form?.controls["ext2"].setValue("");
    this.form?.controls["ext1Block"].setValue("");
    this.tmcsResponse = undefined;
    if(this.form?.controls["ext1"].valid){
      let ext1 = this.form?.controls["ext1"].value;
      this.loadingList = true;
      this.service.getPossibleExt1Blocks(ext1).subscribe(res => {
        this.tmcsResponse = res;
        this.loadingList = false;
      },
      err => {
        this.toaster.error("Could not able to fetch related Ext 1 Blocks");
        this.loadingList = false;
      });
    }
  }

  getExt2(){
    let ext1Block: string = this.form?.controls["ext1Block"].value;
    this.form?.controls["ext2"].setValue(this.tmcsResponse?.blocks.find(f => f.name == ext1Block)?.ext2);
  }
}
