import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Block } from 'src/app/models/block.model';
import { ZoneLatest } from 'src/app/models/zone-latest.model';
import { ZoneResponseLatest } from 'src/app/models/zone-response-latest.model';
import { Zone } from 'src/app/models/zone.model';
import { TMCSService } from 'src/app/services/tmcs.service';

@Component({
  selector: 'app-tmcs-admin',
  templateUrl: './tmcs-admin-latest.component.html',
  styleUrls: ['./tmcs-admin-latest.component.css']
})
export class TMCSAdminComponentLatest implements OnInit {
  form: FormGroup | undefined;

  loading: boolean = false;
  loadingSaveEdit: boolean = false;
  loadingDelete: boolean = false;

  zones: Zone[] = [];
  zonesLatest: ZoneResponseLatest[] = [];

  page: number = 1;
  take: number = 10;
  search: string = '';
  totalPages: number = 1;
  total: number = 0;

  updateMode: boolean = false;
  zoneIndex: number = -1;
  zoneId: number = -1;
  deleted: boolean = false;

  @ViewChild('btn_add_edit_close') btn_add_edit_close: ElementRef | undefined;
  constructor(private service: TMCSService,
    private fb: FormBuilder,
    private toastr: ToastrService) {
    this.newForm();
  }

  ngOnInit(): void {
    this.loadZones();
  }

  loadZones() {
    this.loading = true;
    this.service.getZonesV1(this.page, this.take, this.search, this.deleted).subscribe(res => {
      console.log("noman");
      console.log(res);
      console.log("noman");
      this.zonesLatest = res.zones;
      this.total = res.total;
      this.totalPages = Math.ceil(res.total / this.take);
      this.loading = false;
    },
      err => {
        this.toastr.error("Unable to load Zones list.", "Error!");
        this.loading = false;
      });
  }

  newForm() {
    this.updateMode = false;
    this.zoneId = -1;

    this.form = this.fb.group({
      'name': ['', Validators.required],
      'fanDirection': ['', Validators.required],
      'cctvLayout': ['', Validators.required],
      'zoneLayout': ['', Validators.required],
      'upName': [''],
      'leftName': [''],
      'rightName': [''],
      'shaftName': [''],
      'blocks': this.fb.array([
        this.initBlock()
      ])
    });
  }

  initBlock() {
    const tempBlock = {
      'name': ['', Validators.required]
    }
    return this.fb.group(tempBlock);
  }

  newBlock() {
    (<FormArray>this.form?.controls['blocks']).push(this.initBlock());
  }

  removeBlock(index: number) {
    (<FormArray>this.form?.controls['blocks']).removeAt(index);
  }

  saveUpdate(zone: ZoneLatest) {
    this.loadingSaveEdit = true;
    if (!this.updateMode) {
      this.service.saveZoneV1(zone).subscribe(res => {
        this.toastr.success("Zone has been saved successfully.", "Success!");
        //this.zonesLatest.unshift(zone);
        this.newForm();
        this.btn_add_edit_close?.nativeElement.click();
        this.loadingSaveEdit = false;
      },
        err => {
          this.toastr.error("Zone already exists, please check deleted also.", "Error!");
          this.loadingSaveEdit = false;
        });
    }
    else {
      zone.id = this.zoneId;
      this.service.updateZoneV1(zone).subscribe(res => {
        this.toastr.success("Zone has been updated successfully.", "Success!");
        //this.zonesLatest[this.zonesLatest.findIndex(f => f.id == zone.id)] = zone;
        this.newForm();
        this.btn_add_edit_close?.nativeElement.click();
        this.loadingSaveEdit = false;
      },
        err => {
          this.toastr.error("Unable to update Zone.", "Error!");
          this.loadingSaveEdit = false;
        });
    }
  }

  deleteZone(index: number) {
    const zoneId = this.zonesLatest[index]?.id;
    if (zoneId) {
      this.loadingDelete = true;
      this.zoneIndex = -1;
      this.service.deleteZoneV1(zoneId).subscribe(res => {
        this.toastr.success("Zone has been deleted successfully.", "Success!");
        this.loadZones();
        this.loadingDelete = false;
      },
        err => {
          this.toastr.error("Unable to delete Zone.", "Error!");
          this.loadingDelete = false;
        });
    }
  }

  recover(index: number) {
    const zoneId = this.zonesLatest[index]?.id;
    if (zoneId) {
      this.service.recoverZoneV1(zoneId).subscribe(res => {
        this.toastr.success("Zone has been recovered successfully.", "Success!");
        this.loadZones();
      },
        err => {
          this.toastr.error("Unable to recover Zone.", "Error!");
        });
    }
  }

  selectZoneForEdit(zone: Zone) {
    this.updateMode = true;
    this.zoneId = zone.zoneId ? zone.zoneId : -1;

    this.form = this.fb.group({
      'name': [zone.name, Validators.required],
      'fanDirection': [zone.fanDirection, Validators.required],
      'cctvLayout': [zone.cctvLayout, Validators.required],
      'zoneLayout': [zone.zoneLayout, Validators.required],
      'upName': [zone.upName],
      'leftName': [zone.leftName],
      'rightName': [zone.rightName],
      'trackNo': [zone.trackNo, Validators.required],
      'blocks': this.initBlockForEdit(zone.blocks)
    });
  }

  initBlockForEdit(blocks: Block[]) {
    let groups: FormGroup[] = [];
    blocks.forEach(block => {
      groups.push(this.fb.group({
        'blockId': block.blockId,
        'name': [block.name, Validators.required],
        'startLength': [block.startLength, [Validators.required, Validators.min(0)]],
        'endLength': [block.endLength, [Validators.required, Validators.min(1)]],
        'shaftName': block.shaftName,
        'zoneId': this.zoneId
      }));
    });
    return this.fb.array(groups);
  }

  getBlocks(index: number) {
    let blocks: string = '';
    this.zonesLatest[index].zoneBlocks.forEach(zb => {
      if (!blocks.includes(zb.block.name))
        blocks += zb.block.name + ',';
    });
    blocks = blocks.replace(/^\,+|\,+$/g, '');
    return blocks;
  }

  getShaftNames(index: number) {
    let shafts: string = '';
    this.zones[index].blocks.forEach(b => {
      if (!shafts.includes(b.shaftName))
        shafts += b.shaftName + ',';
    });
    shafts = shafts.replace(/^\,+|\,+$/g, '');
    return shafts;
  }


  cctvLayoutUpload(event: any) {
    let me = this;
    let file = event.target.files[0];
    if ((new RegExp(event.target.accept.replace('*', '.\*'))).test(file.type)) {
      let reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = function () {
        if (me.form == undefined) return;
        me.form.get('cctvLayout')?.setValue(reader.result as string);
      };
      reader.onerror = function (error) {
        console.log('Error: ', error);
      };
    }
    else {
      me.toastr.error("Please upload layout as image file only.");
      if (me.form == undefined) return;
      me.form.get('layout')?.setValue("");
    }
  }

  zoneLayoutUpload(event: any) {
    let me = this;
    let file = event.target.files[0];
    if ((new RegExp(event.target.accept.replace('*', '.\*'))).test(file.type)) {
      let reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = function () {
        if (me.form == undefined) return;
        me.form.get('zoneLayout')?.setValue(reader.result as string);
      };
      reader.onerror = function (error) {
        console.log('Error: ', error);
      };
    }
    else {
      me.toastr.error("Please upload layout as image file only.");
      if (me.form == undefined) return;
      me.form.get('layout')?.setValue("");
    }
  }

  fanDirectionValidation() {
    if (this.form?.get('fanDirection')?.value == 'Up') {
      if (this.form?.get('upName')?.value != "") {
        return true;
      }
    }
    else if (this.form?.get('fanDirection')?.value == 'Left/Right') {
      if (this.form?.get('leftName')?.value != "" && this.form?.get('rightName')?.value != "") {
        return true;
      }
    }
    return false;
  }

  nextPage() {
    this.page++;
    this.loadZones();
  }

  prevPage() {
    this.page--;
    this.loadZones();
  }

  goToPage(pageNo: number) {
    this.page = pageNo;
    this.loadZones();
  }

}
