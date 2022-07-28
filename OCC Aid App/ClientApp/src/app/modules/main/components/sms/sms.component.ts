import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { SMS } from 'src/app/models/sms.model';
import { Token } from 'src/app/models/token.model';
import { LoggingService } from 'src/app/services/logging.service';
import { NotificationService } from 'src/app/services/notification.service';
import { SMSService } from 'src/app/services/sms.service';

@Component({
  selector: 'app-sms',
  templateUrl: './sms.component.html',
  styleUrls: ['./sms.component.css']
})
export class SMSComponent implements OnInit, OnDestroy {
  error: boolean = false;
  loading: boolean = false;
  token: Token | undefined;
  dropdownValue: string = "";

  smss: SMS[] = [];
  selectedSMS: SMS | undefined;
  receivedSMS: SMS | undefined;

  reviewableArchievedSmss: SMS[] = [];
  reviewableDropdownValue: string = "";

  subscriptions: Subscription[] = [];
  constructor(private router: Router,
    private service: SMSService,
    private notify: NotificationService,
    private toastr: ToastrService,
    private logger: LoggingService) {
      if(localStorage.getItem('ASPNetAuthToken')){
        this.token = JSON.parse(localStorage.getItem('ASPNetAuthToken') as string) as Token;
      }
      else
        this.router.navigate(['']);

      if(sessionStorage.getItem('sms-received')){
        this.receivedSMS = JSON.parse(sessionStorage.getItem('sms-received') as string);
        if(this.receivedSMS && !this.smss.find(f => f.id == this.receivedSMS?.id))
          this.smss.unshift(this.receivedSMS);
      }
  }
  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  ngOnInit(): void {
    if(this.token?.role.toLowerCase() != "dm"){
      this.subscribeToSMSReciever();
      this.loadArchievedSMSs();
    }
    else{
      this.loadReviewableArchievedSMSs();
      this.loadSMSs();
    }
  }

  subscribeToSMSReciever(){
    let sub = this.notify.smsReceived.subscribe((res: SMS) => {
      this.receivedSMS = res;
      this.smss.unshift(res);
    });
    this.subscriptions.push(sub);
  }

  loadSMSs() {
    this.loading = true;
    this.service.getAllSMS().subscribe(res => {
      this.smss = res;
      this.loading = false;
    },
    err => {
      this.toastr.error("Unable to load SMS list.", "Error!");
      this.loading = false;
    });
  }

  getSMSDetails(index: number){
    this.selectedSMS = this.smss[index];
    this.receivedSMS = this.smss[index];

    console.log(this.smss[index]);
  }

  getReviewSMSDetails(index: number){
    this.selectedSMS = this.reviewableArchievedSmss[index];
    this.receivedSMS = this.reviewableArchievedSmss[index];
  }

  sendOrReviewSMS() {
    if (!this.selectedSMS) return;
    if (this.selectedSMS.isRequiredDmReview) {
      this.service.dMReviewArchieveSMS(this.selectedSMS).subscribe(res => {
        if(!this.selectedSMS) return;
        this.reviewableArchievedSmss = this.reviewableArchievedSmss.filter(x=>x.id !== this.selectedSMS?.id);
        this.notify.sendSMSToHub(this.selectedSMS);
        this.toastr.success("Dm has approved SMS successfully.");
        this.logger.log("SMS", `${this.token?.email} has review a SMS`);
        this.selectedSMS = undefined;
      },
      err => {
        this.toastr.error("Unable to review SMS.", "Error!");
      });
    } else {
      this.service.saveArchievedSMS(this.selectedSMS).subscribe(res => {
        if (!this.selectedSMS) return;
        this.notify.sendSMSToHub(this.selectedSMS);
        this.toastr.success("SMS has been sent successfully.");
        this.logger.log("SMS", `${this.token?.email} has sent a SMS`);
        this.selectedSMS = undefined;
      },
        err => {
          this.toastr.error("Unable to Send SMS.", "Error!");
        });
    }
  }

  loadArchievedSMSs() {
    this.loading = true;
    this.service.getAllArchievedSMS().subscribe(res => {
      this.smss = res;
      this.loading = false;
    },
    err => {
      this.toastr.error("Unable to load SMS list.", "Error!");
      this.loading = false;
    });
  }

  loadReviewableArchievedSMSs() {
    this.loading = true;
    this.service.getReviewAbleArchievedSMS().subscribe(res => {
      this.reviewableArchievedSmss = res;
      this.loading = false;
    },
    err => {
      this.toastr.error("Unable to load review able SMS list.", "Error!");
      this.loading = false;
    });
  }

  markAsComplete(){
    if(!this.receivedSMS || !this.receivedSMS.id) return;
    this.service.markCompleteArchieveSMS(this.receivedSMS?.id).subscribe(res => {
      this.toastr.success("SMS Marked as Completed.", "Success!");
      this.logger.log("SMS", `${this.token?.email} has marked SMS as compelete.`);
      this.smss.splice(this.smss.findIndex(f => f.id == this.receivedSMS?.id), 1);
      this.receivedSMS = undefined;
    },
    err => {
      this.toastr.error("Unable to mark SMS as completed.", "Error!");
    });
  }

  reviewFromDm() {
    if (!this.receivedSMS || !this.receivedSMS.id) return;
    if (this.token?.role?.toLowerCase() == 'sc') {
      this.service.markArchieveSMSForDMReview(this.receivedSMS).subscribe(res => {
        this.toastr.success("SMS assigned to DM for review.", "Success!");
        this.logger.log("SMS", `${this.token?.email} has assigned to DM for review.`);
        window.location.reload();
      },
        err => {
          this.toastr.error("Unable to assign SMS to DM for review.", "Error!");
        });
    }
  }

  close(){
    this.selectedSMS = undefined;
    this.receivedSMS = undefined;
    this.dropdownValue = "";
  }

}
