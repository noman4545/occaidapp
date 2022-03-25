import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Log } from 'src/app/models/log.model';
import { CentralService } from 'src/app/services/central.service';
import { LoggingService } from 'src/app/services/logging.service';

@Component({
  selector: 'app-logs',
  templateUrl: './logs.component.html',
  styleUrls: ['./logs.component.css']
})
export class LogsComponent implements OnInit {
  loading: boolean = false;
  page: number = 1;
  take: number = 10;
  search: string = '';
  totalPages: number = 1;
  total: number = 0;

  logs: Log[] = [];
  roles: string[] = [];
  selectedRole: string = "";
  selectedScreen: string = "";
  constructor(private service: LoggingService,
    private cService: CentralService,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    this.loadLogs();
    this.loadRoles();
  }

  loadLogs(){
    this.loading = true;
    this.service.loadSpecialLogs(this.page, this.take, this.search, this.selectedRole, this.selectedScreen).subscribe(res => {
      this.logs = res.logs;
      this.total = res.total;
      this.totalPages = Math.ceil(res.total / this.take);
      this.loading = false;
    },
    err => {
      this.toastr.error("Unable to load Logs.", "Error!");
      this.loading = false;
    });
  }

  loadRoles(){
    this.cService.getRoles().subscribe(res => {
      this.roles = res;
    },
    err => {
      this.toastr.error("Could not able to load roles.");
    });
  }

  nextPage(){
    this.page++;
    this.loadLogs();
  }

  prevPage(){
    this.page--;
    this.loadLogs();
  }

  goToPage(pageNo: number){
    this.page = pageNo;
    this.loadLogs();
  }

}
