import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ACID } from 'src/app/models/acid.model';
import { Token } from 'src/app/models/token.model';
import { ACIDService } from 'src/app/services/acid.service';
import { LoggingService } from 'src/app/services/logging.service';
import { NotificationService } from 'src/app/services/notification.service';

@Component({
  selector: 'app-acid',
  templateUrl: './acid.component.html',
  styleUrls: ['./acid.component.css']
})
export class ACIDComponent implements OnInit {
  territories: string[] = []; 
  loadingTerritories: boolean = false;
  acids: string[] = [];
  loadingAcids: boolean = false;
  acid: ACID | undefined;
  error: boolean = false;
  loading: boolean = false;
  token: Token | undefined;
  selectedAcid: string = "";
  constructor(private service: ACIDService, 
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private lService: LoggingService,
    private router: Router) {
      if(localStorage.getItem('ASPNetAuthToken')){
        this.token = JSON.parse(localStorage.getItem('ASPNetAuthToken') as string) as Token;
      }
      else
        this.router.navigate(['']);
    }

  ngOnInit(): void {
    this.loadTerritories();
  }

  loadTerritories(){
    this.loadingTerritories = true;
    this.service.getTerritories().subscribe(res => {
      this.territories = res;
      this.loadingTerritories = false;
    },
    err => {
      this.toastr.error("Could not load territories.");
      this.loadingTerritories = false;
    });
  }

  search(atsName: string ){
    if(atsName != ""){
      this.loading = true;
      this.service.getACIDDetailsByATSNumber(atsName).subscribe(res => {
        this.acid = res;
        this.error = false;
        this.loading = false;
        this.lService.log("ACID CCTV", `${this.token?.email} has activated '${atsName}'.`);
      },
      err => {
        this.error = true;
        this.acid = undefined;
        this.loading = false;
      });
    }
  }

  loadAcids(value: string){
    if(value != ""){
      this.loadingAcids = true;
      this.service.getAcidsByTerritory(value).subscribe(res => {
        this.acids = res;
        this.loadingAcids = false;
      },
      err => {
        this.toastr.error("Could not load ACID CCTV's.");
        this.loadingAcids = false;
      });
    }
    else{
      this.acids = [];
    }
  }

  close(){
    this.selectedAcid = "";
    this.acid = undefined;
  }
}
