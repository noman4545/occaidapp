import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CentralService } from 'src/app/services/central.service';

@Component({
  selector: 'app-central',
  templateUrl: './central.component.html',
  styleUrls: ['./central.component.css']
})
export class CentralComponent implements OnInit {
  constructor(private service: CentralService,
    private toastr: ToastrService,
    private router: Router) { }

  ngOnInit(): void {
  }

  goToPage(page: string) {
    this.router.navigate([page]);
  }

  logout() {
    localStorage.removeItem("ASPNetAuthToken");
    this.router.navigate(['auth']);
  }

}
