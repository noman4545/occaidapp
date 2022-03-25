import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { SMS } from 'src/app/models/sms.model';
import { SMSService } from 'src/app/services/sms.service';
import { genericType } from 'src/app/types/generic.type';

@Component({
  selector: 'app-sms-admin',
  templateUrl: './sms-admin.component.html',
  styleUrls: ['./sms-admin.component.css']
})
export class SMSAdminComponent implements OnInit {
  form: FormGroup | undefined;

  loading: boolean = false;
  loadingSaveEdit: boolean = false;
  loadingDelete: boolean = false;

  smss: SMS[] = [];
  page: number = 1;
  take: number = 10;
  search: string = '';
  totalPages: number = 1;
  total: number = 0;

  updateMode: boolean = false;
  smsId: number = -1;
  smsIndex: number = -1;
  deleted: boolean = false;

  @ViewChild('btn_add_edit_close') btn_add_edit_close: ElementRef | undefined;
  constructor(private fb: FormBuilder, private service: SMSService, private toastr: ToastrService) {
    this.newForm();
  }

  ngOnInit(): void {
    this.loadSMSs();
  }

  loadSMSs() {
    this.loading = true;
    this.service.getSMSs(this.page, this.take, this.search, this.deleted).subscribe(res => {
      this.smss = res.smSs;
      this.total = res.total;
      this.totalPages = Math.ceil(res.total / this.take);
      this.loading = false;
    },
    err => {
      this.toastr.error("Unable to load SMS list.", "Error!");
      this.loading = false;
    });
  }

  newForm() {
    this.updateMode = false;
    this.smsId = -1;

    const tempForm: genericType<SMS> = {
      'typeOfFailure': ['', Validators.required],
      'systemBehaviour': ['', Validators.required],
      'workInstruction': ['', Validators.required],
      'message': ['', Validators.required],
      'timeToReturnToTimetable': ['', Validators.required]
    };

    this.form = this.fb.group(tempForm);
  }

  saveUpdate(sms: SMS) {
    this.loadingSaveEdit = true;
    if(!this.updateMode) {
      this.service.saveSMS(sms).subscribe(res => {
        this.toastr.success("SMS has been saved successfully.", "Success!");
        this.loadSMSs();
        this.newForm();
        this.btn_add_edit_close?.nativeElement.click();
        this.loadingSaveEdit = false;
      },
      err => {
        this.toastr.error("SMS already exists, please check deleted also.", "Error!");
        this.loadingSaveEdit = false;
      });
    }
    else {
      sms.id = this.smsId;
      this.service.updateSMS(sms).subscribe(res => {
        this.toastr.success("SMS has been updated successfully.", "Success!");
        this.loadSMSs();
        this.newForm();
        this.btn_add_edit_close?.nativeElement.click();
        this.loadingSaveEdit = false;
      },
      err => {
        this.toastr.error("Unable to update SMS.", "Error!");
        this.loadingSaveEdit = false;
      });
    }
  }

  deleteSMS(index: number) {
    if(this.smss[index].id){
      this.loadingDelete = true;
      this.smsIndex = -1;
      this.service.deleteSMS(this.smss[index].id as number).subscribe(res => {
        this.toastr.success("SMS has been deleted successfully.", "Success!");
        this.loadSMSs();
        this.loadingDelete = false;
      },
      err => {
        this.toastr.error("Unable to delete SMS.", "Error!");
        this.loadingDelete = false;
      });
    }
  }

  recover(index: number) {
    if(this.smss[index].id){
      this.service.recoverSMS(this.smss[index].id as number).subscribe(res => {
        this.toastr.success("SMS has been recovered successfully.", "Success!");
        this.loadSMSs();
      },
      err => {
        this.toastr.error("Unable to recover SMS.", "Error!");
      });
    }
  }

  selectSMSForEdit(sms: SMS) {
    this.updateMode = true;
    this.smsId = sms.id ? sms.id : -1;

    const tempForm: genericType<SMS> = {
      'typeOfFailure': [sms.typeOfFailure, Validators.required],
      'systemBehaviour': [sms.systemBehaviour, Validators.required],
      'workInstruction': [sms.workInstruction, Validators.required],
      'message': [sms.message, Validators.required],
      'timeToReturnToTimetable': [sms.timeToReturnToTimetable, Validators.required]
    };

    this.form = this.fb.group(tempForm);
  }

  nextPage(){
    this.page++;
    this.loadSMSs();
  }

  prevPage(){
    this.page--;
    this.loadSMSs();
  }

  goToPage(pageNo: number){
    this.page = pageNo;
    this.loadSMSs();
  }
}
