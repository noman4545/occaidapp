import { ToastrService } from 'ngx-toastr';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { genericType } from 'src/app/types/generic.type';
import { AuthenticationService } from '../../../../services/authentication.service';
import { NewPassword } from '../../../../models/newPassword.model';

@Component({
  selector: 'app-new-password',
  templateUrl: './new-password.component.html',
  styleUrls: ['./new-password.component.css']
})
export class NewPasswordComponent implements OnInit {
  newPasswordError: string = '';
  newPasswordForm: FormGroup;
  loading: boolean = false;
  constructor(private formBuilder: FormBuilder, 
    private service: AuthenticationService, 
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService) {
      let userId: string = route.snapshot.paramMap.get("id") as string;
      const form: genericType<NewPassword> = {
        'userId': [userId],
        'password': ['', [Validators.required, Validators.minLength(8), Validators.pattern("(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z0-9]).{8,}")]],
        'confirmPassword': ['', [Validators.required]]
      };
      this.newPasswordForm = this.formBuilder.group(form, { validators: this.checkPasswords });
  }

  ngOnInit(): void {
  }

  checkPasswords(group: FormGroup) {
    let pass = group.controls.password.value;
    let confirmPass = group.controls.confirmPassword.value;

    return pass === confirmPass ? null : { notSame: true }
  }

  newPasswordSave(passwordData: NewPassword){
    this.loadingStart();
    this.service.setNewPassword(passwordData).subscribe(res => {
      if(res)
      {
        this.toastr.success("Password has been changed successfully.")
        setTimeout(() => {
          this.router.navigate(['/admin-controls/manage-users']);
        }, 2000);
      }
      this.loadingEnd();
    },
    err => {
      if(err.status == 0)
        this.newPasswordError = 'Server is not responding, please try again.';
      else if(err.status == 401){
        this.router.navigate(['login']);
      }
      else
        this.newPasswordError = err.error.message;
      this.loadingEnd();
    });
  }

  loadingStart(){
    this.loading = true;
    this.newPasswordForm.disable();
    this.newPasswordForm.reset();
  }

  loadingEnd(){
    this.loading = false;
    this.newPasswordForm.enable();
  }

}
