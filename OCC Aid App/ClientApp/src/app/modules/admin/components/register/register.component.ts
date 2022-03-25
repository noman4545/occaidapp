import { ToastrService } from 'ngx-toastr';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Register } from '../../../../models/register.model';
import { User } from '../../../../models/user.model';
import { genericType } from '../../../../types/generic.type';
import { AuthenticationService } from '../../../../services/authentication.service';
import { Router } from '@angular/router';
import { CentralService } from 'src/app/services/central.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  currentYear: number = new Date().getFullYear();
  phoneNumberValid: boolean = true;
  registerForm: FormGroup;
  registerError: string = '';
  loading: boolean = false;
  roles: string[] = [];

  constructor(private formBuilder: FormBuilder, 
    private authenticationService: AuthenticationService, 
    private cService: CentralService,
    private router: Router,
    private toastr: ToastrService) { 
    const form: genericType<Register> = {
      'email': ['', [Validators.required, Validators.email]],
      'name': ['', [Validators.required]],
      'password': ['', [Validators.required, Validators.minLength(8), Validators.pattern("(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z0-9]).{8,}")]],
      'confirmPassword': ['', [Validators.required]],
      'role': ['', [Validators.required]]
    };

    this.registerForm = this.formBuilder.group(form, { validators: this.checkPasswords });
  }

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles(){
    this.cService.getRoles().subscribe(res => {
      this.roles = res;
    },
    err => {
      this.toastr.error("Could not able to load roles.");
    });
  }

  checkPasswords(group: FormGroup) {
    let pass = group.controls.password.value;
    let confirmPass = group.controls.confirmPassword.value;

    return pass === confirmPass ? null : { notSame: true }
  }

  signUp(userData: Register){
    this.registerError = '';
    this.loadingStart();
    let newUser: User = this.mapUser(userData);
    this.authenticationService.signupProcess(newUser).subscribe(res => {
      if(res){
        this.toastr.success("User has been registered successfully");
      }
      this.loadingEnd();
    },
    err => {
      if(err.status == 0)
        this.registerError = 'Server is not responding, please try again.';
      else
        this.registerError = err.error.message as any
      this.loadingEnd();
    });
  }

  mapUser(userData: Register): User{
    let newUser: User = {
      email: userData.email,
      name: userData.name,
      passwordHash: userData.password,
      role: userData.role
    };
    return newUser;
  }

  loadingStart(){
    this.loading = true;
    this.registerForm.disable();
  }

  loadingEnd(){
    this.loading = false;
    this.registerForm.enable();
    this.registerForm.reset();
  }
}
