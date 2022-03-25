import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from './../../../../services/authentication.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Users } from 'src/app/models/users.model';

@Component({
  selector: 'app-manage-users',
  templateUrl: './manage-users.component.html',
  styleUrls: ['./manage-users.component.css']
})
export class ManageUsersComponent implements OnInit {
  loading: boolean = true;
  error: string = '';
  usersList: Users[] = [];
  displayedColumns: string[] = ['firstname', 'lastname', 'email', 'verified', 'action'];
  constructor(private service: AuthenticationService, 
    private router: Router,
    private toastr: ToastrService) {
  }

  ngOnInit(): void {
    this.getAllUsers();
  }

  getAllUsers(){
    this.service.getAllUser().subscribe(res => {
      this.usersList = res;
      this.loading = false;
    },
    err => {
      if(err.status == 0)
        this.error = 'Server is not responding, please try again';
      else if(err.status == 401)
        this.router.navigate(['/login']);
      else
        this.error = err.error.message;
      this.loading = false;
    });
  }

  deleteUser(userId: string, index: number){
    this.service.deleteUser(userId).subscribe(res => {
      if(res){
        this.usersList.splice(index);
        this.toastr.success("User has been deleted.");
      }
    },
    err => {
      this.toastr.error("Unable to delete user, system error.");
    });
  }
}
