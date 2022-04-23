import { GetTMCSResponse } from '../../../../models/getTMCSResponse.model';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { TMCSEmergency } from 'src/app/models/tmcsEmergency.model';
import { Zone } from 'src/app/models/zone.model';
import { ZoneSearch } from 'src/app/models/zoneSearch.model';
import { LoggingService } from 'src/app/services/logging.service';
import { NotifyTMCSService } from 'src/app/services/notifyTMCS.service';
import { TMCSService } from 'src/app/services/tmcs.service';
import { Token } from 'src/app/models/token.model';
import { BlockLatest } from 'src/app/models/block-latest.model';
import { ZoneLatest } from 'src/app/models/zone-latest.model';
import { ZoneResponseLatest } from 'src/app/models/zone-response-latest.model';
import { TMCSEmergencyLatest } from 'src/app/models/tmcsEmergencyLatest.model';

@Component({
  selector: 'app-tmcs-latest',
  templateUrl: './tmcs-latest.component.html',
  styleUrls: ['./tmcs-latest.component.css']
})
export class TMCSLatestComponent implements OnInit, OnDestroy {
  form: FormGroup | undefined;

  zone: Zone | undefined;
  tmcsEmergency: TMCSEmergency[] = [];
  selectedTMCSEmergency: TMCSEmergency | undefined;

  tmcsEmergencyLatest: TMCSEmergencyLatest[] = [];
  selectedTMCSEmergencyLatest: TMCSEmergencyLatest | undefined;

  activatedMessage: string = '';
  activatedMessage2: string = '';
  error: boolean = false;
  loading: boolean = false;
  loadingList: boolean = false;
  token: Token | undefined;
  dropdownValue: string = "";

  tmcsResponse: GetTMCSResponse | undefined;
  blocksData: BlockLatest[] | undefined;
  zonesData: ZoneResponseLatest[] | undefined;
  selectedZone: ZoneResponseLatest | undefined;

  subscriptions: Subscription[] = [];
  constructor(private router: Router,
    private fb: FormBuilder,
    private service: TMCSService,
    private toaster: ToastrService,
    private log: LoggingService,
    private notify: NotifyTMCSService) {
    if (localStorage.getItem('ASPNetAuthToken')) {
      this.token = JSON.parse(localStorage.getItem('ASPNetAuthToken') as string) as Token;
    }
    else
      this.router.navigate(['']);

    this.newForm();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  subscribeToTMCSReciever() {
    let sub = this.notify.tmcsReceived.subscribe((res: TMCSEmergency) => {
      if (this.tmcsEmergency.find(f => f.id == res.id)) {
        this.tmcsEmergency[this.tmcsEmergency.findIndex(f => f.id == res.id)] = res;
        this.dropdownValue = this.tmcsEmergency.findIndex(f => f.id == res.id).toString();
      }
      else {
        this.tmcsEmergency.unshift(res);
        this.dropdownValue = "0";
      }
      this.selectedTMCSEmergency = res;
    });
    this.subscriptions.push(sub);

    sub = this.notify.markCompleteTMCSReceived.subscribe((res: number) => {
      let tmcs = this.tmcsEmergency.find(f => f.id == res);
      if (tmcs) {
        this.toaster.success(tmcs.zone.name + " has been deactivated and marked as complete.");
        this.tmcsEmergency.splice(this.tmcsEmergency.findIndex(f => f.id == res), 1);
        if (this.selectedTMCSEmergency = tmcs) {
          this.dropdownValue = "";
          this.selectedTMCSEmergency = undefined;
        }
      }
    });
    this.subscriptions.push(sub);
  }

  ngOnInit(): void {
    if (this.token?.role.toLowerCase() != "tc_l6" && this.token?.role.toLowerCase() != "tc_l4") {
      this.loadEmergencyZones();
      this.subscribeToTMCSReciever();
    } else {
      this.loadBlocks();
    }
  }

  loadBlocks() {
    this.loading = true;
    this.service.loadAllBlocksV1().subscribe(res => {
      this.blocksData = res;
      this.loading = false;
    },
      err => {
        this.toaster.error("Unable to load blocks.");
        this.loading = false;
      }
    );
  }

  loadZones() {
    let blockId = this.form?.controls["blocks"].value;
    if (blockId && +blockId > 0) {
      this.loading = true;
      this.service.loadBlockZonesV1(blockId).subscribe(res => {
        this.zonesData = res;
        this.loading = false;
        this.selectedZone = undefined;
        this.form?.controls["zones"].setValue("");
        this.activatedMessage = '';
        this.activatedMessage2 = '';
      },
        err => {
          this.toaster.error("Unable to load blocks.");
          this.loading = false;
          this.selectedZone = undefined;
          this.activatedMessage = '';
          this.activatedMessage2 = '';
        }
      );
    }

  }

  blob2Base64 = (blob:Blob):Promise<string> => {
    return new Promise<string> ((resolve,reject)=> {
         const reader = new FileReader();
         reader.readAsDataURL(blob);
         reader.onload = () => resolve(reader.result!.toString());
         reader.onerror = error => reject(error);
     })
    }

  changeSelectedZone(event: any) {
    this.loading = true;
    const zoneId = +event.target.value;
    this.selectedZone = this.zonesData?.find(x => x.id == zoneId);
    const zoneFileName = this.selectedZone?.zoneLayout;
    if(zoneFileName){
      this.service.getZonesImageV1(zoneFileName).subscribe(res => {
        this.blob2Base64(res).then(res=>this.selectedZone!.zoneLayout = res);
        this.loading = false;
      },
        err => {
          this.toaster.error("Unable to load zone image.");
          this.loading = false;
        });
    }else{
      this.loading = false;
    }
    
    this.activatedMessage = '';
    this.activatedMessage2 = '';
  }

  loadEmergencyZones() {
    this.loading = true;
    this.service.getEmergencyZonesV1().subscribe(res => {
      this.tmcsEmergencyLatest = res;
      this.loading = false;
    },
      err => {
        this.toaster.error("Unable to load activated zones.");
        this.loading = false;
      });
  }

  getZoneDetails(index: number) {
    this.selectedTMCSEmergencyLatest = this.tmcsEmergencyLatest[index];
    this.service.getZonesImageV1(this.selectedTMCSEmergencyLatest.zone.zoneLayout).subscribe(res => {
      this.blob2Base64(res).then(res => this.selectedTMCSEmergencyLatest!.zone.zoneLayout = res);
      this.loading = false;
    },
      err => {
        this.toaster.error("Unable to load zone image.");
        this.loading = false;
      });

      this.service.getCctvImageV1(this.selectedTMCSEmergencyLatest.zone.cctvLayout).subscribe(res => {
        this.blob2Base64(res).then(res => this.selectedTMCSEmergencyLatest!.zone.cctvLayout = res);
        this.loading = false;
      },
        err => {
          this.toaster.error("Unable to load zone image.");
          this.loading = false;
        });
  }

  markAsComplete() {
    if (this.selectedTMCSEmergencyLatest && this.selectedTMCSEmergencyLatest.id) {
      if (this.selectedTMCSEmergencyLatest.efcMarkedCompleted) {
        this.service.markAsCompleteV1(this.selectedTMCSEmergencyLatest.id).subscribe(res => {
          this.notify.markCompleteTMCSToHub(this.selectedTMCSEmergency?.id)
          this.loadEmergencyZones();
          this.dropdownValue = "";
          this.toaster.success("Zone has been deactivated and marked as complete.");
          this.log.log("TMCS", `${this.token?.email} has marked the zone "${this.selectedTMCSEmergencyLatest?.zone.name}" complete.`);
          this.selectedTMCSEmergencyLatest = undefined;
        },
          err => {
            this.toaster.error("Unable to mark zone as complete.");
          });
      } else {
        this.toaster.info(`The EFC has not yet marked "${this.selectedTMCSEmergencyLatest?.zone.name}" as complete.`);
      }
    }
  }

  markEfcAsComplete() {
    if (this.selectedTMCSEmergencyLatest && this.selectedTMCSEmergencyLatest.id) {
      if (!this.selectedTMCSEmergencyLatest.efcMarkedCompleted) {
        this.service.markEfcAsCompleteV1(this.selectedTMCSEmergencyLatest.id).subscribe(res => {
          this.notify.markCompleteTMCSToHub(this.selectedTMCSEmergency?.id)
          this.loadEmergencyZones();
          this.dropdownValue = "";
          this.toaster.success(`"${this.selectedTMCSEmergencyLatest?.zone.name}" has been marked as complete.`);
          this.log.log("EFC", `${this.token?.email} has marked the zone "${this.selectedTMCSEmergencyLatest?.zone.name}" as completed.`);
          this.selectedTMCSEmergencyLatest = undefined;
        },
          err => {
            this.toaster.error("Unable to mark zone as complete.");
          });
      } else {
        this.toaster.info(`"${this.selectedTMCSEmergencyLatest?.zone.name}" is already marked as completed.`);
      }
    }
  }

  selectFanDirection(direction: string) {
    if (this.selectedTMCSEmergencyLatest && this.selectedTMCSEmergencyLatest.id) {
      this.service.selectFanDirectionV1(this.selectedTMCSEmergencyLatest.id, direction).subscribe(res => {
        if (this.selectedTMCSEmergencyLatest) {
          this.selectedTMCSEmergencyLatest.dmDecision = direction;
          this.tmcsEmergencyLatest[this.tmcsEmergencyLatest.findIndex(f => f.id == this.selectedTMCSEmergencyLatest?.id)] = this.selectedTMCSEmergencyLatest;
          this.dropdownValue = this.tmcsEmergencyLatest.findIndex(f => f.id == this.selectedTMCSEmergencyLatest?.id).toString();
          this.notify.sendTMCSToHub(this.selectedTMCSEmergencyLatest);
          if (this.selectedTMCSEmergencyLatest.dmDecision.toLowerCase() == 'right')
            this.log.log("TMCS", `${this.token?.email} has selected ${this.selectedTMCSEmergencyLatest.zone.rightName} fan direction for zone "${this.selectedTMCSEmergency?.zone.name}".`);
          else if (this.selectedTMCSEmergencyLatest.dmDecision.toLowerCase() == 'left')
            this.log.log("TMCS", `${this.token?.email} has selected ${this.selectedTMCSEmergencyLatest.zone.leftName} fan direction for zone "${this.selectedTMCSEmergency?.zone.name}".`);
          else
            this.log.log("TMCS", `${this.token?.email} has selected ${this.selectedTMCSEmergencyLatest.zone.upName} fan direction for zone "${this.selectedTMCSEmergency?.zone.name}".`);
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
      'blocks': ['', Validators.required],
      'zones': ['', [Validators.required]],
    });
  }

  activateZone() {
    const currentZone = this.selectedZone;
    this.loading = true;
    const blockId = currentZone?.zoneBlocks[0]?.blockId;
    if (currentZone && currentZone.id && blockId) {
      this.service.activateZoneV1(currentZone.id, blockId).subscribe(res => {
        this.loading = false;
        this.log.log("TMCS", `${this.token?.email} has activated "${currentZone.name}", train is in block "${this.blocksData?.find(b => b.id == blockId)?.name}" ${currentZone.shaftName ? (' and there is a shaft "' + currentZone.shaftName + '".') : '.'}`);
        this.activatedMessage = `"${currentZone.name}" is activated.`;
        this.notify.sendTMCSToHub(res);
      },
        err => {
          this.activatedMessage = `"${currentZone.name}" is already activated.`;
          this.activatedMessage2 = `Train is in block "${this.blocksData?.find(b => b.id == blockId)?.name}" ${currentZone.shaftName ? ' and there is a shaft "' + currentZone.shaftName + '".' : '.'}`
          this.loading = false;
        });
    }
  }
  //#endregion

  close() {
    this.zone = undefined;
    this.selectedTMCSEmergency = undefined;
    this.selectedZone = undefined;
    this.zonesData = undefined;
    this.form?.controls["blocks"].setValue("");
    this.activatedMessage = '';
    this.activatedMessage2 = '';
    this.selectedTMCSEmergencyLatest = undefined;
  }
}
