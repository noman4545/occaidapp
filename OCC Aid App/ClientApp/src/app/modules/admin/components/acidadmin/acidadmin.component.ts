import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { genericType } from 'src/app/types/generic.type';
import { ACID } from '../../../../models/acid.model';
import { ACIDService } from '../../../../services/acid.service';

@Component({
  selector: 'app-acidadmin',
  templateUrl: './acidadmin.component.html',
  styleUrls: ['./acidadmin.component.css']
})
export class ACIDAdminComponent implements OnInit {
  form: FormGroup | undefined;

  loading: boolean = false;
  loadingSaveEdit: boolean = false;
  loadingDelete: boolean = false;

  acids: ACID[] = [];
  page: number = 1;
  take: number = 10;
  search: string = '';
  totalPages: number = 1;
  total: number = 0;

  updateMode: boolean = false;
  acidId: number = -1;
  acidIndex: number = -1;
  deleted: boolean = false;

  @ViewChild('btn_add_edit_close') btn_add_edit_close: ElementRef | undefined;
  constructor(private fb: FormBuilder, private service: ACIDService, private toastr: ToastrService) {
    this.newForm();
  }

  ngOnInit(): void {
    this.loadAcids();
  }

  loadAcids() {
    this.loading = true;
    this.service.getAcids(this.page, this.take, this.search, this.deleted).subscribe(res => {
      this.acids = res.acids;
      this.total = res.total;
      this.totalPages = Math.ceil(res.total / this.take);
      this.loading = false;
    },
    err => {
      this.toastr.error("Unable to load ACID CCTV list.", "Error!");
      this.loading = false;
    });
  }

  newForm() {
    this.updateMode = false;
    this.acidId = -1;

    const tempForm: genericType<ACID> = {
      'territory': ['', Validators.required],
      'acidNameInAts': ['', Validators.required],
      'acidNameInIsm': ['', Validators.required],
      'pedEepEesName': ['', Validators.required],
      'trackNo': ['', Validators.required],
      'cctv': ['', Validators.required],
      'layout': ['', Validators.required],
    };

    this.form = this.fb.group(tempForm);
  }

  saveUpdate(acid: ACID) {
    this.loadingSaveEdit = true;
    if(!this.updateMode) {
      this.service.saveAcid(acid).subscribe(res => {
        this.toastr.success("ACID CCTV has been saved successfully.", "Success!");
        this.loadAcids();
        this.newForm();
        this.btn_add_edit_close?.nativeElement.click();
        this.loadingSaveEdit = false;
      },
      err => {
        this.toastr.error("ACID CCTV already exists, please check deleted also.", "Error!");
        this.loadingSaveEdit = false;
      });
    }
    else {
      acid.id = this.acidId;
      this.service.updateAcid(acid).subscribe(res => {
        this.toastr.success("ACID CCTV has been updated successfully.", "Success!");
        this.loadAcids();
        this.newForm();
        this.btn_add_edit_close?.nativeElement.click();
        this.loadingSaveEdit = false;
      },
      err => {
        this.toastr.error("Unable to update ACID CCTV.", "Error!");
        this.loadingSaveEdit = false;
      });
    }
  }

  deleteAcid(index: number) {
    if(this.acids[index].id){
      this.loadingDelete = true;
      this.acidIndex = -1;
      this.service.deleteAcid(this.acids[index].id as number).subscribe(res => {
        this.toastr.success("ACID CCTV has been deleted successfully.", "Success!");
        this.loadAcids();
        this.loadingDelete = false;
      },
      err => {
        this.toastr.error("Unable to delete ACID CCTV.", "Error!");
        this.loadingDelete = false;
      });
    }
  }

  recover(index: number) {
    if(this.acids[index].id){
      this.acidIndex = -1;
      this.service.recoverAcid(this.acids[index].id as number).subscribe(res => {
        this.toastr.success("ACID CCTV has been recovered successfully.", "Success!");
        this.loadAcids();
      },
      err => {
        this.toastr.error("Unable to recover ACID CCTV.", "Error!");
      });
    }
  }

  selectAcidForEdit(acid: ACID) {
    this.updateMode = true;
    this.acidId = acid.id ? acid.id : -1;

    const tempForm: genericType<ACID> = {
      'territory': [acid.territory, Validators.required],
      'acidNameInAts': [acid.acidNameInAts, Validators.required],
      'acidNameInIsm': [acid.acidNameInIsm, Validators.required],
      'pedEepEesName': [acid.pedEepEesName, Validators.required],
      'trackNo': [acid.trackNo, Validators.required],
      'cctv': [acid.cctv, Validators.required],
      'layout': [acid.layout, Validators.required],
    };

    this.form = this.fb.group(tempForm);
  }

  layoutUpload(event: any){
    let me = this;
    let file = event.target.files[0];
    if(( new RegExp( event.target.accept.replace( '*', '.\*' ) ) ).test( file.type )){
      let reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = function () {
        if(me.form == undefined) return;
        me.form.get('layout')?.setValue(reader.result as string);
      };
      reader.onerror = function (error) {
        console.log('Error: ', error);
      };
    }
    else{
      me.toastr.error("Please upload layout as image file only.");
      if(me.form == undefined) return;
        me.form.get('layout')?.setValue("");
    }
  }

  nextPage(){
    this.page++;
    this.loadAcids();
  }

  prevPage(){
    this.page--;
    this.loadAcids();
  }

  goToPage(pageNo: number){
    this.page = pageNo;
    this.loadAcids();
  }
}
