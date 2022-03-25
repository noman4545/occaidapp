import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { genericType } from 'src/app/types/generic.type';
import { AuthenticationService } from '../../../../services/authentication.service';
import { Login } from '../../../../models/login.model';
import { Token } from '../../../../models/token.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  currentYear: number = new Date().getFullYear();
  loginError: string = '';
  loginForm: FormGroup;
  loading: boolean = false;

  constructor(private formBuilder: FormBuilder, private authenticationService: AuthenticationService, private router: Router) {
    this.clearSessionStorage();

    const form: genericType<Login> = {
      'email': ['', [Validators.required, Validators.email]],
      'password': ['', [Validators.required]],
    };

    this.loginForm = this.formBuilder.group(form);
  }

  ngOnInit(): void {
  }

  logIn(userData: Login){
    this.loginError = '';
    this.loadingStart();
    this.authenticationService.loginProcess(userData).subscribe(res => {
      let tokenData = res;
      localStorage.setItem('ASPNetAuthToken', JSON.stringify(tokenData));
      if(tokenData.role.toLowerCase() == 'admin'){
        this.router.navigate(['admin-controls']);
      }
      else{
        this.router.navigate(['central']);
      }
      this.loadingEnd();
    },
    err => {
      if(err.status == 0)
        this.loginError = 'Server is not responding, please try again.';
      else
        this.loginError = err.error.message;
      this.loadingEnd();
    });
  }

  clearSessionStorage(){
    sessionStorage.clear();
  }

  loadingStart(){
    this.loading = true;
    this.loginForm.disable();
  }

  loadingEnd(){
    this.loading = false;
    this.loginForm.enable();
  }

}
