import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IOSCode } from 'src/app/models/iosCode.model';
import { Token } from 'src/app/models/token.model';
import { IOSCodesService } from 'src/app/services/ioscodes.service';
import { LoggingService } from 'src/app/services/logging.service';
import { NotificationService } from 'src/app/services/notification.service';

@Component({
  selector: 'app-ioscodes',
  templateUrl: './ioscodes.component.html',
  styleUrls: ['./ioscodes.component.css']
})
export class IOSCodesComponent implements OnInit {
  iosCode: IOSCode | undefined;
  error: boolean = false;
  loading: boolean = false;
  token: Token | undefined;
  constructor(private service: IOSCodesService, 
    private router: Router,
    private logService: LoggingService) {
      if(localStorage.getItem('ASPNetAuthToken')){
        this.token = JSON.parse(localStorage.getItem('ASPNetAuthToken') as string) as Token;
      }
      else
        this.router.navigate(['']);
    }

  ngOnInit(): void {
  }

  search(isoNumber: string ){
    if(isoNumber && isoNumber != ""){
      this.loading = true;
      this.service.getIOSCodeDetailsByIOSNumber(isoNumber).subscribe(res => {
        this.logService.log("IOS Codes", `${this.token?.email} has shared one of activated '${isoNumber}' on a train.`);
        this.iosCode = res;
        this.loading = false;
        this.error = false;
      },
      err => {
        this.error = true;
        this.loading = false;
        this.iosCode = undefined;
      });
    }
  }

  close(){
    this.iosCode = undefined;
  }

}
