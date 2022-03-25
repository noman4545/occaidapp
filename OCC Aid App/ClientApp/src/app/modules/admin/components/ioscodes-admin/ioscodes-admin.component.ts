import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { genericType } from 'src/app/types/generic.type';
import { IOSCode } from '../../../../models/iosCode.model';
import { IOSCodesService } from '../../../../services/ioscodes.service';

@Component({
  selector: 'app-ioscodes-admin',
  templateUrl: './ioscodes-admin.component.html',
  styleUrls: ['./ioscodes-admin.component.css']
})
export class IOSCodesAdminComponent implements OnInit {
  form: FormGroup | undefined;

  loading: boolean = false;
  loadingSaveEdit: boolean = false;
  loadingDelete: boolean = false;

  iOSCodes: IOSCode[] = [];
  page: number = 1;
  take: number = 10;
  search: string = '';
  totalPages: number = 1;
  total: number = 0;

  updateMode: boolean = false;
  iosCodeId: number = -1;
  iosIndex: number = -1;
  deleted: boolean = false;

  @ViewChild('btn_add_edit_close') btn_add_edit_close: ElementRef | undefined;
  constructor(private fb: FormBuilder, private service: IOSCodesService, private toastr: ToastrService) {
    this.newForm();
  }

  ngOnInit(): void {
    this.loadIOSCodes();
  }

  loadIOSCodes() {
    this.loading = true;
    this.service.getIOSCodes(this.page, this.take, this.search, this.deleted).subscribe(res => {
      this.iOSCodes = res.iosCodes;
      this.total = res.total;
      this.totalPages = Math.ceil(res.total / this.take);
      this.loading = false;
    },
    err => {
      this.toastr.error("Unable to load IOS Codes list.", "Error!");
      this.loading = false;
    });
  }

  newForm() {
    this.updateMode = false;
    this.iosCodeId = -1;

    const tempForm: genericType<IOSCode> = {
      'iosNumber': ['', Validators.required],
      'function': ['', Validators.required],
      'description': ['', Validators.required],
      'level': ['', Validators.required],
      'occAction': ['', Validators.required],
      'trainRescueAction': ['', Validators.required],
      'maintenanceAction': ['', Validators.required],
      'remarks': [''],
    };

    this.form = this.fb.group(tempForm);
  }

  saveUpdate(iosCode: IOSCode) {
    this.loadingSaveEdit = true;
    if(!this.updateMode) {
      this.service.saveIOSCode(iosCode).subscribe(res => {
        this.toastr.success("IOS Code has been saved successfully.", "Success!");
        this.loadIOSCodes();
        this.newForm();
        this.btn_add_edit_close?.nativeElement.click();
        this.loadingSaveEdit = false;
      },
      err => {
        this.toastr.error("IOS Code already exists, please check deleted also.", "Error!");
        this.loadingSaveEdit = false;
      });
    }
    else {
      iosCode.id = this.iosCodeId;
      this.service.updateIOSCode(iosCode).subscribe(res => {
        this.toastr.success("IOS Code has been updated successfully.", "Success!");
        this.loadIOSCodes();
        this.newForm();
        this.btn_add_edit_close?.nativeElement.click();
        this.loadingSaveEdit = false;
      },
      err => {
        this.toastr.error("Unable to update IOS Code.", "Error!");
        this.loadingSaveEdit = false;
      });
    }
  }

  deleteIOSCode(index: number) {
    if(this.iOSCodes[index].id){
      this.loadingDelete = true;
      this.iosIndex = -1;
      this.service.deleteIOSCodes(this.iOSCodes[index].id as number).subscribe(res => {
        this.toastr.success("IOS Code has been deleted successfully.", "Success!");
        this.loadIOSCodes();
        this.loadingDelete = false;
      },
      err => {
        this.toastr.error("Unable to delete IOS Code.", "Error!");
        this.loadingDelete = false;
      });
    }
  }

  recover(index: number) {
    if(this.iOSCodes[index].id){
      this.iosIndex = -1;
      this.service.recoverIOSCodes(this.iOSCodes[index].id as number).subscribe(res => {
        this.toastr.success("IOS Code has been recovered successfully.", "Success!");
        this.loadIOSCodes();
      },
      err => {
        this.toastr.error("Unable to recover IOS Code.", "Error!");
      });
    }
  }

  selectIOSForEdit(iosCode: IOSCode) {
    this.updateMode = true;
    this.iosCodeId = iosCode.id ? iosCode.id : -1;

    const tempForm: genericType<IOSCode> = {
      'iosNumber': [iosCode.iosNumber, Validators.required],
      'function': [iosCode.function, Validators.required],
      'description': [iosCode.description, Validators.required],
      'level': [iosCode.level, Validators.required],
      'occAction': [iosCode.occAction, Validators.required],
      'trainRescueAction': [iosCode.trainRescueAction, Validators.required],
      'maintenanceAction': [iosCode.maintenanceAction, Validators.required],
      'remarks': [iosCode.remarks],
    };

    this.form = this.fb.group(tempForm);
  }

  nextPage(){
    this.page++;
    this.loadIOSCodes();
  }

  prevPage(){
    this.page--;
    this.loadIOSCodes();
  }

  goToPage(pageNo: number){
    this.page = pageNo;
    this.loadIOSCodes();
  }
}
